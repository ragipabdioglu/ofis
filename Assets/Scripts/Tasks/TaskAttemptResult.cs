namespace OFIS.Tasks
{
    public enum TaskAttemptResult
    {
        None = 0,
        Started = 10,
        Blocked = 20,
        Completed = 30,
        FaultyCompleted = 40
    }
}