namespace OFIS.Meetings
{
    public readonly struct MeetingPhaseRuntimeHookState
    {
        public MeetingRuntimePhaseType PhaseType { get; }
        public float DurationSeconds { get; }
        public float ElapsedSeconds { get; }
        public float RemainingSeconds { get; }
        public float JoinLockThresholdSeconds { get; }
        public bool HasActivePhase { get; }
        public bool IsMeetingPhase { get; }
        public bool IsJoinLocked { get; }
        public bool HasEnded { get; }
        public bool PipelineTriggered { get; }
        public string Message { get; }

        public MeetingPhaseRuntimeHookState(
            MeetingRuntimePhaseType phaseType,
            float durationSeconds,
            float elapsedSeconds,
            float joinLockThresholdSeconds,
            bool hasActivePhase,
            bool pipelineTriggered,
            string message)
        {
            PhaseType = phaseType;
            DurationSeconds = durationSeconds < 0f ? 0f : durationSeconds;
            ElapsedSeconds = elapsedSeconds < 0f ? 0f : elapsedSeconds;
            JoinLockThresholdSeconds = joinLockThresholdSeconds < 0f ? 0f : joinLockThresholdSeconds;
            HasActivePhase = hasActivePhase;
            PipelineTriggered = pipelineTriggered;

            RemainingSeconds = DurationSeconds - ElapsedSeconds;
            if (RemainingSeconds < 0f)
                RemainingSeconds = 0f;

            IsMeetingPhase = phaseType == MeetingRuntimePhaseType.Meeting
                || phaseType == MeetingRuntimePhaseType.FinalMeeting;

            IsJoinLocked = HasActivePhase
                && IsMeetingPhase
                && RemainingSeconds <= JoinLockThresholdSeconds
                && RemainingSeconds > 0f;

            HasEnded = HasActivePhase
                && IsMeetingPhase
                && RemainingSeconds <= 0f;

            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting phase runtime hook state updated."
                : message;
        }

        public override string ToString()
        {
            return $"Phase={PhaseType}, Active={HasActivePhase}, Duration={DurationSeconds:0.00}s, Elapsed={ElapsedSeconds:0.00}s, Remaining={RemainingSeconds:0.00}s, JoinLocked={IsJoinLocked}, Ended={HasEnded}, PipelineTriggered={PipelineTriggered}, Message={Message}";
        }
    }
}
