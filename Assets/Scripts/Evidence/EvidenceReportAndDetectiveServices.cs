using System.Collections.Generic;
using OFIS.Meetings;
using OFIS.Rooms;
using OFIS.Sabotage;

namespace OFIS.Evidence
{
    public enum EvidenceReportClarityLevel
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public readonly struct DetectiveTraceViewItem
    {
        public EvidenceTraceRecord TraceRecord { get; }
        public bool CanPin { get; }
        public bool CanFlag { get; }
        public string Label { get; }

        public DetectiveTraceViewItem(EvidenceTraceRecord traceRecord, bool canPin, bool canFlag, string label)
        {
            TraceRecord = traceRecord;
            CanPin = canPin;
            CanFlag = canFlag;
            Label = string.IsNullOrWhiteSpace(label) ? traceRecord.TraceType.ToString() : label;
        }
    }

    public sealed class SabotageTraceSafetyGuardService
    {
        public bool IsSafe(EvidenceTraceRecord record)
        {
            if (record.TraceType != EvidenceTraceType.SabotageTrace)
                return true;

            string text = record.Summary == null ? string.Empty : record.Summary.ToLowerInvariant();
            return !text.Contains("saboteur=")
                && !text.Contains("killer=")
                && !text.Contains("actor=")
                && !text.Contains("identity");
        }
    }

    public sealed class RoomInspectionTraceReportService
    {
        public MeetingReportData BuildRoomInspectionReport(
            string reportId,
            string reporterPlayerId,
            OfficeRoomType roomType,
            IReadOnlyList<EvidenceTraceRecord> traces,
            EvidenceReportClarityLevel clarityLevel)
        {
            int traceCount = traces == null ? 0 : traces.Count;
            string message = $"Room inspection found {traceCount} trace signal(s). Clarity={clarityLevel}. No identity confirmed.";

            return new MeetingReportData(
                reportId,
                MeetingReportType.CorpseReport,
                reporterPlayerId,
                "none",
                roomType,
                0,
                0,
                SabotageObjectiveState.None,
                message);
        }
    }

    public sealed class CompanyTraceClarityService
    {
        public EvidenceReportClarityLevel ResolveClarity(int companyHealth)
        {
            if (companyHealth >= 75)
                return EvidenceReportClarityLevel.High;

            if (companyHealth >= 50)
                return EvidenceReportClarityLevel.Medium;

            return EvidenceReportClarityLevel.Low;
        }
    }

    public sealed class DetectiveTraceViewService
    {
        public IReadOnlyList<DetectiveTraceViewItem> BuildDetectiveView(
            IReadOnlyList<EvidenceTraceRecord> records,
            bool viewerIsDetective)
        {
            List<DetectiveTraceViewItem> result = new List<DetectiveTraceViewItem>();

            if (!viewerIsDetective || records == null)
                return result;

            for (int i = 0; i < records.Count; i++)
            {
                result.Add(new DetectiveTraceViewItem(
                    records[i],
                    true,
                    true,
                    $"Trace: {records[i].TraceType}"));
            }

            return result;
        }
    }
}
