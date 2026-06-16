using OFIS.Core.Ids;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Logs
{
    public sealed class LogsPhaseNineDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private LogsPhaseNinePackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly ServerRecordStore _recordStore = new ServerRecordStore();
        private readonly PublicReportProjectionService _projectionService = new PublicReportProjectionService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Logs Phase 9 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case LogsPhaseNinePackageType.ServerRecordPublicReportBoundary:
                    ValidatePhaseNineA();
                    break;
                case LogsPhaseNinePackageType.RecordCategoryCoverage:
                    ValidateRecordCategoryCoverage();
                    break;
                case LogsPhaseNinePackageType.KillServerOnlyRecords:
                    ValidateKillRecordStaysServerOnly();
                    break;
                case LogsPhaseNinePackageType.PublicReportPrivacy:
                    ValidatePublicReportRejectsIdentityLeak();
                    break;
                case LogsPhaseNinePackageType.DoorCardLogs:
                    ValidateDoorCardLogs();
                    break;
                case LogsPhaseNinePackageType.CameraPassageLogs:
                    ValidateCameraPassageLogs();
                    break;
                case LogsPhaseNinePackageType.TaskLifecycleLogs:
                    ValidateTaskLifecycleLogs();
                    break;
                case LogsPhaseNinePackageType.SecurityReviewWindow:
                    ValidateSecurityReviewWindow();
                    break;
                case LogsPhaseNinePackageType.PersonnelAuditReport:
                    ValidatePersonnelAuditReport();
                    break;
                case LogsPhaseNinePackageType.RoomInspectionReport:
                    ValidateRoomInspectionReport();
                    break;
                case LogsPhaseNinePackageType.TaskReportAudit:
                    ValidateTaskReportAudit();
                    break;
                case LogsPhaseNinePackageType.SecurityRecordReview:
                    ValidateSecurityRecordReview();
                    break;
                case LogsPhaseNinePackageType.CompanyQualityClarity:
                    ValidateCompanyQualityClarity();
                    break;
                case LogsPhaseNinePackageType.SabotageLogBlur:
                    ValidateSabotageLogBlur();
                    break;
                case LogsPhaseNinePackageType.DetectiveSafeReportData:
                    ValidateDetectiveSafeReportData();
                    break;
            }
        }

        private void ValidatePhaseNineA()
        {
            ValidateServerRecordAccepted();
            ValidateKillRecordStaysServerOnly();
            ValidatePublicReportRejectsIdentityLeak();
            ValidatePublicReportProjectsSafeRecord();
        }

        private void ValidateServerRecordAccepted()
        {
            bool passed = _recordStore.TryAdd(BuildRecord(
                "record_9a_task",
                RecordCategory.Task,
                RecordVisibility.PublicSafe,
                "Task start at archive desk.",
                "task=archive_sort"));

            LogResult("ServerRecordAccepted", passed, $"StoreCount={_recordStore.Records.Count}");
        }

        private void ValidateKillRecordStaysServerOnly()
        {
            ServerRecord killRecord = BuildRecord(
                "record_9a_kill",
                RecordCategory.KillServerOnly,
                RecordVisibility.PublicSafe,
                "Kill event captured.",
                "killer=player_01;victim=player_02");

            PublicReportProjectionResult result = _projectionService.TryProject(killRecord);
            bool passed = killRecord.IsServerOnly && !result.Success;

            LogResult("KillRecordStaysServerOnly", passed, result.Message);
        }

        private void ValidatePublicReportRejectsIdentityLeak()
        {
            ServerRecord unsafeRecord = BuildRecord(
                "record_9a_unsafe",
                RecordCategory.Sabotage,
                RecordVisibility.PublicSafe,
                "Sabotage near server room.",
                "saboteur=player_03");

            PublicReportProjectionResult result = _projectionService.TryProject(unsafeRecord);

            LogResult("PublicReportRejectsIdentityLeak", !result.Success, result.Message);
        }

        private void ValidatePublicReportProjectsSafeRecord()
        {
            ServerRecord safeRecord = BuildRecord(
                "record_9a_public",
                RecordCategory.DoorAccess,
                RecordVisibility.PublicSafe,
                "Critical door access anomaly detected.",
                "door=security_corridor");

            PublicReportProjectionResult result = _projectionService.TryProject(safeRecord);
            bool passed = result.Success
                && result.Report.Category == RecordCategory.DoorAccess
                && result.Report.Summary.Contains("anomaly")
                && !result.Report.ToString().ToLowerInvariant().Contains("killer");

            LogResult("PublicReportProjectsSafeRecord", passed, result.Report.ToString());
        }

        private static ServerRecord BuildRecord(
            string recordId,
            RecordCategory category,
            RecordVisibility visibility,
            string summary,
            string rawPayload)
        {
            return new ServerRecord(
                recordId,
                new MatchId("match_9a"),
                category,
                visibility,
                new PlayerId("player_9a"),
                "subject_9a",
                OfficeRoomType.SecurityRoom,
                900f,
                summary,
                rawPayload);
        }

        private void ValidateRecordCategoryCoverage()
        {
            bool passed = (int)RecordCategory.PlayerMovement == 0
                && (int)RecordCategory.DoorAccess > 0
                && (int)RecordCategory.CameraPassage > 0
                && (int)RecordCategory.Task > 0
                && (int)RecordCategory.Meeting > 0
                && (int)RecordCategory.KillServerOnly > 0
                && (int)RecordCategory.Corpse > 0
                && (int)RecordCategory.Evidence > 0
                && (int)RecordCategory.Sabotage > 0
                && (int)RecordCategory.Repair > 0
                && (int)RecordCategory.Company > 0;

            LogResult("RecordCategoryCoverage", passed, "All MVP Faz 9 record categories exist.");
        }

        private void ValidateDoorCardLogs()
        {
            ServerRecord record = RecordFactory.CreateDoorAccess(
                "door_9e",
                new MatchId("match_9e"),
                new PlayerId("player_9e"),
                OfficeRoomType.SecurityRoom,
                910f,
                "Security room card reader anomaly.");

            bool passed = record.Category == RecordCategory.DoorAccess
                && record.Visibility == RecordVisibility.PublicSafe
                && _projectionService.TryProject(record).Success;

            LogResult("DoorCardLogs", passed, record.ToString());
        }

        private void ValidateCameraPassageLogs()
        {
            ServerRecord record = RecordFactory.CreateCameraPassage(
                "camera_9f",
                new MatchId("match_9f"),
                new PlayerId("player_9f"),
                OfficeRoomType.Hallway,
                920f,
                "Camera passage gap near hallway.");

            bool passed = record.Category == RecordCategory.CameraPassage
                && record.Visibility == RecordVisibility.PublicSafe
                && _projectionService.TryProject(record).Success;

            LogResult("CameraPassageLogs", passed, record.ToString());
        }

        private void ValidateTaskLifecycleLogs()
        {
            ServerRecord start = RecordFactory.CreateTaskLifecycle("task_9g_start", new MatchId("match_9g"), new PlayerId("player_9g"), OfficeRoomType.ArchiveRoom, 930f, TaskLogState.Started);
            ServerRecord faulty = RecordFactory.CreateTaskLifecycle("task_9g_faulty", new MatchId("match_9g"), new PlayerId("player_9g"), OfficeRoomType.ArchiveRoom, 940f, TaskLogState.FaultyCompleted);
            ServerRecord interrupted = RecordFactory.CreateTaskLifecycle("task_9g_interrupted", new MatchId("match_9g"), new PlayerId("player_9g"), OfficeRoomType.ArchiveRoom, 950f, TaskLogState.Interrupted);

            bool passed = start.Category == RecordCategory.Task
                && faulty.Summary.Contains("FaultyCompleted")
                && interrupted.Summary.Contains("Interrupted");

            LogResult("TaskLifecycleLogs", passed, "Task start/faulty/interrupted logs created.");
        }

        private void ValidateSecurityReviewWindow()
        {
            SecurityReviewWindowService service = new SecurityReviewWindowService();
            ServerRecord[] records =
            {
                RecordFactory.CreateDoorAccess("door_old", new MatchId("match_9h"), new PlayerId("player_9h"), OfficeRoomType.SecurityRoom, 100f, "Old door anomaly."),
                RecordFactory.CreateCameraPassage("camera_in", new MatchId("match_9h"), new PlayerId("player_9h"), OfficeRoomType.Hallway, 150f, "Window camera gap."),
                RecordFactory.CreateDoorAccess("door_in", new MatchId("match_9h"), new PlayerId("player_9h"), OfficeRoomType.SecurityRoom, 180f, "Window door anomaly.")
            };

            var window = service.Filter(records, 120f, 180f, RecordCategory.DoorAccess, RecordCategory.CameraPassage);
            bool passed = window.Count == 2;

            LogResult("SecurityReviewWindow", passed, $"WindowCount={window.Count}");
        }

        private void ValidatePersonnelAuditReport()
        {
            PersonnelAuditReportService service = new PersonnelAuditReportService();
            PublicReport report = service.Build(new PlayerId("player_9i"), BuildAuditRecords());

            bool passed = report.Category == RecordCategory.Meeting
                && report.Summary.Contains("personnel")
                && !report.ToString().ToLowerInvariant().Contains("role");

            LogResult("PersonnelAuditReport", passed, report.ToString());
        }

        private void ValidateRoomInspectionReport()
        {
            RoomInspectionLogReportService service = new RoomInspectionLogReportService();
            PublicReport report = service.Build(OfficeRoomType.ArchiveRoom, BuildAuditRecords());

            bool passed = report.RoomType == OfficeRoomType.ArchiveRoom
                && report.Summary.Contains("room inspection")
                && !report.ToString().ToLowerInvariant().Contains("killer");

            LogResult("RoomInspectionReport", passed, report.ToString());
        }

        private void ValidateTaskReportAudit()
        {
            TaskReportAuditService service = new TaskReportAuditService();
            PublicReport report = service.Build(BuildAuditRecords());

            bool passed = report.Category == RecordCategory.Task
                && report.Summary.Contains("task audit")
                && report.Summary.Contains("faulty=1");

            LogResult("TaskReportAudit", passed, report.ToString());
        }

        private void ValidateSecurityRecordReview()
        {
            SecurityRecordReviewService service = new SecurityRecordReviewService();
            PublicReport report = service.Build(BuildAuditRecords(), 880f, 980f);

            bool passed = report.Category == RecordCategory.CameraPassage
                && report.Summary.Contains("security review")
                && !report.ToString().ToLowerInvariant().Contains("saboteur");

            LogResult("SecurityRecordReview", passed, report.ToString());
        }

        private void ValidateCompanyQualityClarity()
        {
            CompanyRecordQualityService service = new CompanyRecordQualityService();
            bool passed = service.ResolveClarity(80) == LogReportClarityLevel.High
                && service.ResolveClarity(55) == LogReportClarityLevel.Medium
                && service.ResolveClarity(10) == LogReportClarityLevel.Low;

            LogResult("CompanyQualityClarity", passed, "Company quality affects log report clarity.");
        }

        private void ValidateSabotageLogBlur()
        {
            SabotageLogBlurService service = new SabotageLogBlurService();
            ServerRecord source = BuildRecord("record_9n_sabotage", RecordCategory.Sabotage, RecordVisibility.PublicSafe, "Sabotage actor=player_09 at server room.", "saboteur=player_09");
            ServerRecord blurred = service.Blur(source);
            PublicReportProjectionResult result = _projectionService.TryProject(blurred);

            bool passed = result.Success
                && blurred.RawPayload.Length == 0
                && !blurred.Summary.ToLowerInvariant().Contains("actor");

            LogResult("SabotageLogBlur", passed, result.Message);
        }

        private void ValidateDetectiveSafeReportData()
        {
            DetectiveSafeReportDataService service = new DetectiveSafeReportDataService();
            var reports = service.BuildSafeReports(BuildAuditRecords());
            bool passed = reports.Count == 5;

            LogResult("DetectiveSafeReportData", passed, $"SafeReports={reports.Count}");
        }

        private static ServerRecord[] BuildAuditRecords()
        {
            MatchId matchId = new MatchId("match_9_audit");
            PlayerId playerId = new PlayerId("player_9_audit");

            return new[]
            {
                RecordFactory.CreateDoorAccess("door_audit", matchId, playerId, OfficeRoomType.SecurityRoom, 900f, "Door access anomaly."),
                RecordFactory.CreateCameraPassage("camera_audit", matchId, playerId, OfficeRoomType.Hallway, 910f, "Camera passage gap."),
                RecordFactory.CreateTaskLifecycle("task_audit_done", matchId, playerId, OfficeRoomType.ArchiveRoom, 920f, TaskLogState.Completed),
                RecordFactory.CreateTaskLifecycle("task_audit_fault", matchId, playerId, OfficeRoomType.ArchiveRoom, 930f, TaskLogState.FaultyCompleted),
                RecordFactory.CreateCompany("company_audit", matchId, OfficeRoomType.ManagerOffice, 940f, "Company quality changed."),
                new ServerRecord("kill_audit", matchId, RecordCategory.KillServerOnly, RecordVisibility.ServerOnly, playerId, "subject", OfficeRoomType.StorageRoom, 950f, "Kill server-only.", "killer=hidden")
            };
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[LogsPhaseNineDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[LogsPhaseNineDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
