using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingSummaryUiStateService
    {
        public MeetingSummaryUiState BuildSummary(
            IReadOnlyList<MeetingReportData> reports,
            MeetingVoteEvaluationResult voteResult,
            DeductionResult deductionResult)
        {
            int reportCount = reports == null ? 0 : reports.Count;
            bool hasReports = reportCount > 0;
            bool hasVoteResult = voteResult.HasVotes;
            bool hasDeductionResult = deductionResult.OutcomeType != DeductionOutcomeType.None;

            string reportSummary = BuildReportSummary(reportCount);
            string voteSummary = BuildVoteSummary(voteResult);
            string deductionSummary = BuildDeductionSummary(deductionResult);
            string hint = BuildActionHint(deductionResult);

            return new MeetingSummaryUiState(
                "Meeting Summary",
                reportSummary,
                voteSummary,
                deductionSummary,
                hint,
                hasReports,
                hasVoteResult,
                hasDeductionResult);
        }

        private static string BuildReportSummary(int reportCount)
        {
            if (reportCount <= 0)
                return "No reports submitted.";

            if (reportCount == 1)
                return "1 report submitted.";

            return $"{reportCount} reports submitted.";
        }

        private static string BuildVoteSummary(MeetingVoteEvaluationResult voteResult)
        {
            if (!voteResult.HasVotes)
                return "No votes submitted.";

            if (voteResult.IsTie)
                return $"Vote tied with {voteResult.WinnerVoteCount} vote(s).";

            if (voteResult.HasWinner)
                return $"Most voted: {voteResult.WinnerPlayerId} with {voteResult.WinnerVoteCount} vote(s).";

            return "Vote result unresolved.";
        }

        private static string BuildDeductionSummary(DeductionResult deductionResult)
        {
            switch (deductionResult.OutcomeType)
            {
                case DeductionOutcomeType.NoVotes:
                    return "No accusation was made.";

                case DeductionOutcomeType.Tie:
                    return "Accusation unresolved because vote was tied.";

                case DeductionOutcomeType.CorrectAccusation:
                    return $"Correct accusation: {deductionResult.AccusedPlayerId}.";

                case DeductionOutcomeType.WrongAccusation:
                    return $"Wrong accusation: {deductionResult.AccusedPlayerId}.";

                case DeductionOutcomeType.InvalidTarget:
                    return "Accusation target was invalid.";

                default:
                    return "No deduction result.";
            }
        }

        private static string BuildActionHint(DeductionResult deductionResult)
        {
            if (deductionResult.OutcomeType == DeductionOutcomeType.CorrectAccusation)
                return "Continue deduction.";

            if (deductionResult.OutcomeType == DeductionOutcomeType.WrongAccusation)
                return "Review evidence again.";

            if (deductionResult.OutcomeType == DeductionOutcomeType.Tie)
                return "Discuss and vote again.";

            if (deductionResult.OutcomeType == DeductionOutcomeType.NoVotes)
                return "Submit stronger reports.";

            return "Continue.";
        }
    }
}
