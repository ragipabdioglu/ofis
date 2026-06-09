using OFIS.PlayerControl;

namespace OFIS.Interactions
{
    public readonly struct InteractionPermissionResult
    {
        public bool CanInteract { get; }
        public WorldInteractionResolveResult ResolveResult { get; }
        public PlayerControlRestrictionResult RestrictionResult { get; }
        public string Reason { get; }

        public InteractionPermissionResult(
            bool canInteract,
            WorldInteractionResolveResult resolveResult,
            PlayerControlRestrictionResult restrictionResult,
            string reason)
        {
            CanInteract = canInteract;
            ResolveResult = resolveResult;
            RestrictionResult = restrictionResult;
            Reason = reason;
        }

        public override string ToString()
        {
            return $"CanInteract={CanInteract}, Reason={Reason}, Resolve={ResolveResult}, Restriction={RestrictionResult}";
        }
    }
}
