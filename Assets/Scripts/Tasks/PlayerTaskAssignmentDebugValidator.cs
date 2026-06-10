using System.Collections.Generic;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Tasks
{
    public sealed class PlayerTaskAssignmentDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly PlayerTaskAssignmentService _assignmentService = new PlayerTaskAssignmentService();
        private readonly OfficeTaskCompletionService _completionService = new OfficeTaskCompletionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidatePlayerTaskAssignment();
        }

        [ContextMenu("Validate Player Task Assignment")]
        public void ValidatePlayerTaskAssignment()
        {
            ValidateAssignTasksSuccess();
            ValidateProgressCountsAfterCompletion();
            ValidateAllCompletedAfterAllTasksDone();
            ValidateMissingPlayerFails();
            ValidateEmptyDefinitionsFail();
            ValidateFindTaskById();
        }

        private void ValidateAssignTasksSuccess()
        {
            PlayerTaskAssignmentResult result = _assignmentService.AssignTasks("player_01", BuildDefinitions());
            bool passed = result.Success && result.TaskList != null && result.TaskList.TotalCount == 3 && result.TaskList.RemainingCount == 3;

            LogResult("AssignTasksSuccess", passed, result);
        }

        private void ValidateProgressCountsAfterCompletion()
        {
            PlayerTaskAssignmentResult result = _assignmentService.AssignTasks("player_01", BuildDefinitions());
            OfficeTaskRuntimeState task = result.TaskList.FindTaskById("task_review_invoices");
            _completionService.CompleteTask(task, "player_01");

            bool passed = result.TaskList.TotalCount == 3 && result.TaskList.CompletedCount == 1 && result.TaskList.RemainingCount == 2 && !result.TaskList.AllCompleted;

            LogResult("ProgressCountsAfterCompletion", passed, result);
        }

        private void ValidateAllCompletedAfterAllTasksDone()
        {
            PlayerTaskAssignmentResult result = _assignmentService.AssignTasks("player_01", BuildDefinitions());

            foreach (OfficeTaskRuntimeState task in result.TaskList.Tasks)
                _completionService.CompleteTask(task, "player_01");

            bool passed = result.TaskList.TotalCount == 3 && result.TaskList.CompletedCount == 3 && result.TaskList.RemainingCount == 0 && result.TaskList.AllCompleted;

            LogResult("AllCompletedAfterAllTasksDone", passed, result);
        }

        private void ValidateMissingPlayerFails()
        {
            PlayerTaskAssignmentResult result = _assignmentService.AssignTasks("", BuildDefinitions());
            bool passed = !result.Success && result.TaskList == null;

            LogResult("MissingPlayerFails", passed, result);
        }

        private void ValidateEmptyDefinitionsFail()
        {
            PlayerTaskAssignmentResult result = _assignmentService.AssignTasks("player_01", new List<OfficeTaskDefinition>());
            bool passed = !result.Success && result.TaskList == null;

            LogResult("EmptyDefinitionsFail", passed, result);
        }

        private void ValidateFindTaskById()
        {
            PlayerTaskAssignmentResult result = _assignmentService.AssignTasks("player_01", BuildDefinitions());
            OfficeTaskRuntimeState task = result.TaskList.FindTaskById("task_check_server_logs");
            bool passed = task != null && task.Definition.RoomType == OfficeRoomType.ServerRoom;

            LogResult("FindTaskById", passed, result);
        }

        private static List<OfficeTaskDefinition> BuildDefinitions()
        {
            return new List<OfficeTaskDefinition>
            {
                new OfficeTaskDefinition("task_review_invoices", "Review invoices", OfficeRoomType.Accounting, 3f),
                new OfficeTaskDefinition("task_check_server_logs", "Check server logs", OfficeRoomType.ServerRoom, 4f),
                new OfficeTaskDefinition("task_sort_archive", "Sort archive files", OfficeRoomType.ArchiveRoom, 5f)
            };
        }

        private static void LogResult(string testName, bool passed, PlayerTaskAssignmentResult result)
        {
            if (passed)
                Debug.Log($"[PlayerTaskAssignmentValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[PlayerTaskAssignmentValidator] FAIL {testName}: {result}");
        }
    }
}
