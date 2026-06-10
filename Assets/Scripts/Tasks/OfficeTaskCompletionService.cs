namespace OFIS.Tasks
{
    public sealed class OfficeTaskCompletionService
    {
        public OfficeTaskCompletionResult CompleteTask(OfficeTaskRuntimeState taskState, string playerId)
        {
            if (taskState == null)
                return OfficeTaskCompletionResult.Failed("unknown_task", OfficeTaskState.None, "Task state is missing.");

            if (string.IsNullOrWhiteSpace(playerId))
                return OfficeTaskCompletionResult.Failed(taskState.Definition.TaskId, taskState.State, "Player id is missing.");

            if (taskState.AssignedPlayerId != "unassigned" && taskState.AssignedPlayerId != playerId)
            {
                return OfficeTaskCompletionResult.Failed(
                    taskState.Definition.TaskId,
                    taskState.State,
                    "Task is assigned to another player.");
            }

            if (taskState.State == OfficeTaskState.Completed)
            {
                return OfficeTaskCompletionResult.Failed(
                    taskState.Definition.TaskId,
                    taskState.State,
                    "Task is already completed.");
            }

            if (taskState.State == OfficeTaskState.Blocked)
            {
                return OfficeTaskCompletionResult.Failed(
                    taskState.Definition.TaskId,
                    taskState.State,
                    "Task is blocked.");
            }

            taskState.MarkCompleted();

            return new OfficeTaskCompletionResult(
                true,
                taskState.Definition.TaskId,
                taskState.State,
                "Task completed.");
        }
    }
}
