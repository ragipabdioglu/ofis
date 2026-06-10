using OFIS.Rooms;

namespace OFIS.Tasks
{
    public readonly struct OfficeTaskDefinition
    {
        public string TaskId { get; }
        public string DisplayName { get; }
        public OfficeRoomType RoomType { get; }
        public float BaseDurationSeconds { get; }

        public OfficeTaskDefinition(string taskId, string displayName, OfficeRoomType roomType, float baseDurationSeconds)
        {
            TaskId = string.IsNullOrWhiteSpace(taskId) ? "unknown_task" : taskId;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? TaskId : displayName;
            RoomType = roomType;
            BaseDurationSeconds = baseDurationSeconds < 0f ? 0f : baseDurationSeconds;
        }

        public override string ToString()
        {
            return $"TaskId={TaskId}, DisplayName={DisplayName}, Room={RoomType}, BaseDuration={BaseDurationSeconds:0.##}s";
        }
    }
}
