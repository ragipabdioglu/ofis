namespace OFIS.Meetings
{
    public readonly struct MeetingReportSubmitResult
    {
        public bool Success { get; }
        public MeetingReportData Report { get; }
        public string Message { get; }

        public MeetingReportSubmitResult(bool success, MeetingReportData report, string message)
        {
            Success = success;
            Report = report;
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public static MeetingReportSubmitResult Failed(MeetingReportData report, string message)
        {
            return new MeetingReportSubmitResult(false, report, message);
        }

        public override string ToString()
        {
            return $"Success={Success}, Report={Report}, Message={Message}";
        }
    }
}
