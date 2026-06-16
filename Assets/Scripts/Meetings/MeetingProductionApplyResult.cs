namespace OFIS.Meetings
{
    public readonly struct MeetingProductionApplyResult
    {
        public MeetingProductionBridgeCommand Command { get; }
        public bool HasCompanyHealthService { get; }
        public bool AppliedCompanyHealthDelta { get; }
        public int CompanyHealthBefore { get; }
        public int CompanyHealthAfter { get; }
        public bool RequestedCloseMeeting { get; }
        public bool RequestedWinBranchResolution { get; }
        public bool RequestedMeetingEndPipeline { get; }
        public bool HasSummaryUiState { get; }
        public MeetingSummaryUiState SummaryUiState { get; }
        public string Message { get; }

        public MeetingProductionApplyResult(
            MeetingProductionBridgeCommand command,
            bool hasCompanyHealthService,
            bool appliedCompanyHealthDelta,
            int companyHealthBefore,
            int companyHealthAfter,
            bool requestedCloseMeeting,
            bool requestedWinBranchResolution,
            bool requestedMeetingEndPipeline,
            bool hasSummaryUiState,
            MeetingSummaryUiState summaryUiState,
            string message)
        {
            Command = command;
            HasCompanyHealthService = hasCompanyHealthService;
            AppliedCompanyHealthDelta = appliedCompanyHealthDelta;
            CompanyHealthBefore = companyHealthBefore < 0 ? 0 : companyHealthBefore;
            CompanyHealthAfter = companyHealthAfter < 0 ? 0 : companyHealthAfter;
            RequestedCloseMeeting = requestedCloseMeeting;
            RequestedWinBranchResolution = requestedWinBranchResolution;
            RequestedMeetingEndPipeline = requestedMeetingEndPipeline;
            HasSummaryUiState = hasSummaryUiState;
            SummaryUiState = summaryUiState;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting production apply result completed."
                : message;
        }

        public override string ToString()
        {
            return $"AppliedHealth={AppliedCompanyHealthDelta}, Health={CompanyHealthBefore}->{CompanyHealthAfter}, Close={RequestedCloseMeeting}, WinBranch={RequestedWinBranchResolution}, Pipeline={RequestedMeetingEndPipeline}, HasSummary={HasSummaryUiState}, Message={Message}";
        }
    }
}
