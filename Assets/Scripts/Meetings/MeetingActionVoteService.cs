using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingActionVoteService
    {
        private readonly List<MeetingActionVoteData> _votes =
            new List<MeetingActionVoteData>();

        public IReadOnlyList<MeetingActionVoteData> Votes => _votes;
        public int VoteCount => _votes.Count;

        public MeetingActionVoteSubmitResult SubmitVote(
            MeetingActionVoteData vote,
            IReadOnlyList<MeetingActionProposalData> proposals)
        {
            if (string.IsNullOrWhiteSpace(vote.VoteId))
                return MeetingActionVoteSubmitResult.Failed(vote, "Vote id is missing.");

            if (string.IsNullOrWhiteSpace(vote.VoterPlayerId))
                return MeetingActionVoteSubmitResult.Failed(vote, "Voter player id is missing.");

            if (string.IsNullOrWhiteSpace(vote.ProposalId))
                return MeetingActionVoteSubmitResult.Failed(vote, "Proposal id is missing.");

            if (HasVoteFrom(vote.VoterPlayerId))
                return MeetingActionVoteSubmitResult.Failed(vote, "Player has already voted.");

            if (!HasOpenProposal(proposals, vote.ProposalId))
                return MeetingActionVoteSubmitResult.Failed(
                    vote,
                    "Proposal is missing or not open.");

            _votes.Add(vote);

            return new MeetingActionVoteSubmitResult(true, vote, "Meeting action vote submitted.");
        }

        public bool HasVoteFrom(string voterPlayerId)
        {
            if (string.IsNullOrWhiteSpace(voterPlayerId))
                return false;

            for (int i = 0; i < _votes.Count; i++)
            {
                if (_votes[i].VoterPlayerId == voterPlayerId)
                    return true;
            }

            return false;
        }

        public int CountVotesForProposal(string proposalId)
        {
            int count = 0;

            if (string.IsNullOrWhiteSpace(proposalId))
                return count;

            for (int i = 0; i < _votes.Count; i++)
            {
                if (_votes[i].ProposalId == proposalId)
                    count++;
            }

            return count;
        }

        public MeetingActionVoteCountResult GetVoteCountForProposal(string proposalId)
        {
            return new MeetingActionVoteCountResult(
                proposalId,
                CountVotesForProposal(proposalId));
        }

        public void ClearVotes()
        {
            _votes.Clear();
        }

        private static bool HasOpenProposal(
            IReadOnlyList<MeetingActionProposalData> proposals,
            string proposalId)
        {
            if (proposals == null || string.IsNullOrWhiteSpace(proposalId))
                return false;

            for (int i = 0; i < proposals.Count; i++)
            {
                if (proposals[i].ProposalId == proposalId
                    && proposals[i].Status == MeetingActionProposalStatus.Open)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
