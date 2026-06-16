namespace OFIS.Meetings
{
    public readonly struct MeetingActionPanelCommand
    {
        public string CommandId { get; }
        public string MeetingId { get; }
        public string ProposerPlayerId { get; }
        public MeetingActionType ActionType { get; }
        public MeetingActionTargetData Target { get; }
        public string Reason { get; }

        public MeetingActionPanelCommand(
            string commandId,
            string meetingId,
            string proposerPlayerId,
            MeetingActionType actionType,
            MeetingActionTargetData target,
            string reason)
        {
            CommandId = string.IsNullOrWhiteSpace(commandId) ? string.Empty : commandId;
            MeetingId = string.IsNullOrWhiteSpace(meetingId) ? string.Empty : meetingId;
            ProposerPlayerId = string.IsNullOrWhiteSpace(proposerPlayerId)
                ? string.Empty
                : proposerPlayerId;
            ActionType = actionType;
            Target = target;
            Reason = string.IsNullOrWhiteSpace(reason) ? string.Empty : reason;
        }

        public override string ToString()
        {
            return $"CommandId={CommandId}, MeetingId={MeetingId}, Proposer={ProposerPlayerId}, Action={ActionType}, Target=({Target}), Reason={Reason}";
        }
    }
}
