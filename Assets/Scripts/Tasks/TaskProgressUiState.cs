namespace OFIS.Tasks
{
    public readonly struct TaskProgressUiState
    {
        public bool HasTasks { get; }
        public int TotalCount { get; }
        public int CompletedCount { get; }
        public int RemainingCount { get; }
        public bool AllCompleted { get; }
        public string HeaderText { get; }
        public string ProgressText { get; }
        public string StatusText { get; }

        public TaskProgressUiState(
            bool hasTasks,
            int totalCount,
            int completedCount,
            int remainingCount,
            bool allCompleted,
            string headerText,
            string progressText,
            string statusText)
        {
            HasTasks = hasTasks;
            TotalCount = totalCount < 0 ? 0 : totalCount;
            CompletedCount = completedCount < 0 ? 0 : completedCount;
            RemainingCount = remainingCount < 0 ? 0 : remainingCount;
            AllCompleted = allCompleted;
            HeaderText = string.IsNullOrWhiteSpace(headerText) ? "Tasks" : headerText;
            ProgressText = string.IsNullOrWhiteSpace(progressText) ? "0/0" : progressText;
            StatusText = string.IsNullOrWhiteSpace(statusText) ? "No status." : statusText;
        }

        public override string ToString()
        {
            return $"HasTasks={HasTasks}, Total={TotalCount}, Completed={CompletedCount}, Remaining={RemainingCount}, AllCompleted={AllCompleted}, Header={HeaderText}, Progress={ProgressText}, Status={StatusText}";
        }
    }
}
