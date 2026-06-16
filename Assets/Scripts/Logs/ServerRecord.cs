using OFIS.Core.Ids;
using OFIS.Rooms;

namespace OFIS.Logs
{
    public readonly struct ServerRecord
    {
        public string RecordId { get; }
        public MatchId MatchId { get; }
        public RecordCategory Category { get; }
        public RecordVisibility Visibility { get; }
        public PlayerId ActorPlayerId { get; }
        public string SubjectId { get; }
        public OfficeRoomType RoomType { get; }
        public float ServerTimeSeconds { get; }
        public string Summary { get; }
        public string RawPayload { get; }

        public bool IsServerOnly => Visibility == RecordVisibility.ServerOnly;

        public ServerRecord(
            string recordId,
            MatchId matchId,
            RecordCategory category,
            RecordVisibility visibility,
            PlayerId actorPlayerId,
            string subjectId,
            OfficeRoomType roomType,
            float serverTimeSeconds,
            string summary,
            string rawPayload)
        {
            RecordId = string.IsNullOrWhiteSpace(recordId) ? "unknown_record" : recordId;
            MatchId = matchId;
            Category = category;
            Visibility = category == RecordCategory.KillServerOnly ? RecordVisibility.ServerOnly : visibility;
            ActorPlayerId = actorPlayerId;
            SubjectId = string.IsNullOrWhiteSpace(subjectId) ? "none" : subjectId;
            RoomType = roomType;
            ServerTimeSeconds = serverTimeSeconds < 0f ? 0f : serverTimeSeconds;
            Summary = string.IsNullOrWhiteSpace(summary) ? "Record captured." : summary;
            RawPayload = string.IsNullOrWhiteSpace(rawPayload) ? string.Empty : rawPayload;
        }

        public override string ToString()
        {
            return $"Record={RecordId}, Category={Category}, Visibility={Visibility}, Room={RoomType}, Time={ServerTimeSeconds:0.##}, Subject={SubjectId}";
        }
    }
}
