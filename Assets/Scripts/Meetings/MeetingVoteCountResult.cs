namespace OFIS.Meetings
{
    public readonly struct MeetingVoteCountResult
    {
        public string TargetPlayerId { get; }
        public int VoteCount { get; }

        public MeetingVoteCountResult(string targetPlayerId, int voteCount)
        {
            TargetPlayerId = string.IsNullOrWhiteSpace(targetPlayerId) ? "unknown_target" : targetPlayerId;
            VoteCount = voteCount < 0 ? 0 : voteCount;
        }

        public override string ToString()
        {
            return $"Target={TargetPlayerId}, VoteCount={VoteCount}";
        }
    }
}
