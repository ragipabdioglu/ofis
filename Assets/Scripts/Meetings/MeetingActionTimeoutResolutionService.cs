using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingActionTimeoutResolutionService
    {
        public MeetingActionProposalResolutionResult EvaluateTimeoutHighestVote(
            IReadOnlyList<MeetingActionProposalData> proposals,
            IReadOnlyList<MeetingActionVoteData> votes)
        {
            if (proposals == null || proposals.Count == 0)
                return NoResolution("No action proposals are available for timeout resolution.");

            MeetingActionProposalData highestProposal = default(MeetingActionProposalData);
            int highestVoteCount = 0;
            bool hasHighestProposal = false;
            bool hasTie = false;

            for (int i = 0; i < proposals.Count; i++)
            {
                MeetingActionProposalData proposal = proposals[i];

                if (proposal.Status != MeetingActionProposalStatus.Open)
                    continue;

                int voteCount = CountVotesForProposal(votes, proposal.ProposalId);

                if (voteCount <= 0)
                    continue;

                if (!hasHighestProposal || voteCount > highestVoteCount)
                {
                    highestProposal = proposal;
                    highestVoteCount = voteCount;
                    hasHighestProposal = true;
                    hasTie = false;
                    continue;
                }

                if (voteCount == highestVoteCount)
                    hasTie = true;
            }

            if (!hasHighestProposal)
                return NoResolution("No open proposal has votes at timeout.");

            if (hasTie)
                return NoResolution("Highest vote tie detected; tie cancellation is handled by the next package.");

            return new MeetingActionProposalResolutionResult(
                true,
                MeetingActionProposalResolutionType.TimeoutHighestVote,
                highestProposal.WithStatus(MeetingActionProposalStatus.Resolved),
                0,
                0,
                highestVoteCount,
                "Timeout resolved with highest voted proposal.");
        }

        public MeetingActionProposalResolutionResult ResolveTimeoutHighestVote(
            MeetingActionProposalService proposalService,
            IReadOnlyList<MeetingActionVoteData> votes)
        {
            MeetingActionProposalResolutionResult result = EvaluateTimeoutHighestVote(
                proposalService?.Proposals,
                votes);

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
                "Timeout highest vote resolved and service state was updated.");
        }

        private static MeetingActionProposalResolutionResult NoResolution(string message)
        {
            return new MeetingActionProposalResolutionResult(
                false,
                MeetingActionProposalResolutionType.None,
                default(MeetingActionProposalData),
                0,
                0,
                0,
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
