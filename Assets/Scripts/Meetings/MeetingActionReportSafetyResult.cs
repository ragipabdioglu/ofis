namespace OFIS.Meetings
{
    public readonly struct MeetingActionReportSafetyResult
    {
        public bool IsSafe { get; }
        public bool RevealsRole { get; }
        public bool RevealsDefiniteKiller { get; }
        public string Message { get; }

        public MeetingActionReportSafetyResult(
            bool isSafe,
            bool revealsRole,
            bool revealsDefiniteKiller,
            string message)
        {
            IsSafe = isSafe;
            RevealsRole = revealsRole;
            RevealsDefiniteKiller = revealsDefiniteKiller;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action report safety resolved."
                : message;
        }

        public override string ToString()
        {
            return $"Safe={IsSafe}, RevealsRole={RevealsRole}, RevealsDefiniteKiller={RevealsDefiniteKiller}, Message={Message}";
        }
    }
}
