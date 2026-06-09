using OFIS.Players;
using OFIS.Roles;
using OFIS.Roles.Identity;

namespace OFIS.MatchContext
{
    public sealed class PlayerRegistryEntry
    {
        public MockLobbyPlayer LobbyPlayer { get; }
        public PlayerPublicIdentity PublicIdentity { get; }
        public PlayerRoleAssignment RoleAssignment { get; }
        public PlayerPublicState PublicState { get; }
        public PlayerPrivateState PrivateState { get; }

        public PlayerRegistryEntry(
            MockLobbyPlayer lobbyPlayer,
            PlayerPublicIdentity publicIdentity,
            PlayerRoleAssignment roleAssignment,
            PlayerPublicState publicState,
            PlayerPrivateState privateState)
        {
            LobbyPlayer = lobbyPlayer;
            PublicIdentity = publicIdentity;
            RoleAssignment = roleAssignment;
            PublicState = publicState;
            PrivateState = privateState;
        }

        public bool HasSecretRoleInPublicState()
        {
            // Bilerek false. PublicState modelinde role alanı yok.
            return false;
        }

        public override string ToString()
        {
            return $"{LobbyPlayer.DisplayName} | Public={PublicIdentity} | SecretRole={RoleAssignment.Role}";
        }
    }
}