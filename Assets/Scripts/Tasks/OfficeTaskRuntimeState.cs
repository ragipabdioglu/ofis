namespace OFIS.Tasks
{
    public sealed class OfficeTaskRuntimeState
    {
        public OfficeTaskDefinition Definition { get; }
        public OfficeTaskState State { get; private set; }
        public string AssignedPlayerId { get; }

        public bool IsCompleted => State == OfficeTaskState.Completed;

        public OfficeTaskRuntimeState(OfficeTaskDefinition definition, string assignedPlayerId, OfficeTaskState initialState = OfficeTaskState.Available)
        {
            Definition = definition;
            AssignedPlayerId = string.IsNullOrWhiteSpace(assignedPlayerId) ? "unassigned" : assignedPlayerId;
            State = initialState == OfficeTaskState.None ? OfficeTaskState.Available : initialState;
        }

        public void MarkCompleted()
        {
            State = OfficeTaskState.Completed;
        }

        public void MarkBlocked()
        {
            State = OfficeTaskState.Blocked;
        }

        public override string ToString()
        {
            return $"Task={Definition.DisplayName}, State={State}, AssignedPlayerId={AssignedPlayerId}";
        }
    }
}
