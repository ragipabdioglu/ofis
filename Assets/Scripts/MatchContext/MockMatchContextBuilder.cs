using System.Linq;
using OFIS.Core.Ids;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Roles.Identity;

namespace OFIS.MatchContext
{
    public sealed class MockMatchContextBuilder
    {
        private readonly RoleAssignmentService _roleAssignmentService;
        private readonly IdentityAssignmentService _identityAssignmentService;
        private readonly PlayerStateBuilder _playerStateBuilder;

        public MockMatchContextBuilder(
            RoleAssignmentService roleAssignmentService,
            IdentityAssignmentService identityAssignmentService,
            PlayerStateBuilder playerStateBuilder)
        {
            _roleAssignmentService = roleAssignmentService;
            _identityAssignmentService = identityAssignmentService;
            _playerStateBuilder = playerStateBuilder;
        }

        public MockMatchContextBuildResult Build(int playerCount)
        {
            var lobbyPlayers = MockPlayerFactory.CreateMockLobbyPlayers(playerCount);
            var playerIds = MockPlayerFactory.ExtractPlayerIds(lobbyPlayers);

            var identityResult = _identityAssignmentService.AssignIdentities(playerIds);

            if (!identityResult.Success)
                return MockMatchContextBuildResult.Failed(identityResult.ErrorMessage);

            var roleResult = _roleAssignmentService.AssignRolesToLobbyPlayers(lobbyPlayers);

            if (!roleResult.Success)
                return MockMatchContextBuildResult.Failed(roleResult.ErrorMessage);

            var stateResult = _playerStateBuilder.BuildStates(
                lobbyPlayers,
                identityResult.Identities,
                roleResult.Assignments);

            if (!stateResult.Success)
                return MockMatchContextBuildResult.Failed(stateResult.ErrorMessage);

            var registry = BuildRegistry(
                lobbyPlayers,
                identityResult,
                roleResult,
                stateResult);

            var context = new MockMatchContext(
                MatchId.New(),
                lobbyPlayers,
                identityResult.Identities,
                roleResult.Assignments,
                stateResult.PublicStates,
                stateResult.PrivateStates,
                registry);

            return MockMatchContextBuildResult.Completed(context);
        }

        private static PlayerRegistry BuildRegistry(
            System.Collections.Generic.IReadOnlyList<MockLobbyPlayer> lobbyPlayers,
            IdentityAssignmentResult identityResult,
            RoleAssignmentResult roleResult,
            PlayerStateBuildResult stateResult)
        {
            var registry = new PlayerRegistry();

            foreach (var lobbyPlayer in lobbyPlayers)
            {
                var identity = identityResult.Identities
                    .First(x => x.PlayerId == lobbyPlayer.PlayerId);

                var roleAssignment = roleResult.Assignments
                    .First(x => x.PlayerId == lobbyPlayer.PlayerId);

                var publicState = stateResult.PublicStates
                    .First(x => x.PlayerId == lobbyPlayer.PlayerId);

                var privateState = stateResult.PrivateStates
                    .First(x => x.OwnerPlayerId == lobbyPlayer.PlayerId);

                registry.Add(new PlayerRegistryEntry(
                    lobbyPlayer,
                    identity,
                    roleAssignment,
                    publicState,
                    privateState));
            }

            return registry;
        }
    }
}