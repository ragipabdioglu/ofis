using OFIS.Core.Ids;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Corpse
{
    public readonly struct CorpseMovementTraceEvent
    {
        public EvidenceTraceId TraceId { get; }
        public CorpseMovementTraceType TraceType { get; }
        public CorpseId CorpseId { get; }
        public PlayerId ActorPlayerId { get; }
        public string VictimDisplayName { get; }
        public OfficeRoomType RoomType { get; }
        public Vector3 WorldPosition { get; }
        public float ServerTimeSeconds { get; }
        public bool HiddenUntilCorpseInspect { get; }

        public CorpseMovementTraceEvent(
            EvidenceTraceId traceId,
            CorpseMovementTraceType traceType,
            CorpseId corpseId,
            PlayerId actorPlayerId,
            string victimDisplayName,
            OfficeRoomType roomType,
            Vector3 worldPosition,
            float serverTimeSeconds,
            bool hiddenUntilCorpseInspect)
        {
            TraceId = traceId;
            TraceType = traceType;
            CorpseId = corpseId;
            ActorPlayerId = actorPlayerId;
            VictimDisplayName = string.IsNullOrWhiteSpace(victimDisplayName)
                ? "Unknown Victim"
                : victimDisplayName;
            RoomType = roomType;
            WorldPosition = worldPosition;
            ServerTimeSeconds = serverTimeSeconds;
            HiddenUntilCorpseInspect = hiddenUntilCorpseInspect;
        }

        public override string ToString()
        {
            return $"Trace={TraceId}, Type={TraceType}, Corpse={CorpseId}, Actor={ActorPlayerId}, Room={RoomType}, HiddenUntilInspect={HiddenUntilCorpseInspect}";
        }
    }
}
