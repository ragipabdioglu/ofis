namespace OFIS.Meetings
{
    public readonly struct MeetingSummaryUiState
    {
        public string HeaderText { get; }
        public string ReportSummaryText { get; }
        public string VoteSummaryText { get; }
        public string DeductionSummaryText { get; }
        public string ActionHintText { get; }
        public bool HasReports { get; }
        public bool HasVoteResult { get; }
        public bool HasDeductionResult { get; }

        public MeetingSummaryUiState(
            string headerText,
            string reportSummaryText,
            string voteSummaryText,
            string deductionSummaryText,
            string actionHintText,
            bool hasReports,
            bool hasVoteResult,
            bool hasDeductionResult)
        {
            HeaderText = string.IsNullOrWhiteSpace(headerText) ? "Meeting Summary" : headerText;
            ReportSummaryText = string.IsNullOrWhiteSpace(reportSummaryText) ? "No reports." : reportSummaryText;
            VoteSummaryText = string.IsNullOrWhiteSpace(voteSummaryText) ? "No vote result." : voteSummaryText;
            DeductionSummaryText = string.IsNullOrWhiteSpace(deductionSummaryText) ? "No deduction result." : deductionSummaryText;
            ActionHintText = string.IsNullOrWhiteSpace(actionHintText) ? "Continue" : actionHintText;
            HasReports = hasReports;
            HasVoteResult = hasVoteResult;
            HasDeductionResult = hasDeductionResult;
        }

        public override string ToString()
        {
            return $"Header={HeaderText}, Reports={ReportSummaryText}, Vote={VoteSummaryText}, Deduction={DeductionSummaryText}, Hint={ActionHintText}";
        }
    }
}
