using System.Collections.Generic;
using OFIS.Core.Ids;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Roles.Identity;

namespace OFIS.MatchContext
{
    public sealed class MockMatchContext
    {
        public MatchId MatchId { get; }
        public IReadOnlyList<MockLobbyPlayer> LobbyPlayers { get; }
        public IReadOnlyList<PlayerPublicIdentity> PublicIdentities { get; }
        public IReadOnlyList<PlayerRoleAssignment> RoleAssignments { get; }
        public IReadOnlyList<PlayerPublicState> PublicStates { get; }
        public IReadOnlyList<PlayerPrivateState> PrivateStates { get; }
        public PlayerRegistry Registry { get; }

        public MockMatchContext(
            MatchId matchId,
            IReadOnlyList<MockLobbyPlayer> lobbyPlayers,
            IReadOnlyList<PlayerPublicIdentity> publicIdentities,
            IReadOnlyList<PlayerRoleAssignment> roleAssignments,
            IReadOnlyList<PlayerPublicState> publicStates,
            IReadOnlyList<PlayerPrivateState> privateStates,
            PlayerRegistry registry)
        {
            MatchId = matchId;
            LobbyPlayers = lobbyPlayers;
            PublicIdentities = publicIdentities;
            RoleAssignments = roleAssignments;
            PublicStates = publicStates;
            PrivateStates = privateStates;
            Registry = registry;
        }
    }
}