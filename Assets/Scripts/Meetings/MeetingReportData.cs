using OFIS.Rooms;
using OFIS.Sabotage;
using OFIS.Tasks;

namespace OFIS.Meetings
{
    public readonly struct MeetingReportData
    {
        public string ReportId { get; }
        public MeetingReportType ReportType { get; }
        public string ReporterPlayerId { get; }
        public string TargetPlayerId { get; }
        public OfficeRoomType RoomType { get; }
        public int TaskCompletedCount { get; }
        public int TaskTotalCount { get; }
        public SabotageObjectiveState SabotageState { get; }
        public string Message { get; }

        public MeetingReportData(
            string reportId,
            MeetingReportType reportType,
            string reporterPlayerId,
            string targetPlayerId,
            OfficeRoomType roomType,
            int taskCompletedCount,
            int taskTotalCount,
            SabotageObjectiveState sabotageState,
            string message)
        {
            ReportId = string.IsNullOrWhiteSpace(reportId) ? "unknown_report" : reportId;
            ReportType = reportType;
            ReporterPlayerId = string.IsNullOrWhiteSpace(reporterPlayerId) ? "unknown_reporter" : reporterPlayerId;
            TargetPlayerId = string.IsNullOrWhiteSpace(targetPlayerId) ? "none" : targetPlayerId;
            RoomType = roomType;
            TaskCompletedCount = taskCompletedCount < 0 ? 0 : taskCompletedCount;
            TaskTotalCount = taskTotalCount < 0 ? 0 : taskTotalCount;
            SabotageState = sabotageState;
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public static MeetingReportData FromTaskProgress(
            string reportId,
            string reporterPlayerId,
            OfficeRoomType roomType,
            PlayerTaskList taskList,
            string message)
        {
            int completed = taskList == null ? 0 : taskList.CompletedCount;
            int total = taskList == null ? 0 : taskList.TotalCount;

            return new MeetingReportData(
                reportId,
                MeetingReportType.TaskProgress,
                reporterPlayerId,
                "none",
                roomType,
                completed,
                total,
                SabotageObjectiveState.None,
                message);
        }

        public static MeetingReportData FromSabotageStatus(
            string reportId,
            string reporterPlayerId,
            OfficeRoomType roomType,
            SabotageObjectiveRuntimeState sabotageState,
            string message)
        {
            SabotageObjectiveState state = sabotageState == null ? SabotageObjectiveState.None : sabotageState.State;

            return new MeetingReportData(
                reportId,
                MeetingReportType.SabotageStatus,
                reporterPlayerId,
                "none",
                roomType,
                0,
                0,
                state,
                message);
        }

        public override string ToString()
        {
            return $"ReportId={ReportId}, Type={ReportType}, Reporter={ReporterPlayerId}, Target={TargetPlayerId}, Room={RoomType}, Task={TaskCompletedCount}/{TaskTotalCount}, Sabotage={SabotageState}, Message={Message}";
        }
    }
}
