namespace OFIS.Meetings
{
    public readonly struct MeetingParticipationGuardResult
    {
        public string PlayerId { get; }
        public bool CanAttendMeeting { get; }
        public bool CanVote { get; }
        public bool CanUseMeetingVoice { get; }
        public bool IsLateObserver { get; }
        public string Reason { get; }

        public MeetingParticipationGuardResult(
            string playerId,
            bool canAttendMeeting,
            bool canVote,
            bool canUseMeetingVoice,
            bool isLateObserver,
            string reason)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? string.Empty : playerId;
            CanAttendMeeting = canAttendMeeting;
            CanVote = canVote;
            CanUseMeetingVoice = canUseMeetingVoice;
            IsLateObserver = isLateObserver;
            Reason = string.IsNullOrWhiteSpace(reason)
                ? "Meeting participation guard resolved."
                : reason;
        }

        public override string ToString()
        {
            return $"Player={PlayerId}, Attend={CanAttendMeeting}, Vote={CanVote}, Voice={CanUseMeetingVoice}, LateObserver={IsLateObserver}, Reason={Reason}";
        }
    }
}
