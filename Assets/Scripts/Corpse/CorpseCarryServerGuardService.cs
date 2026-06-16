using OFIS.Players;
using OFIS.Roles;
using OFIS.Rooms;

namespace OFIS.Corpse
{
    public sealed class CorpseCarryServerGuardService
    {
        public CorpseCarryCommandResult TryStartCarry(CorpseCarryCommandContext context)
        {
            if (string.IsNullOrWhiteSpace(context.CommandId))
                return CorpseCarryCommandResult.Rejected("Carry command id is required.");

            if (string.IsNullOrWhiteSpace(context.CarrierPlayerId.Value))
                return CorpseCarryCommandResult.Rejected("Carrier player id is required.");

            if (context.CarrierRole != PlayerRole.Killer)
                return CorpseCarryCommandResult.Rejected($"Only Killer can carry corpse. Role={context.CarrierRole}");

            if (context.CarrierLifeState != PlayerLifeState.Alive)
                return CorpseCarryCommandResult.Rejected($"Carrier is not alive. LifeState={context.CarrierLifeState}");

            if (context.TargetCorpse == null)
                return CorpseCarryCommandResult.Rejected("Target corpse is required.");

            if (!context.TargetCorpse.IsPublicWorldObject)
                return CorpseCarryCommandResult.Rejected("Target corpse is not a public world object.");

            if (context.AlreadyCarryingCorpse)
                return CorpseCarryCommandResult.Rejected("Carrier is already carrying a corpse.");

            if (!context.RoomAllowsCarry || context.CarrierRoom == OfficeRoomType.MeetingRoom)
                return CorpseCarryCommandResult.Rejected($"Room blocks corpse carry. Room={context.CarrierRoom}");

            return CorpseCarryCommandResult.Accepted(context.TargetCorpse);
        }
    }
}
