namespace OFIS.Meetings
{
    public readonly struct MeetingProductionBridgeCommand
    {
        public MeetingProductionBridgeActionType ActionType { get; }
        public bool ShouldContinueMeeting { get; }
        public bool ShouldApplyCompanyHealthDelta { get; }
        public bool ShouldCloseMeeting { get; }
        public bool ShouldResolveWinBranch { get; }
        public bool ShouldRunMeetingEndPipeline { get; }
        public int CompanyHealthBefore { get; }
        public int CompanyHealthAfter { get; }
        public int CompanyHealthDelta { get; }
        public bool HasSummaryUiState { get; }
        public MeetingSummaryUiState SummaryUiState { get; }
        public string Reason { get; }

        public MeetingProductionBridgeCommand(
            MeetingProductionBridgeActionType actionType,
            bool shouldContinueMeeting,
            bool shouldApplyCompanyHealthDelta,
            bool shouldCloseMeeting,
            bool shouldResolveWinBranch,
            bool shouldRunMeetingEndPipeline,
            int companyHealthBefore,
            int companyHealthAfter,
            bool hasSummaryUiState,
            MeetingSummaryUiState summaryUiState,
            string reason)
        {
            ActionType = actionType;
            ShouldContinueMeeting = shouldContinueMeeting;
            ShouldApplyCompanyHealthDelta = shouldApplyCompanyHealthDelta;
            ShouldCloseMeeting = shouldCloseMeeting;
            ShouldResolveWinBranch = shouldResolveWinBranch;
            ShouldRunMeetingEndPipeline = shouldRunMeetingEndPipeline;
            CompanyHealthBefore = companyHealthBefore < 0 ? 0 : companyHealthBefore;
            CompanyHealthAfter = companyHealthAfter < 0 ? 0 : companyHealthAfter;
            CompanyHealthDelta = CompanyHealthAfter - CompanyHealthBefore;
            HasSummaryUiState = hasSummaryUiState;
            SummaryUiState = summaryUiState;
            Reason = string.IsNullOrWhiteSpace(reason)
                ? "Meeting production bridge command resolved."
                : reason;
        }

        public override string ToString()
        {
            return $"Action={ActionType}, Continue={ShouldContinueMeeting}, HealthDelta={CompanyHealthDelta}, Close={ShouldCloseMeeting}, WinBranch={ShouldResolveWinBranch}, RunPipeline={ShouldRunMeetingEndPipeline}, HasSummary={HasSummaryUiState}, Reason={Reason}";
        }
    }
}
