using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Sabotage
{
    public sealed class SabotageRepairUiDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly SabotageRepairUiStateService _uiStateService = new SabotageRepairUiStateService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateSabotageRepairUi();
        }

        [ContextMenu("Validate Sabotage Repair UI")]
        public void ValidateSabotageRepairUi()
        {
            ValidateNoSabotageState();
            ValidateInactiveState();
            ValidateActiveState();
            ValidateRepairingState();
            ValidateRepairedState();
            ValidateExpiredState();
        }

        private void ValidateNoSabotageState()
        {
            SabotageRepairUiState state = _uiStateService.Build(null);
            bool passed = !state.HasSabotage && state.State == SabotageObjectiveState.None && !state.CanShowRepairPrompt;

            LogResult("NoSabotageState", passed, state);
        }

        private void ValidateInactiveState()
        {
            SabotageRepairUiState state = _uiStateService.Build(BuildSabotage(SabotageObjectiveState.Inactive));
            bool passed = state.HasSabotage && state.State == SabotageObjectiveState.Inactive && !state.CanShowRepairPrompt;

            LogResult("InactiveState", passed, state);
        }

        private void ValidateActiveState()
        {
            SabotageRepairUiState state = _uiStateService.Build(BuildSabotage(SabotageObjectiveState.Active));
            bool passed = state.HasSabotage && state.State == SabotageObjectiveState.Active && state.CanShowRepairPrompt && state.ActionHintText == "Repair sabotage";

            LogResult("ActiveState", passed, state);
        }

        private void ValidateRepairingState()
        {
            SabotageRepairUiState state = _uiStateService.Build(BuildSabotage(SabotageObjectiveState.Repairing));
            bool passed = state.HasSabotage && state.State == SabotageObjectiveState.Repairing && state.CanShowRepairPrompt && state.ActionHintText == "Continue repair";

            LogResult("RepairingState", passed, state);
        }

        private void ValidateRepairedState()
        {
            SabotageRepairUiState state = _uiStateService.Build(BuildSabotage(SabotageObjectiveState.Repaired));
            bool passed = state.HasSabotage && state.State == SabotageObjectiveState.Repaired && !state.CanShowRepairPrompt;

            LogResult("RepairedState", passed, state);
        }

        private void ValidateExpiredState()
        {
            SabotageRepairUiState state = _uiStateService.Build(BuildSabotage(SabotageObjectiveState.Expired));
            bool passed = state.HasSabotage && state.State == SabotageObjectiveState.Expired && !state.CanShowRepairPrompt;

            LogResult("ExpiredState", passed, state);
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

        private static void LogResult(string testName, bool passed, SabotageRepairUiState state)
        {
            if (passed)
                Debug.Log($"[SabotageRepairUiValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[SabotageRepairUiValidator] FAIL {testName}: {state}");
        }
    }
}
