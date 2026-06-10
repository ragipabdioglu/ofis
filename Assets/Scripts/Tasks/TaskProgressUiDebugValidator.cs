using System.Collections.Generic;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Tasks
{
    public sealed class TaskProgressUiDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly PlayerTaskAssignmentService _assignmentService = new PlayerTaskAssignmentService();
        private readonly OfficeTaskCompletionService _completionService = new OfficeTaskCompletionService();
        private readonly TaskProgressUiStateService _uiStateService = new TaskProgressUiStateService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateTaskProgressUi();
        }

        [ContextMenu("Validate Task Progress UI")]
        public void ValidateTaskProgressUi()
        {
            ValidateNoTasksState();
            ValidateInitialProgressState();
            ValidatePartialProgressState();
            ValidateAllCompletedState();
        }

        private void ValidateNoTasksState()
        {
            TaskProgressUiState state = _uiStateService.Build(null);
            bool passed = !state.HasTasks && state.TotalCount == 0 && state.ProgressText == "0/0";

            LogResult("NoTasksState", passed, state);
        }

        private void ValidateInitialProgressState()
        {
            PlayerTaskList taskList = BuildTaskList();
            TaskProgressUiState state = _uiStateService.Build(taskList);
            bool passed = state.HasTasks && state.TotalCount == 3 && state.CompletedCount == 0 && state.RemainingCount == 3 && state.ProgressText == "0/3";

            LogResult("InitialProgressState", passed, state);
        }

        private void ValidatePartialProgressState()
        {
            PlayerTaskList taskList = BuildTaskList();
            _completionService.CompleteTask(taskList.FindTaskById("task_review_invoices"), "player_01");

            TaskProgressUiState state = _uiStateService.Build(taskList);
            bool passed = state.HasTasks && state.TotalCount == 3 && state.CompletedCount == 1 && state.RemainingCount == 2 && state.ProgressText == "1/3" && !state.AllCompleted;

            LogResult("PartialProgressState", passed, state);
        }

        private void ValidateAllCompletedState()
        {
            PlayerTaskList taskList = BuildTaskList();

            foreach (OfficeTaskRuntimeState task in taskList.Tasks)
                _completionService.CompleteTask(task, "player_01");

            TaskProgressUiState state = _uiStateService.Build(taskList);
            bool passed = state.HasTasks && state.TotalCount == 3 && state.CompletedCount == 3 && state.RemainingCount == 0 && state.ProgressText == "3/3" && state.AllCompleted;

            LogResult("AllCompletedState", passed, state);
        }

        private PlayerTaskList BuildTaskList()
        {
            PlayerTaskAssignmentResult assignment = _assignmentService.AssignTasks("player_01", BuildDefinitions());
            return assignment.TaskList;
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

        private static void LogResult(string testName, bool passed, TaskProgressUiState state)
        {
            if (passed)
                Debug.Log($"[TaskProgressUiValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[TaskProgressUiValidator] FAIL {testName}: {state}");
        }
    }
}
