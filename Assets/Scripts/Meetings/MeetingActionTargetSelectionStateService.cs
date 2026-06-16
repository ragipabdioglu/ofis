namespace OFIS.Meetings
{
    public sealed class MeetingActionTargetSelectionStateService
    {
        public MeetingActionTargetSelectionState BuildState(
            MeetingActionType actionType,
            MeetingActionTargetData selectedTarget)
        {
            bool canSelectPlayer = CanSelectPlayer(actionType);
            bool canSelectRoom = actionType == MeetingActionType.RoomInspection;
            bool canSelectDepartment = actionType == MeetingActionType.TaskReportAudit;
            bool canSelectSecurityArea = actionType == MeetingActionType.SecurityRecordReview;
            bool requiresTarget = actionType != MeetingActionType.NoAction
                && actionType != MeetingActionType.None;

            bool hasValidSelection = HasValidSelection(
                selectedTarget,
                canSelectPlayer,
                canSelectRoom,
                canSelectDepartment,
                canSelectSecurityArea,
                requiresTarget);

            return new MeetingActionTargetSelectionState(
                actionType,
                selectedTarget,
                canSelectPlayer,
                canSelectRoom,
                canSelectDepartment,
                canSelectSecurityArea,
                requiresTarget,
                hasValidSelection,
                BuildMessage(actionType, requiresTarget, hasValidSelection));
        }

        public MeetingActionTargetData CoerceTargetForAction(
            MeetingActionType actionType,
            MeetingActionTargetData selectedTarget)
        {
            MeetingActionTargetSelectionState state = BuildState(actionType, selectedTarget);

            if (state.HasValidSelection)
                return selectedTarget;

            return MeetingActionTargetData.None();
        }

        private static bool CanSelectPlayer(MeetingActionType actionType)
        {
            return actionType == MeetingActionType.PersonnelAudit
                || actionType == MeetingActionType.TaskReportAudit
                || actionType == MeetingActionType.OfficialAccusation;
        }

        private static bool HasValidSelection(
            MeetingActionTargetData selectedTarget,
            bool canSelectPlayer,
            bool canSelectRoom,
            bool canSelectDepartment,
            bool canSelectSecurityArea,
            bool requiresTarget)
        {
            if (!requiresTarget)
                return selectedTarget.IsEmpty;

            if (selectedTarget.HasPlayerTarget)
                return canSelectPlayer;

            if (selectedTarget.HasRoomTarget)
                return canSelectRoom;

            if (selectedTarget.HasDepartmentTarget)
                return canSelectDepartment;

            if (selectedTarget.HasSecurityAreaTarget)
                return canSelectSecurityArea;

            return false;
        }

        private static string BuildMessage(
            MeetingActionType actionType,
            bool requiresTarget,
            bool hasValidSelection)
        {
            if (actionType == MeetingActionType.None)
                return "Action type is invalid.";

            if (!requiresTarget && hasValidSelection)
                return "Action does not require a target.";

            if (hasValidSelection)
                return "Selected target is valid for action.";

            return "Selected target is not valid for action.";
        }
    }
}
