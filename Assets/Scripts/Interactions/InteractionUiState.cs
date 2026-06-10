namespace OFIS.Interactions
{
    public readonly struct InteractionUiState
    {
        public bool HasSelection { get; }
        public bool CanInteract { get; }
        public WorldInteractionType InteractionType { get; }
        public string PromptText { get; }
        public string StatusText { get; }
        public string LastActionText { get; }

        public InteractionUiState(
            bool hasSelection,
            bool canInteract,
            WorldInteractionType interactionType,
            string promptText,
            string statusText,
            string lastActionText)
        {
            HasSelection = hasSelection;
            CanInteract = canInteract;
            InteractionType = interactionType;
            PromptText = string.IsNullOrWhiteSpace(promptText) ? "No interaction" : promptText;
            StatusText = string.IsNullOrWhiteSpace(statusText) ? "Idle" : statusText;
            LastActionText = string.IsNullOrWhiteSpace(lastActionText) ? "No action yet" : lastActionText;
        }

        public override string ToString()
        {
            return $"HasSelection={HasSelection}, CanInteract={CanInteract}, Type={InteractionType}, Prompt={PromptText}, Status={StatusText}, LastAction={LastActionText}";
        }
    }
}
