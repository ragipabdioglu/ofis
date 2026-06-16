namespace OFIS.Meetings
{
    public readonly struct MeetingMatchFlowCommandConsumerConfig
    {
        public bool DryRunOnly { get; }
        public bool AllowRuntimeMutation { get; }

        public MeetingMatchFlowCommandConsumerConfig(
            bool dryRunOnly,
            bool allowRuntimeMutation)
        {
            DryRunOnly = dryRunOnly;
            AllowRuntimeMutation = allowRuntimeMutation;
        }

        public static MeetingMatchFlowCommandConsumerConfig SafeDryRun =>
            new MeetingMatchFlowCommandConsumerConfig(
                dryRunOnly: true,
                allowRuntimeMutation: false);

        public static MeetingMatchFlowCommandConsumerConfig ApplyModeWithoutMutation =>
            new MeetingMatchFlowCommandConsumerConfig(
                dryRunOnly: false,
                allowRuntimeMutation: false);
    }
}
