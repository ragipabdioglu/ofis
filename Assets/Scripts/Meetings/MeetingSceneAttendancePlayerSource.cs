using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingSceneAttendancePlayerSource : MonoBehaviour
    {
        [SerializeField] private string playerId;
        [SerializeField] private PlayerRoomTracker roomTracker;
        [SerializeField] private bool isAlive = true;
        [SerializeField] private bool isConnected = true;
        [SerializeField] private bool isExposed = false;
        [SerializeField] private bool isLocalPlayer = false;

        private void Awake()
        {
            if (roomTracker == null)
                roomTracker = GetComponent<PlayerRoomTracker>();
        }

        public MeetingAttendancePlayerSnapshot BuildSnapshot()
        {
            OfficeRoomType roomType = roomTracker == null
                ? OfficeRoomType.None
                : roomTracker.CurrentRoomType;

            return new MeetingAttendancePlayerSnapshot(
                playerId,
                roomType,
                isAlive,
                isConnected,
                isLocalPlayer,
                isExposed);
        }
    }
}
