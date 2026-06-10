using System.Collections.Generic;
using OFIS.Interactions;
using OFIS.Players;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Sabotage
{
    public sealed class SabotageRepairInteractionExecutionDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly LocalInteractionResolver _resolver = new LocalInteractionResolver();
        private readonly InteractionExecutionService _executionService = new InteractionExecutionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateSabotageRepairInteractionExecution();
        }

        [ContextMenu("Validate Sabotage Repair Interaction Execution")]
        public void ValidateSabotageRepairInteractionExecution()
        {
            ValidateActiveRepairInteractionSucceeds();
            ValidateInactiveRepairInteractionFails();
            ValidateAlreadyRepairedInteractionFails();
            ValidateNonRepairSelectionFails();
            ValidateDeadPlayerBlocked();
        }

        private void ValidateActiveRepairInteractionSucceeds()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Active);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.SabotageRepair, "Repair server sabotage");
            InteractionExecutionResult result = _executionService.ExecuteSabotageRepair(PlayerLifeState.Alive, selection, sabotage);
            bool passed = result.Success && sabotage.State == SabotageObjectiveState.Repaired && result.ActionKey == "SabotageRepair";

            LogResult("ActiveRepairInteractionSucceeds", passed, result);
        }

        private void ValidateInactiveRepairInteractionFails()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Inactive);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.SabotageRepair, "Repair server sabotage");
            InteractionExecutionResult result = _executionService.ExecuteSabotageRepair(PlayerLifeState.Alive, selection, sabotage);
            bool passed = !result.Success && result.Message.Contains("not active");

            LogResult("InactiveRepairInteractionFails", passed, result);
        }

        private void ValidateAlreadyRepairedInteractionFails()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Repaired);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.SabotageRepair, "Repair server sabotage");
            InteractionExecutionResult result = _executionService.ExecuteSabotageRepair(PlayerLifeState.Alive, selection, sabotage);
            bool passed = !result.Success && result.Message.Contains("already repaired");

            LogResult("AlreadyRepairedInteractionFails", passed, result);
        }

        private void ValidateNonRepairSelectionFails()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Active);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.Task, "Review invoices");
            InteractionExecutionResult result = _executionService.ExecuteSabotageRepair(PlayerLifeState.Alive, selection, sabotage);
            bool passed = !result.Success && result.Message.Contains("not a sabotage repair");

            LogResult("NonRepairSelectionFails", passed, result);
        }

        private void ValidateDeadPlayerBlocked()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Active);
            WorldInteractionResolveResult selection = BuildSelection(WorldInteractionType.SabotageRepair, "Repair server sabotage");
            InteractionExecutionResult result = _executionService.ExecuteSabotageRepair(PlayerLifeState.Dead, selection, sabotage);
            bool passed = !result.Success && sabotage.State == SabotageObjectiveState.Active;

            LogResult("DeadPlayerBlocked", passed, result);
        }

        private WorldInteractionResolveResult BuildSelection(WorldInteractionType type, string displayName)
        {
            List<WorldInteractionCandidate> candidates = new List<WorldInteractionCandidate>
            {
                new WorldInteractionCandidate(type, displayName, 0.25f, true)
            };

            return _resolver.Resolve(candidates);
        }

        private static SabotageObjectiveRuntimeState BuildSabotage(SabotageObjectiveState initialState)
        {
            SabotageObjectiveDefinition definition = new SabotageObjectiveDefinition(
                "sabotage_server_room",
                "Server room sabotage",
                OfficeRoomType.ServerRoom,
                5f);

            return new SabotageObjectiveRuntimeState(definition, initialState);
        }

        private static void LogResult(string testName, bool passed, InteractionExecutionResult result)
        {
            if (passed)
                Debug.Log($"[SabotageRepairInteractionExecutionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[SabotageRepairInteractionExecutionValidator] FAIL {testName}: {result}");
        }
    }
}
