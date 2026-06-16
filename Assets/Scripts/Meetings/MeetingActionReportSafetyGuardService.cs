namespace OFIS.Meetings
{
    public sealed class MeetingActionReportSafetyGuardService
    {
        public MeetingActionReportSafetyResult Evaluate(MeetingActionReportPanelState state)
        {
            string visibleText = BuildVisibleText(state);
            bool revealsRole = ContainsUnsafeToken(visibleText, "role")
                || ContainsUnsafeToken(visibleText, "impostor")
                || ContainsUnsafeToken(visibleText, "crewmate");
            bool revealsDefiniteKiller = ContainsUnsafeToken(visibleText, "killer")
                || ContainsUnsafeToken(visibleText, "murderer")
                || ContainsUnsafeToken(visibleText, "culprit");

            bool isSafe = state.IsRoleSafe
                && !state.RevealsDefiniteKiller
                && !revealsRole
                && !revealsDefiniteKiller;

            string message = isSafe
                ? "Report panel is safe to show."
                : "Report panel contains unsafe role or definite killer wording.";

            return new MeetingActionReportSafetyResult(
                isSafe,
                revealsRole,
                revealsDefiniteKiller,
                message);
        }

        private static string BuildVisibleText(MeetingActionReportPanelState state)
        {
            return state.HeaderText
                + " "
                + state.ActionSummaryText
                + " "
                + state.TargetSummaryText
                + " "
                + state.ResolutionSummaryText
                + " "
                + state.EffectSummaryText;
        }

        private static bool ContainsUnsafeToken(string source, string token)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(token))
                return false;

            return source.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
