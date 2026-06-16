namespace OFIS.Meetings
{
    public readonly struct MeetingMissingPlayerPenaltyResult
    {
        public int MissingEligiblePlayerCount { get; }
        public int RegisteredPlayerCount { get; }
        public int RawPenaltyAmount { get; }
        public int AppliedPenaltyAmount { get; }
        public bool ShouldApplyPenalty { get; }
        public bool WasCapped { get; }
        public string Reason { get; }

        public MeetingMissingPlayerPenaltyResult(
            int missingEligiblePlayerCount,
            int registeredPlayerCount,
            int rawPenaltyAmount,
            int appliedPenaltyAmount,
            bool shouldApplyPenalty,
            bool wasCapped,
            string reason)
        {
            MissingEligiblePlayerCount = missingEligiblePlayerCount < 0 ? 0 : missingEligiblePlayerCount;
            RegisteredPlayerCount = registeredPlayerCount < 0 ? 0 : registeredPlayerCount;
            RawPenaltyAmount = rawPenaltyAmount < 0 ? 0 : rawPenaltyAmount;
            AppliedPenaltyAmount = appliedPenaltyAmount < 0 ? 0 : appliedPenaltyAmount;
            ShouldApplyPenalty = shouldApplyPenalty;
            WasCapped = wasCapped;
            Reason = string.IsNullOrWhiteSpace(reason)
                ? "Meeting missing player penalty evaluated."
                : reason;
        }

        public override string ToString()
        {
            return $"MissingEligible={MissingEligiblePlayerCount}, Registered={RegisteredPlayerCount}, RawPenalty={RawPenaltyAmount}, AppliedPenalty={AppliedPenaltyAmount}, ShouldApply={ShouldApplyPenalty}, Capped={WasCapped}, Reason={Reason}";
        }
    }
}
