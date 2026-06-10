using OFIS.Rooms;

namespace OFIS.Sabotage
{
    public readonly struct SabotageObjectiveDefinition
    {
        public string SabotageId { get; }
        public string DisplayName { get; }
        public OfficeRoomType RoomType { get; }
        public float RepairDurationSeconds { get; }

        public SabotageObjectiveDefinition(string sabotageId, string displayName, OfficeRoomType roomType, float repairDurationSeconds)
        {
            SabotageId = string.IsNullOrWhiteSpace(sabotageId) ? "unknown_sabotage" : sabotageId;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? SabotageId : displayName;
            RoomType = roomType;
            RepairDurationSeconds = repairDurationSeconds < 0f ? 0f : repairDurationSeconds;
        }

        public override string ToString()
        {
            return $"SabotageId={SabotageId}, DisplayName={DisplayName}, Room={RoomType}, RepairDuration={RepairDurationSeconds:0.##}s";
        }
    }
}
