using OFIS.MatchFlow.States;

namespace OFIS.Meetings
{
    public sealed class MeetingMatchFlowCommandConsumerService
    {
        private readonly MeetingMatchFlowCommandConsumerConfig _config;

        public MeetingMatchFlowCommandConsumerService()
            : this(MeetingMatchFlowCommandConsumerConfig.SafeDryRun)
        {
        }

        public MeetingMatchFlowCommandConsumerService(
            MeetingMatchFlowCommandConsumerConfig config)
        {
            _config = config;
        }

        public MeetingMatchFlowCommandConsumerResult Consume(
            MeetingProductionRuntimeListenerState listenerState,
            MatchState currentMatchState)
        {
            if (!listenerState.HasRuntimeEvent)
            {
                return BuildResult(
                    MeetingMatchFlowTransitionType.None,
                    currentMatchState,
                    MatchState.None,
                    hasTransitionRequest: false,
                    reason: "No runtime event is available.");
            }

            if (listenerState.LastRequestedWinBranchResolution)
            {
                return BuildResult(
                    MeetingMatchFlowTransitionType.ResolveFinalMeeting,
                    currentMatchState,
                    MatchState.ResolvingMatch,
                    hasTransitionRequest: true,
                    reason: "Final meeting win branch resolution was requested.");
            }

            if (listenerState.LastRequestedCloseMeeting)
            {
                return BuildResult(
                    MeetingMatchFlowTransitionType.CloseNormalMeeting,
                    currentMatchState,
                    GetNextStateAfterMeeting(currentMatchState),
                    hasTransitionRequest: true,
                    reason: "Normal meeting close was requested.");
            }

            if (listenerState.LastRequestedMeetingEndPipeline)
            {
                return BuildResult(
                    MeetingMatchFlowTransitionType.ShowMeetingEndSummary,
                    currentMatchState,
                    currentMatchState,
                    hasTransitionRequest: true,
                    reason: "Meeting end pipeline summary should be shown before a later transition.");
            }

            if (listenerState.LastAppliedCompanyHealthDelta)
            {
                return BuildResult(
                    MeetingMatchFlowTransitionType.ApplyCompanyHealthOnly,
                    currentMatchState,
                    currentMatchState,
                    hasTransitionRequest: true,
                    reason: "Only company health was applied; meeting continues.");
            }

            return BuildResult(
                MeetingMatchFlowTransitionType.ContinueMeeting,
                currentMatchState,
                currentMatchState,
                hasTransitionRequest: false,
                reason: "Meeting should continue.");
        }

        private MeetingMatchFlowCommandConsumerResult BuildResult(
            MeetingMatchFlowTransitionType transitionType,
            MatchState sourceMatchState,
            MatchState suggestedNextMatchState,
            bool hasTransitionRequest,
            string reason)
        {
            return new MeetingMatchFlowCommandConsumerResult(
                transitionType,
                sourceMatchState,
                suggestedNextMatchState,
                hasTransitionRequest,
                _config.DryRunOnly,
                _config.AllowRuntimeMutation,
                runtimeMutationApplied: false,
                reason: reason);
        }

        private static MatchState GetNextStateAfterMeeting(MatchState currentMatchState)
        {
            switch (currentMatchState)
            {
                case MatchState.Meeting1:
                    return MatchState.OfficePhase2;

                case MatchState.Meeting2:
                    return MatchState.OfficePhase3;

                case MatchState.FinalMeeting:
                    return MatchState.ResolvingMatch;

                default:
                    return MatchState.None;
            }
        }
    }
}
