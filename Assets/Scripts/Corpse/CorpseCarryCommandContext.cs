using OFIS.Core.Ids;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Rooms;

namespace OFIS.Corpse
{
    public readonly struct CorpseCarryCommandContext
    {
        public string CommandId { get; }
        public PlayerId CarrierPlayerId { get; }
        public PlayerRole CarrierRole { get; }
        public PlayerLifeState CarrierLifeState { get; }
        public CorpsePlaceholder TargetCorpse { get; }
        public bool AlreadyCarryingCorpse { get; }
        public OfficeRoomType CarrierRoom { get; }
        public bool RoomAllowsCarry { get; }

        public CorpseCarryCommandContext(
            string commandId,
            PlayerId carrierPlayerId,
            PlayerRole carrierRole,
            PlayerLifeState carrierLifeState,
            CorpsePlaceholder targetCorpse,
            bool alreadyCarryingCorpse,
            OfficeRoomType carrierRoom,
            bool roomAllowsCarry)
        {
            CommandId = string.IsNullOrWhiteSpace(commandId) ? "unknown_carry_command" : commandId;
            CarrierPlayerId = carrierPlayerId;
            CarrierRole = carrierRole;
            CarrierLifeState = carrierLifeState;
            TargetCorpse = targetCorpse;
            AlreadyCarryingCorpse = alreadyCarryingCorpse;
            CarrierRoom = carrierRoom;
            RoomAllowsCarry = roomAllowsCarry;
        }
    }
}
