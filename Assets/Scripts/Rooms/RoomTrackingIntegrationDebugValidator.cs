using UnityEngine;

namespace OFIS.Rooms
{
    public sealed class RoomTrackingIntegrationDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly OfficeRoomQueryService _queryService = new OfficeRoomQueryService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateRoomTrackingIntegration();
        }

        [ContextMenu("Validate Room Tracking Integration")]
        public void ValidateRoomTrackingIntegration()
        {
            GameObject layoutRoot = new GameObject("RoomTrackingIntegration_TestRoot");
            OfficeLayoutDebugBuilder builder = layoutRoot.AddComponent<OfficeLayoutDebugBuilder>();
            builder.BuildLayout();

            OfficeRoomZone[] zones = layoutRoot.GetComponentsInChildren<OfficeRoomZone>();

            ValidatePosition(zones, new Vector2(0f, 0f), OfficeRoomType.Hallway, "HallwayPosition");
            ValidatePosition(zones, new Vector2(0f, 4.8f), OfficeRoomType.MeetingRoom, "MeetingRoomPosition");
            ValidatePosition(zones, new Vector2(4.5f, 4.8f), OfficeRoomType.ServerRoom, "ServerRoomPosition");
            ValidatePosition(zones, new Vector2(-3.9f, -4.8f), OfficeRoomType.HumanResources, "HumanResourcesPosition");
            ValidatePosition(zones, new Vector2(7.3f, -4.8f), OfficeRoomType.StorageRoom, "StorageRoomPosition");
            ValidatePosition(zones, new Vector2(6.8f, 7.9f), OfficeRoomType.Kitchen, "KitchenPosition");
            ValidatePosition(zones, new Vector2(20f, 20f), OfficeRoomType.None, "OutsidePosition");
        }

        private void ValidatePosition(OfficeRoomZone[] zones, Vector2 position, OfficeRoomType expectedType, string testName)
        {
            OfficeRoomQueryResult result = _queryService.QueryRoomAtPosition(position, zones);
            bool passed = result.RoomType == expectedType;

            if (passed)
                Debug.Log($"[RoomTrackingIntegrationValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[RoomTrackingIntegrationValidator] FAIL {testName}: Expected={expectedType}, Actual={result}");
        }
    }
}
