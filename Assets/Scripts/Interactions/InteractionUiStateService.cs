using OFIS.Players;

namespace OFIS.Interactions
{
    public sealed class InteractionUiStateService
    {
        private readonly InteractionPermissionService _permissionService = new InteractionPermissionService();

        public InteractionUiState Build(PlayerLifeState lifeState, WorldInteractionResolveResult resolveResult, InteractionExecutionResult lastExecutionResult)
        {
            InteractionPermissionResult permission = _permissionService.Evaluate(lifeState, resolveResult);

            if (!resolveResult.HasSelection)
            {
                return new InteractionUiState(
                    hasSelection: false,
                    canInteract: false,
                    interactionType: WorldInteractionType.None,
                    promptText: "No interaction",
                    statusText: permission.Reason,
                    lastActionText: lastExecutionResult.Message);
            }

            WorldInteractionCandidate selected = resolveResult.SelectedCandidate;
            string prompt = permission.CanInteract
                ? $"Press E: {selected.DisplayName}"
                : $"Blocked: {selected.DisplayName}";

            string status = permission.CanInteract
                ? "Ready"
                : permission.Reason;

            return new InteractionUiState(
                hasSelection: true,
                canInteract: permission.CanInteract,
                interactionType: selected.Type,
                promptText: prompt,
                statusText: status,
                lastActionText: lastExecutionResult.Message);
        }
    }
}
