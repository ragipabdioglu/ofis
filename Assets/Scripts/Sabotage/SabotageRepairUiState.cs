namespace OFIS.Sabotage
{
    public readonly struct SabotageRepairUiState
    {
        public bool HasSabotage { get; }
        public SabotageObjectiveState State { get; }
        public string HeaderText { get; }
        public string StatusText { get; }
        public string ActionHintText { get; }
        public bool CanShowRepairPrompt { get; }

        public SabotageRepairUiState(
            bool hasSabotage,
            SabotageObjectiveState state,
            string headerText,
            string statusText,
            string actionHintText,
            bool canShowRepairPrompt)
        {
            HasSabotage = hasSabotage;
            State = state;
            HeaderText = string.IsNullOrWhiteSpace(headerText) ? "Sabotage" : headerText;
            StatusText = string.IsNullOrWhiteSpace(statusText) ? "No status." : statusText;
            ActionHintText = string.IsNullOrWhiteSpace(actionHintText) ? "" : actionHintText;
            CanShowRepairPrompt = canShowRepairPrompt;
        }

        public override string ToString()
        {
            return $"HasSabotage={HasSabotage}, State={State}, Header={HeaderText}, Status={StatusText}, ActionHint={ActionHintText}, CanShowRepairPrompt={CanShowRepairPrompt}";
        }
    }
}
