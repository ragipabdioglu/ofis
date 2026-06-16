namespace OFIS.Meetings
{
    public sealed class MeetingRuntimeSceneBridgeService
    {
        private readonly MeetingRuntimeDecisionAggregatorService _aggregatorService;

        public MeetingRuntimeSceneBridgeService()
        {
            _aggregatorService = new MeetingRuntimeDecisionAggregatorService();
        }

        public MeetingRuntimeSceneBridgeService(MeetingRuntimeDecisionAggregatorService aggregatorService)
        {
            _aggregatorService = aggregatorService ?? new MeetingRuntimeDecisionAggregatorService();
        }

        public void Reset()
        {
            _aggregatorService.Reset();
        }

        public MeetingRuntimeSceneBridgeState ResolveSceneState(
            MeetingRuntimeDebugScenarioType scenarioType,
            MeetingRuntimeDecisionInput input)
        {
            MeetingRuntimeDecisionResult decisionResult = _aggregatorService.Resolve(input);

            int healthBefore = input.CurrentCompanyHealth;
            int healthAfter = input.CurrentCompanyHealth;

            if (decisionResult.HasHealthPenaltyBridgeResult)
                healthAfter = decisionResult.HealthPenaltyBridgeResult.CompanyHealthAfter;

            return new MeetingRuntimeSceneBridgeState(
                scenarioType,
                decisionResult,
                healthBefore,
                healthAfter);
        }
    }
}
