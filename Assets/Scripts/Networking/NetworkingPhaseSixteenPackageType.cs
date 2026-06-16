namespace OFIS.Networking
{
    public enum NetworkingPhaseSixteenPackageType
    {
        CommandDispatcher = 0,
        ModuleHandlers = 1,
        OwnerOnlyPayloadTests = 2,
        PublicStateSnapshot = 3,
        MovementPredictionCorrection = 4,
        ReliableEventOrdering = 5,
        ReconnectSnapshot = 6,
        Heartbeat = 7,
        DisconnectHandling = 8,
        RateLimit = 9,
        PrivacyGuard = 10,
        ServerOnlyAssertTests = 11,
        EightPlayerLocalMultiplayerTest = 12,
        EightPlayerNetworkTest = 13,
        PhaseClosure = 14
    }
}
