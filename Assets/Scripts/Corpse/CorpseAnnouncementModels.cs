using OFIS.Core.Ids;
using OFIS.Meetings;
using OFIS.Rooms;
using OFIS.Sabotage;

namespace OFIS.Corpse
{
    public readonly struct CorpseAnnouncementCommandContext
    {
        public string CommandId { get; }
        public PlayerId AnnouncerPlayerId { get; }
        public MeetingRuntimePhaseType PhaseType { get; }
        public CorpseOwnerKnowledge Knowledge { get; }

        public CorpseAnnouncementCommandContext(
            string commandId,
            PlayerId announcerPlayerId,
            MeetingRuntimePhaseType phaseType,
            CorpseOwnerKnowledge knowledge)
        {
            CommandId = string.IsNullOrWhiteSpace(commandId) ? "unknown_announce_command" : commandId;
            AnnouncerPlayerId = announcerPlayerId;
            PhaseType = phaseType;
            Knowledge = knowledge;
        }
    }

    public readonly struct CorpseAnnouncementData
    {
        public string AnnouncementId { get; }
        public PlayerId AnnouncerPlayerId { get; }
        public CorpseId CorpseId { get; }
        public string VictimDisplayName { get; }
        public OfficeRoomType FoundRoom { get; }
        public bool IsPublic { get; }

        public CorpseAnnouncementData(
            string announcementId,
            PlayerId announcerPlayerId,
            CorpseId corpseId,
            string victimDisplayName,
            OfficeRoomType foundRoom,
            bool isPublic)
        {
            AnnouncementId = string.IsNullOrWhiteSpace(announcementId) ? "unknown_announcement" : announcementId;
            AnnouncerPlayerId = announcerPlayerId;
            CorpseId = corpseId;
            VictimDisplayName = string.IsNullOrWhiteSpace(victimDisplayName) ? "Unknown Victim" : victimDisplayName;
            FoundRoom = foundRoom;
            IsPublic = isPublic;
        }
    }

    public readonly struct CorpseAnnouncementResult
    {
        public bool Success { get; }
        public CorpseAnnouncementData Announcement { get; }
        public string Message { get; }

        private CorpseAnnouncementResult(bool success, CorpseAnnouncementData announcement, string message)
        {
            Success = success;
            Announcement = announcement;
            Message = string.IsNullOrWhiteSpace(message) ? "No announcement message." : message;
        }

        public static CorpseAnnouncementResult Accepted(CorpseAnnouncementData announcement)
        {
            return new CorpseAnnouncementResult(true, announcement, "Corpse information announced publicly.");
        }

        public static CorpseAnnouncementResult Rejected(string message)
        {
            return new CorpseAnnouncementResult(false, default, message);
        }
    }

    public sealed class CorpseAnnouncementService
    {
        public CorpseAnnouncementResult Announce(CorpseAnnouncementCommandContext context)
        {
            if (string.IsNullOrWhiteSpace(context.CommandId))
                return CorpseAnnouncementResult.Rejected("Announcement command id is required.");

            if (string.IsNullOrWhiteSpace(context.AnnouncerPlayerId.Value))
                return CorpseAnnouncementResult.Rejected("Announcer player id is required.");

            bool inMeeting = context.PhaseType == MeetingRuntimePhaseType.Meeting
                || context.PhaseType == MeetingRuntimePhaseType.FinalMeeting;

            if (!inMeeting)
                return CorpseAnnouncementResult.Rejected($"Announcement requires meeting phase. Phase={context.PhaseType}");

            if (!context.Knowledge.IsOwnerOnly || context.Knowledge.OwnerPlayerId != context.AnnouncerPlayerId)
                return CorpseAnnouncementResult.Rejected("Announcer does not own corpse knowledge.");

            CorpseAnnouncementData announcement = new CorpseAnnouncementData(
                context.CommandId,
                context.AnnouncerPlayerId,
                context.Knowledge.CorpseId,
                context.Knowledge.VictimDisplayName,
                context.Knowledge.FoundRoom,
                true);

            return CorpseAnnouncementResult.Accepted(announcement);
        }
    }

    public sealed class CorpsePublicReportService
    {
        public MeetingReportData BuildPublicReport(CorpseAnnouncementData announcement)
        {
            string message = $"Corpse report: {announcement.VictimDisplayName} was found in {announcement.FoundRoom}.";

            return new MeetingReportData(
                $"corpse_report_{announcement.AnnouncementId}",
                MeetingReportType.CorpseReport,
                announcement.AnnouncerPlayerId.ToString(),
                "none",
                announcement.FoundRoom,
                0,
                0,
                SabotageObjectiveState.None,
                message);
        }
    }

    public sealed class CorpsePrivateInfoGuardService
    {
        public bool CanPublish(CorpseAnnouncementResult announcementResult)
        {
            return announcementResult.Success && announcementResult.Announcement.IsPublic;
        }
    }

    public sealed class CorpseReportSafetyGuardService
    {
        public bool IsSafe(MeetingReportData report)
        {
            string message = report.Message == null ? string.Empty : report.Message.ToLowerInvariant();
            return !message.Contains("killer")
                && !message.Contains("role")
                && !message.Contains("murderer")
                && !message.Contains("culprit");
        }
    }
}
