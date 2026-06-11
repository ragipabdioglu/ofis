using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingReportService
    {
        private readonly List<MeetingReportData> _reports = new List<MeetingReportData>();

        public IReadOnlyList<MeetingReportData> Reports => _reports;
        public int ReportCount => _reports.Count;

        public MeetingReportSubmitResult SubmitReport(MeetingReportData report)
        {
            if (string.IsNullOrWhiteSpace(report.ReporterPlayerId) || report.ReporterPlayerId == "unknown_reporter")
                return MeetingReportSubmitResult.Failed(report, "Reporter player id is missing.");

            if (report.ReportType == MeetingReportType.None)
                return MeetingReportSubmitResult.Failed(report, "Report type is missing.");

            _reports.Add(report);

            return new MeetingReportSubmitResult(true, report, "Meeting report submitted.");
        }

        public IReadOnlyList<MeetingReportData> GetReportsByReporter(string reporterPlayerId)
        {
            List<MeetingReportData> result = new List<MeetingReportData>();

            for (int i = 0; i < _reports.Count; i++)
            {
                if (_reports[i].ReporterPlayerId == reporterPlayerId)
                    result.Add(_reports[i]);
            }

            return result;
        }

        public void ClearReports()
        {
            _reports.Clear();
        }
    }
}
