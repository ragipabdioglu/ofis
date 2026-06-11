using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingVoteDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly MeetingVoteService _voteService = new MeetingVoteService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMeetingVoteCore();
        }

        [ContextMenu("Validate Meeting Vote Core")]
        public void ValidateMeetingVoteCore()
        {
            ValidateSubmitVoteSuccess();
            ValidateMissingVoterFails();
            ValidateMissingTargetFails();
            ValidateSelfVoteFails();
            ValidateDuplicateVoteFails();
            ValidateVoteCountForTarget();
        }

        private void ValidateSubmitVoteSuccess()
        {
            _voteService.ClearVotes();

            MeetingVoteData vote = new MeetingVoteData("vote_001", "player_01", "player_02", "Suspicious movement.");
            MeetingVoteSubmitResult result = _voteService.SubmitVote(vote);
            bool passed = result.Success && _voteService.VoteCount == 1 && _voteService.HasVoteFrom("player_01");

            LogResult("SubmitVoteSuccess", passed, result);
        }

        private void ValidateMissingVoterFails()
        {
            _voteService.ClearVotes();

            MeetingVoteData vote = new MeetingVoteData("vote_002", "", "player_02", "Missing voter.");
            MeetingVoteSubmitResult result = _voteService.SubmitVote(vote);
            bool passed = !result.Success && _voteService.VoteCount == 0;

            LogResult("MissingVoterFails", passed, result);
        }

        private void ValidateMissingTargetFails()
        {
            _voteService.ClearVotes();

            MeetingVoteData vote = new MeetingVoteData("vote_003", "player_01", "", "Missing target.");
            MeetingVoteSubmitResult result = _voteService.SubmitVote(vote);
            bool passed = !result.Success && _voteService.VoteCount == 0;

            LogResult("MissingTargetFails", passed, result);
        }

        private void ValidateSelfVoteFails()
        {
            _voteService.ClearVotes();

            MeetingVoteData vote = new MeetingVoteData("vote_004", "player_01", "player_01", "Self vote.");
            MeetingVoteSubmitResult result = _voteService.SubmitVote(vote);
            bool passed = !result.Success && _voteService.VoteCount == 0;

            LogResult("SelfVoteFails", passed, result);
        }

        private void ValidateDuplicateVoteFails()
        {
            _voteService.ClearVotes();

            MeetingVoteData firstVote = new MeetingVoteData("vote_005", "player_01", "player_02", "First vote.");
            MeetingVoteData secondVote = new MeetingVoteData("vote_006", "player_01", "player_03", "Second vote.");

            MeetingVoteSubmitResult firstResult = _voteService.SubmitVote(firstVote);
            MeetingVoteSubmitResult secondResult = _voteService.SubmitVote(secondVote);
            bool passed = firstResult.Success && !secondResult.Success && _voteService.VoteCount == 1;

            LogResult("DuplicateVoteFails", passed, secondResult);
        }

        private void ValidateVoteCountForTarget()
        {
            _voteService.ClearVotes();

            _voteService.SubmitVote(new MeetingVoteData("vote_007", "player_01", "player_04", "Reason 1."));
            _voteService.SubmitVote(new MeetingVoteData("vote_008", "player_02", "player_04", "Reason 2."));
            _voteService.SubmitVote(new MeetingVoteData("vote_009", "player_03", "player_05", "Reason 3."));

            MeetingVoteCountResult countResult = _voteService.GetVoteCountForTarget("player_04");
            bool passed = countResult.VoteCount == 2;

            LogCountResult("VoteCountForTarget", passed, countResult);
        }

        private static void LogResult(string testName, bool passed, MeetingVoteSubmitResult result)
        {
            if (passed)
                Debug.Log($"[MeetingVoteValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingVoteValidator] FAIL {testName}: {result}");
        }

        private static void LogCountResult(string testName, bool passed, MeetingVoteCountResult result)
        {
            if (passed)
                Debug.Log($"[MeetingVoteValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingVoteValidator] FAIL {testName}: {result}");
        }
    }
}
