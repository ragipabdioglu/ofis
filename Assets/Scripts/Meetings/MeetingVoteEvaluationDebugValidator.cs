using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingVoteEvaluationDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly MeetingVoteService _voteService = new MeetingVoteService();
        private readonly MeetingVoteEvaluationService _evaluationService = new MeetingVoteEvaluationService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMeetingVoteEvaluation();
        }

        [ContextMenu("Validate Meeting Vote Evaluation")]
        public void ValidateMeetingVoteEvaluation()
        {
            ValidateNoVotesResult();
            ValidateSingleWinnerResult();
            ValidateTieResult();
            ValidateInvalidTargetIgnored();
            ValidateServiceVotesEvaluation();
        }

        private void ValidateNoVotesResult()
        {
            MeetingVoteEvaluationResult result = _evaluationService.Evaluate(new List<MeetingVoteData>());
            bool passed = !result.HasVotes && !result.HasWinner && !result.IsTie;

            LogResult("NoVotesResult", passed, result);
        }

        private void ValidateSingleWinnerResult()
        {
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_001", "player_01", "player_04", "Reason 1."),
                new MeetingVoteData("vote_002", "player_02", "player_04", "Reason 2."),
                new MeetingVoteData("vote_003", "player_03", "player_05", "Reason 3.")
            };

            MeetingVoteEvaluationResult result = _evaluationService.Evaluate(votes);
            bool passed = result.HasVotes && result.HasWinner && !result.IsTie && result.WinnerPlayerId == "player_04" && result.WinnerVoteCount == 2;

            LogResult("SingleWinnerResult", passed, result);
        }

        private void ValidateTieResult()
        {
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_004", "player_01", "player_04", "Reason 1."),
                new MeetingVoteData("vote_005", "player_02", "player_05", "Reason 2.")
            };

            MeetingVoteEvaluationResult result = _evaluationService.Evaluate(votes);
            bool passed = result.HasVotes && !result.HasWinner && result.IsTie && result.WinnerVoteCount == 1 && result.TiedPlayerIds.Count == 2;

            LogResult("TieResult", passed, result);
        }

        private void ValidateInvalidTargetIgnored()
        {
            List<MeetingVoteData> votes = new List<MeetingVoteData>
            {
                new MeetingVoteData("vote_006", "player_01", "", "Invalid target."),
                new MeetingVoteData("vote_007", "player_02", "player_05", "Valid target.")
            };

            MeetingVoteEvaluationResult result = _evaluationService.Evaluate(votes);
            bool passed = result.HasVotes && result.HasWinner && result.WinnerPlayerId == "player_05" && result.WinnerVoteCount == 1;

            LogResult("InvalidTargetIgnored", passed, result);
        }

        private void ValidateServiceVotesEvaluation()
        {
            _voteService.ClearVotes();

            _voteService.SubmitVote(new MeetingVoteData("vote_008", "player_01", "player_04", "Reason 1."));
            _voteService.SubmitVote(new MeetingVoteData("vote_009", "player_02", "player_04", "Reason 2."));
            _voteService.SubmitVote(new MeetingVoteData("vote_010", "player_03", "player_05", "Reason 3."));

            MeetingVoteEvaluationResult result = _evaluationService.Evaluate(_voteService.Votes);
            bool passed = result.HasWinner && result.WinnerPlayerId == "player_04" && result.WinnerVoteCount == 2;

            LogResult("ServiceVotesEvaluation", passed, result);
        }

        private static void LogResult(string testName, bool passed, MeetingVoteEvaluationResult result)
        {
            if (passed)
                Debug.Log($"[MeetingVoteEvaluationValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingVoteEvaluationValidator] FAIL {testName}: {result}");
        }
    }
}
