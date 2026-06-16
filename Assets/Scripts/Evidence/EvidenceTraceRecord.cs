using OFIS.Core.Ids;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Evidence
{
    public readonly struct EvidenceTraceRecord
    {
        public EvidenceTraceId TraceId { get; }
        public EvidenceTraceType TraceType { get; }
        public string SourceId { get; }
        public OfficeRoomType RoomType { get; }
        public Vector3 WorldPosition { get; }
        public float ServerTimeSeconds { get; }
        public string Summary { get; }

        public EvidenceTraceRecord(
            EvidenceTraceId traceId,
            EvidenceTraceType traceType,
            string sourceId,
            OfficeRoomType roomType,
            Vector3 worldPosition,
            float serverTimeSeconds,
            string summary)
        {
            TraceId = traceId;
            TraceType = traceType;
            SourceId = string.IsNullOrWhiteSpace(sourceId) ? "unknown_source" : sourceId;
            RoomType = roomType;
            WorldPosition = worldPosition;
            ServerTimeSeconds = serverTimeSeconds;
            Summary = string.IsNullOrWhiteSpace(summary) ? "No trace summary." : summary;
        }

        public override string ToString()
        {
            return $"Trace={TraceId}, Type={TraceType}, Source={SourceId}, Room={RoomType}, Time={ServerTimeSeconds:0.##}, Summary={Summary}";
        }
    }
}
