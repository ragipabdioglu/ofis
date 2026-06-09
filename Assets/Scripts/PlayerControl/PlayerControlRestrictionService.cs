using OFIS.Players;

namespace OFIS.PlayerControl
{
    public sealed class PlayerControlRestrictionService
    {
        public PlayerControlRestrictionResult Evaluate(PlayerLifeState lifeState)
        {
            switch (lifeState)
            {
                case PlayerLifeState.Alive:
                    return new PlayerControlRestrictionResult(
                        canMove: true,
                        canInteract: true,
                        canUseMeetingVote: true,
                        isSpectatorLike: false,
                        reason: "Player is alive.");

                case PlayerLifeState.Dead:
                    return new PlayerControlRestrictionResult(
                        canMove: false,
                        canInteract: false,
                        canUseMeetingVote: false,
                        isSpectatorLike: true,
                        reason: "Player is dead and cannot control body interactions.");

                case PlayerLifeState.Disconnected:
                    return new PlayerControlRestrictionResult(
                        canMove: false,
                        canInteract: false,
                        canUseMeetingVote: false,
                        isSpectatorLike: false,
                        reason: "Player is disconnected.");

                default:
                    return new PlayerControlRestrictionResult(
                        canMove: false,
                        canInteract: false,
                        canUseMeetingVote: false,
                        isSpectatorLike: false,
                        reason: "Unknown player life state.");
            }
        }
    }
}
