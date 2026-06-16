using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionVoteDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastVoteId;
        [SerializeField] private string lastVoterPlayerId;
        [SerializeField] private string lastProposalId;
        [SerializeField] private int lastVoteCount;
        [SerializeField] private bool lastSuccess;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionProposalService _proposalService =
            new MeetingActionProposalService();

        private readonly MeetingActionVoteService _voteService =
            new MeetingActionVoteService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMeetingActionVoteCore();
        }

        [ContextMenu("Validate Meeting Action Vote Core")]
        public void ValidateMeetingActionVoteCore()
        {
            _proposalService.ClearProposals();
            _voteService.ClearVotes();

            CreateProposal("proposal_vote_a", OfficeRoomType.ArchiveRoom);
            CreateProposal("proposal_vote_b", OfficeRoomType.SecurityRoom);

            ValidateSubmitsVoteForOpenProposal();
            ValidateRejectsDuplicateVoter();
            ValidateRejectsMissingProposal();
            ValidateRejectsClosedProposal();
            ValidateCountsProposalVotes();
            ValidateClearVotes();
        }

        private void ValidateSubmitsVoteForOpenProposal()
        {
            MeetingActionVoteSubmitResult result = _voteService.SubmitVote(
                new MeetingActionVoteData(
                    "vote_action_001",
                    "player_01",
                    "proposal_vote_a",
                    "Archive room needs inspection."),
                _proposalService.Proposals);

            bool passed = result.Success && _voteService.HasVoteFrom("player_01");
            LogResult("SubmitsVoteForOpenProposal", passed, result);
        }

        private void ValidateRejectsDuplicateVoter()
        {
            MeetingActionVoteSubmitResult result = _voteService.SubmitVote(
                new MeetingActionVoteData(
                    "vote_action_002",
                    "player_01",
                    "proposal_vote_b",
                    "Second vote attempt."),
                _proposalService.Proposals);

            bool passed = !result.Success
                && _voteService.CountVotesForProposal("proposal_vote_b") == 0;

            LogResult("RejectsDuplicateVoter", passed, result);
        }

        private void ValidateRejectsMissingProposal()
        {
            MeetingActionVoteSubmitResult result = _voteService.SubmitVote(
                new MeetingActionVoteData(
                    "vote_action_003",
                    "player_02",
                    "proposal_missing",
                    "Missing proposal."),
                _proposalService.Proposals);

            bool passed = !result.Success && !_voteService.HasVoteFrom("player_02");
            LogResult("RejectsMissingProposal", passed, result);
        }

        private void ValidateRejectsClosedProposal()
        {
            MeetingActionProposalData closedProposal = new MeetingActionProposalData(
                "proposal_closed",
                "meeting_vote_debug",
                BuildRequest("proposal_closed", OfficeRoomType.MeetingRoom),
                MeetingActionProposalStatus.Resolved);

            MeetingActionVoteSubmitResult result = _voteService.SubmitVote(
                new MeetingActionVoteData(
                    "vote_action_004",
                    "player_03",
                    "proposal_closed",
                    "Closed proposal."),
                new[] { closedProposal });

            bool passed = !result.Success && !_voteService.HasVoteFrom("player_03");
            LogResult("RejectsClosedProposal", passed, result);
        }

        private void ValidateCountsProposalVotes()
        {
            _voteService.SubmitVote(
                new MeetingActionVoteData(
                    "vote_action_005",
                    "player_04",
                    "proposal_vote_a",
                    "Extra support."),
                _proposalService.Proposals);

            MeetingActionVoteCountResult countResult =
                _voteService.GetVoteCountForProposal("proposal_vote_a");

            MeetingActionVoteSubmitResult result = new MeetingActionVoteSubmitResult(
                countResult.VoteCount == 2,
                new MeetingActionVoteData(
                    "vote_count_result",
                    "validator",
                    countResult.ProposalId,
                    countResult.ToString()),
                countResult.ToString());

            LogResult("CountsProposalVotes", result.Success, result);
        }

        private void ValidateClearVotes()
        {
            _voteService.ClearVotes();

            MeetingActionVoteSubmitResult result = new MeetingActionVoteSubmitResult(
                _voteService.VoteCount == 0,
                default(MeetingActionVoteData),
                "Action vote store cleared.");

            LogResult("ClearVotes", result.Success, result);
        }

        private void CreateProposal(string proposalId, OfficeRoomType roomType)
        {
            _proposalService.CreateProposal(
                "meeting_vote_debug",
                BuildRequest(proposalId, roomType));
        }

        private static MeetingActionRequestData BuildRequest(
            string proposalId,
            OfficeRoomType roomType)
        {
            return new MeetingActionRequestData(
                proposalId,
                "player_proposer",
                MeetingActionType.RoomInspection,
                MeetingActionTargetData.ForRoom(roomType),
                "Debug vote proposal.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionVoteSubmitResult result)
        {
            lastVoteId = result.Vote.VoteId;
            lastVoterPlayerId = result.Vote.VoterPlayerId;
            lastProposalId = result.Vote.ProposalId;
            lastVoteCount = _voteService.VoteCount;
            lastSuccess = result.Success;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionVoteValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionVoteValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
