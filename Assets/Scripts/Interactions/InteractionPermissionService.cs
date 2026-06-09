using OFIS.PlayerControl;
using OFIS.Players;

namespace OFIS.Interactions
{
    public sealed class InteractionPermissionService
    {
        private readonly PlayerControlRestrictionService _restrictionService = new PlayerControlRestrictionService();

        public InteractionPermissionResult Evaluate(PlayerLifeState lifeState, WorldInteractionResolveResult resolveResult)
        {
            PlayerControlRestrictionResult restriction = _restrictionService.Evaluate(lifeState);

            if (!restriction.CanInteract)
            {
                return new InteractionPermissionResult(
                    canInteract: false,
                    resolveResult,
                    restriction,
                    restriction.Reason);
            }

            if (!resolveResult.HasSelection)
            {
                return new InteractionPermissionResult(
                    canInteract: false,
                    resolveResult,
                    restriction,
                    "No selected interaction.");
            }

            return new InteractionPermissionResult(
                canInteract: true,
                resolveResult,
                restriction,
                "Player can interact with selected candidate.");
        }
    }
}
