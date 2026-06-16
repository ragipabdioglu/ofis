namespace OFIS.Meetings
{
    public readonly struct MeetingRuntimeDecisionAggregatorConfig
    {
        public bool EvaluateMissingPlayerPenalty { get; }
        public bool EvaluateEmptyStateResolution { get; }
        public bool EvaluateMeetingEndPipeline { get; }
        public float JoinLockThresholdSeconds { get; }

        public MeetingRuntimeDecisionAggregatorConfig(
            bool evaluateMissingPlayerPenalty,
            bool evaluateEmptyStateResolution,
            bool evaluateMeetingEndPipeline,
            float joinLockThresholdSeconds)
        {
            EvaluateMissingPlayerPenalty = evaluateMissingPlayerPenalty;
            EvaluateEmptyStateResolution = evaluateEmptyStateResolution;
            EvaluateMeetingEndPipeline = evaluateMeetingEndPipeline;
            JoinLockThresholdSeconds = joinLockThresholdSeconds < 0f ? 0f : joinLockThresholdSeconds;
        }

        public static MeetingRuntimeDecisionAggregatorConfig Default =>
            new MeetingRuntimeDecisionAggregatorConfig(
                evaluateMissingPlayerPenalty: true,
                evaluateEmptyStateResolution: true,
                evaluateMeetingEndPipeline: true,
                joinLockThresholdSeconds: 20f);

        public override string ToString()
        {
            return $"Penalty={EvaluateMissingPlayerPenalty}, EmptyState={EvaluateEmptyStateResolution}, EndPipeline={EvaluateMeetingEndPipeline}, JoinLockThreshold={JoinLockThresholdSeconds:0.00}s";
        }
    }
}
