using OFIS.Core.Ids;
using OFIS.Meetings;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpseAnnouncementDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly CorpseAnnouncementService _announcementService = new CorpseAnnouncementService();
        private readonly CorpsePublicReportService _publicReportService = new CorpsePublicReportService();
        private readonly CorpsePrivateInfoGuardService _privateInfoGuard = new CorpsePrivateInfoGuardService();
        private readonly CorpseReportSafetyGuardService _safetyGuard = new CorpseReportSafetyGuardService();

        private void Start()
        {
            if (validateOnStart)
                ValidateAnnouncementFlow();
        }

        [ContextMenu("Validate Corpse Announcement Flow")]
        public void ValidateAnnouncementFlow()
        {
            ValidateMeetingAnnouncementAccepted();
            ValidateOfficeAnnouncementRejected();
            ValidatePublicReportIsSafe();
            ValidateUnannouncedInfoStaysPrivate();
        }

        private void ValidateMeetingAnnouncementAccepted()
        {
            CorpseAnnouncementResult result = _announcementService.Announce(BuildContext(MeetingRuntimePhaseType.Meeting));
            LogResult("MeetingAnnouncementAccepted", result.Success && result.Announcement.IsPublic, result.Message);
        }

        private void ValidateOfficeAnnouncementRejected()
        {
            CorpseAnnouncementResult result = _announcementService.Announce(BuildContext(MeetingRuntimePhaseType.Office));
            LogResult("OfficeAnnouncementRejected", !result.Success, result.Message);
        }

        private void ValidatePublicReportIsSafe()
        {
            CorpseAnnouncementResult result = _announcementService.Announce(BuildContext(MeetingRuntimePhaseType.Meeting));
            MeetingReportData report = _publicReportService.BuildPublicReport(result.Announcement);
            bool passed = result.Success
                && report.ReportType == MeetingReportType.CorpseReport
                && _safetyGuard.IsSafe(report);

            LogResult("PublicReportIsSafe", passed, report.ToString());
        }

        private void ValidateUnannouncedInfoStaysPrivate()
        {
            CorpseAnnouncementResult rejected = CorpseAnnouncementResult.Rejected("Not announced.");
            LogResult("UnannouncedInfoStaysPrivate", !_privateInfoGuard.CanPublish(rejected), rejected.Message);
        }

        private static CorpseAnnouncementCommandContext BuildContext(MeetingRuntimePhaseType phaseType)
        {
            PlayerId owner = new PlayerId("detective_announce_01");
            CorpseOwnerKnowledge knowledge = new CorpseOwnerKnowledge(
                owner,
                new CorpseId("corpse_announce_01"),
                new PlayerId("victim_announce_01"),
                "Merve Kaya",
                OfficeRoomType.ArchiveRoom,
                650f,
                true);

            return new CorpseAnnouncementCommandContext(
                "announce_7l",
                owner,
                phaseType,
                knowledge);
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;
            if (passed)
                Debug.Log($"[CorpseAnnouncementDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpseAnnouncementDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
