using OFIS.Core.Events;

namespace OFIS.Meetings
{
    public sealed class MeetingProductionRuntimeEvent : IGameEvent
    {
        public float CreatedAtRealtime { get; }
        public MeetingProductionBridgeActionType ActionType { get; }
        public bool AppliedCompanyHealthDelta { get; }
        public int CompanyHealthBefore { get; }
        public int CompanyHealthAfter { get; }
        public bool RequestedCloseMeeting { get; }
        public bool RequestedWinBranchResolution { get; }
        public bool RequestedMeetingEndPipeline { get; }
        public bool HasSummaryUiState { get; }
        public MeetingSummaryUiState SummaryUiState { get; }
        public string Message { get; }

        public MeetingProductionRuntimeEvent(
            MeetingProductionApplyResult applyResult,
            float createdAtRealtime)
        {
            CreatedAtRealtime = createdAtRealtime;
            ActionType = applyResult.Command.ActionType;
            AppliedCompanyHealthDelta = applyResult.AppliedCompanyHealthDelta;
            CompanyHealthBefore = applyResult.CompanyHealthBefore;
            CompanyHealthAfter = applyResult.CompanyHealthAfter;
            RequestedCloseMeeting = applyResult.RequestedCloseMeeting;
            RequestedWinBranchResolution = applyResult.RequestedWinBranchResolution;
            RequestedMeetingEndPipeline = applyResult.RequestedMeetingEndPipeline;
            HasSummaryUiState = applyResult.HasSummaryUiState;
            SummaryUiState = applyResult.SummaryUiState;
            Message = string.IsNullOrWhiteSpace(applyResult.Message)
                ? "Meeting production runtime event published."
                : applyResult.Message;
        }

        public override string ToString()
        {
            return $"Action={ActionType}, Health={CompanyHealthBefore}->{CompanyHealthAfter}, Close={RequestedCloseMeeting}, WinBranch={RequestedWinBranchResolution}, Pipeline={RequestedMeetingEndPipeline}, HasSummary={HasSummaryUiState}, Message={Message}";
        }
    }
}
