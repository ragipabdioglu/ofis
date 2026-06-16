namespace OFIS.Meetings
{
    public sealed class MeetingMissingPlayerPenaltyService
    {
        private readonly MeetingMissingPlayerPenaltyConfig _config;

        public MeetingMissingPlayerPenaltyService()
        {
            _config = MeetingMissingPlayerPenaltyConfig.Default;
        }

        public MeetingMissingPlayerPenaltyService(MeetingMissingPlayerPenaltyConfig config)
        {
            _config = config;
        }

        public MeetingMissingPlayerPenaltyConfig Config => _config;

        public MeetingMissingPlayerPenaltyResult Evaluate(MeetingAttendanceRegistrationResult attendanceResult)
        {
            if (attendanceResult == null)
            {
                return new MeetingMissingPlayerPenaltyResult(
                    missingEligiblePlayerCount: 0,
                    registeredPlayerCount: 0,
                    rawPenaltyAmount: 0,
                    appliedPenaltyAmount: 0,
                    shouldApplyPenalty: false,
                    wasCapped: false,
                    reason: "Attendance result is null.");
            }

            if (attendanceResult.MissingEligibleCount <= 0)
            {
                return new MeetingMissingPlayerPenaltyResult(
                    missingEligiblePlayerCount: attendanceResult.MissingEligibleCount,
                    registeredPlayerCount: attendanceResult.RegisteredCount,
                    rawPenaltyAmount: 0,
                    appliedPenaltyAmount: 0,
                    shouldApplyPenalty: false,
                    wasCapped: false,
                    reason: "No missing eligible players.");
            }

            if (attendanceResult.RegisteredCount <= 0 && !_config.ApplyPenaltyWhenNoRegisteredPlayers)
            {
                return new MeetingMissingPlayerPenaltyResult(
                    missingEligiblePlayerCount: attendanceResult.MissingEligibleCount,
                    registeredPlayerCount: attendanceResult.RegisteredCount,
                    rawPenaltyAmount: 0,
                    appliedPenaltyAmount: 0,
                    shouldApplyPenalty: false,
                    wasCapped: false,
                    reason: "No registered players and config blocks penalty.");
            }

            int rawPenalty = attendanceResult.MissingEligibleCount * _config.PenaltyPerMissingPlayer;
            int appliedPenalty = rawPenalty;
            bool wasCapped = false;

            if (_config.MaxPenaltyPerMeeting > 0 && appliedPenalty > _config.MaxPenaltyPerMeeting)
            {
                appliedPenalty = _config.MaxPenaltyPerMeeting;
                wasCapped = true;
            }

            bool shouldApply = appliedPenalty > 0;

            return new MeetingMissingPlayerPenaltyResult(
                missingEligiblePlayerCount: attendanceResult.MissingEligibleCount,
                registeredPlayerCount: attendanceResult.RegisteredCount,
                rawPenaltyAmount: rawPenalty,
                appliedPenaltyAmount: appliedPenalty,
                shouldApplyPenalty: shouldApply,
                wasCapped: wasCapped,
                reason: shouldApply
                    ? "Missing eligible players produced company health penalty."
                    : "Penalty amount is zero.");
        }
    }
}
