using System.Collections.Generic;
using OFIS.Core.Ids;
using OFIS.Evidence;
using OFIS.Logs;
using OFIS.Roles;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Sabotage
{
    public readonly struct SabotageCompanyEffect
    {
        public int StartDelta { get; }
        public int ActiveTickDelta { get; }
        public int RepairedDelta { get; }
        public int UnresolvedMeetingDelta { get; }

        public SabotageCompanyEffect(int startDelta, int activeTickDelta, int repairedDelta, int unresolvedMeetingDelta)
        {
            StartDelta = startDelta;
            ActiveTickDelta = activeTickDelta;
            RepairedDelta = repairedDelta;
            UnresolvedMeetingDelta = unresolvedMeetingDelta;
        }
    }

    public readonly struct SabotageNetworkEvent
    {
        public string EventId { get; }
        public string DeviceId { get; }
        public SabotageType SabotageType { get; }
        public OfficeRoomType RoomType { get; }
        public bool IsPublicSafe { get; }
        public string PublicMessage { get; }

        public SabotageNetworkEvent(string eventId, string deviceId, SabotageType sabotageType, OfficeRoomType roomType, bool isPublicSafe, string publicMessage)
        {
            EventId = string.IsNullOrWhiteSpace(eventId) ? "unknown_event" : eventId;
            DeviceId = string.IsNullOrWhiteSpace(deviceId) ? "unknown_device" : deviceId;
            SabotageType = sabotageType;
            RoomType = roomType;
            IsPublicSafe = isPublicSafe;
            PublicMessage = string.IsNullOrWhiteSpace(publicMessage) ? "Sabotage alert." : publicMessage;
        }
    }

    public sealed class SabotageDeviceCatalogService
    {
        public IReadOnlyList<SabotageDeviceDefinition> BuildMvpDevices()
        {
            return new[]
            {
                new SabotageDeviceDefinition("printer_fault_device", SabotageType.PrinterFault, OfficeRoomType.PrintRoom, new Vector3(2f, 0f, 0f), 1.75f),
                new SabotageDeviceDefinition("server_outage_device", SabotageType.ServerOutage, OfficeRoomType.ServerRoom, new Vector3(4f, 0f, 0f), 1.75f),
                new SabotageDeviceDefinition("archive_disorder_device", SabotageType.ArchiveDisorder, OfficeRoomType.ArchiveRoom, new Vector3(6f, 0f, 0f), 1.75f),
                new SabotageDeviceDefinition("door_card_failure_device", SabotageType.DoorCardFailure, OfficeRoomType.SecurityRoom, new Vector3(8f, 0f, 0f), 1.75f),
                new SabotageDeviceDefinition("camera_blackout_device", SabotageType.CameraBlackout, OfficeRoomType.SecurityRoom, new Vector3(10f, 0f, 0f), 1.75f),
                new SabotageDeviceDefinition("meeting_disruption_device", SabotageType.MeetingDisruption, OfficeRoomType.MeetingRoom, new Vector3(12f, 0f, 0f), 1.75f)
            };
        }
    }

    public sealed class SabotageCooldownState
    {
        private readonly Dictionary<PlayerId, float> _lastUseTimes = new Dictionary<PlayerId, float>();

        public bool IsReady(PlayerId playerId, float currentServerTimeSeconds, float cooldownSeconds)
        {
            if (!_lastUseTimes.TryGetValue(playerId, out float lastUseTime))
                return true;

            return currentServerTimeSeconds - lastUseTime >= cooldownSeconds;
        }

        public void RecordUse(PlayerId playerId, float currentServerTimeSeconds)
        {
            _lastUseTimes[playerId] = currentServerTimeSeconds;
        }
    }

    public sealed class SabotageActiveLimitService
    {
        public bool CanActivate(IReadOnlyList<SabotageObjectiveRuntimeState> activeStates, SabotageDeviceDefinition device)
        {
            bool sameTypeActive = false;
            bool sameRoomActive = false;

            if (activeStates != null)
            {
                for (int i = 0; i < activeStates.Count; i++)
                {
                    SabotageObjectiveRuntimeState state = activeStates[i];
                    if (state == null || !state.IsActive)
                        continue;

                    if (state.Definition.SabotageId.Contains(device.SabotageType.ToString()))
                        sameTypeActive = true;

                    if (state.Definition.RoomType == device.RoomType)
                        sameRoomActive = true;
                }
            }

            return !sameTypeActive && !sameRoomActive;
        }
    }

    public sealed class SabotageCommandValidationService
    {
        public const float DefaultCooldownSeconds = 45f;

        private readonly SabotageActiveLimitService _activeLimitService = new SabotageActiveLimitService();

        public SabotageCommandResult TryStart(
            SabotageCommand command,
            IReadOnlyList<SabotageObjectiveRuntimeState> activeStates,
            SabotageCooldownState cooldownState)
        {
            if (command.SenderRole != PlayerRole.Killer)
                return Reject("Only killers can start sabotage.");

            float distance = Vector3.Distance(command.PlayerPosition, command.Device.WorldPosition);
            if (distance > command.Device.InteractionRange)
                return Reject("Player is outside sabotage range.");

            if (command.IsCarryingCorpse)
                return Reject("Sabotage blocked while carrying corpse.");

            if (cooldownState != null && !cooldownState.IsReady(command.SenderPlayerId, command.ServerTimeSeconds, DefaultCooldownSeconds))
                return Reject("Sabotage cooldown is active.");

            if (!_activeLimitService.CanActivate(activeStates, command.Device))
                return Reject("Same type or same room sabotage already active.");

            SabotageObjectiveDefinition definition = new SabotageObjectiveDefinition(
                $"{command.Device.SabotageType}_{command.Device.DeviceId}",
                command.Device.SabotageType.ToString(),
                command.Device.RoomType,
                8f);

            SabotageObjectiveRuntimeState state = new SabotageObjectiveRuntimeState(definition);
            state.Activate();
            cooldownState?.RecordUse(command.SenderPlayerId, command.ServerTimeSeconds);

            return new SabotageCommandResult(true, "Sabotage started.", state);
        }

        private static SabotageCommandResult Reject(string message)
        {
            return new SabotageCommandResult(false, message, null);
        }
    }

    public sealed class SabotageRepairSpeedService
    {
        public float ResolveRepairDuration(float baseDurationSeconds, int workerCount)
        {
            int clampedWorkers = workerCount <= 0 ? 1 : workerCount;
            float divisor = clampedWorkers >= 2 ? 1.75f : 1f;
            return Mathf.Max(1f, baseDurationSeconds / divisor);
        }
    }

    public sealed class SabotageCompanyEffectService
    {
        public SabotageCompanyEffect GetMvpEffect()
        {
            return new SabotageCompanyEffect(-2, -1, +3, -3);
        }
    }

    public sealed class SabotageTraceAndLogService
    {
        public EvidenceTraceRecord BuildTrace(SabotageObjectiveRuntimeState state, float serverTimeSeconds)
        {
            return new EvidenceTraceRecord(
                EvidenceTraceId.New(),
                EvidenceTraceType.SabotageTrace,
                state.Definition.SabotageId,
                state.Definition.RoomType,
                Vector3.zero,
                serverTimeSeconds,
                "Sabotage trace detected; origin unclear.");
        }

        public ServerRecord BuildLog(MatchId matchId, SabotageObjectiveRuntimeState state, float serverTimeSeconds)
        {
            return new ServerRecord(
                $"sabotage_log_{state.Definition.SabotageId}",
                matchId,
                RecordCategory.Sabotage,
                RecordVisibility.PublicSafe,
                new PlayerId("system_sabotage"),
                state.Definition.SabotageId,
                state.Definition.RoomType,
                serverTimeSeconds,
                "Sabotage log recorded as public-safe signal only.",
                "sabotage=active");
        }
    }

    public sealed class SabotageUiAlertService
    {
        public string BuildPublicAlert(SabotageObjectiveRuntimeState state)
        {
            return $"Sabotage alert: {state.Definition.DisplayName} in {state.Definition.RoomType}.";
        }
    }

    public sealed class SabotageNetworkFlowService
    {
        public SabotageNetworkEvent BuildPublicEvent(SabotageObjectiveRuntimeState state)
        {
            return new SabotageNetworkEvent(
                $"evt_{state.Definition.SabotageId}",
                state.Definition.SabotageId,
                ParseType(state.Definition.DisplayName),
                state.Definition.RoomType,
                true,
                "Sabotage state changed.");
        }

        private static SabotageType ParseType(string displayName)
        {
            return System.Enum.TryParse(displayName, out SabotageType type) ? type : SabotageType.PrinterFault;
        }
    }
}
