namespace OFIS.Tasks
{
    public sealed class TaskProgressUiStateService
    {
        public TaskProgressUiState Build(PlayerTaskList taskList)
        {
            if (taskList == null || taskList.TotalCount == 0)
            {
                return new TaskProgressUiState(
                    false,
                    0,
                    0,
                    0,
                    false,
                    "Tasks",
                    "0/0",
                    "No tasks assigned.");
            }

            string progressText = $"{taskList.CompletedCount}/{taskList.TotalCount}";
            string statusText = taskList.AllCompleted
                ? "All tasks completed."
                : $"{taskList.RemainingCount} tasks remaining.";

            return new TaskProgressUiState(
                true,
                taskList.TotalCount,
                taskList.CompletedCount,
                taskList.RemainingCount,
                taskList.AllCompleted,
                "Tasks",
                progressText,
                statusText);
        }
    }
}
