using OFIS.Core.Ids;
using OFIS.Meetings;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpsePhaseSevenIntegrationDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly CorpseTraceVisibilityService _visibilityService = new CorpseTraceVisibilityService();
        private readonly CorpseAnnouncementService _announcementService = new CorpseAnnouncementService();
        private readonly CorpsePublicReportService _reportService = new CorpsePublicReportService();
        private readonly CorpseReportSafetyGuardService _safetyGuard = new CorpseReportSafetyGuardService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePhaseSevenIntegration();
        }

        [ContextMenu("Validate Phase Seven Integration")]
        public void ValidatePhaseSevenIntegration()
        {
            CorpseMovementTraceEvent traceEvent = CorpseTraceVisibilityDebugHarness.BuildTrace();
            CorpseOwnerKnowledge knowledge = CorpseTraceVisibilityDebugHarness.BuildKnowledge("detective_phase7");
            PlayerId viewer = new PlayerId("detective_phase7");

            CorpseTraceVisibilityResult beforeAnnounce =
                _visibilityService.ResolveForViewer(traceEvent, viewer, knowledge, false);

            CorpseAnnouncementResult announcement = _announcementService.Announce(
                new CorpseAnnouncementCommandContext(
                    "announce_phase7",
                    viewer,
                    MeetingRuntimePhaseType.Meeting,
                    knowledge));

            CorpseTraceVisibilityResult afterAnnounce =
                _visibilityService.ResolveForViewer(traceEvent, new PlayerId("player_public"), default, announcement.Success);
            MeetingReportData report = _reportService.BuildPublicReport(announcement.Announcement);

            bool passed = beforeAnnounce.CanView
                && beforeAnnounce.VisibilityType == CorpseTraceVisibilityType.InspectorOnly
                && announcement.Success
                && afterAnnounce.CanView
                && afterAnnounce.VisibilityType == CorpseTraceVisibilityType.Public
                && _safetyGuard.IsSafe(report);

            LogResult("PhaseSevenIntegration", passed, report.ToString());
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;
            if (passed)
                Debug.Log($"[CorpsePhaseSevenIntegrationDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpsePhaseSevenIntegrationDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
