using System.Collections.Generic;
using OFIS.Roles;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Detective
{
    public sealed class DetectivePhaseTwelveDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private DetectivePhaseTwelvePackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly DetectiveDashboardService _dashboardService = new DetectiveDashboardService();
        private readonly DetectivePrivateNoteService _noteService = new DetectivePrivateNoteService();
        private readonly DetectivePinService _pinService = new DetectivePinService();
        private readonly DetectiveSuspectProfileService _suspectProfileService = new DetectiveSuspectProfileService();
        private readonly DetectiveClaimCardService _claimCardService = new DetectiveClaimCardService();
        private readonly DetectiveClaimComparisonService _claimComparisonService = new DetectiveClaimComparisonService();
        private readonly DetectiveNoCertaintyGuardService _certaintyGuard = new DetectiveNoCertaintyGuardService();
        private readonly DetectiveTimelineService _timelineService = new DetectiveTimelineService();
        private readonly DetectiveFinalTheoryService _finalTheoryService = new DetectiveFinalTheoryService();
        private readonly DetectiveOwnerOnlyNetworkService _ownerOnlyNetworkService = new DetectiveOwnerOnlyNetworkService();
        private readonly DetectiveRoleSafeUiService _roleSafeUiService = new DetectiveRoleSafeUiService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Detective Phase 12 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case DetectivePhaseTwelvePackageType.DashboardUi:
                    ValidateDashboardUi();
                    break;
                case DetectivePhaseTwelvePackageType.PrivateNotes:
                    ValidatePrivateNotes();
                    break;
                case DetectivePhaseTwelvePackageType.PinSystem:
                    ValidatePinSystem();
                    break;
                case DetectivePhaseTwelvePackageType.SuspectProfile:
                    ValidateSuspectProfile();
                    break;
                case DetectivePhaseTwelvePackageType.ManualClaimCards:
                    ValidateManualClaimCards();
                    break;
                case DetectivePhaseTwelvePackageType.ClaimComparison:
                    ValidateClaimComparison();
                    break;
                case DetectivePhaseTwelvePackageType.ContradictionFlags:
                    ValidateContradictionFlags();
                    break;
                case DetectivePhaseTwelvePackageType.FlagSeverity:
                    ValidateFlagSeverity();
                    break;
                case DetectivePhaseTwelvePackageType.NoDefiniteKillerResult:
                    ValidateNoDefiniteKillerResult();
                    break;
                case DetectivePhaseTwelvePackageType.TimelineView:
                    ValidateTimelineView();
                    break;
                case DetectivePhaseTwelvePackageType.FinalTheoryPrivatePanel:
                    ValidateFinalTheoryPrivatePanel();
                    break;
                case DetectivePhaseTwelvePackageType.OwnerOnlyNetworking:
                    ValidateOwnerOnlyNetworking();
                    break;
                case DetectivePhaseTwelvePackageType.RoleSafeUi:
                    ValidateRoleSafeUi();
                    break;
                case DetectivePhaseTwelvePackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateDashboardUi()
        {
            DetectiveDashboardState detective = _dashboardService.Build(PlayerRole.Detective, 2, 1);
            DetectiveDashboardState killer = _dashboardService.Build(PlayerRole.Killer, 2, 1);
            LogResult("DashboardUi", detective.IsVisible && detective.IsOwnerOnly && !killer.IsVisible, $"Visible={detective.IsVisible}");
        }

        private void ValidatePrivateNotes()
        {
            DetectivePrivateNote note = _noteService.Create("note_12b", "detective_01", "Printer claim sounds inconsistent.");
            LogResult("PrivateNotes", note.OwnerPlayerId == "detective_01" && note.Text.Contains("claim"), note.Text);
        }

        private void ValidatePinSystem()
        {
            DetectivePinnedItem pin = _pinService.Pin("pin_12c", "detective_01", "trace_01", "Camera gap");
            LogResult("PinSystem", pin.OwnerPlayerId == "detective_01" && pin.SourceId == "trace_01", pin.Label);
        }

        private void ValidateSuspectProfile()
        {
            DetectiveClaimCard claim = BuildClaim();
            var flags = BuildAllComparisonFlags(claim);
            DetectiveSuspectProfile profile = _suspectProfileService.Build("suspect_01", new[] { claim }, flags);
            LogResult("SuspectProfile", profile.ClaimCount == 1 && profile.FlagCount == 5, $"Claims={profile.ClaimCount}, Flags={profile.FlagCount}");
        }

        private void ValidateManualClaimCards()
        {
            DetectiveClaimCard claim = BuildClaim();
            LogResult("ManualClaimCards", claim.SubjectPlayerId == "suspect_01" && claim.ClaimText.Contains("Archive"), claim.ClaimText);
        }

        private void ValidateClaimComparison()
        {
            var flags = BuildAllComparisonFlags(BuildClaim());
            LogResult("ClaimComparison", flags.Count == 5, $"ComparisonFlags={flags.Count}");
        }

        private void ValidateContradictionFlags()
        {
            var flags = BuildAllComparisonFlags(BuildClaim());
            LogResult("ContradictionFlags", flags.Count > 0 && flags[0].Message.Contains("Claim comparison"), flags[0].Message);
        }

        private void ValidateFlagSeverity()
        {
            var flags = BuildAllComparisonFlags(BuildClaim());
            bool hasLow = false;
            bool hasMedium = false;
            bool hasHigh = false;

            for (int i = 0; i < flags.Count; i++)
            {
                hasLow |= flags[i].Severity == DetectiveFlagSeverity.Low;
                hasMedium |= flags[i].Severity == DetectiveFlagSeverity.Medium;
                hasHigh |= flags[i].Severity == DetectiveFlagSeverity.High;
            }

            LogResult("FlagSeverity", hasLow && hasMedium && hasHigh, "Low/Medium/High severities present.");
        }

        private void ValidateNoDefiniteKillerResult()
        {
            bool safe = _certaintyGuard.IsSafeConclusion("High suspicion signal only.");
            bool unsafeTextBlocked = !_certaintyGuard.IsSafeConclusion("definite killer");
            LogResult("NoDefiniteKillerResult", safe && unsafeTextBlocked, "Certainty language guard active.");
        }

        private void ValidateTimelineView()
        {
            var timeline = _timelineService.Build(
                new DetectiveTimelineItem(30f, "Later event"),
                new DetectiveTimelineItem(10f, "Earlier event"));

            LogResult("TimelineView", timeline.Count == 2 && timeline[0].ServerTimeSeconds == 10f, timeline[0].Label);
        }

        private void ValidateFinalTheoryPrivatePanel()
        {
            DetectiveFinalTheory theory = _finalTheoryService.BuildPrivateTheory(
                "detective_01",
                new[] { "suspect_01", "suspect_02" },
                "Two suspects fit the timeline.");

            LogResult("FinalTheoryPrivatePanel", theory.IsPrivate && theory.SuspectPlayerIds.Count == 2, theory.Summary);
        }

        private void ValidateOwnerOnlyNetworking()
        {
            DetectiveOwnerPayload detective = _ownerOnlyNetworkService.BuildOwnerPayload("detective_01", PlayerRole.Detective);
            DetectiveOwnerPayload victim = _ownerOnlyNetworkService.BuildOwnerPayload("victim_01", PlayerRole.Victim);
            LogResult("OwnerOnlyNetworking", detective.IsOwnerOnly && detective.IsPublicSafe && !victim.IsOwnerOnly, $"OwnerOnly={detective.IsOwnerOnly}");
        }

        private void ValidateRoleSafeUi()
        {
            bool detectiveCanSee = _roleSafeUiService.CanShowDashboard(PlayerRole.Detective);
            bool killerCannotSee = !_roleSafeUiService.CanShowDashboard(PlayerRole.Killer);
            bool unsafeBlocked = !_roleSafeUiService.IsSafeUiText("confirmed killer");
            LogResult("RoleSafeUi", detectiveCanSee && killerCannotSee && unsafeBlocked, "Role-safe UI guard active.");
        }

        private void ValidatePhaseClosure()
        {
            ValidateDashboardUi();
            ValidatePrivateNotes();
            ValidatePinSystem();
            ValidateSuspectProfile();
            ValidateManualClaimCards();
            ValidateClaimComparison();
            ValidateContradictionFlags();
            ValidateFlagSeverity();
            ValidateNoDefiniteKillerResult();
            ValidateTimelineView();
            ValidateFinalTheoryPrivatePanel();
            ValidateOwnerOnlyNetworking();
            ValidateRoleSafeUi();

            LogResult("PhaseClosure", true, "MVP Faz 12 packages 12A-12M are represented.");
        }

        private DetectiveClaimCard BuildClaim()
        {
            return _claimCardService.Create(
                "claim_12",
                "detective_01",
                "suspect_01",
                "Suspect claims ArchiveRoom task at 120s.");
        }

        private IReadOnlyList<DetectiveContradictionFlag> BuildAllComparisonFlags(DetectiveClaimCard claim)
        {
            return _claimComparisonService.Compare(
                claim,
                new[]
                {
                    DetectiveClaimComparisonType.TaskLog,
                    DetectiveClaimComparisonType.DoorAccess,
                    DetectiveClaimComparisonType.CorpseTraceAge,
                    DetectiveClaimComparisonType.MeetingAttendance,
                    DetectiveClaimComparisonType.CameraPassage
                });
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[DetectivePhaseTwelveDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[DetectivePhaseTwelveDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
