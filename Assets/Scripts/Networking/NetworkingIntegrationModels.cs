using System.Collections.Generic;

namespace OFIS.Networking
{
    public enum NetworkModuleType
    {
        Player = 0,
        Task = 1,
        Kill = 2,
        Corpse = 3,
        Sabotage = 4,
        Meeting = 5,
        Voting = 6,
        Detective = 7,
        Victim = 8,
        Voice = 9
    }

    public enum NetworkDeliveryType
    {
        Reliable = 0,
        Unreliable = 1
    }

    public enum NetworkPayloadVisibility
    {
        Public = 0,
        OwnerOnly = 1,
        ServerOnly = 2
    }

    public enum NetworkConnectionState
    {
        Connected = 0,
        TimedOut = 1,
        Disconnected = 2,
        Reconnecting = 3
    }

    public readonly struct NetworkCommand
    {
        public string CommandId { get; }
        public string SenderPlayerId { get; }
        public NetworkModuleType ModuleType { get; }
        public NetworkDeliveryType DeliveryType { get; }
        public NetworkPayloadVisibility Visibility { get; }
        public int ClientSequence { get; }
        public string PayloadSummary { get; }

        public NetworkCommand(
            string commandId,
            string senderPlayerId,
            NetworkModuleType moduleType,
            NetworkDeliveryType deliveryType,
            NetworkPayloadVisibility visibility,
            int clientSequence,
            string payloadSummary)
        {
            CommandId = string.IsNullOrWhiteSpace(commandId) ? "command_unknown" : commandId;
            SenderPlayerId = string.IsNullOrWhiteSpace(senderPlayerId) ? "unknown_player" : senderPlayerId;
            ModuleType = moduleType;
            DeliveryType = deliveryType;
            Visibility = visibility;
            ClientSequence = clientSequence < 0 ? 0 : clientSequence;
            PayloadSummary = string.IsNullOrWhiteSpace(payloadSummary) ? "payload.empty" : payloadSummary;
        }
    }

    public readonly struct NetworkDispatchResult
    {
        public bool Accepted { get; }
        public NetworkModuleType ModuleType { get; }
        public int ServerTick { get; }
        public string Message { get; }

        public NetworkDispatchResult(bool accepted, NetworkModuleType moduleType, int serverTick, string message)
        {
            Accepted = accepted;
            ModuleType = moduleType;
            ServerTick = serverTick < 0 ? 0 : serverTick;
            Message = string.IsNullOrWhiteSpace(message) ? "Dispatch resolved." : message;
        }
    }

    public readonly struct NetworkModuleHandler
    {
        public NetworkModuleType ModuleType { get; }
        public bool ServerAuthoritative { get; }
        public bool SupportsReconnectSnapshot { get; }

        public NetworkModuleHandler(NetworkModuleType moduleType, bool serverAuthoritative, bool supportsReconnectSnapshot)
        {
            ModuleType = moduleType;
            ServerAuthoritative = serverAuthoritative;
            SupportsReconnectSnapshot = supportsReconnectSnapshot;
        }
    }

    public readonly struct OwnerOnlyPayload
    {
        public string OwnerPlayerId { get; }
        public NetworkModuleType ModuleType { get; }
        public NetworkPayloadVisibility Visibility { get; }
        public string PayloadSummary { get; }

        public OwnerOnlyPayload(string ownerPlayerId, NetworkModuleType moduleType, NetworkPayloadVisibility visibility, string payloadSummary)
        {
            OwnerPlayerId = string.IsNullOrWhiteSpace(ownerPlayerId) ? "unknown_owner" : ownerPlayerId;
            ModuleType = moduleType;
            Visibility = visibility;
            PayloadSummary = string.IsNullOrWhiteSpace(payloadSummary) ? "payload.empty" : payloadSummary;
        }
    }

    public sealed class PublicStateSnapshot
    {
        private readonly List<string> _playerIds = new List<string>();
        private readonly List<string> _publicEvents = new List<string>();

        public int ServerTick { get; }
        public IReadOnlyList<string> PlayerIds => _playerIds;
        public IReadOnlyList<string> PublicEvents => _publicEvents;
        public int CompanyHealth { get; }
        public string MatchPhaseKey { get; }

