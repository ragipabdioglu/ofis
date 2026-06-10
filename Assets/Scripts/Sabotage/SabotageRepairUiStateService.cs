namespace OFIS.Sabotage
{
    public sealed class SabotageRepairUiStateService
    {
        public SabotageRepairUiState Build(SabotageObjectiveRuntimeState sabotageState)
        {
            if (sabotageState == null)
            {
                return new SabotageRepairUiState(
                    false,
                    SabotageObjectiveState.None,
                    "Sabotage",
                    "No sabotage data.",
                    "",
                    false);
            }

            switch (sabotageState.State)
            {
                case SabotageObjectiveState.Inactive:
                    return BuildState(sabotageState, "No active sabotage.", "", false);

                case SabotageObjectiveState.Active:
                    return BuildState(sabotageState, "Sabotage active.", "Repair sabotage", true);

                case SabotageObjectiveState.Repairing:
                    return BuildState(sabotageState, "Repair in progress.", "Continue repair", true);

                case SabotageObjectiveState.Repaired:
                    return BuildState(sabotageState, "Sabotage repaired.", "", false);

                case SabotageObjectiveState.Expired:
                    return BuildState(sabotageState, "Sabotage expired.", "", false);

                default:
                    return BuildState(sabotageState, "Unknown sabotage state.", "", false);
            }
        }

        private static SabotageRepairUiState BuildState(
            SabotageObjectiveRuntimeState sabotageState,
            string statusText,
            string actionHintText,
            bool canShowRepairPrompt)
        {
            return new SabotageRepairUiState(
                true,
                sabotageState.State,
                sabotageState.Definition.DisplayName,
                statusText,
                actionHintText,
                canShowRepairPrompt);
        }
    }
}
