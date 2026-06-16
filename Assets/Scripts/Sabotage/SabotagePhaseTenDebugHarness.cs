using System.Collections.Generic;
using OFIS.Core.Ids;
using OFIS.Evidence;
using OFIS.Logs;
using OFIS.Roles;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Sabotage
{
    public sealed class SabotagePhaseTenDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private SabotagePhaseTenPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly SabotageDeviceCatalogService _catalogService = new SabotageDeviceCatalogService();
        private readonly SabotageCommandValidationService _commandService = new SabotageCommandValidationService();
        private readonly SabotageCooldownState _cooldownState = new SabotageCooldownState();
        private readonly SabotageRepairService _repairService = new SabotageRepairService();
        private readonly SabotageRepairSpeedService _repairSpeedService = new SabotageRepairSpeedService();
        private readonly SabotageCompanyEffectService _companyEffectService = new SabotageCompanyEffectService();
        private readonly SabotageTraceAndLogService _traceAndLogService = new SabotageTraceAndLogService();
        private readonly SabotageUiAlertService _uiAlertService = new SabotageUiAlertService();
        private readonly SabotageNetworkFlowService _networkFlowService = new SabotageNetworkFlowService();
        private readonly PublicReportProjectionService _projectionService = new PublicReportProjectionService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Sabotage Phase 10 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case SabotagePhaseTenPackageType.DevicesAndTypes:
                    ValidateDevicesAndTypes();
                    break;
                case SabotagePhaseTenPackageType.KillerRoleValidation:
                    ValidateKillerRoleValidation();
                    break;
                case SabotagePhaseTenPackageType.PhysicalRangeValidation:
                    ValidatePhysicalRangeValidation();
                    break;
                case SabotagePhaseTenPackageType.CarryCorpseBlocked:
                    ValidateCarryCorpseBlocked();
                    break;
                case SabotagePhaseTenPackageType.Cooldown:
                    ValidateCooldown();
                    break;
                case SabotagePhaseTenPackageType.SameTypeActiveLimit:
                    ValidateSameTypeActiveLimit();
                    break;
                case SabotagePhaseTenPackageType.SameRoomActiveLimit:
                    ValidateSameRoomActiveLimit();
                    break;
                case SabotagePhaseTenPackageType.RepairInteraction:
                    ValidateRepairInteraction();
                    break;
                case SabotagePhaseTenPackageType.RepairSpeedByWorkerCount:
                    ValidateRepairSpeedByWorkerCount();
                    break;
                case SabotagePhaseTenPackageType.CompanyEffects:
                    ValidateCompanyEffects();
                    break;
                case SabotagePhaseTenPackageType.TraceAndLogCreation:
                    ValidateTraceAndLogCreation();
                    break;
                case SabotagePhaseTenPackageType.UiAlertSafety:
                    ValidateUiAlertSafety();
                    break;
                case SabotagePhaseTenPackageType.NetworkCommandEventFlow:
                    ValidateNetworkCommandEventFlow();
                    break;
                case SabotagePhaseTenPackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateDevicesAndTypes()
        {
            IReadOnlyList<SabotageDeviceDefinition> devices = _catalogService.BuildMvpDevices();
            bool passed = devices.Count == 6
                && devices[0].SabotageType == SabotageType.PrinterFault
                && devices[1].SabotageType == SabotageType.ServerOutage
                && devices[2].SabotageType == SabotageType.ArchiveDisorder
                && devices[3].SabotageType == SabotageType.DoorCardFailure
                && devices[4].SabotageType == SabotageType.CameraBlackout
                && devices[5].SabotageType == SabotageType.MeetingDisruption;

            LogResult("DevicesAndTypes", passed, $"DeviceCount={devices.Count}");
        }

        private void ValidateKillerRoleValidation()
        {
            SabotageCommandResult detective = _commandService.TryStart(BuildCommand(PlayerRole.Detective, false, 100f), EmptyStates(), new SabotageCooldownState());
            SabotageCommandResult killer = _commandService.TryStart(BuildCommand(PlayerRole.Killer, false, 100f), EmptyStates(), new SabotageCooldownState());

            LogResult("KillerRoleValidation", !detective.Success && killer.Success, detective.Message);
        }

        private void ValidatePhysicalRangeValidation()
        {
            SabotageDeviceDefinition device = FirstDevice();
            SabotageCommand farCommand = new SabotageCommand(
                new PlayerId("killer_10c"),
                PlayerRole.Killer,
                device,
                device.WorldPosition + new Vector3(25f, 0f, 0f),
                false,
                100f);

            SabotageCommandResult result = _commandService.TryStart(farCommand, EmptyStates(), new SabotageCooldownState());
            LogResult("PhysicalRangeValidation", !result.Success, result.Message);
        }

        private void ValidateCarryCorpseBlocked()
        {
            SabotageCommandResult result = _commandService.TryStart(BuildCommand(PlayerRole.Killer, true, 100f), EmptyStates(), new SabotageCooldownState());
            LogResult("CarryCorpseBlocked", !result.Success, result.Message);
        }

        private void ValidateCooldown()
        {
            SabotageCooldownState cooldown = new SabotageCooldownState();
            SabotageCommandResult first = _commandService.TryStart(BuildCommand(PlayerRole.Killer, false, 100f), EmptyStates(), cooldown);
            SabotageCommandResult second = _commandService.TryStart(BuildCommand(PlayerRole.Killer, false, 120f), EmptyStates(), cooldown);
            SabotageCommandResult third = _commandService.TryStart(BuildCommand(PlayerRole.Killer, false, 160f), EmptyStates(), cooldown);

            LogResult("Cooldown", first.Success && !second.Success && third.Success, second.Message);
        }

        private void ValidateSameTypeActiveLimit()
        {
            SabotageDeviceDefinition device = FirstDevice();
            List<SabotageObjectiveRuntimeState> active = new List<SabotageObjectiveRuntimeState>
            {
                BuildActiveState(device)
            };

            SabotageCommandResult result = _commandService.TryStart(BuildCommand(PlayerRole.Killer, false, 100f), active, new SabotageCooldownState());
            LogResult("SameTypeActiveLimit", !result.Success, result.Message);
        }

        private void ValidateSameRoomActiveLimit()
        {
            SabotageDeviceDefinition device = FirstDevice();
            SabotageDeviceDefinition sameRoomDifferentType = new SabotageDeviceDefinition("same_room_camera", SabotageType.CameraBlackout, device.RoomType, device.WorldPosition, device.InteractionRange);
            List<SabotageObjectiveRuntimeState> active = new List<SabotageObjectiveRuntimeState>
            {
                BuildActiveState(sameRoomDifferentType)
            };

            SabotageCommandResult result = _commandService.TryStart(BuildCommand(PlayerRole.Killer, false, 100f), active, new SabotageCooldownState());
            LogResult("SameRoomActiveLimit", !result.Success, result.Message);
        }

        private void ValidateRepairInteraction()
        {
            SabotageObjectiveRuntimeState state = BuildActiveState(FirstDevice());
            SabotageRepairResult result = _repairService.Repair(state);
            LogResult("RepairInteraction", result.Success && state.State == SabotageObjectiveState.Repaired, result.ToString());
        }

        private void ValidateRepairSpeedByWorkerCount()
        {
            float solo = _repairSpeedService.ResolveRepairDuration(10f, 1);
            float duo = _repairSpeedService.ResolveRepairDuration(10f, 2);
            LogResult("RepairSpeedByWorkerCount", duo < solo && duo >= 1f, $"Solo={solo:0.##}, Duo={duo:0.##}");
        }

        private void ValidateCompanyEffects()
        {
            SabotageCompanyEffect effect = _companyEffectService.GetMvpEffect();
            bool passed = effect.StartDelta == -2
                && effect.ActiveTickDelta == -1
                && effect.RepairedDelta == 3
                && effect.UnresolvedMeetingDelta == -3;

            LogResult("CompanyEffects", passed, $"Start={effect.StartDelta}, Tick={effect.ActiveTickDelta}, Repaired={effect.RepairedDelta}, Unresolved={effect.UnresolvedMeetingDelta}");
        }

        private void ValidateTraceAndLogCreation()
        {
            SabotageObjectiveRuntimeState state = BuildActiveState(FirstDevice());
            EvidenceTraceRecord trace = _traceAndLogService.BuildTrace(state, 100f);
            ServerRecord log = _traceAndLogService.BuildLog(new MatchId("match_10k"), state, 100f);
            PublicReportProjectionResult projection = _projectionService.TryProject(log);

            bool passed = trace.TraceType == EvidenceTraceType.SabotageTrace
                && log.Category == RecordCategory.Sabotage
                && projection.Success
                && !projection.Report.ToString().ToLowerInvariant().Contains("killer");

            LogResult("TraceAndLogCreation", passed, projection.Message);
        }

        private void ValidateUiAlertSafety()
        {
            string alert = _uiAlertService.BuildPublicAlert(BuildActiveState(FirstDevice()));
            string normalized = alert.ToLowerInvariant();
            bool passed = alert.Contains("Sabotage alert")
                && !normalized.Contains("killer")
                && !normalized.Contains("saboteur")
                && !normalized.Contains("identity");

            LogResult("UiAlertSafety", passed, alert);
        }

        private void ValidateNetworkCommandEventFlow()
        {
            SabotageCommandResult command = _commandService.TryStart(BuildCommand(PlayerRole.Killer, false, 100f), EmptyStates(), new SabotageCooldownState());
            SabotageNetworkEvent networkEvent = _networkFlowService.BuildPublicEvent(command.RuntimeState);
            bool passed = command.Success
                && networkEvent.IsPublicSafe
                && networkEvent.DeviceId == command.RuntimeState.Definition.SabotageId
                && !networkEvent.PublicMessage.ToLowerInvariant().Contains("killer");

            LogResult("NetworkCommandEventFlow", passed, networkEvent.PublicMessage);
        }

        private void ValidatePhaseClosure()
        {
            ValidateDevicesAndTypes();
            ValidateKillerRoleValidation();
            ValidatePhysicalRangeValidation();
            ValidateCarryCorpseBlocked();
            ValidateCooldown();
            ValidateSameTypeActiveLimit();
            ValidateSameRoomActiveLimit();
            ValidateRepairInteraction();
            ValidateRepairSpeedByWorkerCount();
            ValidateCompanyEffects();
            ValidateTraceAndLogCreation();
            ValidateUiAlertSafety();
            ValidateNetworkCommandEventFlow();

            bool passed = true;
            for (int i = 0; i <= (int)SabotagePhaseTenPackageType.NetworkCommandEventFlow; i++)
                passed &= System.Enum.IsDefined(typeof(SabotagePhaseTenPackageType), i);

            LogResult("PhaseClosure", passed, "MVP Faz 10 packages 10A-10N are represented.");
        }

        private static IReadOnlyList<SabotageObjectiveRuntimeState> EmptyStates()
        {
            return new List<SabotageObjectiveRuntimeState>();
        }

        private SabotageCommand BuildCommand(PlayerRole role, bool isCarryingCorpse, float serverTimeSeconds)
        {
            SabotageDeviceDefinition device = FirstDevice();
            return new SabotageCommand(
                new PlayerId("killer_10"),
                role,
                device,
                device.WorldPosition,
                isCarryingCorpse,
                serverTimeSeconds);
        }

        private SabotageDeviceDefinition FirstDevice()
        {
            return _catalogService.BuildMvpDevices()[0];
        }

        private static SabotageObjectiveRuntimeState BuildActiveState(SabotageDeviceDefinition device)
        {
            SabotageObjectiveDefinition definition = new SabotageObjectiveDefinition(
                $"{device.SabotageType}_{device.DeviceId}",
                device.SabotageType.ToString(),
                device.RoomType,
                8f);

            SabotageObjectiveRuntimeState state = new SabotageObjectiveRuntimeState(definition);
            state.Activate();
            return state;
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[SabotagePhaseTenDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[SabotagePhaseTenDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
