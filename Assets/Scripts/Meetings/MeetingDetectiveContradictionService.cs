using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingDetectiveContradictionService
    {
        public MeetingDetectiveContradictionResult Evaluate(
            IReadOnlyList<MeetingReportData> detectiveReports,
            MeetingActionProposalResolutionResult resolutionResult)
        {
            List<MeetingDetectiveContradictionEvent> events =
                new List<MeetingDetectiveContradictionEvent>();

            if (!resolutionResult.HasResolvedProposal
                || string.IsNullOrWhiteSpace(resolutionResult.Proposal.ProposalId))
            {
                return new MeetingDetectiveContradictionResult(
                    events,
                    "No resolved proposal is available for contradiction checks.");
            }

            if (detectiveReports == null || detectiveReports.Count == 0)
            {
                return new MeetingDetectiveContradictionResult(
                    events,
                    "No detective reports are available for contradiction checks.");
            }

            MeetingActionProposalData proposal = resolutionResult.Proposal;

            for (int i = 0; i < detectiveReports.Count; i++)
            {
                MeetingReportData report = detectiveReports[i];
                MeetingDetectiveContradictionFlagType flagType =
                    EvaluateReportAgainstProposal(report, proposal);

                if (flagType == MeetingDetectiveContradictionFlagType.None)
                    continue;

                events.Add(BuildEvent(report, proposal, flagType, events.Count));
            }

            return new MeetingDetectiveContradictionResult(
                events,
                events.Count > 0
                    ? "Detective contradiction flags raised."
                    : "No detective contradictions found.");
        }

        private static MeetingDetectiveContradictionFlagType EvaluateReportAgainstProposal(
            MeetingReportData report,
            MeetingActionProposalData proposal)
        {
            MeetingActionType actionType = proposal.Request.ActionType;
            MeetingActionTargetData target = proposal.Request.Target;

            if (actionType == MeetingActionType.NoAction && IsActionableReport(report))
                return MeetingDetectiveContradictionFlagType.NoActionAgainstActionableReport;

            if (IsPlayerTargetAction(actionType)
                && target.HasPlayerTarget
                && IsPlayerTargetReport(report)
                && report.TargetPlayerId != target.PlayerId)
            {
                return MeetingDetectiveContradictionFlagType.PlayerTargetMismatch;
            }

            if (actionType == MeetingActionType.RoomInspection
                && target.HasRoomTarget
                && report.ReportType == MeetingReportType.LocationClaim
                && report.RoomType != target.RoomType)
            {
                return MeetingDetectiveContradictionFlagType.RoomTargetMismatch;
            }

            return MeetingDetectiveContradictionFlagType.None;
        }

        private static bool IsActionableReport(MeetingReportData report)
        {
            return report.ReportType == MeetingReportType.Suspicion
                || report.ReportType == MeetingReportType.CorpseReport
                || report.ReportType == MeetingReportType.LocationClaim;
        }

        private static bool IsPlayerTargetAction(MeetingActionType actionType)
        {
            return actionType == MeetingActionType.PersonnelAudit
                || actionType == MeetingActionType.OfficialAccusation;
        }

        private static bool IsPlayerTargetReport(MeetingReportData report)
        {
            return (report.ReportType == MeetingReportType.Suspicion
                || report.ReportType == MeetingReportType.Defense
                || report.ReportType == MeetingReportType.CorpseReport)
                && !string.IsNullOrWhiteSpace(report.TargetPlayerId)
                && report.TargetPlayerId != "none";
        }

        private static MeetingDetectiveContradictionEvent BuildEvent(
            MeetingReportData report,
            MeetingActionProposalData proposal,
            MeetingDetectiveContradictionFlagType flagType,
            int eventIndex)
        {
            string eventId = $"{proposal.MeetingId}_detective_contradiction_{eventIndex + 1}";

            return new MeetingDetectiveContradictionEvent(
                eventId,
                proposal.MeetingId,
                report.ReporterPlayerId,
                report.ReportId,
                proposal.ProposalId,
                flagType,
                BuildMessage(flagType));
        }

        private static string BuildMessage(MeetingDetectiveContradictionFlagType flagType)
        {
            switch (flagType)
            {
                case MeetingDetectiveContradictionFlagType.PlayerTargetMismatch:
                    return "Detective report target conflicts with resolved official action target.";

                case MeetingDetectiveContradictionFlagType.RoomTargetMismatch:
                    return "Detective report room conflicts with resolved official action room.";

                case MeetingDetectiveContradictionFlagType.NoActionAgainstActionableReport:
                    return "Detective submitted an actionable report but the meeting resolved no action.";

                default:
                    return "Detective contradiction flag raised.";
            }
        }
    }
}
