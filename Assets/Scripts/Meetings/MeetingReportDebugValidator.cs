using System.Collections.Generic;
using OFIS.Rooms;
using OFIS.Sabotage;
using OFIS.Tasks;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingReportDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly MeetingReportService _reportService = new MeetingReportService();
        private readonly PlayerTaskAssignmentService _taskAssignmentService = new PlayerTaskAssignmentService();
        private readonly OfficeTaskCompletionService _taskCompletionService = new OfficeTaskCompletionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMeetingReportCore();
        }

        [ContextMenu("Validate Meeting Report Core")]
        public void ValidateMeetingReportCore()
        {
            ValidateSubmitSuspicionReport();
            ValidateMissingReporterFails();
            ValidateMissingReportTypeFails();
            ValidateTaskProgressReport();
            ValidateSabotageStatusReport();
            ValidateReportsByReporter();
        }

        private void ValidateSubmitSuspicionReport()
        {
            _reportService.ClearReports();

            MeetingReportData report = new MeetingReportData(
                "report_001",
                MeetingReportType.Suspicion,
                "detective_01",
                "player_02",
                OfficeRoomType.MeetingRoom,
                0,
                0,
                SabotageObjectiveState.None,
                "I suspect player_02.");

            MeetingReportSubmitResult result = _reportService.SubmitReport(report);
            bool passed = result.Success && _reportService.ReportCount == 1;

            LogResult("SubmitSuspicionReport", passed, result);
        }

        private void ValidateMissingReporterFails()
        {
            _reportService.ClearReports();

            MeetingReportData report = new MeetingReportData(
                "report_002",
                MeetingReportType.Defense,
                "",
                "none",
                OfficeRoomType.MeetingRoom,
                0,
                0,
                SabotageObjectiveState.None,
                "I was in accounting.");

            MeetingReportSubmitResult result = _reportService.SubmitReport(report);
            bool passed = !result.Success && _reportService.ReportCount == 0;

            LogResult("MissingReporterFails", passed, result);
        }

        private void ValidateMissingReportTypeFails()
        {
            _reportService.ClearReports();

            MeetingReportData report = new MeetingReportData(
                "report_003",
                MeetingReportType.None,
                "player_01",
                "none",
                OfficeRoomType.MeetingRoom,
                0,
                0,
                SabotageObjectiveState.None,
                "No type.");

            MeetingReportSubmitResult result = _reportService.SubmitReport(report);
            bool passed = !result.Success && _reportService.ReportCount == 0;

            LogResult("MissingReportTypeFails", passed, result);
        }

        private void ValidateTaskProgressReport()
        {
            _reportService.ClearReports();

            PlayerTaskList taskList = BuildTaskList();
            _taskCompletionService.CompleteTask(taskList.FindTaskById("task_review_invoices"), "player_01");

            MeetingReportData report = MeetingReportData.FromTaskProgress(
                "report_004",
                "player_01",
                OfficeRoomType.Accounting,
                taskList,
                "I completed one task.");

            MeetingReportSubmitResult result = _reportService.SubmitReport(report);
            bool passed = result.Success && result.Report.TaskCompletedCount == 1 && result.Report.TaskTotalCount == 3;

            LogResult("TaskProgressReport", passed, result);
        }

        private void ValidateSabotageStatusReport()
        {
            _reportService.ClearReports();

            SabotageObjectiveRuntimeState sabotage = BuildSabotage(SabotageObjectiveState.Active);

            MeetingReportData report = MeetingReportData.FromSabotageStatus(
                "report_005",
                "player_03",
                OfficeRoomType.ServerRoom,
                sabotage,
                "Server room sabotage is active.");

            MeetingReportSubmitResult result = _reportService.SubmitReport(report);
            bool passed = result.Success && result.Report.SabotageState == SabotageObjectiveState.Active;

            LogResult("SabotageStatusReport", passed, result);
        }

        private void ValidateReportsByReporter()
        {
            _reportService.ClearReports();

            _reportService.SubmitReport(new MeetingReportData("report_006", MeetingReportType.GeneralNote, "player_01", "none", OfficeRoomType.Hallway, 0, 0, SabotageObjectiveState.None, "First note."));
            _reportService.SubmitReport(new MeetingReportData("report_007", MeetingReportType.GeneralNote, "player_02", "none", OfficeRoomType.Hallway, 0, 0, SabotageObjectiveState.None, "Second note."));
            _reportService.SubmitReport(new MeetingReportData("report_008", MeetingReportType.GeneralNote, "player_01", "none", OfficeRoomType.ServerRoom, 0, 0, SabotageObjectiveState.None, "Third note."));

            IReadOnlyList<MeetingReportData> reports = _reportService.GetReportsByReporter("player_01");
            bool passed = reports.Count == 2;

            LogListResult("ReportsByReporter", passed, reports.Count);
        }

        private PlayerTaskList BuildTaskList()
        {
            PlayerTaskAssignmentResult assignment = _taskAssignmentService.AssignTasks("player_01", BuildTaskDefinitions());
            return assignment.TaskList;
        }

        private static List<OfficeTaskDefinition> BuildTaskDefinitions()
        {
            return new List<OfficeTaskDefinition>
            {
                new OfficeTaskDefinition("task_review_invoices", "Review invoices", OfficeRoomType.Accounting, 3f),
                new OfficeTaskDefinition("task_check_server_logs", "Check server logs", OfficeRoomType.ServerRoom, 4f),
                new OfficeTaskDefinition("task_sort_archive", "Sort archive files", OfficeRoomType.ArchiveRoom, 5f)
            };
        }

        private static SabotageObjectiveRuntimeState BuildSabotage(SabotageObjectiveState initialState)
        {
            SabotageObjectiveDefinition definition = new SabotageObjectiveDefinition(
                "sabotage_server_room",
                "Server room sabotage",
                OfficeRoomType.ServerRoom,
                5f);

            return new SabotageObjectiveRuntimeState(definition, initialState);
        }

        private static void LogResult(string testName, bool passed, MeetingReportSubmitResult result)
        {
            if (passed)
                Debug.Log($"[MeetingReportValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingReportValidator] FAIL {testName}: {result}");
        }

        private static void LogListResult(string testName, bool passed, int count)
        {
            if (passed)
                Debug.Log($"[MeetingReportValidator] PASS {testName}: Count={count}");
            else
                Debug.LogError($"[MeetingReportValidator] FAIL {testName}: Count={count}");
        }
    }
}
