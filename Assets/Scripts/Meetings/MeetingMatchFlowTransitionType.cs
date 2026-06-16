namespace OFIS.Meetings
{
    public enum MeetingMatchFlowTransitionType
    {
        None = 0,
        ContinueMeeting = 1,
        ApplyCompanyHealthOnly = 2,
        CloseNormalMeeting = 3,
        ResolveFinalMeeting = 4,
        ShowMeetingEndSummary = 5
    }
}
