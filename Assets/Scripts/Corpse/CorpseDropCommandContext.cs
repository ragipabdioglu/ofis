using OFIS.Core.Ids;
using OFIS.Players;
using UnityEngine;

namespace OFIS.Corpse
{
    public readonly struct CorpseDropCommandContext
    {
        public string CommandId { get; }
        public PlayerId CarrierPlayerId { get; }
        public PlayerLifeState CarrierLifeState { get; }
        public CorpseCarryState CarryState { get; }
        public Vector3 DropWorldPosition { get; }

        public CorpseDropCommandContext(
            string commandId,
            PlayerId carrierPlayerId,
            PlayerLifeState carrierLifeState,
            CorpseCarryState carryState,
            Vector3 dropWorldPosition)
        {
            CommandId = string.IsNullOrWhiteSpace(commandId) ? "unknown_drop_command" : commandId;
            CarrierPlayerId = carrierPlayerId;
            CarrierLifeState = carrierLifeState;
            CarryState = carryState;
            DropWorldPosition = dropWorldPosition;
        }
    }
}
