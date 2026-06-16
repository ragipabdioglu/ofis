namespace OFIS.Meetings
{
    public readonly struct MeetingActionVoteCountResult
    {
        public string ProposalId { get; }
        public int VoteCount { get; }

        public MeetingActionVoteCountResult(string proposalId, int voteCount)
        {
            ProposalId = string.IsNullOrWhiteSpace(proposalId) ? string.Empty : proposalId;
            VoteCount = voteCount;
        }

        public override string ToString()
        {
            return $"ProposalId={ProposalId}, VoteCount={VoteCount}";
        }
    }
}
