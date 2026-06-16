namespace OFIS.Meetings
{
    public enum MeetingProductionBridgeActionType
    {
        None = 0,
        ContinueMeeting = 1,
        ApplyCompanyHealthPenalty = 2,
        CloseMeeting = 3,
        ResolveFinalMeetingWinBranch = 4,
        RunMeetingEndPipeline = 5
    }
}
