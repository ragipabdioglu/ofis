namespace OFIS.Sabotage
{
    public readonly struct SabotageRepairResult
    {
        public bool Success { get; }
        public string SabotageId { get; }
        public SabotageObjectiveState NewState { get; }
        public string Message { get; }

        public SabotageRepairResult(bool success, string sabotageId, SabotageObjectiveState newState, string message)
        {
            Success = success;
            SabotageId = string.IsNullOrWhiteSpace(sabotageId) ? "unknown_sabotage" : sabotageId;
            NewState = newState;
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public static SabotageRepairResult Failed(string sabotageId, SabotageObjectiveState state, string message)
        {
            return new SabotageRepairResult(false, sabotageId, state, message);
        }

        public override string ToString()
        {
            return $"Success={Success}, SabotageId={SabotageId}, NewState={NewState}, Message={Message}";
        }
    }
}
