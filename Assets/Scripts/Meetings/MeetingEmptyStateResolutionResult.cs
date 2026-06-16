namespace OFIS.Meetings
{
    public readonly struct MeetingEmptyStateResolutionResult
    {
        public MeetingRuntimePhaseType PhaseType { get; }
        public MeetingEmptyStateResolutionType ResolutionType { get; }
        public int RegisteredPlayerCount { get; }
        public int LateObserverCount { get; }
        public float EmptyElapsedSeconds { get; }
        public float RequiredDelaySeconds { get; }
        public bool IsEmpty { get; }
        public bool ShouldCloseMeeting { get; }
        public bool ShouldResolveWinBranch { get; }
        public bool IsResolved { get; }
        public string Reason { get; }

        public MeetingEmptyStateResolutionResult(
            MeetingRuntimePhaseType phaseType,
            MeetingEmptyStateResolutionType resolutionType,
            int registeredPlayerCount,
            int lateObserverCount,
            float emptyElapsedSeconds,
            float requiredDelaySeconds,
            bool isEmpty,
            bool shouldCloseMeeting,
            bool shouldResolveWinBranch,
            bool isResolved,
            string reason)
        {
            PhaseType = phaseType;
            ResolutionType = resolutionType;
            RegisteredPlayerCount = registeredPlayerCount < 0 ? 0 : registeredPlayerCount;
            LateObserverCount = lateObserverCount < 0 ? 0 : lateObserverCount;
            EmptyElapsedSeconds = emptyElapsedSeconds < 0f ? 0f : emptyElapsedSeconds;
            RequiredDelaySeconds = requiredDelaySeconds < 0f ? 0f : requiredDelaySeconds;
            IsEmpty = isEmpty;
            ShouldCloseMeeting = shouldCloseMeeting;
            ShouldResolveWinBranch = shouldResolveWinBranch;
            IsResolved = isResolved;
            Reason = string.IsNullOrWhiteSpace(reason)
                ? "Meeting empty state evaluated."
                : reason;
        }

        public override string ToString()
        {
            return $"Phase={PhaseType}, Resolution={ResolutionType}, Registered={RegisteredPlayerCount}, LateObservers={LateObserverCount}, EmptyElapsed={EmptyElapsedSeconds:0.00}s, RequiredDelay={RequiredDelaySeconds:0.00}s, Empty={IsEmpty}, Close={ShouldCloseMeeting}, WinBranch={ShouldResolveWinBranch}, Resolved={IsResolved}, Reason={Reason}";
        }
    }
}
