using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class DeductionEvaluationDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly MeetingVoteEvaluationService _voteEvaluationService = new MeetingVoteEvaluationService();
        private readonly DeductionEvaluationService _deductionEvaluationService = new DeductionEvaluationService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateDeductionEvaluation();
        }

        [ContextMenu("Validate Deduction Evaluation")]
        public void ValidateDeductionEvaluation()
        {
            ValidateNoVotesOutcome();
            ValidateTieOutcome();
            ValidateCorrectAccusation();
            ValidateWrongAccusation();
            ValidateInvalidWinnerOutcome();
        }

        private void ValidateNoVotesOutcome()
        {
            MeetingVoteEvaluationResult voteResult = _voteEvaluationService.Evaluate(new List<MeetingVoteData>());
            DeductionResult result = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));

            bool passed = result.OutcomeType == DeductionOutcomeType.NoVotes && !result.IsResolved && !result.IsCorrectAccusation;

            LogResult("NoVotesOutcome", passed, result);
        }

        private void ValidateTieOutcome()
        {
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_001", "player_01", "killer_01", "Reason 1."),
                new MeetingVoteData("vote_002", "player_02", "player_03", "Reason 2.")
            };

            MeetingVoteEvaluationResult voteResult = _voteEvaluationService.Evaluate(votes);
            DeductionResult result = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));

            bool passed = result.OutcomeType == DeductionOutcomeType.Tie && !result.IsResolved && result.TiedPlayerIds.Count == 2;

            LogResult("TieOutcome", passed, result);
        }

        private void ValidateCorrectAccusation()
        {
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_003", "player_01", "killer_01", "Reason 1."),
                new MeetingVoteData("vote_004", "player_02", "killer_01", "Reason 2."),
                new MeetingVoteData("vote_005", "player_03", "player_04", "Reason 3.")
            };

            MeetingVoteEvaluationResult voteResult = _voteEvaluationService.Evaluate(votes);
            DeductionResult result = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));

            bool passed = result.OutcomeType == DeductionOutcomeType.CorrectAccusation && result.IsResolved && result.IsCorrectAccusation && result.AccusedPlayerId == "killer_01";

            LogResult("CorrectAccusation", passed, result);
        }

        private void ValidateWrongAccusation()
        {
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_006", "player_01", "player_04", "Reason 1."),
                new MeetingVoteData("vote_007", "player_02", "player_04", "Reason 2."),
                new MeetingVoteData("vote_008", "player_03", "killer_01", "Reason 3.")
            };

            MeetingVoteEvaluationResult voteResult = _voteEvaluationService.Evaluate(votes);
            DeductionResult result = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));

            bool passed = result.OutcomeType == DeductionOutcomeType.WrongAccusation && result.IsResolved && !result.IsCorrectAccusation && result.AccusedPlayerId == "player_04";

            LogResult("WrongAccusation", passed, result);
        }

        private void ValidateInvalidWinnerOutcome()
        {
            MeetingVoteEvaluationResult voteResult = new MeetingVoteEvaluationResult(
                true,
                false,
                false,
                "none",
                0,
                new List<string>(),
                "Invalid manual result.");

            DeductionResult result = _deductionEvaluationService.Evaluate(voteResult, BuildKillers("killer_01"));

            bool passed = result.OutcomeType == DeductionOutcomeType.InvalidTarget && !result.IsResolved;

            LogResult("InvalidWinnerOutcome", passed, result);
        }

        private static HashSet<string> BuildKillers(params string[] killerIds)
        {
            return new HashSet<string>(killerIds);
        }

        private static void LogResult(string testName, bool passed, DeductionResult result)
        {
            if (passed)
                Debug.Log($"[DeductionEvaluationValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[DeductionEvaluationValidator] FAIL {testName}: {result}");
        }
    }
}
