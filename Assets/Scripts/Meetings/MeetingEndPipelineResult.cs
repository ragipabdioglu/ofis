namespace OFIS.Meetings
{
    public readonly struct MeetingEndPipelineResult
    {
        public MeetingVoteEvaluationResult VoteEvaluationResult { get; }
        public DeductionResult DeductionResult { get; }
        public MeetingSummaryUiState SummaryUiState { get; }
        public bool IsResolved { get; }
        public string Message { get; }

        public MeetingEndPipelineResult(
            MeetingVoteEvaluationResult voteEvaluationResult,
            DeductionResult deductionResult,
            MeetingSummaryUiState summaryUiState,
            bool isResolved,
            string message)
        {
            VoteEvaluationResult = voteEvaluationResult;
            DeductionResult = deductionResult;
            SummaryUiState = summaryUiState;
            IsResolved = isResolved;
            Message = string.IsNullOrWhiteSpace(message) ? "Meeting end pipeline completed." : message;
        }

        public override string ToString()
        {
            return $"Resolved={IsResolved}, Vote=[{VoteEvaluationResult}], Deduction=[{DeductionResult}], Summary=[{SummaryUiState}], Message={Message}";
        }
    }
}
