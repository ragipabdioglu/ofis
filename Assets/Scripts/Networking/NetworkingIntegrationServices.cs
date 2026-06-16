using System.Collections.Generic;
using System.Linq;

namespace OFIS.Networking
{
    public sealed class NetworkCommandDispatcher
    {
        private int _serverTick = 1600;

        public NetworkDispatchResult Dispatch(NetworkCommand command, NetworkModuleHandlerRegistry registry, NetworkPrivacyGuardService privacyGuard)
        {
            if (!registry.HasHandler(command.ModuleType))
                return new NetworkDispatchResult(false, command.ModuleType, _serverTick, "No handler registered.");

            if (!privacyGuard.IsPayloadSafe(command.Visibility, command.PayloadSummary))
                return new NetworkDispatchResult(false, command.ModuleType, _serverTick, "Payload rejected by privacy guard.");

            _serverTick++;
            return new NetworkDispatchResult(true, command.ModuleType, _serverTick, "Command accepted server-side.");
        }
    }

    public sealed class NetworkModuleHandlerRegistry
    {
        private readonly Dictionary<NetworkModuleType, NetworkModuleHandler> _handlers =
            new Dictionary<NetworkModuleType, NetworkModuleHandler>();

        public void RegisterDefaultHandlers()
        {
            Register(NetworkModuleType.Player);
            Register(NetworkModuleType.Task);
            Register(NetworkModuleType.Kill);
            Register(NetworkModuleType.Corpse);
            Register(NetworkModuleType.Sabotage);
            Register(NetworkModuleType.Meeting);
            Register(NetworkModuleType.Voting);
            Register(NetworkModuleType.Detective);
            Register(NetworkModuleType.Victim);
            Register(NetworkModuleType.Voice);
        }

        public void Register(NetworkModuleType moduleType)
        {
            _handlers[moduleType] = new NetworkModuleHandler(moduleType, true, true);
        }

        public bool HasHandler(NetworkModuleType moduleType)
        {
            return _handlers.ContainsKey(moduleType);
        }

        public bool AllMvpHandlersRegistered()
        {
            return _handlers.Count == 10
                && _handlers.Values.All(x => x.ServerAuthoritative && x.SupportsReconnectSnapshot);
        }

        public int Count => _handlers.Count;
    }

    public sealed class NetworkOwnerPayloadService
    {
        public OwnerOnlyPayload BuildOwnerPayload(string ownerPlayerId, NetworkModuleType moduleType, string payloadSummary)
        {
            return new OwnerOnlyPayload(ownerPlayerId, moduleType, NetworkPayloadVisibility.OwnerOnly, payloadSummary);
        }

        public bool CanDeliverTo(string receiverPlayerId, OwnerOnlyPayload payload)
        {
            return payload.Visibility == NetworkPayloadVisibility.OwnerOnly && payload.OwnerPlayerId == receiverPlayerId;
        }
    }

    public sealed class PublicStateSnapshotService
    {
        public PublicStateSnapshot Build(int serverTick, IReadOnlyList<string> playerIds, int companyHealth, string matchPhaseKey)
        {
            return new PublicStateSnapshot(
                serverTick,
                playerIds,
                new[] { "event.task_progress", "event.company_changed", "event.meeting_timer" },
                companyHealth,
                matchPhaseKey);
        }
    }

    public sealed class MovementPredictionCorrectionService
    {
        public MovementCorrection Evaluate(float predictedX, float predictedY, float serverX, float serverY, float tolerance)
        {
            float dx = predictedX - serverX;
            float dy = predictedY - serverY;
            float distanceSquared = dx * dx + dy * dy;
            float toleranceSquared = tolerance * tolerance;

            if (distanceSquared > toleranceSquared)
                return new MovementCorrection(true, serverX, serverY, "Server correction applied.");

            return new MovementCorrection(false, predictedX, predictedY, "Prediction accepted.");
        }
    }

    public sealed class ReliableEventOrderingService
    {
        public IReadOnlyList<OrderedNetworkEvent> Order(IReadOnlyList<OrderedNetworkEvent> events)
        {
            return events == null
                ? new List<OrderedNetworkEvent>()
                : events.OrderBy(x => x.ServerSequence).ToList();
        }
    }

