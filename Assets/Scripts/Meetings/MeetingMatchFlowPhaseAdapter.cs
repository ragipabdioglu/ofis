using OFIS.MatchFlow.States;

namespace OFIS.Meetings
{
    public static class MeetingMatchFlowPhaseAdapter
    {
        public static MeetingRuntimePhaseType FromMatchState(MatchState matchState)
        {
            switch (matchState)
            {
                case MatchState.Meeting1:
                case MatchState.Meeting2:
                    return MeetingRuntimePhaseType.Meeting;

                case MatchState.FinalMeeting:
                    return MeetingRuntimePhaseType.FinalMeeting;

                case MatchState.OfficePhase1:
                case MatchState.OfficePhase2:
                case MatchState.OfficePhase3:
                    return MeetingRuntimePhaseType.Office;

                default:
                    return MeetingRuntimePhaseType.None;
            }
        }

        public static bool IsMeetingState(MatchState matchState)
        {
            MeetingRuntimePhaseType phaseType = FromMatchState(matchState);
            return phaseType == MeetingRuntimePhaseType.Meeting
                || phaseType == MeetingRuntimePhaseType.FinalMeeting;
        }
    }
}
