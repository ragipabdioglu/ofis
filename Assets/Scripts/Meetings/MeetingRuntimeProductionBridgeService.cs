namespace OFIS.Meetings
{
    public sealed class MeetingRuntimeProductionBridgeService
    {
        private readonly MeetingRuntimeDecisionAggregatorService _aggregatorService;

        public MeetingRuntimeProductionBridgeService()
        {
            _aggregatorService = new MeetingRuntimeDecisionAggregatorService();
        }

        public MeetingRuntimeProductionBridgeService(MeetingRuntimeDecisionAggregatorService aggregatorService)
        {
            _aggregatorService = aggregatorService ?? new MeetingRuntimeDecisionAggregatorService();
        }

        public void Reset()
        {
            _aggregatorService.Reset();
        }

        public MeetingProductionBridgeResult Resolve(MeetingRuntimeDecisionInput input)
        {
            MeetingRuntimeDecisionResult decisionResult = _aggregatorService.Resolve(input);
            MeetingProductionBridgeCommand command = BuildCommand(decisionResult, input.CurrentCompanyHealth);
            bool hasCommand = command.ActionType != MeetingProductionBridgeActionType.None;

            return new MeetingProductionBridgeResult(
                decisionResult,
                command,
                hasCommand,
                "Meeting runtime production bridge resolved.");
        }

        public MeetingProductionBridgeCommand BuildCommand(
            MeetingRuntimeDecisionResult decisionResult,
            int currentCompanyHealth)
        {
            int healthBefore = currentCompanyHealth < 0 ? 0 : currentCompanyHealth;
            int healthAfter = healthBefore;
            bool hasSummary = false;
            MeetingSummaryUiState summary = default;

            if (decisionResult.HasHealthPenaltyBridgeResult)
            {
                healthBefore = decisionResult.HealthPenaltyBridgeResult.CompanyHealthBefore;
                healthAfter = decisionResult.HealthPenaltyBridgeResult.CompanyHealthAfter;
            }

            if (decisionResult.HasRuntimeHookResult
                && decisionResult.RuntimeHookResult.HasPipelineResult)
            {
                hasSummary = true;
                summary = decisionResult.RuntimeHookResult.PipelineResult.SummaryUiState;
            }

            MeetingProductionBridgeActionType actionType = MapActionType(decisionResult);

            return new MeetingProductionBridgeCommand(
                actionType,
                decisionResult.ShouldContinueMeeting,
                decisionResult.ShouldApplyHealthPenalty,
                decisionResult.ShouldCloseMeeting,
                decisionResult.ShouldResolveWinBranch,
                decisionResult.ShouldRunMeetingEndPipeline,
                healthBefore,
                healthAfter,
                hasSummary,
                summary,
                decisionResult.Reason);
        }

        private static MeetingProductionBridgeActionType MapActionType(
            MeetingRuntimeDecisionResult decisionResult)
        {
            switch (decisionResult.DecisionType)
            {
                case MeetingRuntimeDecisionType.ContinueMeeting:
                    return MeetingProductionBridgeActionType.ContinueMeeting;

                case MeetingRuntimeDecisionType.ApplyMissingPlayerPenalty:
                    return MeetingProductionBridgeActionType.ApplyCompanyHealthPenalty;

                case MeetingRuntimeDecisionType.AutoCloseMeeting:
                    return MeetingProductionBridgeActionType.CloseMeeting;

                case MeetingRuntimeDecisionType.ResolveFinalMeetingWinBranch:
                    return MeetingProductionBridgeActionType.ResolveFinalMeetingWinBranch;

                case MeetingRuntimeDecisionType.RunMeetingEndPipeline:
                    return MeetingProductionBridgeActionType.RunMeetingEndPipeline;

                case MeetingRuntimeDecisionType.None:
                default:
                    return MeetingProductionBridgeActionType.None;
            }
        }
    }
}
