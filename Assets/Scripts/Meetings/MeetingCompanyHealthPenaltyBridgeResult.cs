namespace OFIS.Meetings
{
    public readonly struct MeetingCompanyHealthPenaltyBridgeResult
    {
        public MeetingMissingPlayerPenaltyResult PenaltyResult { get; }
        public int CompanyHealthBefore { get; }
        public int CompanyHealthAfter { get; }
        public int AppliedDelta { get; }
        public bool ChangedHealth { get; }
        public string Message { get; }

        public MeetingCompanyHealthPenaltyBridgeResult(
            MeetingMissingPlayerPenaltyResult penaltyResult,
            int companyHealthBefore,
            int companyHealthAfter,
            string message)
        {
            PenaltyResult = penaltyResult;
            CompanyHealthBefore = companyHealthBefore < 0 ? 0 : companyHealthBefore;
            CompanyHealthAfter = companyHealthAfter < 0 ? 0 : companyHealthAfter;
            AppliedDelta = CompanyHealthBefore - CompanyHealthAfter;
            if (AppliedDelta < 0)
                AppliedDelta = 0;

            ChangedHealth = AppliedDelta > 0;

            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting company health penalty bridge completed."
                : message;
        }

        public override string ToString()
        {
            return $"HealthBefore={CompanyHealthBefore}, HealthAfter={CompanyHealthAfter}, Delta={AppliedDelta}, Changed={ChangedHealth}, Penalty=[{PenaltyResult}], Message={Message}";
        }
    }
}
