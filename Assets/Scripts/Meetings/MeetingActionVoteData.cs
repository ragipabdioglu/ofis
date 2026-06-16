namespace OFIS.Meetings
{
    public readonly struct MeetingActionVoteData
    {
        public string VoteId { get; }
        public string VoterPlayerId { get; }
        public string ProposalId { get; }
        public string Reason { get; }

        public MeetingActionVoteData(
            string voteId,
            string voterPlayerId,
            string proposalId,
            string reason)
        {
            VoteId = string.IsNullOrWhiteSpace(voteId) ? string.Empty : voteId;
            VoterPlayerId = string.IsNullOrWhiteSpace(voterPlayerId) ? string.Empty : voterPlayerId;
            ProposalId = string.IsNullOrWhiteSpace(proposalId) ? string.Empty : proposalId;
            Reason = string.IsNullOrWhiteSpace(reason) ? string.Empty : reason;
        }

        public override string ToString()
        {
            return $"VoteId={VoteId}, Voter={VoterPlayerId}, Proposal={ProposalId}, Reason={Reason}";
        }
    }
}
