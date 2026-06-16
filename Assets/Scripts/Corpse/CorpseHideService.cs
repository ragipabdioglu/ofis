using OFIS.Players;

namespace OFIS.Corpse
{
    public sealed class CorpseHideService
    {
        public CorpseHideCommandResult Hide(CorpseHideCommandContext context)
        {
            if (string.IsNullOrWhiteSpace(context.CommandId))
                return CorpseHideCommandResult.Rejected("Hide command id is required.");

            if (string.IsNullOrWhiteSpace(context.CarrierPlayerId.Value))
                return CorpseHideCommandResult.Rejected("Carrier player id is required.");

            if (context.CarrierLifeState != PlayerLifeState.Alive)
                return CorpseHideCommandResult.Rejected($"Carrier is not alive. LifeState={context.CarrierLifeState}");

            if (context.CarryState == null)
                return CorpseHideCommandResult.Rejected("Carry state is required.");

            if (!context.CarryState.IsCarrying || context.CarryState.CarriedCorpse == null)
                return CorpseHideCommandResult.Rejected("Carrier is not carrying a corpse.");

            if (context.HideSpot == null)
                return CorpseHideCommandResult.Rejected("Hide spot is required.");

            if (!context.HideSpot.IsActive)
                return CorpseHideCommandResult.Rejected("Hide spot is inactive.");

            if (context.HideSpot.HasHiddenCorpse)
                return CorpseHideCommandResult.Rejected("Hide spot already contains a corpse.");

            CorpsePlaceholder corpse = context.CarryState.DropCarriedCorpse();

            if (corpse == null)
                return CorpseHideCommandResult.Rejected("No corpse was available to hide.");

            corpse.transform.position = context.HideSpot.WorldPosition;
            corpse.SetPublicWorldObject(false);
            context.HideSpot.StoreCorpse(corpse);

            return CorpseHideCommandResult.Hidden(
                corpse,
                context.HideSpot.HideSpotId,
                !context.CarryState.IsCarrying);
        }
    }
}
