namespace OFIS.Meetings
{
    public readonly struct MeetingEmptyStateResolutionConfig
    {
        public float NormalMeetingAutoCloseDelaySeconds { get; }
        public bool AutoCloseNormalMeetingWhenEmpty { get; }
        public bool ResolveFinalMeetingWhenEmpty { get; }

        public MeetingEmptyStateResolutionConfig(
            float normalMeetingAutoCloseDelaySeconds,
            bool autoCloseNormalMeetingWhenEmpty = true,
            bool resolveFinalMeetingWhenEmpty = true)
        {
            NormalMeetingAutoCloseDelaySeconds = normalMeetingAutoCloseDelaySeconds < 0f
                ? 0f
                : normalMeetingAutoCloseDelaySeconds;

            AutoCloseNormalMeetingWhenEmpty = autoCloseNormalMeetingWhenEmpty;
            ResolveFinalMeetingWhenEmpty = resolveFinalMeetingWhenEmpty;
        }

        public static MeetingEmptyStateResolutionConfig Default =>
            new MeetingEmptyStateResolutionConfig(
                normalMeetingAutoCloseDelaySeconds: 10f,
                autoCloseNormalMeetingWhenEmpty: true,
                resolveFinalMeetingWhenEmpty: true);

        public override string ToString()
        {
            return $"AutoCloseDelay={NormalMeetingAutoCloseDelaySeconds:0.00}s, AutoCloseNormal={AutoCloseNormalMeetingWhenEmpty}, ResolveFinal={ResolveFinalMeetingWhenEmpty}";
        }
    }
}
