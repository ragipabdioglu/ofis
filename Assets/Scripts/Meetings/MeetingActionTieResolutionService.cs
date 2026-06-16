using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingActionTieResolutionService
    {
        public MeetingActionProposalResolutionResult EvaluateTieCancel(
            IReadOnlyList<MeetingActionProposalData> proposals,
            IReadOnlyList<MeetingActionVoteData> votes)
        {
            int highestVoteCount;
            int tiedProposalCount = CountHighestVoteTies(
                proposals,
                votes,
                out highestVoteCount);

            if (highestVoteCount <= 0)
                return NoResolution("No voted proposals are available for tie cancellation.");

            if (tiedProposalCount < 2)
                return NoResolution("No highest vote tie detected.");

            return new MeetingActionProposalResolutionResult(
                false,
                MeetingActionProposalResolutionType.TieCancelled,
                default(MeetingActionProposalData),
                0,
                0,
                highestVoteCount,
                "Highest vote tie detected; official action cancelled.");
        }

        public MeetingActionProposalResolutionResult ResolveTieCancel(
            MeetingActionProposalService proposalService,
            IReadOnlyList<MeetingActionVoteData> votes)
        {
            MeetingActionProposalResolutionResult result = EvaluateTieCancel(
                proposalService?.Proposals,
                votes);

            if (result.ResolutionType != MeetingActionProposalResolutionType.TieCancelled
                || proposalService == null)
            {
                return result;
            }

            CancelTiedOpenProposals(proposalService, votes, result.VoteCount);

            return new MeetingActionProposalResolutionResult(
                false,
                MeetingActionProposalResolutionType.TieCancelled,
                default(MeetingActionProposalData),
                result.EligibleVoterCount,
                result.RequiredVotes,
                result.VoteCount,
                "Highest vote tie cancelled and tied proposals were marked cancelled.");
        }

        private static int CountHighestVoteTies(
            IReadOnlyList<MeetingActionProposalData> proposals,
            IReadOnlyList<MeetingActionVoteData> votes,
            out int highestVoteCount)
        {
            highestVoteCount = 0;
            int tiedProposalCount = 0;

            if (proposals == null)
                return tiedProposalCount;

            for (int i = 0; i < proposals.Count; i++)
            {
                MeetingActionProposalData proposal = proposals[i];

                if (proposal.Status != MeetingActionProposalStatus.Open)
                    continue;

                int voteCount = CountVotesForProposal(votes, proposal.ProposalId);

                if (voteCount <= 0)
                    continue;

                if (voteCount > highestVoteCount)
                {
                    highestVoteCount = voteCount;
                    tiedProposalCount = 1;
                    continue;
                }

                if (voteCount == highestVoteCount)
                    tiedProposalCount++;
            }

            return tiedProposalCount;
        }

        private static void CancelTiedOpenProposals(
            MeetingActionProposalService proposalService,
            IReadOnlyList<MeetingActionVoteData> votes,
            int tiedVoteCount)
        {
            if (proposalService == null || tiedVoteCount <= 0)
                return;

            for (int i = 0; i < proposalService.Proposals.Count; i++)
            {
                MeetingActionProposalData proposal = proposalService.Proposals[i];

                if (proposal.Status != MeetingActionProposalStatus.Open)
                    continue;

                if (CountVotesForProposal(votes, proposal.ProposalId) != tiedVoteCount)
                    continue;

                MeetingActionProposalData updatedProposal;
                proposalService.TryUpdateProposalStatus(
                    proposal.ProposalId,
                    MeetingActionProposalStatus.Cancelled,
                    out updatedProposal);
            }
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
