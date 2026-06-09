using OFIS.MatchFlow.States;

namespace OFIS.MatchFlow.Timeline
{
    public static class MatchTimelineFactory
    {
        public const float ProductionTotalDurationSeconds = 1080f;

        public static MatchTimelineDefinition CreateProductionTimeline()
        {
            return new MatchTimelineDefinition(
                new MatchTimelineSegment(MatchState.OfficePhase1, 240f),
                new MatchTimelineSegment(MatchState.Meeting1, 120f),
                new MatchTimelineSegment(MatchState.OfficePhase2, 240f),
                new MatchTimelineSegment(MatchState.Meeting2, 120f),
                new MatchTimelineSegment(MatchState.OfficePhase3, 240f),
                new MatchTimelineSegment(MatchState.FinalMeeting, 120f));
        }

        public static MatchTimelineDefinition CreateFastDebugTimeline()
        {
            return new MatchTimelineDefinition(
                new MatchTimelineSegment(MatchState.OfficePhase1, 10f),
                new MatchTimelineSegment(MatchState.Meeting1, 6f),
                new MatchTimelineSegment(MatchState.OfficePhase2, 10f),
                new MatchTimelineSegment(MatchState.Meeting2, 6f),
                new MatchTimelineSegment(MatchState.OfficePhase3, 10f),
                new MatchTimelineSegment(MatchState.FinalMeeting, 6f));
        }
    }
}
