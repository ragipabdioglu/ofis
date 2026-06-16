using OFIS.Rooms;

namespace OFIS.Meetings
{
    public readonly struct MeetingAttendancePlayerSnapshot
    {
        public string PlayerId { get; }
        public OfficeRoomType CurrentRoom { get; }
        public bool IsAlive { get; }
        public bool IsConnected { get; }
        public bool IsLocalPlayer { get; }
        public bool IsExposed { get; }

        public bool IsInMeetingRoom => CurrentRoom == OfficeRoomType.MeetingRoom;
        public bool IsEligible => !string.IsNullOrWhiteSpace(PlayerId) && IsAlive && IsConnected && !IsExposed;
        public bool CanRegisterForMeeting => IsEligible && IsInMeetingRoom;

        public MeetingAttendancePlayerSnapshot(
            string playerId,
            OfficeRoomType currentRoom,
            bool isAlive,
            bool isConnected,
            bool isLocalPlayer = false,
            bool isExposed = false)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? string.Empty : playerId;
            CurrentRoom = currentRoom;
            IsAlive = isAlive;
            IsConnected = isConnected;
            IsLocalPlayer = isLocalPlayer;
            IsExposed = isExposed;
        }

        public override string ToString()
        {
            return $"PlayerId={PlayerId}, Room={CurrentRoom}, Alive={IsAlive}, Connected={IsConnected}, Exposed={IsExposed}, Local={IsLocalPlayer}, Eligible={IsEligible}, CanRegister={CanRegisterForMeeting}";
        }
    }
}
