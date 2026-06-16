namespace OFIS.Meetings
{
    public readonly struct MeetingProductionBridgeResult
    {
        public MeetingRuntimeDecisionResult RuntimeDecisionResult { get; }
        public MeetingProductionBridgeCommand Command { get; }
        public bool HasCommand { get; }
        public bool IsTerminalCommand { get; }
        public string Message { get; }

        public MeetingProductionBridgeResult(
            MeetingRuntimeDecisionResult runtimeDecisionResult,
            MeetingProductionBridgeCommand command,
            bool hasCommand,
            string message)
        {
            RuntimeDecisionResult = runtimeDecisionResult;
            Command = command;
            HasCommand = hasCommand;
            IsTerminalCommand = command.ShouldCloseMeeting
                || command.ShouldResolveWinBranch
                || command.ShouldRunMeetingEndPipeline;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting production bridge result completed."
                : message;
        }

        public override string ToString()
        {
            return $"HasCommand={HasCommand}, Terminal={IsTerminalCommand}, Command=[{Command}], Runtime=[{RuntimeDecisionResult}], Message={Message}";
        }
    }
}
