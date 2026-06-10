using System.Collections;
using UnityEngine;

namespace OFIS.Rooms
{
    public sealed class PlayerRoomTrackerPhysicsDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private PlayerRoomTracker _tracker;
        private Transform _playerTransform;

        private IEnumerator Start()
        {
            if (!validateOnStart)
                yield break;

            yield return ValidatePhysicsRoomTracking();
        }

        [ContextMenu("Validate Player Room Tracker Physics")]
        public void ValidateFromContextMenu()
        {
            StartCoroutine(ValidatePhysicsRoomTracking());
        }

        private IEnumerator ValidatePhysicsRoomTracking()
        {
            BuildTestScene();

            yield return MoveAndValidate(new Vector2(0f, 0f), OfficeRoomType.Hallway, "EnterHallway");
            yield return MoveAndValidate(new Vector2(0f, 4.8f), OfficeRoomType.MeetingRoom, "EnterMeetingRoom");
            yield return MoveAndValidate(new Vector2(4.5f, 4.8f), OfficeRoomType.ServerRoom, "EnterServerRoom");
            yield return MoveAndValidate(new Vector2(20f, 20f), OfficeRoomType.None, "ExitAllRooms");
        }

        private void BuildTestScene()
        {
            GameObject layoutRoot = new GameObject("PlayerRoomTrackerPhysics_TestLayout");
            OfficeLayoutDebugBuilder builder = layoutRoot.AddComponent<OfficeLayoutDebugBuilder>();
            builder.BuildLayout();

            GameObject playerObject = new GameObject("PlayerRoomTrackerPhysics_TestPlayer");
            _playerTransform = playerObject.transform;
            _playerTransform.position = new Vector3(20f, 20f, 0f);

            Rigidbody2D rigidbody = playerObject.AddComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            rigidbody.gravityScale = 0f;
            rigidbody.simulated = true;

            CircleCollider2D collider = playerObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = false;
            collider.radius = 0.25f;

            _tracker = playerObject.AddComponent<PlayerRoomTracker>();
        }

        private IEnumerator MoveAndValidate(Vector2 position, OfficeRoomType expectedType, string testName)
        {
            _playerTransform.position = new Vector3(position.x, position.y, 0f);
            Physics2D.SyncTransforms();

            yield return new WaitForFixedUpdate();
            yield return null;

            bool passed = _tracker != null && _tracker.CurrentRoomType == expectedType;

            if (passed)
            {
                Debug.Log($"[PlayerRoomTrackerPhysicsValidator] PASS {testName}: CurrentRoom={_tracker.CurrentRoomType} ({_tracker.CurrentRoomDisplayName})");
            }
            else
            {
                string actual = _tracker == null ? "NoTracker" : _tracker.CurrentRoomType.ToString();
                Debug.LogError($"[PlayerRoomTrackerPhysicsValidator] FAIL {testName}: Expected={expectedType}, Actual={actual}");
            }
        }
    }
}
