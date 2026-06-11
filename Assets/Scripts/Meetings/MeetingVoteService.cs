using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingVoteService
    {
        private readonly List<MeetingVoteData> _votes = new List<MeetingVoteData>();

        public IReadOnlyList<MeetingVoteData> Votes => _votes;
        public int VoteCount => _votes.Count;

        public MeetingVoteSubmitResult SubmitVote(MeetingVoteData vote)
        {
            if (string.IsNullOrWhiteSpace(vote.VoterPlayerId) || vote.VoterPlayerId == "unknown_voter")
                return MeetingVoteSubmitResult.Failed(vote, "Voter player id is missing.");

            if (string.IsNullOrWhiteSpace(vote.TargetPlayerId) || vote.TargetPlayerId == "unknown_target")
                return MeetingVoteSubmitResult.Failed(vote, "Target player id is missing.");

            if (vote.VoterPlayerId == vote.TargetPlayerId)
                return MeetingVoteSubmitResult.Failed(vote, "Player cannot vote for self.");

            if (HasVoteFrom(vote.VoterPlayerId))
                return MeetingVoteSubmitResult.Failed(vote, "Player has already voted.");

            _votes.Add(vote);

            return new MeetingVoteSubmitResult(true, vote, "Vote submitted.");
        }

        public bool HasVoteFrom(string voterPlayerId)
        {
            for (int i = 0; i < _votes.Count; i++)
            {
                if (_votes[i].VoterPlayerId == voterPlayerId)
                    return true;
            }

            return false;
        }

        public int CountVotesForTarget(string targetPlayerId)
        {
            int count = 0;

            for (int i = 0; i < _votes.Count; i++)
            {
                if (_votes[i].TargetPlayerId == targetPlayerId)
                    count++;
            }

            return count;
        }

        public MeetingVoteCountResult GetVoteCountForTarget(string targetPlayerId)
        {
            return new MeetingVoteCountResult(targetPlayerId, CountVotesForTarget(targetPlayerId));
        }

        public void ClearVotes()
        {
            _votes.Clear();
        }
    }
}
