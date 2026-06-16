namespace OFIS.Product.Hardening
{
    public enum ProductHardeningPhaseOnePackageType
    {
        DedicatedServerBuildPipeline = 0,
        ServerLogging = 1,
        CrashRecovery = 2,
        MatchCancellationHandling = 3,
        NetworkPacketLossTest = 4,
        HighPingSimulation = 5,
        ReconnectEdgeCaseTest = 6,
        ServerOnlyDataAudit = 7,
        PrivacyGuardExpansion = 8,
        AutomatedIntegrationTests = 9,
        LoadTestPlayerCounts = 10,
        MemoryAllocationOptimization = 11,
        GcSpikeControl = 12,
        SceneLoadOptimization = 13,
        RealVoiceProviderImplementation = 14,
        PhaseClosure = 15
    }
}
