namespace OFIS.Meetings
{
    public readonly struct MeetingActionPanelState
    {
        public string MeetingId { get; }
        public MeetingRuntimePhaseType PhaseType { get; }
        public bool IsOpen { get; }
        public bool CanSelectAction { get; }
        public bool ShouldShowTargetPicker { get; }
        public string Message { get; }

        public MeetingActionPanelState(
            string meetingId,
            MeetingRuntimePhaseType phaseType,
            bool isOpen,
            bool canSelectAction,
            bool shouldShowTargetPicker,
            string message)
        {
            MeetingId = string.IsNullOrWhiteSpace(meetingId) ? string.Empty : meetingId;
            PhaseType = phaseType;
            IsOpen = isOpen;
            CanSelectAction = canSelectAction;
            ShouldShowTargetPicker = shouldShowTargetPicker;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action panel state resolved."
                : message;
        }

        public override string ToString()
        {
            return $"MeetingId={MeetingId}, Phase={PhaseType}, Open={IsOpen}, CanSelect={CanSelectAction}, TargetPicker={ShouldShowTargetPicker}, Message={Message}";
        }
    }
}
