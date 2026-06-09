using UnityEngine;

namespace OFIS.Rooms
{
    public sealed class RoomDebugLabel : MonoBehaviour
    {
        [SerializeField] private OfficeRoomZone roomZone;
        [SerializeField] private Vector3 labelOffset = new(0f, 0f, 0f);

        private void Reset()
        {
            roomZone = GetComponent<OfficeRoomZone>();
        }

        private void OnDrawGizmos()
        {
            if (roomZone == null)
                roomZone = GetComponent<OfficeRoomZone>();

            if (roomZone == null)
                return;

#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                transform.position + labelOffset,
                $"{roomZone.DisplayName}\n{roomZone.RoomType}");
#endif
        }
    }
}