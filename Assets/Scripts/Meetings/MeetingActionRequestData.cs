namespace OFIS.Meetings
{
    public readonly struct MeetingActionRequestData
    {
        public string ActionId { get; }
        public string ProposerPlayerId { get; }
        public MeetingActionType ActionType { get; }
        public MeetingActionTargetData Target { get; }
        public string Reason { get; }

        public MeetingActionRequestData(
            string actionId,
            string proposerPlayerId,
            MeetingActionType actionType,
            MeetingActionTargetData target,
            string reason)
        {
            ActionId = string.IsNullOrWhiteSpace(actionId) ? string.Empty : actionId;
            ProposerPlayerId = string.IsNullOrWhiteSpace(proposerPlayerId)
                ? string.Empty
                : proposerPlayerId;
            ActionType = actionType;
            Target = target;
            Reason = string.IsNullOrWhiteSpace(reason) ? string.Empty : reason;
        }

        public override string ToString()
        {
            return $"ActionId={ActionId}, Proposer={ProposerPlayerId}, ActionType={ActionType}, Target=({Target}), Reason={Reason}";
        }
    }
}
