namespace OFIS.Meetings
{
    public readonly struct MeetingActionTargetSelectionState
    {
        public MeetingActionType ActionType { get; }
        public MeetingActionTargetData SelectedTarget { get; }
        public bool CanSelectPlayer { get; }
        public bool CanSelectRoom { get; }
        public bool CanSelectDepartment { get; }
        public bool CanSelectSecurityArea { get; }
        public bool RequiresTarget { get; }
        public bool HasValidSelection { get; }
        public string Message { get; }

        public MeetingActionTargetSelectionState(
            MeetingActionType actionType,
            MeetingActionTargetData selectedTarget,
            bool canSelectPlayer,
            bool canSelectRoom,
            bool canSelectDepartment,
            bool canSelectSecurityArea,
            bool requiresTarget,
            bool hasValidSelection,
            string message)
        {
            ActionType = actionType;
            SelectedTarget = selectedTarget;
            CanSelectPlayer = canSelectPlayer;
            CanSelectRoom = canSelectRoom;
            CanSelectDepartment = canSelectDepartment;
            CanSelectSecurityArea = canSelectSecurityArea;
            RequiresTarget = requiresTarget;
            HasValidSelection = hasValidSelection;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action target selection state resolved."
                : message;
        }

        public bool CanSelectTargetType(MeetingActionTargetType targetType)
        {
            switch (targetType)
            {
                case MeetingActionTargetType.Player:
                    return CanSelectPlayer;

                case MeetingActionTargetType.Room:
                    return CanSelectRoom;

                case MeetingActionTargetType.Department:
                    return CanSelectDepartment;

                case MeetingActionTargetType.SecurityArea:
                    return CanSelectSecurityArea;

                case MeetingActionTargetType.None:
                    return !RequiresTarget;

                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return $"Action={ActionType}, Target=({SelectedTarget}), Player={CanSelectPlayer}, Room={CanSelectRoom}, Department={CanSelectDepartment}, Security={CanSelectSecurityArea}, Requires={RequiresTarget}, Valid={HasValidSelection}, Message={Message}";
        }
    }
}
