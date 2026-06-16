using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionTimeoutResolutionDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastProposalId;
        [SerializeField] private MeetingActionProposalResolutionType lastResolutionType;
        [SerializeField] private int lastVoteCount;
        [SerializeField] private bool lastHasResolvedProposal;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionTimeoutResolutionService _service =
            new MeetingActionTimeoutResolutionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateTimeoutResolution();
        }

        [ContextMenu("Validate Meeting Action Timeout Resolution")]
        public void ValidateTimeoutResolution()
        {
            ValidateHighestVoteResolvesOnTimeout();
            ValidateNoVotesDoNotResolve();
            ValidateClosedProposalIsIgnored();
            ValidateTieDoesNotResolveInThisPackage();
            ValidateResolveTimeoutUpdatesProposalService();
        }

        private void ValidateHighestVoteResolvesOnTimeout()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = new MeetingActionVoteService();
            SubmitVote(voteService, proposalService, "vote_timeout_001", "player_01", "proposal_timeout_a");
            SubmitVote(voteService, proposalService, "vote_timeout_002", "player_02", "proposal_timeout_a");
            SubmitVote(voteService, proposalService, "vote_timeout_003", "player_03", "proposal_timeout_b");

            MeetingActionProposalResolutionResult result =
                _service.EvaluateTimeoutHighestVote(
                    proposalService.Proposals,
                    voteService.Votes);

            bool passed = result.HasResolvedProposal
                && result.ResolutionType == MeetingActionProposalResolutionType.TimeoutHighestVote
                && result.Proposal.ProposalId == "proposal_timeout_a"
                && result.VoteCount == 2;

            LogResult("HighestVoteResolvesOnTimeout", passed, result);
        }

        private void ValidateNoVotesDoNotResolve()
        {
            MeetingActionProposalService proposalService = BuildProposalService();

            MeetingActionProposalResolutionResult result =
                _service.EvaluateTimeoutHighestVote(
                    proposalService.Proposals,
                    new MeetingActionVoteData[0]);

            bool passed = !result.HasResolvedProposal;
            LogResult("NoVotesDoNotResolve", passed, result);
        }

        private void ValidateClosedProposalIsIgnored()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionProposalData updatedProposal;
            proposalService.TryUpdateProposalStatus(
                "proposal_timeout_a",
                MeetingActionProposalStatus.Resolved,
                out updatedProposal);

            MeetingActionVoteService voteService = new MeetingActionVoteService();
            SubmitVote(voteService, proposalService, "vote_timeout_004", "player_01", "proposal_timeout_b");

            MeetingActionProposalResolutionResult result =
                _service.EvaluateTimeoutHighestVote(
                    proposalService.Proposals,
                    voteService.Votes);

            bool passed = result.HasResolvedProposal
                && result.Proposal.ProposalId == "proposal_timeout_b";

            LogResult("ClosedProposalIsIgnored", passed, result);
        }

        private void ValidateTieDoesNotResolveInThisPackage()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = new MeetingActionVoteService();
            SubmitVote(voteService, proposalService, "vote_timeout_005", "player_01", "proposal_timeout_a");
            SubmitVote(voteService, proposalService, "vote_timeout_006", "player_02", "proposal_timeout_b");

            MeetingActionProposalResolutionResult result =
                _service.EvaluateTimeoutHighestVote(
                    proposalService.Proposals,
                    voteService.Votes);

            bool passed = !result.HasResolvedProposal
                && result.ResolutionType == MeetingActionProposalResolutionType.None;

            LogResult("TieDoesNotResolveInThisPackage", passed, result);
        }

        private void ValidateResolveTimeoutUpdatesProposalService()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = new MeetingActionVoteService();
            SubmitVote(voteService, proposalService, "vote_timeout_007", "player_01", "proposal_timeout_a");
            SubmitVote(voteService, proposalService, "vote_timeout_008", "player_02", "proposal_timeout_a");

            MeetingActionProposalResolutionResult result =
                _service.ResolveTimeoutHighestVote(
                    proposalService,
                    voteService.Votes);

            MeetingActionProposalData storedProposal;
            bool hasStoredProposal = proposalService.TryGetProposal(
                "proposal_timeout_a",
                out storedProposal);

            bool passed = result.HasResolvedProposal
                && hasStoredProposal
                && storedProposal.Status == MeetingActionProposalStatus.Resolved;

            LogResult("ResolveTimeoutUpdatesProposalService", passed, result);
        }

        private static MeetingActionProposalService BuildProposalService()
        {
            MeetingActionProposalService proposalService =
                new MeetingActionProposalService();

            proposalService.CreateProposal(
                "meeting_timeout_debug",
                BuildRequest("proposal_timeout_a", OfficeRoomType.ArchiveRoom));

            proposalService.CreateProposal(
                "meeting_timeout_debug",
                BuildRequest("proposal_timeout_b", OfficeRoomType.SecurityRoom));

            return proposalService;
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
                    "Timeout resolution debug vote."),
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
                "Timeout debug proposal.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionProposalResolutionResult result)
        {
            lastProposalId = result.Proposal.ProposalId;
            lastResolutionType = result.ResolutionType;
            lastVoteCount = result.VoteCount;
            lastHasResolvedProposal = result.HasResolvedProposal;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionTimeoutResolutionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionTimeoutResolutionValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
