using OFIS.Corpse;
using OFIS.Players;

namespace OFIS.PlayerControl
{
    public sealed class PlayerMovementModifierService
    {
        private const float NormalSpeedMultiplier = 1f;
        private const float CorpseCarrySpeedMultiplier = 0.65f;
        private const float BlockedSpeedMultiplier = 0f;

        private readonly PlayerControlRestrictionService _restrictionService = new PlayerControlRestrictionService();

        public PlayerMovementModifierResult Evaluate(PlayerLifeState lifeState, CorpseCarryState carryState)
        {
            PlayerControlRestrictionResult restriction = _restrictionService.Evaluate(lifeState);

            if (!restriction.CanMove)
            {
                return new PlayerMovementModifierResult(
                    canMove: false,
                    speedMultiplier: BlockedSpeedMultiplier,
                    reason: restriction.Reason);
            }

            bool isCarryingCorpse = carryState != null && carryState.IsCarrying;

            if (isCarryingCorpse)
            {
                return new PlayerMovementModifierResult(
                    canMove: true,
                    speedMultiplier: CorpseCarrySpeedMultiplier,
                    reason: "Player is carrying a corpse.");
            }

            return new PlayerMovementModifierResult(
                canMove: true,
                speedMultiplier: NormalSpeedMultiplier,
                reason: "Normal movement.");
        }
    }
}
