using System.Collections.Generic;
using OFIS.MatchContext;
using OFIS.Players;
using OFIS.Roles.Identity;
using UnityEngine;

namespace OFIS.Roles
{
    public sealed class RolePayloadDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private int mockPlayerCount = 8;

        private readonly RolePayloadValidator _validator = new();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateRolePayloads();
        }

        [ContextMenu("Validate Role Payloads")]
        public void ValidateRolePayloads()
        {
            MockMatchContextBuilder builder = new(
                new RoleAssignmentService(seed: 12345),
                new IdentityAssignmentService(seed: 54321),
                new PlayerStateBuilder());

            MockMatchContextBuildResult buildResult = builder.Build(mockPlayerCount);

            if (!buildResult.Success)
            {
                Debug.LogError($"[RolePayloadValidator] Context build failed: {buildResult.ErrorMessage}");
                return;
            }

            MockMatchContext context = buildResult.Context;
            bool isValid = _validator.IsValid(
                context.RoleAssignments,
                context.PublicStates,
                context.PrivateStates,
                out List<RolePayloadValidationResult> results);

            Debug.Log($"[RolePayloadValidator] Validation started. Players={mockPlayerCount}, IsValid={isValid}");

            for (int i = 0; i < results.Count; i++)
            {
                RolePayloadValidationResult result = results[i];

                if (result.Success)
                    Debug.Log($"[RolePayloadValidator] {result}");
                else
                    Debug.LogError($"[RolePayloadValidator] {result}");
            }

            LogRoleSummary(context);
        }

        private static void LogRoleSummary(MockMatchContext context)
        {
            for (int i = 0; i < context.RoleAssignments.Count; i++)
            {
                PlayerRoleAssignment assignment = context.RoleAssignments[i];
                string targetSummary = assignment.KnownVictimTargets == null || assignment.KnownVictimTargets.Count == 0
                    ? "No targets"
                    : string.Join(", ", assignment.KnownVictimTargets);

                Debug.Log($"[RolePayloadValidator] {assignment.DisplayName}: Role={assignment.Role}, KnownTargets={targetSummary}");
            }
        }
    }
}
