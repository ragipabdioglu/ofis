using OFIS.Roles.Departments;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionModelDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastActionId;
        [SerializeField] private MeetingActionType lastActionType;
        [SerializeField] private MeetingActionTargetType lastTargetType;
        [SerializeField] private bool lastIsValid;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionValidationService _service =
            new MeetingActionValidationService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMeetingActionModel();
        }

        [ContextMenu("Validate Meeting Action Model")]
        public void ValidateMeetingActionModel()
        {
            ValidatePersonnelAuditRequiresPlayer();
            ValidateRoomInspectionRequiresRoom();
            ValidateTaskReportAuditAcceptsPlayerOrDepartment();
            ValidateSecurityReviewRequiresSecurityArea();
            ValidateNoActionHasNoTarget();
            ValidateNoneActionInvalid();
        }

        private void ValidatePersonnelAuditRequiresPlayer()
        {
            MeetingActionValidationResult validResult = _service.Validate(
                BuildRequest(
                    "action_personnel_valid",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForPlayer("player_01")));

            MeetingActionValidationResult invalidResult = _service.Validate(
                BuildRequest(
                    "action_personnel_invalid",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.SecurityRoom)));

            LogResult(
                "PersonnelAuditRequiresPlayer",
                validResult.IsValid && !invalidResult.IsValid,
                invalidResult);
        }

        private void ValidateRoomInspectionRequiresRoom()
        {
            MeetingActionValidationResult validResult = _service.Validate(
                BuildRequest(
                    "action_room_valid",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom)));

            MeetingActionValidationResult invalidResult = _service.Validate(
                BuildRequest(
                    "action_room_invalid",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForDepartment(DepartmentType.Archive)));

            LogResult(
                "RoomInspectionRequiresRoom",
                validResult.IsValid && !invalidResult.IsValid,
                invalidResult);
        }

        private void ValidateTaskReportAuditAcceptsPlayerOrDepartment()
        {
            MeetingActionValidationResult playerResult = _service.Validate(
                BuildRequest(
                    "action_task_player",
                    MeetingActionType.TaskReportAudit,
                    MeetingActionTargetData.ForPlayer("player_02")));

            MeetingActionValidationResult departmentResult = _service.Validate(
                BuildRequest(
                    "action_task_department",
                    MeetingActionType.TaskReportAudit,
                    MeetingActionTargetData.ForDepartment(DepartmentType.Logistics)));

            MeetingActionValidationResult invalidResult = _service.Validate(
                BuildRequest(
                    "action_task_invalid",
                    MeetingActionType.TaskReportAudit,
                    MeetingActionTargetData.ForSecurityArea(MeetingSecurityAreaType.CameraSystem)));

            LogResult(
                "TaskReportAuditAcceptsPlayerOrDepartment",
                playerResult.IsValid && departmentResult.IsValid && !invalidResult.IsValid,
                invalidResult);
        }

        private void ValidateSecurityReviewRequiresSecurityArea()
        {
            MeetingActionValidationResult validResult = _service.Validate(
                BuildRequest(
                    "action_security_valid",
                    MeetingActionType.SecurityRecordReview,
                    MeetingActionTargetData.ForSecurityArea(MeetingSecurityAreaType.DoorAccessLog)));

            MeetingActionValidationResult invalidResult = _service.Validate(
                BuildRequest(
                    "action_security_invalid",
                    MeetingActionType.SecurityRecordReview,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.SecurityRoom)));

            LogResult(
                "SecurityReviewRequiresSecurityArea",
                validResult.IsValid && !invalidResult.IsValid,
                invalidResult);
        }

        private void ValidateNoActionHasNoTarget()
        {
            MeetingActionValidationResult validResult = _service.Validate(
                BuildRequest(
                    "action_no_action_valid",
                    MeetingActionType.NoAction,
                    MeetingActionTargetData.None()));

            MeetingActionValidationResult invalidResult = _service.Validate(
                BuildRequest(
                    "action_no_action_invalid",
                    MeetingActionType.NoAction,
                    MeetingActionTargetData.ForPlayer("player_03")));

            LogResult(
                "NoActionHasNoTarget",
                validResult.IsValid && !invalidResult.IsValid,
                invalidResult);
        }

        private void ValidateNoneActionInvalid()
        {
            MeetingActionValidationResult result = _service.Validate(
                BuildRequest(
                    "action_none_invalid",
                    MeetingActionType.None,
                    MeetingActionTargetData.None()));

            LogResult("NoneActionInvalid", !result.IsValid, result);
        }

        private static MeetingActionRequestData BuildRequest(
            string actionId,
            MeetingActionType actionType,
            MeetingActionTargetData target)
        {
            return new MeetingActionRequestData(
                actionId,
                "player_proposer",
                actionType,
                target,
                "Debug action request.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionValidationResult result)
        {
            lastActionId = result.Request.ActionId;
            lastActionType = result.Request.ActionType;
            lastTargetType = result.Request.Target.TargetType;
            lastIsValid = result.IsValid;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionModelValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionModelValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
