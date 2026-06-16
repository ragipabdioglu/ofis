using OFIS.Rooms;

namespace OFIS.Logs
{
    public readonly struct PublicReport
    {
        public string ReportId { get; }
        public RecordCategory Category { get; }
        public OfficeRoomType RoomType { get; }
        public float ServerTimeSeconds { get; }
        public string Summary { get; }

        public PublicReport(
            string reportId,
            RecordCategory category,
            OfficeRoomType roomType,
            float serverTimeSeconds,
            string summary)
        {
            ReportId = string.IsNullOrWhiteSpace(reportId) ? "unknown_report" : reportId;
            Category = category;
            RoomType = roomType;
            ServerTimeSeconds = serverTimeSeconds < 0f ? 0f : serverTimeSeconds;
            Summary = string.IsNullOrWhiteSpace(summary) ? "Report available." : summary;
        }

        public override string ToString()
        {
            return $"Report={ReportId}, Category={Category}, Room={RoomType}, Time={ServerTimeSeconds:0.##}, Summary={Summary}";
        }
    }
}
