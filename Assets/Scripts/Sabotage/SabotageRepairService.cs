namespace OFIS.Sabotage
{
    public sealed class SabotageRepairService
    {
        public SabotageRepairResult Repair(SabotageObjectiveRuntimeState sabotageState)
        {
            if (sabotageState == null)
                return SabotageRepairResult.Failed("unknown_sabotage", SabotageObjectiveState.None, "Sabotage state is missing.");

            if (sabotageState.State == SabotageObjectiveState.Inactive)
            {
                return SabotageRepairResult.Failed(
                    sabotageState.Definition.SabotageId,
                    sabotageState.State,
                    "Sabotage is not active.");
            }

            if (sabotageState.State == SabotageObjectiveState.Repaired)
            {
                return SabotageRepairResult.Failed(
                    sabotageState.Definition.SabotageId,
                    sabotageState.State,
                    "Sabotage is already repaired.");
            }

            if (sabotageState.State == SabotageObjectiveState.Expired)
            {
                return SabotageRepairResult.Failed(
                    sabotageState.Definition.SabotageId,
                    sabotageState.State,
                    "Sabotage has expired.");
            }

            sabotageState.MarkRepaired();

            return new SabotageRepairResult(
                true,
                sabotageState.Definition.SabotageId,
                sabotageState.State,
                "Sabotage repaired.");
        }
    }
}
