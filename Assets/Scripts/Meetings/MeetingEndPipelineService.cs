using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingEndPipelineService
    {
        private readonly MeetingVoteEvaluationService _voteEvaluationService;
        private readonly DeductionEvaluationService _deductionEvaluationService;
        private readonly MeetingSummaryUiStateService _summaryUiStateService;

        public MeetingEndPipelineService()
        {
            _voteEvaluationService = new MeetingVoteEvaluationService();
            _deductionEvaluationService = new DeductionEvaluationService();
            _summaryUiStateService = new MeetingSummaryUiStateService();
        }

        public MeetingEndPipelineResult Run(
            IReadOnlyList<MeetingReportData> reports,
            IReadOnlyList<MeetingVoteData> votes,
            IReadOnlyCollection<string> culpritPlayerIds)
        {
            MeetingVoteEvaluationResult voteEvaluationResult = _voteEvaluationService.Evaluate(votes);
            DeductionResult deductionResult = _deductionEvaluationService.Evaluate(voteEvaluationResult, culpritPlayerIds);
            MeetingSummaryUiState summaryUiState = _summaryUiStateService.BuildSummary(reports, voteEvaluationResult, deductionResult);

            return new MeetingEndPipelineResult(
                voteEvaluationResult,
                deductionResult,
                summaryUiState,
                deductionResult.IsResolved,
                "Meeting end pipeline completed.");
        }
    }
}