    public sealed class ReconnectSnapshotService
    {
        public ReconnectStateSnapshot Build(string playerId, PublicStateSnapshot publicSnapshot, IReadOnlyList<OwnerOnlyPayload> allOwnerPayloads)
        {
            List<OwnerOnlyPayload> ownerPayloads = new List<OwnerOnlyPayload>();

            if (allOwnerPayloads != null)
            {
                for (int i = 0; i < allOwnerPayloads.Count; i++)
                {
                    if (allOwnerPayloads[i].OwnerPlayerId == playerId)
                        ownerPayloads.Add(allOwnerPayloads[i]);
                }
            }

            int tick = publicSnapshot == null ? 0 : publicSnapshot.ServerTick;
            return new ReconnectStateSnapshot(playerId, tick, publicSnapshot, ownerPayloads);
        }
    }

    public sealed class HeartbeatService
    {
        public HeartbeatStatus Evaluate(int currentServerTick, int lastSeenServerTick, int timeoutTicks)
        {
            int missed = currentServerTick - lastSeenServerTick;
            if (missed >= timeoutTicks)
                return new HeartbeatStatus(NetworkConnectionState.TimedOut, missed, lastSeenServerTick);

            return new HeartbeatStatus(NetworkConnectionState.Connected, missed, lastSeenServerTick);
        }
    }

    public sealed class DisconnectHandlingService
    {
        public DisconnectResolution Resolve(NetworkConnectionState connectionState)
        {
            if (connectionState == NetworkConnectionState.TimedOut || connectionState == NetworkConnectionState.Disconnected)
                return new DisconnectResolution(NetworkConnectionState.Reconnecting, true, true, "Player slot preserved for reconnect.");

            return new DisconnectResolution(NetworkConnectionState.Connected, true, false, "Connection remains active.");
        }
    }

    public sealed class RateLimitService
    {
        public RateLimitResult Evaluate(int commandsInWindow, int maxCommandsInWindow)
        {
            if (commandsInWindow >= maxCommandsInWindow)
                return new RateLimitResult(false, 0, "Command rate limit exceeded.");

            return new RateLimitResult(true, maxCommandsInWindow - commandsInWindow - 1, "Command accepted within rate limit.");
        }
    }

    public sealed class NetworkPrivacyGuardService
    {
        private static readonly string[] HiddenTokens =
        {
            "killer",
            "victim",
            "detective",
            "role=",
            "role:",
            "target=",
            "target:",
            "saboteur",
            "murderer"
        };

        public bool IsPayloadSafe(NetworkPayloadVisibility visibility, string payloadSummary)
        {
            if (visibility != NetworkPayloadVisibility.Public)
                return true;

            if (string.IsNullOrWhiteSpace(payloadSummary))
                return true;

            string lower = payloadSummary.ToLowerInvariant();
            for (int i = 0; i < HiddenTokens.Length; i++)
            {
                if (lower.Contains(HiddenTokens[i]))
                    return false;
            }

            return true;
        }
    }

    public sealed class ServerOnlyAssertService
    {
        public bool AssertServerOnly(NetworkPayloadVisibility visibility, bool isServerContext)
        {
            return visibility != NetworkPayloadVisibility.ServerOnly || isServerContext;
        }
    }

    public sealed class MultiplayerSimulationService
    {
        public MultiplayerSimulationResult RunLocalEightPlayerSimulation(NetworkModuleHandlerRegistry registry, PublicStateSnapshot snapshot)
        {
            bool complete = registry != null && registry.AllMvpHandlersRegistered() && snapshot != null && snapshot.PlayerIds.Count == 8;
            bool leak = snapshot != null && snapshot.PublicEvents.Any(x => x.ToLowerInvariant().Contains("role"));
            return new MultiplayerSimulationResult(snapshot == null ? 0 : snapshot.PlayerIds.Count, complete && !leak, leak, "Local 8-player simulation resolved.");
        }

        public MultiplayerSimulationResult RunNetworkEightPlayerSimulation(
            IReadOnlyList<NetworkDispatchResult> dispatchResults,
            HeartbeatStatus heartbeatStatus,
            ReconnectStateSnapshot reconnectSnapshot)
        {
            bool allAccepted = dispatchResults != null && dispatchResults.Count >= 8 && dispatchResults.All(x => x.Accepted);
            bool connected = heartbeatStatus.ConnectionState == NetworkConnectionState.Connected;
            bool reconnectReady = reconnectSnapshot != null && reconnectSnapshot.PublicSnapshot != null;
            return new MultiplayerSimulationResult(8, allAccepted && connected && reconnectReady, false, "Network 8-player simulation resolved.");
        }
    }
}
