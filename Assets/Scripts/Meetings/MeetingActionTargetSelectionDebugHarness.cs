using OFIS.Roles.Departments;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionTargetSelectionDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private MeetingActionType lastActionType;
        [SerializeField] private MeetingActionTargetType lastTargetType;
        [SerializeField] private bool lastRequiresTarget;
        [SerializeField] private bool lastHasValidSelection;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionTargetSelectionStateService _service =
            new MeetingActionTargetSelectionStateService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateTargetSelectionState();
        }

        [ContextMenu("Validate Meeting Action Target Selection")]
        public void ValidateTargetSelectionState()
        {
            ValidatePersonnelAuditEnablesOnlyPlayer();
            ValidateRoomInspectionEnablesOnlyRoom();
            ValidateTaskReportAuditEnablesPlayerAndDepartment();
            ValidateSecurityReviewEnablesOnlySecurityArea();
            ValidateNoActionRequiresNoTarget();
            ValidateInvalidSelectionIsRejected();
            ValidateCoerceInvalidSelectionToNone();
        }

        private void ValidatePersonnelAuditEnablesOnlyPlayer()
        {
            MeetingActionTargetSelectionState state = _service.BuildState(
                MeetingActionType.PersonnelAudit,
                MeetingActionTargetData.ForPlayer("player_01"));

            bool passed = state.CanSelectPlayer
                && !state.CanSelectRoom
                && !state.CanSelectDepartment
                && !state.CanSelectSecurityArea
                && state.HasValidSelection;

            LogResult("PersonnelAuditEnablesOnlyPlayer", passed, state);
        }

        private void ValidateRoomInspectionEnablesOnlyRoom()
        {
            MeetingActionTargetSelectionState state = _service.BuildState(
                MeetingActionType.RoomInspection,
                MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom));

            bool passed = !state.CanSelectPlayer
                && state.CanSelectRoom
                && !state.CanSelectDepartment
                && !state.CanSelectSecurityArea
                && state.HasValidSelection;

            LogResult("RoomInspectionEnablesOnlyRoom", passed, state);
        }

        private void ValidateTaskReportAuditEnablesPlayerAndDepartment()
        {
            MeetingActionTargetSelectionState playerState = _service.BuildState(
                MeetingActionType.TaskReportAudit,
                MeetingActionTargetData.ForPlayer("player_02"));

            MeetingActionTargetSelectionState departmentState = _service.BuildState(
                MeetingActionType.TaskReportAudit,
                MeetingActionTargetData.ForDepartment(DepartmentType.Logistics));

            bool passed = playerState.CanSelectPlayer
                && playerState.CanSelectDepartment
                && !playerState.CanSelectRoom
                && !playerState.CanSelectSecurityArea
                && playerState.HasValidSelection
                && departmentState.HasValidSelection;

            LogResult("TaskReportAuditEnablesPlayerAndDepartment", passed, departmentState);
        }

        private void ValidateSecurityReviewEnablesOnlySecurityArea()
        {
            MeetingActionTargetSelectionState state = _service.BuildState(
                MeetingActionType.SecurityRecordReview,
                MeetingActionTargetData.ForSecurityArea(MeetingSecurityAreaType.CameraSystem));

            bool passed = !state.CanSelectPlayer
                && !state.CanSelectRoom
                && !state.CanSelectDepartment
                && state.CanSelectSecurityArea
                && state.HasValidSelection;

            LogResult("SecurityReviewEnablesOnlySecurityArea", passed, state);
        }

        private void ValidateNoActionRequiresNoTarget()
        {
            MeetingActionTargetSelectionState state = _service.BuildState(
                MeetingActionType.NoAction,
                MeetingActionTargetData.None());

            bool passed = !state.RequiresTarget
                && state.CanSelectTargetType(MeetingActionTargetType.None)
                && state.HasValidSelection;

            LogResult("NoActionRequiresNoTarget", passed, state);
        }

        private void ValidateInvalidSelectionIsRejected()
        {
            MeetingActionTargetSelectionState state = _service.BuildState(
                MeetingActionType.RoomInspection,
                MeetingActionTargetData.ForPlayer("player_03"));

            LogResult("InvalidSelectionIsRejected", !state.HasValidSelection, state);
        }

        private void ValidateCoerceInvalidSelectionToNone()
        {
            MeetingActionTargetData coercedTarget = _service.CoerceTargetForAction(
                MeetingActionType.OfficialAccusation,
                MeetingActionTargetData.ForRoom(OfficeRoomType.SecurityRoom));

            MeetingActionTargetSelectionState state = _service.BuildState(
                MeetingActionType.OfficialAccusation,
                coercedTarget);

            bool passed = coercedTarget.IsEmpty && !state.HasValidSelection;
            LogResult("CoerceInvalidSelectionToNone", passed, state);
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionTargetSelectionState state)
        {
            lastActionType = state.ActionType;
            lastTargetType = state.SelectedTarget.TargetType;
            lastRequiresTarget = state.RequiresTarget;
            lastHasValidSelection = state.HasValidSelection;
            lastMessage = state.Message;

            if (passed)
                Debug.Log($"[MeetingActionTargetSelectionValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[MeetingActionTargetSelectionValidator] FAIL {testName}: {state}");
        }
    }
}
#pragma warning restore 0414
