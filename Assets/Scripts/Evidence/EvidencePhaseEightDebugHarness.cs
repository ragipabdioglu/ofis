using System.Collections.Generic;
using OFIS.Core.Ids;
using OFIS.Meetings;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Evidence
{
    public sealed class EvidencePhaseEightDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private EvidencePhaseEightPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly EvidenceTraceVisibilityService _visibilityService = new EvidenceTraceVisibilityService();
        private readonly EvidenceCorpseTraceVisibilityService _corpseVisibilityService = new EvidenceCorpseTraceVisibilityService();
        private readonly SabotageTraceSafetyGuardService _sabotageSafetyGuard = new SabotageTraceSafetyGuardService();
        private readonly RoomInspectionTraceReportService _roomReportService = new RoomInspectionTraceReportService();
        private readonly CompanyTraceClarityService _clarityService = new CompanyTraceClarityService();
        private readonly DetectiveTraceViewService _detectiveTraceViewService = new DetectiveTraceViewService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Evidence Phase Eight Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case EvidencePhaseEightPackageType.VisibilityStates:
                    ValidateVisibilityStates();
                    break;
                case EvidencePhaseEightPackageType.CorpsePreInspectHidden:
                    ValidateCorpsePreInspectHidden();
                    break;
                case EvidencePhaseEightPackageType.CorpseInspectorOnly:
                    ValidateCorpseInspectorOnly();
                    break;
                case EvidencePhaseEightPackageType.CorpseAnnouncementPublic:
                    ValidateCorpseAnnouncementPublic();
                    break;
                case EvidencePhaseEightPackageType.SabotageIdentitySafety:
                    ValidateSabotageIdentitySafety();
                    break;
                case EvidencePhaseEightPackageType.RoomInspectionReport:
                    ValidateRoomInspectionReport();
                    break;
                case EvidencePhaseEightPackageType.CompanyQualityClarity:
                    ValidateCompanyQualityClarity();
                    break;
                case EvidencePhaseEightPackageType.DetectiveTraceView:
                    ValidateDetectiveTraceView();
                    break;
            }
        }

        private void ValidateVisibilityStates()
        {
            bool passed = (int)EvidenceTraceVisibilityType.Hidden == 0
                && (int)EvidenceTraceVisibilityType.InspectorOnly > 0
                && (int)EvidenceTraceVisibilityType.MeetingParticipants > 0
                && (int)EvidenceTraceVisibilityType.Public > 0
                && (int)EvidenceTraceVisibilityType.DetectiveOnly > 0;

            LogResult("VisibilityStates", passed, "All planned visibility states exist.");
        }

        private void ValidateCorpsePreInspectHidden()
        {
            EvidenceTraceVisibilityType visibility = _corpseVisibilityService.ResolveCorpseMovementVisibility(
                "corpse_8d",
                "none",
                false,
                false);

            EvidenceTraceVisibilityResult result = _visibilityService.Resolve(
                BuildRecord("corpse_8d", EvidenceTraceType.CarryTrace),
                visibility,
                new PlayerId("viewer_8d"),
                new PlayerId("owner_8d"),
                false,
                false);

            LogResult("CorpsePreInspectHidden", !result.CanView && result.VisibilityType == EvidenceTraceVisibilityType.Hidden, result.Message);
        }

        private void ValidateCorpseInspectorOnly()
        {
            bool canSee = _corpseVisibilityService.CanInspectorSee(
                BuildRecord("corpse_8e", EvidenceTraceType.DropTrace),
                new PlayerId("inspector_8e"),
                new PlayerId("inspector_8e"),
                "corpse_8e");

            LogResult("CorpseInspectorOnly", canSee, "Inspector can see matching corpse trace.");
        }

        private void ValidateCorpseAnnouncementPublic()
        {
            EvidenceTraceVisibilityType visibility = _corpseVisibilityService.ResolveCorpseMovementVisibility(
                "corpse_8f",
                "none",
                false,
                true);

            EvidenceTraceVisibilityResult result = _visibilityService.Resolve(
                BuildRecord("corpse_8f", EvidenceTraceType.HideSpotTrace),
                visibility,
                new PlayerId("viewer_8f"),
                default,
                false,
                false);

            LogResult("CorpseAnnouncementPublic", result.CanView && result.VisibilityType == EvidenceTraceVisibilityType.Public, result.Message);
        }

        private void ValidateSabotageIdentitySafety()
        {
            EvidenceTraceRecord safeRecord = BuildRecord("sabotage_8g", EvidenceTraceType.SabotageTrace, "Sabotage residue near server rack.");
            EvidenceTraceRecord unsafeRecord = BuildRecord("sabotage_8g_bad", EvidenceTraceType.SabotageTrace, "saboteur=killer_01");
            bool passed = _sabotageSafetyGuard.IsSafe(safeRecord) && !_sabotageSafetyGuard.IsSafe(unsafeRecord);

            LogResult("SabotageIdentitySafety", passed, "Sabotage trace identity leak guarded.");
        }

        private void ValidateRoomInspectionReport()
        {
            List<EvidenceTraceRecord> records = new List<EvidenceTraceRecord>
            {
                BuildRecord("room_8h", EvidenceTraceType.BloodTrace),
                BuildRecord("room_8h", EvidenceTraceType.DragTrace)
            };

            MeetingReportData report = _roomReportService.BuildRoomInspectionReport(
                "room_report_8h",
                "detective_8h",
                OfficeRoomType.ArchiveRoom,
                records,
                EvidenceReportClarityLevel.Medium);

            bool passed = report.ReporterPlayerId == "detective_8h"
                && report.RoomType == OfficeRoomType.ArchiveRoom
                && !report.Message.ToLowerInvariant().Contains("killer");

            LogResult("RoomInspectionReport", passed, report.ToString());
        }

        private void ValidateCompanyQualityClarity()
        {
            bool passed = _clarityService.ResolveClarity(90) == EvidenceReportClarityLevel.High
                && _clarityService.ResolveClarity(55) == EvidenceReportClarityLevel.Medium
                && _clarityService.ResolveClarity(20) == EvidenceReportClarityLevel.Low;

            LogResult("CompanyQualityClarity", passed, "Company health maps to trace clarity.");
        }

        private void ValidateDetectiveTraceView()
        {
            List<EvidenceTraceRecord> records = new List<EvidenceTraceRecord>
            {
                BuildRecord("detective_8j", EvidenceTraceType.CameraGapTrace),
                BuildRecord("detective_8j", EvidenceTraceType.TaskMismatchTrace)
            };

            IReadOnlyList<DetectiveTraceViewItem> detectiveView = _detectiveTraceViewService.BuildDetectiveView(records, true);
            IReadOnlyList<DetectiveTraceViewItem> nonDetectiveView = _detectiveTraceViewService.BuildDetectiveView(records, false);
            bool passed = detectiveView.Count == 2
                && detectiveView[0].CanPin
                && detectiveView[0].CanFlag
                && nonDetectiveView.Count == 0;

            LogResult("DetectiveTraceView", passed, "Detective trace view supports pin/flag without public leak.");
        }

        private static EvidenceTraceRecord BuildRecord(
            string sourceId,
            EvidenceTraceType traceType,
            string summary = "Trace signal.")
        {
            return new EvidenceTraceRecord(
                EvidenceTraceId.New(),
                traceType,
                sourceId,
                OfficeRoomType.StorageRoom,
                new Vector3(1f, 1f, 0f),
                800f,
                summary);
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[EvidencePhaseEightDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[EvidencePhaseEightDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
