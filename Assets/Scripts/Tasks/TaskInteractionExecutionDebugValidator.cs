using System.Collections.Generic;
using OFIS.Interactions;
using OFIS.Players;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Tasks
{
    public sealed class TaskInteractionExecutionDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly LocalInteractionResolver _resolver = new LocalInteractionResolver();
        private readonly InteractionExecutionService _executionService = new InteractionExecutionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateTaskInteractionExecution();
        }

        [ContextMenu("Validate Task Interaction Execution")]
        public void ValidateTaskInteractionExecution()
        {
            ValidateTaskExecutionCompletesTask();
            ValidateAlreadyCompletedTaskFails();
            ValidateWrongPlayerFails();
            ValidateNonTaskSelectionFails();
            ValidateDeadPlayerBlocked();
        }

        private void ValidateTaskExecutionCompletesTask()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Available);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.Task, "Review invoices");
            InteractionExecutionResult result = _executionService.ExecuteTask(PlayerLifeState.Alive, selection, task, "player_01");
            bool passed = result.Success && task.State == OfficeTaskState.Completed && result.ActionKey == "TaskCompletion";

            LogResult("TaskExecutionCompletesTask", passed, result);
        }

        private void ValidateAlreadyCompletedTaskFails()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Completed);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.Task, "Review invoices");
            InteractionExecutionResult result = _executionService.ExecuteTask(PlayerLifeState.Alive, selection, task, "player_01");
            bool passed = !result.Success && result.Message.Contains("already completed");

            LogResult("AlreadyCompletedTaskFails", passed, result);
        }

        private void ValidateWrongPlayerFails()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Available);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.Task, "Review invoices");
            InteractionExecutionResult result = _executionService.ExecuteTask(PlayerLifeState.Alive, selection, task, "player_02");
            bool passed = !result.Success && result.Message.Contains("another player");

            LogResult("WrongPlayerFails", passed, result);
        }

        private void ValidateNonTaskSelectionFails()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Available);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.MeetingJoin, "Join meeting");
            InteractionExecutionResult result = _executionService.ExecuteTask(PlayerLifeState.Alive, selection, task, "player_01");
            bool passed = !result.Success && result.Message.Contains("not a task");

            LogResult("NonTaskSelectionFails", passed, result);
        }

        private void ValidateDeadPlayerBlocked()
        {
            OfficeTaskRuntimeState task = BuildTask("player_01", OfficeTaskState.Available);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.Task, "Review invoices");
            InteractionExecutionResult result = _executionService.ExecuteTask(PlayerLifeState.Dead, selection, task, "player_01");
            bool passed = !result.Success && task.State == OfficeTaskState.Available;

            LogResult("DeadPlayerBlocked", passed, result);
        }

        private WorldInteractionResolveResult BuildSelection(WorldInteractionType type, string displayName)
        {
            List<WorldInteractionCandidate> candidates = new List<WorldInteractionCandidate>
            {
                new WorldInteractionCandidate(type, displayName, 0.25f, true)
            };

            return _resolver.Resolve(candidates);
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

        private static void LogResult(string testName, bool passed, InteractionExecutionResult result)
        {
            if (passed)
                Debug.Log($"[TaskInteractionExecutionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[TaskInteractionExecutionValidator] FAIL {testName}: {result}");
        }
    }
}
