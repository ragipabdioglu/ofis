namespace OFIS.Meetings
{
    public readonly struct MeetingVoteData
    {
        public string VoteId { get; }
        public string VoterPlayerId { get; }
        public string TargetPlayerId { get; }
        public string Reason { get; }

        public MeetingVoteData(string voteId, string voterPlayerId, string targetPlayerId, string reason)
        {
            VoteId = string.IsNullOrWhiteSpace(voteId) ? "unknown_vote" : voteId;
            VoterPlayerId = string.IsNullOrWhiteSpace(voterPlayerId) ? "unknown_voter" : voterPlayerId;
            TargetPlayerId = string.IsNullOrWhiteSpace(targetPlayerId) ? "unknown_target" : targetPlayerId;
            Reason = string.IsNullOrWhiteSpace(reason) ? "No reason." : reason;
        }

        public override string ToString()
        {
            return $"VoteId={VoteId}, Voter={VoterPlayerId}, Target={TargetPlayerId}, Reason={Reason}";
        }
    }
}
