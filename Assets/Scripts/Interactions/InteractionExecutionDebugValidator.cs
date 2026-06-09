using System.Collections.Generic;
using OFIS.Players;
using UnityEngine;

namespace OFIS.Interactions
{
    public sealed class InteractionExecutionDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly LocalInteractionResolver _resolver = new LocalInteractionResolver();
        private readonly InteractionExecutionService _executionService = new InteractionExecutionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateExecutionBridge();
        }

        [ContextMenu("Validate Interaction Execution Bridge")]
        public void ValidateExecutionBridge()
        {
            WorldInteractionResolveResult selectedTask = BuildSelectedTaskInteraction();
            WorldInteractionResolveResult noSelection = WorldInteractionResolveResult.None("No candidates in range.");

            ValidateAliveTaskExecution(selectedTask);
            ValidateDeadExecutionBlocked(selectedTask);
            ValidateDisconnectedExecutionBlocked(selectedTask);
            ValidateNoSelectionBlocked(noSelection);
            ValidateAllExecutableTypes();
        }

        private WorldInteractionResolveResult BuildSelectedTaskInteraction()
        {
            List<WorldInteractionCandidate> candidates = new List<WorldInteractionCandidate>
            {
                new WorldInteractionCandidate(WorldInteractionType.Task, "Debug Task", 0.25f, true)
            };

            return _resolver.Resolve(candidates);
        }

        private void ValidateAliveTaskExecution(WorldInteractionResolveResult selectedTask)
        {
            InteractionExecutionResult result = _executionService.Execute(PlayerLifeState.Alive, selectedTask);
            bool passed = result.Success && result.InteractionType == WorldInteractionType.Task && result.ActionKey == "Task";

            LogResult("AliveTaskExecution", passed, result);
        }

        private void ValidateDeadExecutionBlocked(WorldInteractionResolveResult selectedTask)
        {
            InteractionExecutionResult result = _executionService.Execute(PlayerLifeState.Dead, selectedTask);
            bool passed = !result.Success;

            LogResult("DeadExecutionBlocked", passed, result);
        }

        private void ValidateDisconnectedExecutionBlocked(WorldInteractionResolveResult selectedTask)
        {
            InteractionExecutionResult result = _executionService.Execute(PlayerLifeState.Disconnected, selectedTask);
            bool passed = !result.Success;

            LogResult("DisconnectedExecutionBlocked", passed, result);
        }

        private void ValidateNoSelectionBlocked(WorldInteractionResolveResult noSelection)
        {
            InteractionExecutionResult result = _executionService.Execute(PlayerLifeState.Alive, noSelection);
            bool passed = !result.Success;

            LogResult("NoSelectionBlocked", passed, result);
        }

        private void ValidateAllExecutableTypes()
        {
            ValidateExecutableType(WorldInteractionType.CorpseInspectOrCarry, "CorpseInspectOrCarry");
            ValidateExecutableType(WorldInteractionType.MeetingJoin, "MeetingJoin");
            ValidateExecutableType(WorldInteractionType.SabotageRepair, "SabotageRepair");
            ValidateExecutableType(WorldInteractionType.Task, "Task");
            ValidateExecutableType(WorldInteractionType.Sabotage, "Sabotage");
            ValidateExecutableType(WorldInteractionType.VictimNote, "VictimNote");
            ValidateExecutableType(WorldInteractionType.DoorPanel, "DoorPanel");
        }

        private void ValidateExecutableType(WorldInteractionType type, string expectedActionKey)
        {
            List<WorldInteractionCandidate> candidates = new List<WorldInteractionCandidate>
            {
                new WorldInteractionCandidate(type, $"Debug {type}", 0.5f, true)
            };

            WorldInteractionResolveResult resolveResult = _resolver.Resolve(candidates);
            InteractionExecutionResult result = _executionService.Execute(PlayerLifeState.Alive, resolveResult);
            bool passed = result.Success && result.InteractionType == type && result.ActionKey == expectedActionKey;

            LogResult($"ExecutableType_{type}", passed, result);
        }

        private static void LogResult(string testName, bool passed, InteractionExecutionResult result)
        {
            if (passed)
                Debug.Log($"[InteractionExecutionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[InteractionExecutionValidator] FAIL {testName}: {result}");
        }
    }
}
