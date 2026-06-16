namespace OFIS.Meetings
{
    public readonly struct MeetingActionProposalResolutionResult
    {
        public bool HasResolvedProposal { get; }
        public MeetingActionProposalResolutionType ResolutionType { get; }
        public MeetingActionProposalData Proposal { get; }
        public int EligibleVoterCount { get; }
        public int RequiredVotes { get; }
        public int VoteCount { get; }
        public string Message { get; }

        public MeetingActionProposalResolutionResult(
            bool hasResolvedProposal,
            MeetingActionProposalResolutionType resolutionType,
            MeetingActionProposalData proposal,
            int eligibleVoterCount,
            int requiredVotes,
            int voteCount,
            string message)
        {
            HasResolvedProposal = hasResolvedProposal;
            ResolutionType = resolutionType;
            Proposal = proposal;
            EligibleVoterCount = eligibleVoterCount < 0 ? 0 : eligibleVoterCount;
            RequiredVotes = requiredVotes < 0 ? 0 : requiredVotes;
            VoteCount = voteCount < 0 ? 0 : voteCount;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action proposal resolution evaluated."
                : message;
        }

        public override string ToString()
        {
            return $"Resolved={HasResolvedProposal}, Type={ResolutionType}, Proposal={Proposal.ProposalId}, Eligible={EligibleVoterCount}, Required={RequiredVotes}, Votes={VoteCount}, Message={Message}";
        }
    }
}
