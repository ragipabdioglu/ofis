using System;
using System.Collections.Generic;
using System.Linq;
using OFIS.Core.Ids;
using UnityEngine;

namespace OFIS.Roles
{
    public sealed class RoleAssignmentService
    {
        private readonly System.Random _random;

        public RoleAssignmentService(int seed = 0)
        {
            _random = seed == 0 ? new System.Random() : new System.Random(seed);
        }

        public RoleAssignmentResult AssignRoles(IReadOnlyList<PlayerId> playerIds)
        {
            if (playerIds == null || playerIds.Count == 0)
                return RoleAssignmentResult.Failed("Player list is empty.");

            var distributionResult = TryGetDistribution(playerIds.Count, out var distribution);

            if (!distributionResult)
                return RoleAssignmentResult.Failed($"Unsupported player count: {playerIds.Count}. Supported counts: 6, 8, 10, 12.");

            var shuffledPlayers = playerIds.ToList();
            Shuffle(shuffledPlayers);

            var assignments = new List<PlayerRoleAssignment>();

            int cursor = 0;

            for (int i = 0; i < distribution.KillerCount; i++)
            {
                assignments.Add(new PlayerRoleAssignment(
                    shuffledPlayers[cursor],
                    $"Player {cursor + 1}",
                    PlayerRole.Killer));

                cursor++;
            }

            for (int i = 0; i < distribution.VictimCount; i++)
            {
                assignments.Add(new PlayerRoleAssignment(
                    shuffledPlayers[cursor],
                    $"Player {cursor + 1}",
                    PlayerRole.Victim));

                cursor++;
            }

            for (int i = 0; i < distribution.DetectiveCount; i++)
            {
                assignments.Add(new PlayerRoleAssignment(
                    shuffledPlayers[cursor],
                    $"Player {cursor + 1}",
                    PlayerRole.Detective));

                cursor++;
            }

            ApplyKillerVictimKnowledge(assignments);

            return RoleAssignmentResult.Completed(distribution, assignments);
        }

        public RoleAssignmentResult AssignRolesToLobbyPlayers(IReadOnlyList<MockLobbyPlayer> lobbyPlayers)
        {
            if (lobbyPlayers == null || lobbyPlayers.Count == 0)
                return RoleAssignmentResult.Failed("Lobby player list is empty.");

            var playerIds = MockPlayerFactory.ExtractPlayerIds(lobbyPlayers);

            var result = AssignRoles(playerIds);

            if (!result.Success)
                return result;

            var fixedNameAssignments = new List<PlayerRoleAssignment>();

            foreach (var assignment in result.Assignments)
            {
                var lobbyPlayer = lobbyPlayers.FirstOrDefault(x => x.PlayerId == assignment.PlayerId);

                string displayName = lobbyPlayer != null
                    ? lobbyPlayer.DisplayName
                    : assignment.DisplayName;

                var fixedAssignment = new PlayerRoleAssignment(
                    assignment.PlayerId,
                    displayName,
                    assignment.Role);

                foreach (var target in assignment.KnownVictimTargets)
                    fixedAssignment.AddKnownVictimTarget(target);

                fixedNameAssignments.Add(fixedAssignment);
            }

            return RoleAssignmentResult.Completed(result.Distribution, fixedNameAssignments);
        }

        public RoleRevealData BuildRevealDataForOwner(
            PlayerId ownerPlayerId,
            IReadOnlyList<PlayerRoleAssignment> assignments)
        {
            var assignment = assignments.FirstOrDefault(x => x.PlayerId == ownerPlayerId);

            if (assignment == null)
            {
                Debug.LogError($"[RoleAssignment] Cannot build reveal data. Player not found: {ownerPlayerId}");
                return new RoleRevealData(ownerPlayerId, PlayerRole.None, Array.Empty<PlayerId>());
            }

            return new RoleRevealData(
                assignment.PlayerId,
                assignment.Role,
                assignment.KnownVictimTargets);
        }

        public RoleRevealDebugView BuildDebugRevealViewForOwner(
            MockLobbyPlayer owner,
            IReadOnlyList<MockLobbyPlayer> lobbyPlayers,
            IReadOnlyList<PlayerRoleAssignment> assignments)
        {
            if (owner == null)
                return null;

            var revealData = BuildRevealDataForOwner(owner.PlayerId, assignments);

            var knownTargets = new List<MockLobbyPlayer>();

            foreach (var targetId in revealData.KnownVictimTargets)
            {
                var targetPlayer = lobbyPlayers.FirstOrDefault(x => x.PlayerId == targetId);

                if (targetPlayer != null)
                    knownTargets.Add(targetPlayer);
            }

            return new RoleRevealDebugView(
                owner.PlayerId,
                owner.DisplayName,
                revealData.OwnRole,
                knownTargets);
        }

        public bool TryGetDistribution(int playerCount, out RoleDistribution distribution)
        {
            distribution = playerCount switch
            {
                6 => new RoleDistribution(6, 1, 1, 4),
                8 => new RoleDistribution(8, 2, 2, 4),
                10 => new RoleDistribution(10, 2, 2, 6),
                12 => new RoleDistribution(12, 3, 3, 6),
                _ => default
            };

            return distribution.IsValid;
        }

        private static void ApplyKillerVictimKnowledge(List<PlayerRoleAssignment> assignments)
        {
            var victims = assignments
                .Where(x => x.Role == PlayerRole.Victim)
                .Select(x => x.PlayerId)
                .ToList();

            foreach (var assignment in assignments)
            {
                if (assignment.Role != PlayerRole.Killer)
                    continue;

                foreach (var victimId in victims)
                    assignment.AddKnownVictimTarget(victimId);
            }
        }

        private void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = _random.Next(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}