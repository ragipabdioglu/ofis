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

    public readonly struct MeetingVoteSubmitResult
    {
        public bool Success { get; }
        public MeetingVoteData Vote { get; }
        public string Message { get; }

        public MeetingVoteSubmitResult(bool success, MeetingVoteData vote, string message)
        {
            Success = success;
            Vote = vote;
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public static MeetingVoteSubmitResult Failed(MeetingVoteData vote, string message)
        {
            return new MeetingVoteSubmitResult(false, vote, message);
        }

        public override string ToString()
        {
            return $"Success={Success}, Vote={Vote}, Message={Message}";
        }
    }
}
