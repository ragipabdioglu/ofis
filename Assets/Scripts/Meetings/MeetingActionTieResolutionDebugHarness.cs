using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionTieResolutionDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private MeetingActionProposalResolutionType lastResolutionType;
        [SerializeField] private int lastVoteCount;
        [SerializeField] private bool lastProposalACancelled;
        [SerializeField] private bool lastProposalBCancelled;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionTieResolutionService _service =
            new MeetingActionTieResolutionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateTieResolution();
        }

        [ContextMenu("Validate Meeting Action Tie Resolution")]
        public void ValidateTieResolution()
        {
            ValidateHighestVoteTieCancelsAction();
            ValidateNoTieDoesNotCancel();
            ValidateNoVotesDoNotCancel();
            ValidateResolveTieMarksTiedProposalsCancelled();
            ValidateClosedProposalIgnoredInTie();
        }

        private void ValidateHighestVoteTieCancelsAction()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = BuildTieVotes(proposalService);

            MeetingActionProposalResolutionResult result =
                _service.EvaluateTieCancel(proposalService.Proposals, voteService.Votes);

            bool passed = !result.HasResolvedProposal
                && result.ResolutionType == MeetingActionProposalResolutionType.TieCancelled
                && result.VoteCount == 1;

            LogResult("HighestVoteTieCancelsAction", passed, result, proposalService);
        }

        private void ValidateNoTieDoesNotCancel()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = new MeetingActionVoteService();
            SubmitVote(voteService, proposalService, "vote_tie_001", "player_01", "proposal_tie_a");
            SubmitVote(voteService, proposalService, "vote_tie_002", "player_02", "proposal_tie_a");
            SubmitVote(voteService, proposalService, "vote_tie_003", "player_03", "proposal_tie_b");

            MeetingActionProposalResolutionResult result =
                _service.EvaluateTieCancel(proposalService.Proposals, voteService.Votes);

            bool passed = result.ResolutionType == MeetingActionProposalResolutionType.None;
            LogResult("NoTieDoesNotCancel", passed, result, proposalService);
        }

        private void ValidateNoVotesDoNotCancel()
        {
            MeetingActionProposalService proposalService = BuildProposalService();

            MeetingActionProposalResolutionResult result =
                _service.EvaluateTieCancel(
                    proposalService.Proposals,
                    new MeetingActionVoteData[0]);

            bool passed = result.ResolutionType == MeetingActionProposalResolutionType.None;
            LogResult("NoVotesDoNotCancel", passed, result, proposalService);
        }

        private void ValidateResolveTieMarksTiedProposalsCancelled()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = BuildTieVotes(proposalService);

            MeetingActionProposalResolutionResult result =
                _service.ResolveTieCancel(proposalService, voteService.Votes);

            bool proposalACancelled = HasStatus(
                proposalService,
                "proposal_tie_a",
                MeetingActionProposalStatus.Cancelled);

            bool proposalBCancelled = HasStatus(
                proposalService,
                "proposal_tie_b",
                MeetingActionProposalStatus.Cancelled);

            bool passed = result.ResolutionType == MeetingActionProposalResolutionType.TieCancelled
                && proposalACancelled
                && proposalBCancelled;

            LogResult("ResolveTieMarksTiedProposalsCancelled", passed, result, proposalService);
        }

        private void ValidateClosedProposalIgnoredInTie()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionProposalData updatedProposal;
            proposalService.TryUpdateProposalStatus(
                "proposal_tie_b",
                MeetingActionProposalStatus.Resolved,
                out updatedProposal);

            MeetingActionVoteService voteService = BuildTieVotes(proposalService);

            MeetingActionProposalResolutionResult result =
                _service.EvaluateTieCancel(proposalService.Proposals, voteService.Votes);

            bool passed = result.ResolutionType == MeetingActionProposalResolutionType.None;
            LogResult("ClosedProposalIgnoredInTie", passed, result, proposalService);
        }

        private static MeetingActionProposalService BuildProposalService()
        {
            MeetingActionProposalService proposalService =
                new MeetingActionProposalService();

            proposalService.CreateProposal(
                "meeting_tie_debug",
                BuildRequest("proposal_tie_a", OfficeRoomType.ArchiveRoom));

            proposalService.CreateProposal(
                "meeting_tie_debug",
                BuildRequest("proposal_tie_b", OfficeRoomType.SecurityRoom));

            return proposalService;
        }

        private static MeetingActionVoteService BuildTieVotes(
            MeetingActionProposalService proposalService)
        {
            MeetingActionVoteService voteService = new MeetingActionVoteService();
            SubmitVote(voteService, proposalService, "vote_tie_a", "player_01", "proposal_tie_a");
            SubmitVote(voteService, proposalService, "vote_tie_b", "player_02", "proposal_tie_b");
            return voteService;
        }

        private static void SubmitVote(
            MeetingActionVoteService voteService,
            MeetingActionProposalService proposalService,
            string voteId,
            string voterId,
            string proposalId)
        {
            voteService.SubmitVote(
                new MeetingActionVoteData(
                    voteId,
                    voterId,
                    proposalId,
                    "Tie resolution debug vote."),
                proposalService.Proposals);
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
                "Tie debug proposal.");
        }

        private static bool HasStatus(
            MeetingActionProposalService proposalService,
            string proposalId,
            MeetingActionProposalStatus status)
        {
            MeetingActionProposalData proposal;
            return proposalService.TryGetProposal(proposalId, out proposal)
                && proposal.Status == status;
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionProposalResolutionResult result,
            MeetingActionProposalService proposalService)
        {
            lastResolutionType = result.ResolutionType;
            lastVoteCount = result.VoteCount;
            lastProposalACancelled = HasStatus(
                proposalService,
                "proposal_tie_a",
                MeetingActionProposalStatus.Cancelled);
            lastProposalBCancelled = HasStatus(
                proposalService,
                "proposal_tie_b",
                MeetingActionProposalStatus.Cancelled);
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionTieResolutionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionTieResolutionValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
