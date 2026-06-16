namespace OFIS.Meetings
{
    public readonly struct MeetingRuntimeSceneBridgeState
    {
        public MeetingRuntimeDebugScenarioType ScenarioType { get; }
        public MeetingRuntimeDecisionType DecisionType { get; }
        public MeetingRuntimePhaseType PhaseType { get; }
        public bool HasDecision { get; }
        public bool IsTerminalDecision { get; }
        public bool ShouldApplyHealthPenalty { get; }
        public bool ShouldCloseMeeting { get; }
        public bool ShouldResolveWinBranch { get; }
        public bool ShouldRunMeetingEndPipeline { get; }
        public int CompanyHealthBefore { get; }
        public int CompanyHealthAfter { get; }
        public string Summary { get; }

        public MeetingRuntimeSceneBridgeState(
            MeetingRuntimeDebugScenarioType scenarioType,
            MeetingRuntimeDecisionResult decisionResult,
            int companyHealthBefore,
            int companyHealthAfter)
        {
            ScenarioType = scenarioType;
            DecisionType = decisionResult.DecisionType;
            PhaseType = decisionResult.PhaseType;
            HasDecision = decisionResult.DecisionType != MeetingRuntimeDecisionType.None;
            IsTerminalDecision = decisionResult.IsTerminalDecision;
            ShouldApplyHealthPenalty = decisionResult.ShouldApplyHealthPenalty;
            ShouldCloseMeeting = decisionResult.ShouldCloseMeeting;
            ShouldResolveWinBranch = decisionResult.ShouldResolveWinBranch;
            ShouldRunMeetingEndPipeline = decisionResult.ShouldRunMeetingEndPipeline;
            CompanyHealthBefore = companyHealthBefore < 0 ? 0 : companyHealthBefore;
            CompanyHealthAfter = companyHealthAfter < 0 ? 0 : companyHealthAfter;

            Summary = $"Scenario={ScenarioType}, Decision={DecisionType}, Phase={PhaseType}, Terminal={IsTerminalDecision}, Health={CompanyHealthBefore}->{CompanyHealthAfter}";
        }

        public override string ToString()
        {
            return Summary;
        }
    }
}
