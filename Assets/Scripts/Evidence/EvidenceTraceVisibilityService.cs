using OFIS.Core.Ids;

namespace OFIS.Evidence
{
    public sealed class EvidenceTraceVisibilityService
    {
        public EvidenceTraceVisibilityResult Resolve(
            EvidenceTraceRecord traceRecord,
            EvidenceTraceVisibilityType currentVisibility,
            PlayerId viewerPlayerId,
            PlayerId ownerPlayerId,
            bool viewerIsDetective,
            bool viewerIsMeetingParticipant)
        {
            if (string.IsNullOrWhiteSpace(traceRecord.TraceId.Value))
                return Result(false, EvidenceTraceVisibilityType.Hidden, traceRecord, "Trace is missing.");

            switch (currentVisibility)
            {
                case EvidenceTraceVisibilityType.Public:
                    return Result(true, currentVisibility, traceRecord, "Trace is public.");

                case EvidenceTraceVisibilityType.InspectorOnly:
                    return Result(viewerPlayerId == ownerPlayerId, currentVisibility, traceRecord, "Trace is inspector only.");

                case EvidenceTraceVisibilityType.MeetingParticipants:
                    return Result(viewerIsMeetingParticipant, currentVisibility, traceRecord, "Trace is meeting-participant only.");

                case EvidenceTraceVisibilityType.DetectiveOnly:
                    return Result(viewerIsDetective, currentVisibility, traceRecord, "Trace is detective only.");

                default:
                    return Result(false, EvidenceTraceVisibilityType.Hidden, traceRecord, "Trace is hidden.");
            }
        }

        private static EvidenceTraceVisibilityResult Result(
            bool canView,
            EvidenceTraceVisibilityType visibilityType,
            EvidenceTraceRecord traceRecord,
            string message)
        {
            return new EvidenceTraceVisibilityResult(canView, visibilityType, traceRecord, message);
        }
    }
}
