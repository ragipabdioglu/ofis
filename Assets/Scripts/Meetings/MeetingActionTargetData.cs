using OFIS.Roles.Departments;
using OFIS.Rooms;

namespace OFIS.Meetings
{
    public readonly struct MeetingActionTargetData
    {
        public MeetingActionTargetType TargetType { get; }
        public string PlayerId { get; }
        public OfficeRoomType RoomType { get; }
        public DepartmentType DepartmentType { get; }
        public MeetingSecurityAreaType SecurityAreaType { get; }

        public bool HasPlayerTarget =>
            TargetType == MeetingActionTargetType.Player
            && !string.IsNullOrWhiteSpace(PlayerId);

        public bool HasRoomTarget =>
            TargetType == MeetingActionTargetType.Room
            && RoomType != OfficeRoomType.None
            && RoomType != OfficeRoomType.Unknown;

        public bool HasDepartmentTarget =>
            TargetType == MeetingActionTargetType.Department
            && DepartmentType != OFIS.Roles.Departments.DepartmentType.None;

        public bool HasSecurityAreaTarget =>
            TargetType == MeetingActionTargetType.SecurityArea
            && SecurityAreaType != MeetingSecurityAreaType.None;

        public bool IsEmpty => TargetType == MeetingActionTargetType.None;

        private MeetingActionTargetData(
            MeetingActionTargetType targetType,
            string playerId,
            OfficeRoomType roomType,
            DepartmentType departmentType,
            MeetingSecurityAreaType securityAreaType)
        {
            TargetType = targetType;
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? string.Empty : playerId;
            RoomType = roomType;
            DepartmentType = departmentType;
            SecurityAreaType = securityAreaType;
        }

        public static MeetingActionTargetData None()
        {
            return new MeetingActionTargetData(
                MeetingActionTargetType.None,
                string.Empty,
                OfficeRoomType.None,
                OFIS.Roles.Departments.DepartmentType.None,
                MeetingSecurityAreaType.None);
        }

        public static MeetingActionTargetData ForPlayer(string playerId)
        {
            return new MeetingActionTargetData(
                MeetingActionTargetType.Player,
                playerId,
                OfficeRoomType.None,
                OFIS.Roles.Departments.DepartmentType.None,
                MeetingSecurityAreaType.None);
        }

        public static MeetingActionTargetData ForRoom(OfficeRoomType roomType)
        {
            return new MeetingActionTargetData(
                MeetingActionTargetType.Room,
                string.Empty,
                roomType,
                OFIS.Roles.Departments.DepartmentType.None,
                MeetingSecurityAreaType.None);
        }

        public static MeetingActionTargetData ForDepartment(DepartmentType departmentType)
        {
            return new MeetingActionTargetData(
                MeetingActionTargetType.Department,
                string.Empty,
                OfficeRoomType.None,
                departmentType,
                MeetingSecurityAreaType.None);
        }

        public static MeetingActionTargetData ForSecurityArea(
            MeetingSecurityAreaType securityAreaType)
        {
            return new MeetingActionTargetData(
                MeetingActionTargetType.SecurityArea,
                string.Empty,
                OfficeRoomType.None,
                OFIS.Roles.Departments.DepartmentType.None,
                securityAreaType);
        }

        public override string ToString()
        {
            return $"TargetType={TargetType}, Player={PlayerId}, Room={RoomType}, Department={DepartmentType}, SecurityArea={SecurityAreaType}";
        }
    }
}
