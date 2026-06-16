namespace OFIS.Meetings
{
    public readonly struct MeetingRuntimeDecisionResult
    {
        public MeetingRuntimeDecisionType DecisionType { get; }
        public MeetingRuntimePhaseType PhaseType { get; }
        public MeetingPhaseRuntimeHookResult RuntimeHookResult { get; }
        public MeetingCompanyHealthPenaltyBridgeResult HealthPenaltyBridgeResult { get; }
        public MeetingEmptyStateResolutionResult EmptyStateResolutionResult { get; }
        public bool HasRuntimeHookResult { get; }
        public bool HasHealthPenaltyBridgeResult { get; }
        public bool HasEmptyStateResolutionResult { get; }
        public bool ShouldContinueMeeting { get; }
        public bool ShouldApplyHealthPenalty { get; }
        public bool ShouldCloseMeeting { get; }
        public bool ShouldResolveWinBranch { get; }
        public bool ShouldRunMeetingEndPipeline { get; }
        public bool IsTerminalDecision { get; }
        public string Reason { get; }

        public MeetingRuntimeDecisionResult(
            MeetingRuntimeDecisionType decisionType,
            MeetingRuntimePhaseType phaseType,
            MeetingPhaseRuntimeHookResult runtimeHookResult,
            MeetingCompanyHealthPenaltyBridgeResult healthPenaltyBridgeResult,
            MeetingEmptyStateResolutionResult emptyStateResolutionResult,
            bool hasRuntimeHookResult,
            bool hasHealthPenaltyBridgeResult,
            bool hasEmptyStateResolutionResult,
            bool shouldContinueMeeting,
            bool shouldApplyHealthPenalty,
            bool shouldCloseMeeting,
            bool shouldResolveWinBranch,
            bool shouldRunMeetingEndPipeline,
            bool isTerminalDecision,
            string reason)
        {
            DecisionType = decisionType;
            PhaseType = phaseType;
            RuntimeHookResult = runtimeHookResult;
            HealthPenaltyBridgeResult = healthPenaltyBridgeResult;
            EmptyStateResolutionResult = emptyStateResolutionResult;
            HasRuntimeHookResult = hasRuntimeHookResult;
            HasHealthPenaltyBridgeResult = hasHealthPenaltyBridgeResult;
            HasEmptyStateResolutionResult = hasEmptyStateResolutionResult;
            ShouldContinueMeeting = shouldContinueMeeting;
            ShouldApplyHealthPenalty = shouldApplyHealthPenalty;
            ShouldCloseMeeting = shouldCloseMeeting;
            ShouldResolveWinBranch = shouldResolveWinBranch;
            ShouldRunMeetingEndPipeline = shouldRunMeetingEndPipeline;
            IsTerminalDecision = isTerminalDecision;
            Reason = string.IsNullOrWhiteSpace(reason)
                ? "Meeting runtime decision resolved."
                : reason;
        }

        public override string ToString()
        {
            return $"Decision={DecisionType}, Phase={PhaseType}, Continue={ShouldContinueMeeting}, ApplyPenalty={ShouldApplyHealthPenalty}, Close={ShouldCloseMeeting}, WinBranch={ShouldResolveWinBranch}, RunPipeline={ShouldRunMeetingEndPipeline}, Terminal={IsTerminalDecision}, Reason={Reason}";
        }
    }
}
