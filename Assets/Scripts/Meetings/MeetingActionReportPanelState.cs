namespace OFIS.Meetings
{
    public readonly struct MeetingActionReportPanelState
    {
        public string HeaderText { get; }
        public string ActionSummaryText { get; }
        public string TargetSummaryText { get; }
        public string ResolutionSummaryText { get; }
        public string EffectSummaryText { get; }
        public bool HasResolvedAction { get; }
        public bool HasAppliedEffect { get; }
        public bool IsRoleSafe { get; }
        public bool RevealsDefiniteKiller { get; }

        public MeetingActionReportPanelState(
            string headerText,
            string actionSummaryText,
            string targetSummaryText,
            string resolutionSummaryText,
            string effectSummaryText,
            bool hasResolvedAction,
            bool hasAppliedEffect,
            bool isRoleSafe,
            bool revealsDefiniteKiller)
        {
            HeaderText = string.IsNullOrWhiteSpace(headerText) ? "Meeting Action Report" : headerText;
            ActionSummaryText = string.IsNullOrWhiteSpace(actionSummaryText)
                ? "No official action."
                : actionSummaryText;
            TargetSummaryText = string.IsNullOrWhiteSpace(targetSummaryText)
                ? "No target."
                : targetSummaryText;
            ResolutionSummaryText = string.IsNullOrWhiteSpace(resolutionSummaryText)
                ? "No resolution."
                : resolutionSummaryText;
            EffectSummaryText = string.IsNullOrWhiteSpace(effectSummaryText)
                ? "No effect applied."
                : effectSummaryText;
            HasResolvedAction = hasResolvedAction;
            HasAppliedEffect = hasAppliedEffect;
            IsRoleSafe = isRoleSafe;
            RevealsDefiniteKiller = revealsDefiniteKiller;
        }

        public override string ToString()
        {
            return $"Header={HeaderText}, Action={ActionSummaryText}, Target={TargetSummaryText}, Resolution={ResolutionSummaryText}, Effect={EffectSummaryText}, RoleSafe={IsRoleSafe}, RevealsKiller={RevealsDefiniteKiller}";
        }
    }
}
