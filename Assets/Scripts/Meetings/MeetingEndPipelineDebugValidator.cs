using System.Collections.Generic;
using OFIS.Rooms;
using OFIS.Sabotage;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingEndPipelineDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private MeetingSummaryHudStub hudStub;

        private readonly MeetingEndPipelineService _pipelineService = new MeetingEndPipelineService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMeetingEndPipeline();
        }

        [ContextMenu("Validate Meeting End Pipeline")]
        public void ValidateMeetingEndPipeline()
        {
            ValidateNoVotesPipeline();
            ValidateCorrectAccusationPipeline();
            ValidateWrongAccusationPipeline();
            ValidateTiePipeline();
        }

        private void ValidateNoVotesPipeline()
        {
            MeetingEndPipelineResult result = _pipelineService.Run(
                new List<MeetingReportData>(),
                new List<MeetingVoteData>(),
                BuildCulprits("culprit_01"));

            bool passed = !result.IsResolved
                && result.VoteEvaluationResult.HasVotes == false
                && result.DeductionResult.OutcomeType == DeductionOutcomeType.NoVotes
                && result.SummaryUiState.HasDeductionResult;

            LogResult("NoVotesPipeline", passed, result);
        }

        private void ValidateCorrectAccusationPipeline()
        {
            List<MeetingReportData> reports = BuildSampleReports(2, "culprit_01");
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_001", "player_01", "culprit_01", "Reason 1."),
                new MeetingVoteData("vote_002", "player_02", "culprit_01", "Reason 2."),
                new MeetingVoteData("vote_003", "player_03", "player_04", "Reason 3.")
            };

            MeetingEndPipelineResult result = _pipelineService.Run(reports, votes, BuildCulprits("culprit_01"));

            bool passed = result.IsResolved
                && result.VoteEvaluationResult.HasWinner
                && result.DeductionResult.OutcomeType == DeductionOutcomeType.CorrectAccusation
                && result.SummaryUiState.HasReports
                && result.SummaryUiState.HasVoteResult;

            PushHudState(result.SummaryUiState);
            LogResult("CorrectAccusationPipeline", passed, result);
        }

        private void ValidateWrongAccusationPipeline()
        {
            List<MeetingReportData> reports = BuildSampleReports(1, "player_04");
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_004", "player_01", "player_04", "Reason 1."),
                new MeetingVoteData("vote_005", "player_02", "player_04", "Reason 2."),
                new MeetingVoteData("vote_006", "player_03", "culprit_01", "Reason 3.")
            };

            MeetingEndPipelineResult result = _pipelineService.Run(reports, votes, BuildCulprits("culprit_01"));

            bool passed = result.IsResolved
                && result.VoteEvaluationResult.HasWinner
                && result.DeductionResult.OutcomeType == DeductionOutcomeType.WrongAccusation
                && result.SummaryUiState.HasReports
                && result.SummaryUiState.HasVoteResult;

            LogResult("WrongAccusationPipeline", passed, result);
        }

        private void ValidateTiePipeline()
        {
            List<MeetingReportData> reports = BuildSampleReports(2, "culprit_01");
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_007", "player_01", "culprit_01", "Reason 1."),
                new MeetingVoteData("vote_008", "player_02", "player_04", "Reason 2.")
            };

            MeetingEndPipelineResult result = _pipelineService.Run(reports, votes, BuildCulprits("culprit_01"));

            bool passed = !result.IsResolved
                && result.VoteEvaluationResult.IsTie
                && result.DeductionResult.OutcomeType == DeductionOutcomeType.Tie
                && result.SummaryUiState.HasReports
                && result.SummaryUiState.HasVoteResult;

            LogResult("TiePipeline", passed, result);
        }

        private static List<MeetingReportData> BuildSampleReports(int count, string targetPlayerId)
        {
            List<MeetingReportData> reports = new List<MeetingReportData>();

            for (int i = 0; i < count; i++)
            {
                reports.Add(new MeetingReportData(
                    $"report_{i + 1}",
                    MeetingReportType.Suspicion,
                    $"player_{i + 1}",
                    targetPlayerId,
                    OfficeRoomType.MeetingRoom,
                    0,
                    0,
                    SabotageObjectiveState.None,
                    "Suspicious behavior."));
            }

            return reports;
        }

        private static HashSet<string> BuildCulprits(params string[] culpritIds)
        {
            return new HashSet<string>(culpritIds);
        }

        private void PushHudState(MeetingSummaryUiState state)
        {
            if (hudStub != null)
                hudStub.SetState(state);
        }

        private static void LogResult(string testName, bool passed, MeetingEndPipelineResult result)
        {
            if (passed)
                Debug.Log($"[MeetingEndPipelineValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingEndPipelineValidator] FAIL {testName}: {result}");
        }
    }
}
