using System.Collections.Generic;

namespace OFIS.Logs
{
    public readonly struct PublicReportProjectionResult
    {
        public bool Success { get; }
        public PublicReport Report { get; }
        public string Message { get; }

        public PublicReportProjectionResult(bool success, PublicReport report, string message)
        {
            Success = success;
            Report = report;
            Message = string.IsNullOrWhiteSpace(message) ? "No projection message." : message;
        }
    }

    public sealed class ServerRecordStore
    {
        private readonly List<ServerRecord> _records = new List<ServerRecord>();

        public IReadOnlyList<ServerRecord> Records => _records;

        public bool TryAdd(ServerRecord record)
        {
            if (string.IsNullOrWhiteSpace(record.RecordId))
                return false;

            _records.Add(record);
            return true;
        }
    }

    public sealed class RecordPrivacyGuardService
    {
        private static readonly string[] SensitiveTokens =
        {
            "killer",
            "katil",
            "saboteur",
            "role=",
            "secretrole",
            "actor=",
            "identity",
            "victim="
        };

        public bool IsPublicSafeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return true;

            string normalized = text.ToLowerInvariant();

            for (int i = 0; i < SensitiveTokens.Length; i++)
            {
                if (normalized.Contains(SensitiveTokens[i]))
                    return false;
            }

            return true;
        }
    }

    public sealed class PublicReportProjectionService
    {
        private readonly RecordPrivacyGuardService _privacyGuard = new RecordPrivacyGuardService();

        public PublicReportProjectionResult TryProject(ServerRecord record)
        {
            if (record.IsServerOnly)
                return new PublicReportProjectionResult(false, default, "Server-only records cannot be projected.");

            if (!_privacyGuard.IsPublicSafeText(record.Summary) || !_privacyGuard.IsPublicSafeText(record.RawPayload))
                return new PublicReportProjectionResult(false, default, "Record contains sensitive identity text.");

            PublicReport report = new PublicReport(
                $"public_{record.RecordId}",
                record.Category,
                record.RoomType,
                record.ServerTimeSeconds,
                record.Summary);

            return new PublicReportProjectionResult(true, report, "Public report projected.");
        }
    }
}
