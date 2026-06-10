using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Sabotage
{
    public sealed class SabotageRepairDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly SabotageRepairService _repairService = new SabotageRepairService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateSabotageRepairCore();
        }

        [ContextMenu("Validate Sabotage Repair Core")]
        public void ValidateSabotageRepairCore()
        {
            ValidateInactiveRepairFails();
            ValidateActiveRepairSucceeds();
            ValidateRepairingRepairSucceeds();
            ValidateAlreadyRepairedFails();
            ValidateExpiredRepairFails();
            ValidateMissingSabotageFails();
        }

        private void ValidateInactiveRepairFails()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Inactive);
            SabotageRepairResult result = _repairService.Repair(sabotage);
            bool passed = !result.Success && result.NewState == SabotageObjectiveState.Inactive;

            LogResult("InactiveRepairFails", passed, result);
        }

        private void ValidateActiveRepairSucceeds()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Active);
            SabotageRepairResult result = _repairService.Repair(sabotage);
            bool passed = result.Success && sabotage.State == SabotageObjectiveState.Repaired;

            LogResult("ActiveRepairSucceeds", passed, result);
        }

        private void ValidateRepairingRepairSucceeds()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Repairing);
            SabotageRepairResult result = _repairService.Repair(sabotage);
            bool passed = result.Success && sabotage.State == SabotageObjectiveState.Repaired;

            LogResult("RepairingRepairSucceeds", passed, result);
        }

        private void ValidateAlreadyRepairedFails()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Repaired);
            SabotageRepairResult result = _repairService.Repair(sabotage);
            bool passed = !result.Success && result.NewState == SabotageObjectiveState.Repaired;

            LogResult("AlreadyRepairedFails", passed, result);
        }

        private void ValidateExpiredRepairFails()
        {
            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Expired);
            SabotageRepairResult result = _repairService.Repair(sabotage);
            bool passed = !result.Success && result.NewState == SabotageObjectiveState.Expired;

            LogResult("ExpiredRepairFails", passed, result);
        }

        private void ValidateMissingSabotageFails()
        {
            SabotageRepairResult result = _repairService.Repair(null);
            bool passed = !result.Success && result.NewState == SabotageObjectiveState.None;

            LogResult("MissingSabotageFails", passed, result);
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

        private static void LogResult(string testName, bool passed, SabotageRepairResult result)
        {
            if (passed)
                Debug.Log($"[SabotageRepairValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[SabotageRepairValidator] FAIL {testName}: {result}");
        }
    }
}
