namespace OFIS.Meetings
{
    public readonly struct MeetingActionPanelCommandResult
    {
        public bool Success { get; }
        public MeetingActionPanelCommand Command { get; }
        public MeetingActionProposalCreateResult ProposalResult { get; }
        public string Message { get; }

        public MeetingActionPanelCommandResult(
            bool success,
            MeetingActionPanelCommand command,
            MeetingActionProposalCreateResult proposalResult,
            string message)
        {
            Success = success;
            Command = command;
            ProposalResult = proposalResult;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action panel command resolved."
                : message;
        }

        public override string ToString()
        {
            return $"Success={Success}, Command=({Command}), ProposalResult=({ProposalResult}), Message={Message}";
        }
    }
}
