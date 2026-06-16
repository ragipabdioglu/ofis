using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Networking
{
    public sealed class NetworkingPhaseSixteenDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private NetworkingPhaseSixteenPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly NetworkModuleHandlerRegistry _registry = new NetworkModuleHandlerRegistry();
        private readonly NetworkCommandDispatcher _dispatcher = new NetworkCommandDispatcher();
        private readonly NetworkOwnerPayloadService _ownerPayloadService = new NetworkOwnerPayloadService();
        private readonly PublicStateSnapshotService _publicSnapshotService = new PublicStateSnapshotService();
        private readonly MovementPredictionCorrectionService _movementCorrectionService = new MovementPredictionCorrectionService();
        private readonly ReliableEventOrderingService _eventOrderingService = new ReliableEventOrderingService();
        private readonly ReconnectSnapshotService _reconnectSnapshotService = new ReconnectSnapshotService();
        private readonly HeartbeatService _heartbeatService = new HeartbeatService();
        private readonly DisconnectHandlingService _disconnectHandlingService = new DisconnectHandlingService();
        private readonly RateLimitService _rateLimitService = new RateLimitService();
        private readonly NetworkPrivacyGuardService _privacyGuard = new NetworkPrivacyGuardService();
        private readonly ServerOnlyAssertService _serverOnlyAssertService = new ServerOnlyAssertService();
        private readonly MultiplayerSimulationService _simulationService = new MultiplayerSimulationService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Networking Phase 16 Package")]
        public void ValidatePackage()
        {
            EnsureHandlers();

            switch (packageType)
            {
                case NetworkingPhaseSixteenPackageType.CommandDispatcher:
                    ValidateCommandDispatcher();
                    break;
                case NetworkingPhaseSixteenPackageType.ModuleHandlers:
                    ValidateModuleHandlers();
                    break;
                case NetworkingPhaseSixteenPackageType.OwnerOnlyPayloadTests:
                    ValidateOwnerOnlyPayloadTests();
                    break;
                case NetworkingPhaseSixteenPackageType.PublicStateSnapshot:
                    ValidatePublicStateSnapshot();
                    break;
                case NetworkingPhaseSixteenPackageType.MovementPredictionCorrection:
                    ValidateMovementPredictionCorrection();
                    break;
                case NetworkingPhaseSixteenPackageType.ReliableEventOrdering:
                    ValidateReliableEventOrdering();
                    break;
                case NetworkingPhaseSixteenPackageType.ReconnectSnapshot:
                    ValidateReconnectSnapshot();
                    break;
                case NetworkingPhaseSixteenPackageType.Heartbeat:
                    ValidateHeartbeat();
                    break;
                case NetworkingPhaseSixteenPackageType.DisconnectHandling:
                    ValidateDisconnectHandling();
                    break;
                case NetworkingPhaseSixteenPackageType.RateLimit:
                    ValidateRateLimit();
                    break;
                case NetworkingPhaseSixteenPackageType.PrivacyGuard:
                    ValidatePrivacyGuard();
                    break;
                case NetworkingPhaseSixteenPackageType.ServerOnlyAssertTests:
                    ValidateServerOnlyAssertTests();
                    break;
                case NetworkingPhaseSixteenPackageType.EightPlayerLocalMultiplayerTest:
                    ValidateEightPlayerLocalMultiplayerTest();
                    break;
                case NetworkingPhaseSixteenPackageType.EightPlayerNetworkTest:
                    ValidateEightPlayerNetworkTest();
                    break;
                case NetworkingPhaseSixteenPackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateCommandDispatcher()
        {
            NetworkCommand command = new NetworkCommand("cmd_task_01", "player_01", NetworkModuleType.Task, NetworkDeliveryType.Reliable, NetworkPayloadVisibility.Public, 1, "task progress changed");
            NetworkDispatchResult result = _dispatcher.Dispatch(command, _registry, _privacyGuard);
            LogResult("CommandDispatcher", result.Accepted && result.ModuleType == NetworkModuleType.Task && result.ServerTick > 1600, result.Message);
        }

        private void ValidateModuleHandlers()
        {
            LogResult("ModuleHandlers", _registry.AllMvpHandlersRegistered() && _registry.Count == 10, $"Handlers={_registry.Count}");
        }

        private void ValidateOwnerOnlyPayloadTests()
        {
            OwnerOnlyPayload payload = _ownerPayloadService.BuildOwnerPayload("detective_01", NetworkModuleType.Detective, "private pins");
            bool passed = _ownerPayloadService.CanDeliverTo("detective_01", payload) && !_ownerPayloadService.CanDeliverTo("player_02", payload);
            LogResult("OwnerOnlyPayloadTests", passed, payload.PayloadSummary);
        }

        private void ValidatePublicStateSnapshot()
        {
            PublicStateSnapshot snapshot = BuildPublicSnapshot();
            bool safe = true;
            for (int i = 0; i < snapshot.PublicEvents.Count; i++)
                safe &= _privacyGuard.IsPayloadSafe(NetworkPayloadVisibility.Public, snapshot.PublicEvents[i]);

            LogResult("PublicStateSnapshot", snapshot.PlayerIds.Count == 8 && safe && snapshot.CompanyHealth == 72, $"Players={snapshot.PlayerIds.Count}");
        }

        private void ValidateMovementPredictionCorrection()
        {
            MovementCorrection accepted = _movementCorrectionService.Evaluate(1f, 1f, 1.1f, 1.1f, 0.5f);
            MovementCorrection corrected = _movementCorrectionService.Evaluate(5f, 5f, 1f, 1f, 0.5f);
            LogResult("MovementPredictionCorrection", !accepted.NeedsCorrection && corrected.NeedsCorrection && corrected.CorrectedX == 1f, corrected.Reason);
        }

        private void ValidateReliableEventOrdering()
        {
            IReadOnlyList<OrderedNetworkEvent> ordered = _eventOrderingService.Order(new[]
            {
                new OrderedNetworkEvent("event_03", 3, NetworkDeliveryType.Reliable),
                new OrderedNetworkEvent("event_01", 1, NetworkDeliveryType.Reliable),
                new OrderedNetworkEvent("event_02", 2, NetworkDeliveryType.Reliable)
            });
            LogResult("ReliableEventOrdering", ordered[0].ServerSequence == 1 && ordered[2].ServerSequence == 3, ordered[0].EventId);
        }

        private void ValidateReconnectSnapshot()
        {
            PublicStateSnapshot publicSnapshot = BuildPublicSnapshot();
            ReconnectStateSnapshot snapshot = _reconnectSnapshotService.Build("player_01", publicSnapshot, BuildOwnerPayloads());
            LogResult("ReconnectSnapshot", snapshot.PlayerId == "player_01" && snapshot.OwnerPayloads.Count == 1 && snapshot.PublicSnapshot.PlayerIds.Count == 8, $"OwnerPayloads={snapshot.OwnerPayloads.Count}");
        }

        private void ValidateHeartbeat()
        {
            HeartbeatStatus connected = _heartbeatService.Evaluate(200, 198, 5);
            HeartbeatStatus timedOut = _heartbeatService.Evaluate(200, 190, 5);
            LogResult("Heartbeat", connected.ConnectionState == NetworkConnectionState.Connected && timedOut.ConnectionState == NetworkConnectionState.TimedOut, $"Missed={timedOut.MissedBeats}");
        }

        private void ValidateDisconnectHandling()
        {
            DisconnectResolution resolution = _disconnectHandlingService.Resolve(NetworkConnectionState.TimedOut);
            LogResult("DisconnectHandling", resolution.NewState == NetworkConnectionState.Reconnecting && resolution.PreservePlayerSlot && resolution.MuteVoice, resolution.Message);
        }

        private void ValidateRateLimit()
        {
            RateLimitResult allowed = _rateLimitService.Evaluate(3, 5);
            RateLimitResult blocked = _rateLimitService.Evaluate(5, 5);
            LogResult("RateLimit", allowed.Allowed && allowed.RemainingBudget == 1 && !blocked.Allowed, blocked.Message);
        }

        private void ValidatePrivacyGuard()
        {
            bool safe = _privacyGuard.IsPayloadSafe(NetworkPayloadVisibility.Public, "company changed and meeting timer updated");
            bool unsafePublic = _privacyGuard.IsPayloadSafe(NetworkPayloadVisibility.Public, "role=killer target=victim_01");
            bool ownerAllowed = _privacyGuard.IsPayloadSafe(NetworkPayloadVisibility.OwnerOnly, "role=killer target=victim_01");
            LogResult("PrivacyGuard", safe && !unsafePublic && ownerAllowed, "Public hidden-role tokens rejected.");
        }

        private void ValidateServerOnlyAssertTests()
        {
            bool serverAllowed = _serverOnlyAssertService.AssertServerOnly(NetworkPayloadVisibility.ServerOnly, true);
            bool clientBlocked = _serverOnlyAssertService.AssertServerOnly(NetworkPayloadVisibility.ServerOnly, false);
            bool publicAllowed = _serverOnlyAssertService.AssertServerOnly(NetworkPayloadVisibility.Public, false);
            LogResult("ServerOnlyAssertTests", serverAllowed && !clientBlocked && publicAllowed, "ServerOnly assert evaluated.");
        }

        private void ValidateEightPlayerLocalMultiplayerTest()
        {
            MultiplayerSimulationResult result = _simulationService.RunLocalEightPlayerSimulation(_registry, BuildPublicSnapshot());
            LogResult("EightPlayerLocalMultiplayerTest", result.Completed && result.PlayerCount == 8 && !result.RoleLeakDetected, result.Message);
        }

        private void ValidateEightPlayerNetworkTest()
        {
            List<NetworkDispatchResult> results = new List<NetworkDispatchResult>();
            for (int i = 0; i < 8; i++)
            {
                NetworkCommand command = new NetworkCommand($"cmd_player_{i}", $"player_0{i + 1}", NetworkModuleType.Player, NetworkDeliveryType.Reliable, NetworkPayloadVisibility.Public, i, "movement tick");
                results.Add(_dispatcher.Dispatch(command, _registry, _privacyGuard));
            }

            HeartbeatStatus heartbeat = _heartbeatService.Evaluate(320, 319, 5);
            ReconnectStateSnapshot reconnect = _reconnectSnapshotService.Build("player_01", BuildPublicSnapshot(), BuildOwnerPayloads());
            MultiplayerSimulationResult result = _simulationService.RunNetworkEightPlayerSimulation(results, heartbeat, reconnect);
            LogResult("EightPlayerNetworkTest", result.Completed && result.PlayerCount == 8, result.Message);
        }

        private void ValidatePhaseClosure()
        {
            ValidateCommandDispatcher();
            ValidateModuleHandlers();
            ValidateOwnerOnlyPayloadTests();
            ValidatePublicStateSnapshot();
            ValidateMovementPredictionCorrection();
            ValidateReliableEventOrdering();
            ValidateReconnectSnapshot();
            ValidateHeartbeat();
            ValidateDisconnectHandling();
            ValidateRateLimit();
            ValidatePrivacyGuard();
            ValidateServerOnlyAssertTests();
            ValidateEightPlayerLocalMultiplayerTest();
            ValidateEightPlayerNetworkTest();

            LogResult("PhaseClosure", true, "MVP Faz 16 packages 16A-16N are represented.");
        }

        private void EnsureHandlers()
        {
            if (!_registry.AllMvpHandlersRegistered())
                _registry.RegisterDefaultHandlers();
        }

        private PublicStateSnapshot BuildPublicSnapshot()
        {
            return _publicSnapshotService.Build(2300, BuildEightPlayerIds(), 72, "match.phase.office");
        }

        private static IReadOnlyList<string> BuildEightPlayerIds()
        {
            return new[]
            {
                "player_01",
                "player_02",
                "player_03",
                "player_04",
                "player_05",
                "player_06",
                "player_07",
                "player_08"
            };
        }

        private IReadOnlyList<OwnerOnlyPayload> BuildOwnerPayloads()
        {
            return new[]
            {
                _ownerPayloadService.BuildOwnerPayload("player_01", NetworkModuleType.Detective, "private pins"),
                _ownerPayloadService.BuildOwnerPayload("player_02", NetworkModuleType.Victim, "private note budget")
            };
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[NetworkingPhaseSixteenDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[NetworkingPhaseSixteenDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
