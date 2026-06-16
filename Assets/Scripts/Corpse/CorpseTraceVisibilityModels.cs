using System.Collections.Generic;
using OFIS.Core.Ids;

namespace OFIS.Corpse
{
    public enum CorpseTraceVisibilityType
    {
        Hidden = 0,
        InspectorOnly = 1,
        Public = 2
    }

    public readonly struct CorpseTraceVisibilityResult
    {
        public bool CanView { get; }
        public CorpseTraceVisibilityType VisibilityType { get; }
        public CorpseMovementTraceEvent TraceEvent { get; }
        public string Message { get; }

        public CorpseTraceVisibilityResult(
            bool canView,
            CorpseTraceVisibilityType visibilityType,
            CorpseMovementTraceEvent traceEvent,
            string message)
        {
            CanView = canView;
            VisibilityType = visibilityType;
            TraceEvent = traceEvent;
            Message = string.IsNullOrWhiteSpace(message) ? "No visibility message." : message;
        }
    }

    public sealed class CorpseTraceVisibilityService
    {
        public CorpseTraceVisibilityResult ResolveForViewer(
            CorpseMovementTraceEvent traceEvent,
            PlayerId viewerPlayerId,
            CorpseOwnerKnowledge ownerKnowledge,
            bool isAnnouncedPublic)
        {
            if (string.IsNullOrWhiteSpace(traceEvent.TraceId.Value))
            {
                return new CorpseTraceVisibilityResult(
                    false,
                    CorpseTraceVisibilityType.Hidden,
                    traceEvent,
                    "Trace event is missing.");
            }

            if (isAnnouncedPublic)
            {
                return new CorpseTraceVisibilityResult(
                    true,
                    CorpseTraceVisibilityType.Public,
                    traceEvent,
                    "Trace is public after announcement.");
            }

            bool ownerCanView = ownerKnowledge.IsOwnerOnly
                && ownerKnowledge.OwnerPlayerId == viewerPlayerId
                && ownerKnowledge.CorpseId == traceEvent.CorpseId;

            if (ownerCanView)
            {
                return new CorpseTraceVisibilityResult(
                    true,
                    CorpseTraceVisibilityType.InspectorOnly,
                    traceEvent,
                    "Trace is visible to inspector only.");
            }

            return new CorpseTraceVisibilityResult(
                false,
                CorpseTraceVisibilityType.Hidden,
                traceEvent,
                "Trace remains hidden until corpse inspect or announcement.");
        }
    }

    public sealed class CorpseOwnerTraceViewService
    {
        private readonly CorpseTraceVisibilityService _visibilityService = new CorpseTraceVisibilityService();

        public IReadOnlyList<CorpseTraceVisibilityResult> BuildOwnerView(
            IReadOnlyList<CorpseMovementTraceEvent> traceEvents,
            PlayerId viewerPlayerId,
            CorpseOwnerKnowledge ownerKnowledge,
            bool isAnnouncedPublic)
        {
            List<CorpseTraceVisibilityResult> visibleTraces = new List<CorpseTraceVisibilityResult>();

            if (traceEvents == null)
                return visibleTraces;

            for (int i = 0; i < traceEvents.Count; i++)
            {
                CorpseTraceVisibilityResult result = _visibilityService.ResolveForViewer(
                    traceEvents[i],
                    viewerPlayerId,
                    ownerKnowledge,
                    isAnnouncedPublic);

                if (result.CanView)
                    visibleTraces.Add(result);
            }

            return visibleTraces;
        }
    }
}
