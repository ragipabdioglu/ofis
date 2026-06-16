namespace OFIS.Meetings
{
    public readonly struct MeetingActionMajorityThresholdResult
    {
        public int EligibleVoterCount { get; }
        public int RequiredVotes { get; }
        public int CurrentVoteCount { get; }
        public bool HasEligibleVoters { get; }
        public bool HasReachedMajority { get; }
        public string Message { get; }

        public MeetingActionMajorityThresholdResult(
            int eligibleVoterCount,
            int requiredVotes,
            int currentVoteCount,
            bool hasReachedMajority,
            string message)
        {
            EligibleVoterCount = eligibleVoterCount < 0 ? 0 : eligibleVoterCount;
            RequiredVotes = requiredVotes < 0 ? 0 : requiredVotes;
            CurrentVoteCount = currentVoteCount < 0 ? 0 : currentVoteCount;
            HasEligibleVoters = EligibleVoterCount > 0;
            HasReachedMajority = hasReachedMajority;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action majority threshold resolved."
                : message;
        }

        public override string ToString()
        {
            return $"Eligible={EligibleVoterCount}, Required={RequiredVotes}, Current={CurrentVoteCount}, Reached={HasReachedMajority}, Message={Message}";
        }
    }
}
