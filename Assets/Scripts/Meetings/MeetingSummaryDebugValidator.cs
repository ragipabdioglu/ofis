using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingSummaryDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private MeetingSummaryHudStub hudStub;

        private readonly MeetingSummaryUiStateService _summaryService = new MeetingSummaryUiStateService();
        private readonly MeetingVoteEvaluationService _voteEvaluationService = new MeetingVoteEvaluationService();
        private readonly DeductionEvaluationService _deductionEvaluationService = new DeductionEvaluationService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMeetingSummary();
        }

        [ContextMenu("Validate Meeting Summary")]
        public void ValidateMeetingSummary()
        {
            ValidateNoReportsNoVotesSummary();
            ValidateCorrectAccusationSummary();
            ValidateWrongAccusationSummary();
            ValidateTieSummary();
        }

        private void ValidateNoReportsNoVotesSummary()
        {
            List<MeetingReportData> reports = new List<MeetingReportData>();
            MeetingVoteEvaluationResult voteResult = _voteEvaluationService.Evaluate(new List<MeetingVoteData>());
            DeductionResult deductionResult = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));
            MeetingSummaryUiState state = _summaryService.BuildSummary(reports, voteResult, deductionResult);

            bool passed = !state.HasReports && !state.HasVoteResult && state.HasDeductionResult && state.ReportSummaryText == "No reports submitted.";

            LogResult("NoReportsNoVotesSummary", passed, state);
        }

        private void ValidateCorrectAccusationSummary()
        {
            List<MeetingReportData> reports = BuildSampleReports(2);
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_001", "player_01", "killer_01", "Reason 1."),
                new MeetingVoteData("vote_002", "player_02", "killer_01", "Reason 2."),
                new MeetingVoteData("vote_003", "player_03", "player_04", "Reason 3.")
            };

            MeetingVoteEvaluationResult voteResult = _voteEvaluationService.Evaluate(votes);
            DeductionResult deductionResult = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));
            MeetingSummaryUiState state = _summaryService.BuildSummary(reports, voteResult, deductionResult);

            bool passed = state.HasReports && state.HasVoteResult && state.HasDeductionResult && state.DeductionSummaryText.Contains("Correct accusation");

            PushHudState(state);
            LogResult("CorrectAccusationSummary", passed, state);
        }

        private void ValidateWrongAccusationSummary()
        {
            List<MeetingReportData> reports = BuildSampleReports(1);
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_004", "player_01", "player_04", "Reason 1."),
                new MeetingVoteData("vote_005", "player_02", "player_04", "Reason 2."),
                new MeetingVoteData("vote_006", "player_03", "killer_01", "Reason 3.")
            };

            MeetingVoteEvaluationResult voteResult = _voteEvaluationService.Evaluate(votes);
            DeductionResult deductionResult = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));
            MeetingSummaryUiState state = _summaryService.BuildSummary(reports, voteResult, deductionResult);

            bool passed = state.HasReports && state.HasVoteResult && state.HasDeductionResult && state.DeductionSummaryText.Contains("Wrong accusation");

            LogResult("WrongAccusationSummary", passed, state);
        }

        private void ValidateTieSummary()
        {
            List<MeetingReportData> reports = BuildSampleReports(2);
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_007", "player_01", "killer_01", "Reason 1."),
                new MeetingVoteData("vote_008", "player_02", "player_04", "Reason 2.")
            };

            MeetingVoteEvaluationResult voteResult = _voteEvaluationService.Evaluate(votes);
            DeductionResult deductionResult = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));
            MeetingSummaryUiState state = _summaryService.BuildSummary(reports, voteResult, deductionResult);

            bool passed = state.HasReports && state.HasVoteResult && state.HasDeductionResult && state.VoteSummaryText.Contains("tied");

            LogResult("TieSummary", passed, state);
        }

        private static List<MeetingReportData> BuildSampleReports(int count)
        {
            List<MeetingReportData> reports = new List<MeetingReportData>();

            for (int i = 0; i < count; i++)
            {
                reports.Add(new MeetingReportData(
                    $"report_{i + 1}",
                    MeetingReportType.Suspicion,
                    $"player_{i + 1}",
                    "killer_01",
                    Rooms.OfficeRoomType.MeetingRoom,
                    0,
                    0,
                    Sabotage.SabotageObjectiveState.None,
                    "Suspicious behavior."));
            }

            return reports;
        }

        private static HashSet<string> BuildKillers(params string[] killerIds)
        {
            return new HashSet<string>(killerIds);
        }

        private void PushHudState(MeetingSummaryUiState state)
        {
            if (hudStub != null)
                hudStub.SetState(state);
        }

        private static void LogResult(string testName, bool passed, MeetingSummaryUiState state)
        {
            if (passed)
                Debug.Log($"[MeetingSummaryValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[MeetingSummaryValidator] FAIL {testName}: {state}");
        }
    }
}
