using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionProposalResolutionDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastProposalId;
        [SerializeField] private MeetingActionProposalResolutionType lastResolutionType;
        [SerializeField] private int lastRequiredVotes;
        [SerializeField] private int lastVoteCount;
        [SerializeField] private bool lastHasResolvedProposal;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionProposalResolutionService _resolutionService =
            new MeetingActionProposalResolutionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateProposalResolution();
        }

        [ContextMenu("Validate Meeting Action Proposal Resolution")]
        public void ValidateProposalResolution()
        {
            ValidateMajorityResolvesOpenProposal();
            ValidateBelowThresholdDoesNotResolve();
            ValidateClosedProposalIsIgnored();
            ValidateResolveMajorityUpdatesProposalService();
            ValidateEmptyEligibleVotersDoNotResolve();
        }

        private void ValidateMajorityResolvesOpenProposal()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = BuildVoteService(
                proposalService,
                "proposal_resolve_a",
                "player_01",
                "player_02");

            MeetingActionProposalResolutionResult result =
                _resolutionService.EvaluateMajority(
                    proposalService.Proposals,
                    voteService.Votes,
                    new[] { "player_01", "player_02", "player_03" });

            bool passed = result.HasResolvedProposal
                && result.Proposal.ProposalId == "proposal_resolve_a"
                && result.Proposal.Status == MeetingActionProposalStatus.Resolved
                && result.RequiredVotes == 2;

            LogResult("MajorityResolvesOpenProposal", passed, result);
        }

        private void ValidateBelowThresholdDoesNotResolve()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = BuildVoteService(
                proposalService,
                "proposal_resolve_a",
                "player_01");

            MeetingActionProposalResolutionResult result =
                _resolutionService.EvaluateMajority(
                    proposalService.Proposals,
                    voteService.Votes,
                    new[] { "player_01", "player_02", "player_03" });

            bool passed = !result.HasResolvedProposal && result.RequiredVotes == 2;
            LogResult("BelowThresholdDoesNotResolve", passed, result);
        }

        private void ValidateClosedProposalIsIgnored()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionProposalData updatedProposal;
            proposalService.TryUpdateProposalStatus(
                "proposal_resolve_a",
                MeetingActionProposalStatus.Cancelled,
                out updatedProposal);

            MeetingActionVoteService voteService = BuildVoteService(
                proposalService,
                "proposal_resolve_b",
                "player_01",
                "player_02");

            MeetingActionProposalResolutionResult result =
                _resolutionService.EvaluateMajority(
                    proposalService.Proposals,
                    voteService.Votes,
                    new[] { "player_01", "player_02", "player_03" });

            bool passed = result.HasResolvedProposal
                && result.Proposal.ProposalId == "proposal_resolve_b";

            LogResult("ClosedProposalIsIgnored", passed, result);
        }

        private void ValidateResolveMajorityUpdatesProposalService()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = BuildVoteService(
                proposalService,
                "proposal_resolve_a",
                "player_01",
                "player_02");

            MeetingActionProposalResolutionResult result =
                _resolutionService.ResolveMajority(
                    proposalService,
                    voteService.Votes,
                    new[] { "player_01", "player_02", "player_03" });

            MeetingActionProposalData storedProposal;
            bool hasStoredProposal = proposalService.TryGetProposal(
                "proposal_resolve_a",
                out storedProposal);

            bool passed = result.HasResolvedProposal
                && hasStoredProposal
                && storedProposal.Status == MeetingActionProposalStatus.Resolved;

            LogResult("ResolveMajorityUpdatesProposalService", passed, result);
        }

        private void ValidateEmptyEligibleVotersDoNotResolve()
        {
            MeetingActionProposalService proposalService = BuildProposalService();
            MeetingActionVoteService voteService = BuildVoteService(
                proposalService,
                "proposal_resolve_a",
                "player_01");

            MeetingActionProposalResolutionResult result =
                _resolutionService.EvaluateMajority(
                    proposalService.Proposals,
                    voteService.Votes,
                    new string[0]);

            bool passed = !result.HasResolvedProposal && result.RequiredVotes == 0;
            LogResult("EmptyEligibleVotersDoNotResolve", passed, result);
        }

        private static MeetingActionProposalService BuildProposalService()
        {
            MeetingActionProposalService proposalService =
                new MeetingActionProposalService();

            proposalService.CreateProposal(
                "meeting_resolution_debug",
                BuildRequest("proposal_resolve_a", OfficeRoomType.ArchiveRoom));

            proposalService.CreateProposal(
                "meeting_resolution_debug",
                BuildRequest("proposal_resolve_b", OfficeRoomType.SecurityRoom));

            return proposalService;
        }

        private static MeetingActionVoteService BuildVoteService(
            MeetingActionProposalService proposalService,
            string proposalId,
            params string[] voterIds)
        {
            MeetingActionVoteService voteService = new MeetingActionVoteService();

            for (int i = 0; i < voterIds.Length; i++)
            {
                voteService.SubmitVote(
                    new MeetingActionVoteData(
                        $"vote_resolution_{proposalId}_{i}",
                        voterIds[i],
                        proposalId,
                        "Resolution debug vote."),
                    proposalService.Proposals);
            }

            return voteService;
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
                "Resolution debug proposal.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionProposalResolutionResult result)
        {
            lastProposalId = result.Proposal.ProposalId;
            lastResolutionType = result.ResolutionType;
            lastRequiredVotes = result.RequiredVotes;
            lastVoteCount = result.VoteCount;
            lastHasResolvedProposal = result.HasResolvedProposal;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionProposalResolutionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionProposalResolutionValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
