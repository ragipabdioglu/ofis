using System.Collections.Generic;
using OFIS.Core.Ids;
using OFIS.Rooms;

namespace OFIS.Logs
{
    public sealed class SecurityReviewWindowService
    {
        public IReadOnlyList<ServerRecord> Filter(IReadOnlyList<ServerRecord> records, float startTimeSeconds, float endTimeSeconds, params RecordCategory[] categories)
        {
            List<ServerRecord> result = new List<ServerRecord>();

            if (records == null)
                return result;

            for (int i = 0; i < records.Count; i++)
            {
                ServerRecord record = records[i];
                if (record.ServerTimeSeconds < startTimeSeconds || record.ServerTimeSeconds > endTimeSeconds)
                    continue;

                if (categories == null || categories.Length == 0 || Contains(categories, record.Category))
                    result.Add(record);
            }

            return result;
        }

        private static bool Contains(RecordCategory[] categories, RecordCategory category)
        {
            for (int i = 0; i < categories.Length; i++)
            {
                if (categories[i] == category)
                    return true;
            }

            return false;
        }
    }

    public sealed class PersonnelAuditReportService
    {
        public PublicReport Build(PlayerId playerId, IReadOnlyList<ServerRecord> records)
        {
            int count = CountPublicSafe(records);
            return new PublicReport($"personnel_{playerId.Value}", RecordCategory.Meeting, OfficeRoomType.MeetingRoom, 0f, $"personnel audit found {count} safe activity record(s).");
        }

        private static int CountPublicSafe(IReadOnlyList<ServerRecord> records)
        {
            if (records == null)
                return 0;

            int count = 0;
            for (int i = 0; i < records.Count; i++)
            {
                if (!records[i].IsServerOnly)
                    count++;
            }

            return count;
        }
    }

    public sealed class RoomInspectionLogReportService
    {
        public PublicReport Build(OfficeRoomType roomType, IReadOnlyList<ServerRecord> records)
        {
            int count = 0;

            if (records != null)
            {
                for (int i = 0; i < records.Count; i++)
                {
                    if (!records[i].IsServerOnly && records[i].RoomType == roomType)
                        count++;
                }
            }

            return new PublicReport($"room_{roomType}", RecordCategory.Evidence, roomType, 0f, $"room inspection found {count} safe record signal(s).");
        }
    }

    public sealed class TaskReportAuditService
    {
        public PublicReport Build(IReadOnlyList<ServerRecord> records)
        {
            int total = 0;
            int faulty = 0;
            int interrupted = 0;

            if (records != null)
            {
                for (int i = 0; i < records.Count; i++)
                {
                    ServerRecord record = records[i];
                    if (record.Category != RecordCategory.Task || record.IsServerOnly)
                        continue;

                    total++;
                    if (record.Summary.Contains(TaskLogState.FaultyCompleted.ToString()))
                        faulty++;
                    if (record.Summary.Contains(TaskLogState.Interrupted.ToString()))
                        interrupted++;
                }
            }

            return new PublicReport("task_audit", RecordCategory.Task, OfficeRoomType.OfficeSupport, 0f, $"task audit total={total}, faulty={faulty}, interrupted={interrupted}.");
        }
    }

    public sealed class SecurityRecordReviewService
    {
        private readonly SecurityReviewWindowService _windowService = new SecurityReviewWindowService();

        public PublicReport Build(IReadOnlyList<ServerRecord> records, float startTimeSeconds, float endTimeSeconds)
        {
            var filtered = _windowService.Filter(records, startTimeSeconds, endTimeSeconds, RecordCategory.DoorAccess, RecordCategory.CameraPassage);
            return new PublicReport("security_review", RecordCategory.CameraPassage, OfficeRoomType.SecurityRoom, startTimeSeconds, $"security review found {filtered.Count} door/camera signal(s).");
        }
    }

    public sealed class CompanyRecordQualityService
    {
        public LogReportClarityLevel ResolveClarity(int companyHealth)
        {
            if (companyHealth >= 75)
                return LogReportClarityLevel.High;

            if (companyHealth >= 50)
                return LogReportClarityLevel.Medium;

            return LogReportClarityLevel.Low;
        }
    }

    public sealed class SabotageLogBlurService
    {
        public ServerRecord Blur(ServerRecord record)
        {
            if (record.Category != RecordCategory.Sabotage)
                return record;

            return new ServerRecord(
                record.RecordId,
                record.MatchId,
                record.Category,
                record.Visibility,
                record.ActorPlayerId,
                record.SubjectId,
                record.RoomType,
                record.ServerTimeSeconds,
                "Sabotage signal detected; origin unclear.",
                string.Empty);
        }
    }

    public sealed class DetectiveSafeReportDataService
    {
        private readonly PublicReportProjectionService _projectionService = new PublicReportProjectionService();

        public IReadOnlyList<PublicReport> BuildSafeReports(IReadOnlyList<ServerRecord> records)
        {
            List<PublicReport> reports = new List<PublicReport>();

            if (records == null)
                return reports;

            for (int i = 0; i < records.Count; i++)
            {
                PublicReportProjectionResult result = _projectionService.TryProject(records[i]);
                if (result.Success)
                    reports.Add(result.Report);
            }

            return reports;
        }
    }
}
