namespace OFIS.Meetings
{
    public enum MeetingRuntimeDecisionType
    {
        None = 0,
        ContinueMeeting = 1,
        ApplyMissingPlayerPenalty = 2,
        AutoCloseMeeting = 3,
        ResolveFinalMeetingWinBranch = 4,
        RunMeetingEndPipeline = 5
    }
}
