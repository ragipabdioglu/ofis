namespace OFIS.Meetings
{
    public sealed class MeetingCompanyHealthPenaltyBridgeService
    {
        private readonly MeetingMissingPlayerPenaltyService _penaltyService;

        public MeetingCompanyHealthPenaltyBridgeService()
        {
            _penaltyService = new MeetingMissingPlayerPenaltyService();
        }

        public MeetingCompanyHealthPenaltyBridgeService(MeetingMissingPlayerPenaltyService penaltyService)
        {
            _penaltyService = penaltyService ?? new MeetingMissingPlayerPenaltyService();
        }

        public MeetingCompanyHealthPenaltyBridgeResult BuildBridgeResult(
            MeetingAttendanceRegistrationResult attendanceResult,
            int currentCompanyHealth)
        {
            MeetingMissingPlayerPenaltyResult penaltyResult = _penaltyService.Evaluate(attendanceResult);

            int safeCurrentHealth = currentCompanyHealth < 0 ? 0 : currentCompanyHealth;
            int nextHealth = safeCurrentHealth;

            if (penaltyResult.ShouldApplyPenalty)
            {
                nextHealth -= penaltyResult.AppliedPenaltyAmount;
                if (nextHealth < 0)
                    nextHealth = 0;
            }

            return new MeetingCompanyHealthPenaltyBridgeResult(
                penaltyResult,
                safeCurrentHealth,
                nextHealth,
                penaltyResult.ShouldApplyPenalty
                    ? "Company health penalty bridge produced a health reduction."
                    : "Company health penalty bridge produced no health change.");
        }
    }
}
