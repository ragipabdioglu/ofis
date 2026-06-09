using System.Collections.Generic;
using System.Linq;
using OFIS.Core.Ids;
using OFIS.Roles;
using OFIS.Roles.Identity;

namespace OFIS.Players
{
    public sealed class PlayerStateBuilder
    {
        public PlayerStateBuildResult BuildStates(
            IReadOnlyList<MockLobbyPlayer> lobbyPlayers,
            IReadOnlyList<PlayerPublicIdentity> identities,
            IReadOnlyList<PlayerRoleAssignment> roleAssignments)
        {
            if (lobbyPlayers == null || lobbyPlayers.Count == 0)
                return PlayerStateBuildResult.Failed("Lobby player list is empty.");

            if (identities == null || identities.Count == 0)
                return PlayerStateBuildResult.Failed("Public identity list is empty.");

            if (roleAssignments == null || roleAssignments.Count == 0)
                return PlayerStateBuildResult.Failed("Role assignment list is empty.");

            var publicStates = new List<PlayerPublicState>();
            var privateStates = new List<PlayerPrivateState>();

            foreach (var lobbyPlayer in lobbyPlayers)
            {
                var identity = identities.FirstOrDefault(x => x.PlayerId == lobbyPlayer.PlayerId);

                if (identity == null)
                    return PlayerStateBuildResult.Failed($"Missing public identity for player: {lobbyPlayer.DisplayName}");

                var assignment = roleAssignments.FirstOrDefault(x => x.PlayerId == lobbyPlayer.PlayerId);

                if (assignment == null)
                    return PlayerStateBuildResult.Failed($"Missing role assignment for player: {lobbyPlayer.DisplayName}");

                publicStates.Add(new PlayerPublicState(
                    lobbyPlayer.PlayerId,
                    lobbyPlayer.DisplayName,
                    identity,
                    PlayerLifeState.Alive));

                privateStates.Add(new PlayerPrivateState(
                    lobbyPlayer.PlayerId,
                    assignment.Role,
                    assignment.KnownVictimTargets));
            }

            return PlayerStateBuildResult.Completed(publicStates, privateStates);
        }

        public PlayerPublicState GetPublicState(
            PlayerId playerId,
            IReadOnlyList<PlayerPublicState> publicStates)
        {
            if (publicStates == null)
                return null;

            return publicStates.FirstOrDefault(x => x.PlayerId == playerId);
        }

        public PlayerPrivateState GetPrivateState(
            PlayerId ownerPlayerId,
            IReadOnlyList<PlayerPrivateState> privateStates)
        {
            if (privateStates == null)
                return null;

            return privateStates.FirstOrDefault(x => x.OwnerPlayerId == ownerPlayerId);
        }
    }
}