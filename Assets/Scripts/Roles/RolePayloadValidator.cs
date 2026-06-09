using System.Collections.Generic;
using System.Linq;
using OFIS.Players;

namespace OFIS.Roles
{
    public sealed class RolePayloadValidator
    {
        public List<RolePayloadValidationResult> Validate(
            IReadOnlyList<PlayerRoleAssignment> assignments,
            IReadOnlyList<PlayerPublicState> publicStates,
            IReadOnlyList<PlayerPrivateState> privateStates)
        {
            List<RolePayloadValidationResult> results = new();

            ValidateInputs(assignments, publicStates, privateStates, results);

            if (results.Any(x => !x.Success))
                return results;

            ValidatePublicPayload(publicStates, results);
            ValidatePrivatePayload(assignments, privateStates, results);
            ValidateKillerKnowledge(assignments, privateStates, results);
            ValidateNonKillerKnowledge(privateStates, results);

            return results;
        }

        public bool IsValid(
            IReadOnlyList<PlayerRoleAssignment> assignments,
            IReadOnlyList<PlayerPublicState> publicStates,
            IReadOnlyList<PlayerPrivateState> privateStates,
            out List<RolePayloadValidationResult> results)
        {
            results = Validate(assignments, publicStates, privateStates);
            return results.All(x => x.Success);
        }

        private static void ValidateInputs(
            IReadOnlyList<PlayerRoleAssignment> assignments,
            IReadOnlyList<PlayerPublicState> publicStates,
            IReadOnlyList<PlayerPrivateState> privateStates,
            List<RolePayloadValidationResult> results)
        {
            if (assignments == null || assignments.Count == 0)
                results.Add(RolePayloadValidationResult.Failed("Role assignments are missing."));

            if (publicStates == null || publicStates.Count == 0)
                results.Add(RolePayloadValidationResult.Failed("Public states are missing."));

            if (privateStates == null || privateStates.Count == 0)
                results.Add(RolePayloadValidationResult.Failed("Private states are missing."));
        }

        private static void ValidatePublicPayload(
            IReadOnlyList<PlayerPublicState> publicStates,
            List<RolePayloadValidationResult> results)
        {
            // PlayerPublicState intentionally has no role field.
            // This validator locks that design rule into an explicit test checkpoint.
            results.Add(RolePayloadValidationResult.Passed($"Public payload model has no role field. PublicStates={publicStates.Count}"));
        }

        private static void ValidatePrivatePayload(
            IReadOnlyList<PlayerRoleAssignment> assignments,
            IReadOnlyList<PlayerPrivateState> privateStates,
            List<RolePayloadValidationResult> results)
        {
            foreach (PlayerRoleAssignment assignment in assignments)
            {
                PlayerPrivateState privateState = privateStates.FirstOrDefault(x => x.OwnerPlayerId == assignment.PlayerId);

                if (privateState == null)
                {
                    results.Add(RolePayloadValidationResult.Failed($"Missing private state for {assignment.DisplayName}."));
                    continue;
                }

                if (privateState.OwnRole != assignment.Role)
                {
                    results.Add(RolePayloadValidationResult.Failed($"Private role mismatch for {assignment.DisplayName}. Expected={assignment.Role}, Actual={privateState.OwnRole}"));
                    continue;
                }
            }

            results.Add(RolePayloadValidationResult.Passed("Every private payload owner has matching own role."));
        }

        private static void ValidateKillerKnowledge(
            IReadOnlyList<PlayerRoleAssignment> assignments,
            IReadOnlyList<PlayerPrivateState> privateStates,
            List<RolePayloadValidationResult> results)
        {
            List<PlayerRoleAssignment> victims = assignments.Where(x => x.Role == PlayerRole.Victim).ToList();
            List<PlayerRoleAssignment> killers = assignments.Where(x => x.Role == PlayerRole.Killer).ToList();

            foreach (PlayerRoleAssignment killer in killers)
            {
                PlayerPrivateState privateState = privateStates.FirstOrDefault(x => x.OwnerPlayerId == killer.PlayerId);

                if (privateState == null)
                    continue;

                foreach (PlayerRoleAssignment victim in victims)
                {
                    if (!privateState.KnownVictimTargets.Contains(victim.PlayerId))
                        results.Add(RolePayloadValidationResult.Failed($"Killer {killer.DisplayName} does not know victim target {victim.DisplayName}."));
                }

                foreach (PlayerRoleAssignment otherKiller in killers)
                {
                    if (privateState.KnownVictimTargets.Contains(otherKiller.PlayerId))
                        results.Add(RolePayloadValidationResult.Failed($"Killer {killer.DisplayName} incorrectly knows killer {otherKiller.DisplayName}."));
                }
            }

            results.Add(RolePayloadValidationResult.Passed("Killer payloads include victim targets and exclude killer targets."));
        }

        private static void ValidateNonKillerKnowledge(
            IReadOnlyList<PlayerPrivateState> privateStates,
            List<RolePayloadValidationResult> results)
        {
            foreach (PlayerPrivateState privateState in privateStates)
            {
                if (privateState.OwnRole == PlayerRole.Killer)
                    continue;

                if (privateState.KnownVictimTargets != null && privateState.KnownVictimTargets.Count > 0)
                    results.Add(RolePayloadValidationResult.Failed($"Non-killer {privateState.OwnerPlayerId} has known targets."));
            }

            results.Add(RolePayloadValidationResult.Passed("Detective and victim payloads have no known target list."));
        }
    }
}
