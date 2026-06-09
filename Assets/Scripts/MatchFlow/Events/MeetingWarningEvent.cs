using OFIS.Core.Events;
using OFIS.MatchFlow.States;

namespace OFIS.MatchFlow.Events
{
    public sealed class MeetingWarningEvent : IGameEvent
    {
        public float CreatedAtRealtime { get; }

        public MatchState UpcomingMeetingState { get; }
        public float SecondsUntilMeeting { get; }
        public bool IsRedWarning { get; }

        public MeetingWarningEvent(
            MatchState upcomingMeetingState,
            float secondsUntilMeeting,
            bool isRedWarning,
            float createdAtRealtime)
        {
            UpcomingMeetingState = upcomingMeetingState;
            SecondsUntilMeeting = secondsUntilMeeting;
            IsRedWarning = isRedWarning;
            CreatedAtRealtime = createdAtRealtime;
        }
    }
}