        public PublicStateSnapshot(int serverTick, IEnumerable<string> playerIds, IEnumerable<string> publicEvents, int companyHealth, string matchPhaseKey)
        {
            ServerTick = serverTick < 0 ? 0 : serverTick;
            CompanyHealth = companyHealth < 0 ? 0 : companyHealth > 100 ? 100 : companyHealth;
            MatchPhaseKey = string.IsNullOrWhiteSpace(matchPhaseKey) ? "match.phase.unknown" : matchPhaseKey;

            if (playerIds != null)
                _playerIds.AddRange(playerIds);

            if (publicEvents != null)
                _publicEvents.AddRange(publicEvents);
        }
    }

    public readonly struct MovementCorrection
    {
        public bool NeedsCorrection { get; }
        public float CorrectedX { get; }
        public float CorrectedY { get; }
        public string Reason { get; }

        public MovementCorrection(bool needsCorrection, float correctedX, float correctedY, string reason)
        {
            NeedsCorrection = needsCorrection;
            CorrectedX = correctedX;
            CorrectedY = correctedY;
            Reason = string.IsNullOrWhiteSpace(reason) ? "Movement accepted." : reason;
        }
    }

    public readonly struct OrderedNetworkEvent
    {
        public string EventId { get; }
        public int ServerSequence { get; }
        public NetworkDeliveryType DeliveryType { get; }

        public OrderedNetworkEvent(string eventId, int serverSequence, NetworkDeliveryType deliveryType)
        {
            EventId = string.IsNullOrWhiteSpace(eventId) ? "event_unknown" : eventId;
            ServerSequence = serverSequence < 0 ? 0 : serverSequence;
            DeliveryType = deliveryType;
        }
    }

    public sealed class ReconnectStateSnapshot
    {
        private readonly List<OwnerOnlyPayload> _ownerPayloads = new List<OwnerOnlyPayload>();

        public string PlayerId { get; }
        public int ServerTick { get; }
        public PublicStateSnapshot PublicSnapshot { get; }
        public IReadOnlyList<OwnerOnlyPayload> OwnerPayloads => _ownerPayloads;

        public ReconnectStateSnapshot(string playerId, int serverTick, PublicStateSnapshot publicSnapshot, IEnumerable<OwnerOnlyPayload> ownerPayloads)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? "unknown_player" : playerId;
            ServerTick = serverTick < 0 ? 0 : serverTick;
            PublicSnapshot = publicSnapshot;

            if (ownerPayloads != null)
                _ownerPayloads.AddRange(ownerPayloads);
        }
    }

    public readonly struct HeartbeatStatus
    {
        public NetworkConnectionState ConnectionState { get; }
        public int MissedBeats { get; }
        public int LastSeenServerTick { get; }

        public HeartbeatStatus(NetworkConnectionState connectionState, int missedBeats, int lastSeenServerTick)
        {
            ConnectionState = connectionState;
            MissedBeats = missedBeats < 0 ? 0 : missedBeats;
            LastSeenServerTick = lastSeenServerTick < 0 ? 0 : lastSeenServerTick;
        }
    }

    public readonly struct DisconnectResolution
    {
        public NetworkConnectionState NewState { get; }
        public bool PreservePlayerSlot { get; }
        public bool MuteVoice { get; }
        public string Message { get; }

        public DisconnectResolution(NetworkConnectionState newState, bool preservePlayerSlot, bool muteVoice, string message)
        {
            NewState = newState;
            PreservePlayerSlot = preservePlayerSlot;
            MuteVoice = muteVoice;
            Message = string.IsNullOrWhiteSpace(message) ? "Disconnect resolved." : message;
        }
    }

    public readonly struct RateLimitResult
    {
        public bool Allowed { get; }
        public int RemainingBudget { get; }
        public string Message { get; }

        public RateLimitResult(bool allowed, int remainingBudget, string message)
        {
            Allowed = allowed;
            RemainingBudget = remainingBudget < 0 ? 0 : remainingBudget;
            Message = string.IsNullOrWhiteSpace(message) ? "Rate limit resolved." : message;
        }
    }

    public readonly struct MultiplayerSimulationResult
    {
        public int PlayerCount { get; }
        public bool Completed { get; }
        public bool RoleLeakDetected { get; }
        public string Message { get; }

        public MultiplayerSimulationResult(int playerCount, bool completed, bool roleLeakDetected, string message)
        {
            PlayerCount = playerCount < 0 ? 0 : playerCount;
            Completed = completed;
            RoleLeakDetected = roleLeakDetected;
            Message = string.IsNullOrWhiteSpace(message) ? "Simulation resolved." : message;
        }
    }
}
