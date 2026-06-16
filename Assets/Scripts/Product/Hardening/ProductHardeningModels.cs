using System.Collections.Generic;

namespace OFIS.Product.Hardening
{
    public enum ProductBuildTargetType
    {
        Client,
        DedicatedServer
    }

    public enum HardeningGateSeverity
    {
        Info,
        Warning,
        Blocker
    }

    public readonly struct HardeningGateResult
    {
        public HardeningGateResult(bool passed, string message, HardeningGateSeverity severity = HardeningGateSeverity.Blocker)
        {
            Passed = passed;
            Message = message;
            Severity = severity;
        }

        public bool Passed { get; }
        public string Message { get; }
        public HardeningGateSeverity Severity { get; }
    }

    public readonly struct DedicatedServerBuildSnapshot
    {
        public DedicatedServerBuildSnapshot(ProductBuildTargetType target, bool headlessMode, bool deterministicProfile, int sceneCount, bool clientAssetsExcluded)
        {
            Target = target;
            HeadlessMode = headlessMode;
            DeterministicProfile = deterministicProfile;
            SceneCount = sceneCount;
            ClientAssetsExcluded = clientAssetsExcluded;
        }

        public ProductBuildTargetType Target { get; }
        public bool HeadlessMode { get; }
        public bool DeterministicProfile { get; }
        public int SceneCount { get; }
        public bool ClientAssetsExcluded { get; }
    }

    public readonly struct ServerLogRecord
    {
        public ServerLogRecord(string category, string message, int serverTick, bool structured, bool identityRedacted)
        {
            Category = category;
            Message = message;
            ServerTick = serverTick;
            Structured = structured;
            IdentityRedacted = identityRedacted;
        }

        public string Category { get; }
        public string Message { get; }
        public int ServerTick { get; }
        public bool Structured { get; }
        public bool IdentityRedacted { get; }
    }

    public readonly struct CrashRecoverySnapshot
    {
        public CrashRecoverySnapshot(string matchId, int savedTick, int restoredTick, bool serverStateOnly, bool safeRoomState)
        {
            MatchId = matchId;
            SavedTick = savedTick;
            RestoredTick = restoredTick;
            ServerStateOnly = serverStateOnly;
            SafeRoomState = safeRoomState;
        }

        public string MatchId { get; }
        public int SavedTick { get; }
        public int RestoredTick { get; }
        public bool ServerStateOnly { get; }
        public bool SafeRoomState { get; }
    }

    public readonly struct NetworkSimulationSnapshot
    {
        public NetworkSimulationSnapshot(int packetLossPercent, int latencyMs, bool reliableActionsSurvive, bool reconciliationStable)
        {
            PacketLossPercent = packetLossPercent;
            LatencyMs = latencyMs;
            ReliableActionsSurvive = reliableActionsSurvive;
            ReconciliationStable = reconciliationStable;
        }

        public int PacketLossPercent { get; }
        public int LatencyMs { get; }
        public bool ReliableActionsSurvive { get; }
        public bool ReconciliationStable { get; }
    }

    public readonly struct ReconnectEdgeCaseSnapshot
    {
        public ReconnectEdgeCaseSnapshot(bool seatDeduplicated, bool roleHiddenFromClient, bool positionRestored, bool voiceSessionRestored)
        {
            SeatDeduplicated = seatDeduplicated;
            RoleHiddenFromClient = roleHiddenFromClient;
            PositionRestored = positionRestored;
            VoiceSessionRestored = voiceSessionRestored;
        }

        public bool SeatDeduplicated { get; }
        public bool RoleHiddenFromClient { get; }
        public bool PositionRestored { get; }
        public bool VoiceSessionRestored { get; }
    }

    public readonly struct LoadTestSnapshot
    {
        public LoadTestSnapshot(int playerCount, bool matchCompleted, float maxFrameMs, int disconnectCount)
        {
            PlayerCount = playerCount;
            MatchCompleted = matchCompleted;
            MaxFrameMs = maxFrameMs;
            DisconnectCount = disconnectCount;
        }

        public int PlayerCount { get; }
        public bool MatchCompleted { get; }
        public float MaxFrameMs { get; }
        public int DisconnectCount { get; }
    }

    public readonly struct RuntimePerformanceSnapshot
    {
        public RuntimePerformanceSnapshot(int allocationBytesPerTick, float maxGcSpikeMs, float sceneLoadSeconds, bool asyncSceneLoad)
        {
            AllocationBytesPerTick = allocationBytesPerTick;
            MaxGcSpikeMs = maxGcSpikeMs;
            SceneLoadSeconds = sceneLoadSeconds;
            AsyncSceneLoad = asyncSceneLoad;
        }

        public int AllocationBytesPerTick { get; }
        public float MaxGcSpikeMs { get; }
        public float SceneLoadSeconds { get; }
        public bool AsyncSceneLoad { get; }
    }

    public readonly struct VoiceProviderSnapshot
    {
        public VoiceProviderSnapshot(string providerName, bool realProvider, bool reconnectSupported, bool recordsContent)
        {
            ProviderName = providerName;
            RealProvider = realProvider;
            ReconnectSupported = reconnectSupported;
            RecordsContent = recordsContent;
        }

        public string ProviderName { get; }
        public bool RealProvider { get; }
        public bool ReconnectSupported { get; }
        public bool RecordsContent { get; }
    }

    public readonly struct IntegrationTestSnapshot
    {
        public IntegrationTestSnapshot(IReadOnlyList<string> passingSuites)
        {
            PassingSuites = passingSuites;
        }

        public IReadOnlyList<string> PassingSuites { get; }
    }
}
