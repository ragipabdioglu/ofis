namespace OFIS.Tasks
{
    public readonly struct PlayerTaskAssignmentResult
    {
        public bool Success { get; }
        public PlayerTaskList TaskList { get; }
        public string Message { get; }

        public PlayerTaskAssignmentResult(bool success, PlayerTaskList taskList, string message)
        {
            Success = success;
            TaskList = taskList;
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public static PlayerTaskAssignmentResult Failed(string message)
        {
            return new PlayerTaskAssignmentResult(false, null, message);
        }

        public override string ToString()
        {
            string listText = TaskList == null ? "NoTaskList" : TaskList.ToString();
            return $"Success={Success}, {listText}, Message={Message}";
        }
    }
}
