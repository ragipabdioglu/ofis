namespace OFIS.Tasks
{
    public readonly struct OfficeTaskCompletionResult
    {
        public bool Success { get; }
        public string TaskId { get; }
        public OfficeTaskState NewState { get; }
        public string Message { get; }

        public OfficeTaskCompletionResult(bool success, string taskId, OfficeTaskState newState, string message)
        {
            Success = success;
            TaskId = string.IsNullOrWhiteSpace(taskId) ? "unknown_task" : taskId;
            NewState = newState;
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public static OfficeTaskCompletionResult Failed(string taskId, OfficeTaskState state, string message)
        {
            return new OfficeTaskCompletionResult(false, taskId, state, message);
        }

        public override string ToString()
        {
            return $"Success={Success}, TaskId={TaskId}, NewState={NewState}, Message={Message}";
        }
    }
}
