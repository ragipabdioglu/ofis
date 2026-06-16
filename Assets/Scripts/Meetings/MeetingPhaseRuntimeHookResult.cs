namespace OFIS.Meetings
{
    public readonly struct MeetingPhaseRuntimeHookResult
    {
        public MeetingPhaseRuntimeHookState State { get; }
        public bool HasPipelineResult { get; }
        public MeetingEndPipelineResult PipelineResult { get; }
        public string Message { get; }

        public MeetingPhaseRuntimeHookResult(
            MeetingPhaseRuntimeHookState state,
            bool hasPipelineResult,
            MeetingEndPipelineResult pipelineResult,
            string message)
        {
            State = state;
            HasPipelineResult = hasPipelineResult;
            PipelineResult = pipelineResult;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting phase runtime hook tick completed."
                : message;
        }

        public override string ToString()
        {
            if (!HasPipelineResult)
                return $"State=[{State}], Pipeline=None, Message={Message}";

            return $"State=[{State}], Pipeline=[{PipelineResult}], Message={Message}";
        }
    }
}
