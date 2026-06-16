using OFIS.Core.Ids;
using OFIS.Players;

namespace OFIS.Corpse
{
    public readonly struct CorpseHideCommandContext
    {
        public string CommandId { get; }
        public PlayerId CarrierPlayerId { get; }
        public PlayerLifeState CarrierLifeState { get; }
        public CorpseCarryState CarryState { get; }
        public CorpseHideSpotState HideSpot { get; }

        public CorpseHideCommandContext(
            string commandId,
            PlayerId carrierPlayerId,
            PlayerLifeState carrierLifeState,
            CorpseCarryState carryState,
            CorpseHideSpotState hideSpot)
        {
            CommandId = string.IsNullOrWhiteSpace(commandId) ? "unknown_hide_command" : commandId;
            CarrierPlayerId = carrierPlayerId;
            CarrierLifeState = carrierLifeState;
            CarryState = carryState;
            HideSpot = hideSpot;
        }
    }
}
