using System.Collections.Generic;
using OFIS.Players;
using UnityEngine;

namespace OFIS.Interactions
{
    public sealed class InteractionUiStubDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly LocalInteractionResolver _resolver = new LocalInteractionResolver();
        private readonly InteractionExecutionService _executionService = new InteractionExecutionService();
        private readonly InteractionUiStateService _uiStateService = new InteractionUiStateService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateInteractionUiStub();
        }

        [ContextMenu("Validate Interaction UI Stub")]
        public void ValidateInteractionUiStub()
        {
            WorldInteractionResolveResult selectedTask = BuildSelectedTaskInteraction();
            InteractionExecutionResult notExecuted = InteractionExecutionResult.Failed("No action yet.");
            InteractionExecutionResult executed = _executionService.Execute(PlayerLifeState.Alive, selectedTask);
            WorldInteractionResolveResult noSelection = WorldInteractionResolveResult.None("No candidates in range.");

            ValidateAliveReadyState(selectedTask, notExecuted);
            ValidateDeadBlockedState(selectedTask, notExecuted);
            ValidateLastActionState(selectedTask, executed);
            ValidateNoSelectionState(noSelection, notExecuted);
        }

        private WorldInteractionResolveResult BuildSelectedTaskInteraction()
        {
            List<WorldInteractionCandidate> candidates = new List<WorldInteractionCandidate>
            {
                new WorldInteractionCandidate(WorldInteractionType.Task, "Debug Task", 0.25f, true)
            };

            return _resolver.Resolve(candidates);
        }

        private void ValidateAliveReadyState(WorldInteractionResolveResult selectedTask, InteractionExecutionResult notExecuted)
        {
            InteractionUiState state = _uiStateService.Build(PlayerLifeState.Alive, selectedTask, notExecuted);
            bool passed = state.HasSelection && state.CanInteract && state.InteractionType == WorldInteractionType.Task && state.PromptText.Contains("Press E");

            LogResult("AliveReadyState", passed, state);
        }

        private void ValidateDeadBlockedState(WorldInteractionResolveResult selectedTask, InteractionExecutionResult notExecuted)
        {
            InteractionUiState state = _uiStateService.Build(PlayerLifeState.Dead, selectedTask, notExecuted);
            bool passed = state.HasSelection && !state.CanInteract && state.PromptText.Contains("Blocked");

            LogResult("DeadBlockedState", passed, state);
        }

        private void ValidateLastActionState(WorldInteractionResolveResult selectedTask, InteractionExecutionResult executed)
        {
            InteractionUiState state = _uiStateService.Build(PlayerLifeState.Alive, selectedTask, executed);
            bool passed = state.HasSelection && state.CanInteract && state.LastActionText.Contains("Task execute stub");

            LogResult("LastActionState", passed, state);
        }

        private void ValidateNoSelectionState(WorldInteractionResolveResult noSelection, InteractionExecutionResult notExecuted)
        {
            InteractionUiState state = _uiStateService.Build(PlayerLifeState.Alive, noSelection, notExecuted);
            bool passed = !state.HasSelection && !state.CanInteract && state.InteractionType == WorldInteractionType.None;

            LogResult("NoSelectionState", passed, state);
        }

        private static void LogResult(string testName, bool passed, InteractionUiState state)
        {
            if (passed)
                Debug.Log($"[InteractionUiStubValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[InteractionUiStubValidator] FAIL {testName}: {state}");
        }
    }
}
