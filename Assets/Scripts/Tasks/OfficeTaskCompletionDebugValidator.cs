using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Tasks
{
    public sealed class OfficeTaskCompletionDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly OfficeTaskCompletionService _completionService = new OfficeTaskCompletionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateTaskCompletionCore();
        }

        [ContextMenu("Validate Task Completion Core")]
        public void ValidateTaskCompletionCore()
        {
            ValidateCompleteAvailableTask();
            ValidateAlreadyCompletedTaskBlocked();
            ValidateWrongPlayerBlocked();
            ValidateBlockedTaskBlocked();
            ValidateMissingTaskBlocked();
        }

        private void ValidateCompleteAvailableTask()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Available);
            OfficeTaskCompletionResult result = _completionService.CompleteTask(task, "player_01");
            bool passed = result.Success && task.State == OfficeTaskState.Completed;

            LogResult("CompleteAvailableTask", passed, result);
        }

        private void ValidateAlreadyCompletedTaskBlocked()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Completed);
            OfficeTaskCompletionResult result = _completionService.CompleteTask(task, "player_01");
            bool passed = !result.Success && result.NewState == OfficeTaskState.Completed;

            LogResult("AlreadyCompletedTaskBlocked", passed, result);
        }

        private void ValidateWrongPlayerBlocked()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Available);
            OfficeTaskCompletionResult result = _completionService.CompleteTask(task, "player_02");
            bool passed = !result.Success && task.State == OfficeTaskState.Available;

            LogResult("WrongPlayerBlocked", passed, result);
        }

        private void ValidateBlockedTaskBlocked()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Blocked);
            OfficeTaskCompletionResult result = _completionService.CompleteTask(task, "player_01");
            bool passed = !result.Success && result.NewState == OfficeTaskState.Blocked;

            LogResult("BlockedTaskBlocked", passed, result);
        }

        private void ValidateMissingTaskBlocked()
        {
            OfficeTaskCompletionResult result = _completionService.CompleteTask(null, "player_01");
            bool passed = !result.Success && result.NewState == OfficeTaskState.None;

            LogResult("MissingTaskBlocked", passed, result);
        }

        private static OfficeTaskRuntimeState BuildTask(string assignedPlayerId, OfficeTaskState initialState)
        {
            OfficeTaskDefinition definition = new OfficeTaskDefinition(
                "task_review_invoices",
                "Review invoices",
                OfficeRoomType.Accounting,
                3f);

            return new OfficeTaskRuntimeState(definition, assignedPlayerId, initialState);
        }

        private static void LogResult(string testName, bool passed, OfficeTaskCompletionResult result)
        {
            if (passed)
                Debug.Log($"[OfficeTaskCompletionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[OfficeTaskCompletionValidator] FAIL {testName}: {result}");
        }
    }
}
