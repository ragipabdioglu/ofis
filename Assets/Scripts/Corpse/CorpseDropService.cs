using OFIS.Players;

namespace OFIS.Corpse
{
    public sealed class CorpseDropService
    {
        public CorpseDropCommandResult Drop(CorpseDropCommandContext context)
        {
            if (string.IsNullOrWhiteSpace(context.CommandId))
                return CorpseDropCommandResult.Rejected("Drop command id is required.");

            if (string.IsNullOrWhiteSpace(context.CarrierPlayerId.Value))
                return CorpseDropCommandResult.Rejected("Carrier player id is required.");

            if (context.CarrierLifeState != PlayerLifeState.Alive)
                return CorpseDropCommandResult.Rejected($"Carrier is not alive. LifeState={context.CarrierLifeState}");

            if (context.CarryState == null)
                return CorpseDropCommandResult.Rejected("Carry state is required.");

            if (!context.CarryState.IsCarrying || context.CarryState.CarriedCorpse == null)
                return CorpseDropCommandResult.Rejected("Carrier is not carrying a corpse.");

            CorpsePlaceholder droppedCorpse = context.CarryState.DropCarriedCorpse();

            if (droppedCorpse == null)
                return CorpseDropCommandResult.Rejected("No corpse was dropped.");

            droppedCorpse.transform.position = context.DropWorldPosition;

            return CorpseDropCommandResult.Dropped(
                droppedCorpse,
                context.DropWorldPosition,
                !context.CarryState.IsCarrying);
        }
    }
}
