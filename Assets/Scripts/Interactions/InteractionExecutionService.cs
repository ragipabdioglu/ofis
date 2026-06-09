using OFIS.Players;

namespace OFIS.Interactions
{
    public sealed class InteractionExecutionService
    {
        private readonly InteractionPermissionService _permissionService = new InteractionPermissionService();

        public InteractionExecutionResult Execute(PlayerLifeState lifeState, WorldInteractionResolveResult resolveResult)
        {
            InteractionPermissionResult permission = _permissionService.Evaluate(lifeState, resolveResult);

            if (!permission.CanInteract)
                return InteractionExecutionResult.Failed(permission.Reason);

            WorldInteractionCandidate selected = resolveResult.SelectedCandidate;

            switch (selected.Type)
            {
                case WorldInteractionType.CorpseInspectOrCarry:
                    return Success(selected, "CorpseInspectOrCarry", "Corpse interaction execute stub selected.");

                case WorldInteractionType.MeetingJoin:
                    return Success(selected, "MeetingJoin", "Meeting join execute stub selected.");

                case WorldInteractionType.SabotageRepair:
                    return Success(selected, "SabotageRepair", "Sabotage repair execute stub selected.");

                case WorldInteractionType.Task:
                    return Success(selected, "Task", "Task execute stub selected.");

                case WorldInteractionType.Sabotage:
                    return Success(selected, "Sabotage", "Sabotage execute stub selected.");

                case WorldInteractionType.VictimNote:
                    return Success(selected, "VictimNote", "Victim note execute stub selected.");

                case WorldInteractionType.DoorPanel:
                    return Success(selected, "DoorPanel", "Door panel execute stub selected.");

                default:
                    return InteractionExecutionResult.Failed("Selected interaction type is not executable.");
            }
        }

        private static InteractionExecutionResult Success(WorldInteractionCandidate selected, string actionKey, string message)
        {
            return new InteractionExecutionResult(
                true,
                selected.Type,
                selected.DisplayName,
                actionKey,
                message);
        }
    }
}
