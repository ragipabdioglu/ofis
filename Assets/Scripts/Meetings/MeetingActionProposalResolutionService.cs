using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingActionProposalResolutionService
    {
        private readonly MeetingActionMajorityThresholdService _thresholdService;

        public MeetingActionProposalResolutionService()
        {
            _thresholdService = new MeetingActionMajorityThresholdService();
        }

        public MeetingActionProposalResolutionService(
            MeetingActionMajorityThresholdService thresholdService)
        {
            _thresholdService = thresholdService ?? new MeetingActionMajorityThresholdService();
        }

        public MeetingActionProposalResolutionResult EvaluateMajority(
            IReadOnlyList<MeetingActionProposalData> proposals,
            IReadOnlyList<MeetingActionVoteData> votes,
            IReadOnlyList<string> eligibleVoterIds)
        {
            MeetingActionMajorityThresholdResult threshold =
                _thresholdService.Calculate(eligibleVoterIds, 0);

            if (!threshold.HasEligibleVoters)
            {
                return NoResolution(
                    threshold,
                    "No eligible voters are available for majority resolution.");
            }

            if (proposals == null || proposals.Count == 0)
            {
                return NoResolution(
                    threshold,
                    "No action proposals are available for majority resolution.");
            }

            for (int i = 0; i < proposals.Count; i++)
            {
                MeetingActionProposalData proposal = proposals[i];

                if (proposal.Status != MeetingActionProposalStatus.Open)
                    continue;

                int voteCount = CountVotesForProposal(votes, proposal.ProposalId);
                MeetingActionMajorityThresholdResult proposalThreshold =
                    _thresholdService.Calculate(eligibleVoterIds, voteCount);

                if (!proposalThreshold.HasReachedMajority)
                    continue;

                return new MeetingActionProposalResolutionResult(
                    true,
                    MeetingActionProposalResolutionType.MajorityReached,
                    proposal.WithStatus(MeetingActionProposalStatus.Resolved),
                    proposalThreshold.EligibleVoterCount,
                    proposalThreshold.RequiredVotes,
                    proposalThreshold.CurrentVoteCount,
                    "Proposal reached majority and resolved early.");
            }

            return NoResolution(threshold, "No proposal has reached majority.");
        }

        public MeetingActionProposalResolutionResult ResolveMajority(
            MeetingActionProposalService proposalService,
            IReadOnlyList<MeetingActionVoteData> votes,
            IReadOnlyList<string> eligibleVoterIds)
        {
            MeetingActionProposalResolutionResult result = EvaluateMajority(
                proposalService?.Proposals,
                votes,
                eligibleVoterIds);

            if (!result.HasResolvedProposal || proposalService == null)
                return result;

            MeetingActionProposalData updatedProposal;
            bool updated = proposalService.TryUpdateProposalStatus(
                result.Proposal.ProposalId,
                MeetingActionProposalStatus.Resolved,
                out updatedProposal);

            if (!updated)
                return result;

            return new MeetingActionProposalResolutionResult(
                true,
                result.ResolutionType,
                updatedProposal,
                result.EligibleVoterCount,
                result.RequiredVotes,
                result.VoteCount,
                "Proposal reached majority and service state was updated.");
        }

        private static MeetingActionProposalResolutionResult NoResolution(
            MeetingActionMajorityThresholdResult threshold,
            string message)
        {
            return new MeetingActionProposalResolutionResult(
                false,
                MeetingActionProposalResolutionType.None,
                default(MeetingActionProposalData),
                threshold.EligibleVoterCount,
                threshold.RequiredVotes,
                threshold.CurrentVoteCount,
                message);
        }

        private static int CountVotesForProposal(
            IReadOnlyList<MeetingActionVoteData> votes,
            string proposalId)
        {
            int count = 0;

            if (votes == null || string.IsNullOrWhiteSpace(proposalId))
                return count;

            for (int i = 0; i < votes.Count; i++)
            {
                if (votes[i].ProposalId == proposalId)
                    count++;
            }

            return count;
        }
    }
}
