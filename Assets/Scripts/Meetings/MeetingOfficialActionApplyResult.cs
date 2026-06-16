namespace OFIS.Meetings
{
    public readonly struct MeetingOfficialActionApplyResult
    {
        public bool Success { get; }
        public MeetingOfficialActionEffectResult EffectResult { get; }
        public string MeetingId { get; }
        public bool HasAppliedOfficialAction { get; }
        public string Message { get; }

        public MeetingOfficialActionApplyResult(
            bool success,
            MeetingOfficialActionEffectResult effectResult,
            string meetingId,
            bool hasAppliedOfficialAction,
            string message)
        {
            Success = success;
            EffectResult = effectResult;
            MeetingId = string.IsNullOrWhiteSpace(meetingId) ? string.Empty : meetingId;
            HasAppliedOfficialAction = hasAppliedOfficialAction;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting official action apply resolved."
                : message;
        }

        public override string ToString()
        {
            return $"Success={Success}, MeetingId={MeetingId}, Applied={HasAppliedOfficialAction}, Effect=({EffectResult}), Message={Message}";
        }
    }
}
