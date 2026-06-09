using OFIS.Core.Ids;
using OFIS.Roles.Identity;

namespace OFIS.Players
{
    public sealed class PlayerPublicState
    {
        public PlayerId PlayerId { get; }
        public string DisplayName { get; }
        public PlayerPublicIdentity PublicIdentity { get; }
        public PlayerLifeState LifeState { get; }

        public PlayerPublicState(
            PlayerId playerId,
            string displayName,
            PlayerPublicIdentity publicIdentity,
            PlayerLifeState lifeState)
        {
            PlayerId = playerId;
            DisplayName = displayName;
            PublicIdentity = publicIdentity;
            LifeState = lifeState;
        }

        public override string ToString()
        {
            string identityText = PublicIdentity != null
                ? PublicIdentity.ToString()
                : "No public identity";

            return $"{DisplayName} | {identityText} | Life={LifeState}";
        }
    }
}