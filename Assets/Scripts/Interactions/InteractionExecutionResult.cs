namespace OFIS.Interactions
{
    public readonly struct InteractionExecutionResult
    {
        public bool Success { get; }
        public WorldInteractionType InteractionType { get; }
        public string DisplayName { get; }
        public string ActionKey { get; }
        public string Message { get; }

        public InteractionExecutionResult(
            bool success,
            WorldInteractionType interactionType,
            string displayName,
            string actionKey,
            string message)
        {
            Success = success;
            InteractionType = interactionType;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? interactionType.ToString() : displayName;
            ActionKey = string.IsNullOrWhiteSpace(actionKey) ? "None" : actionKey;
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public static InteractionExecutionResult Failed(string message)
        {
            return new InteractionExecutionResult(false, WorldInteractionType.None, "None", "Failed", message);
        }

        public override string ToString()
        {
            return $"Success={Success}, Type={InteractionType}, DisplayName={DisplayName}, ActionKey={ActionKey}, Message={Message}";
        }
    }
}
