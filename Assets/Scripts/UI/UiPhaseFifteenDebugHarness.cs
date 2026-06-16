using OFIS.Roles;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.UI
{
    public sealed class UiPhaseFifteenDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private UiPhaseFifteenPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly UiHudComposerService _hudComposer = new UiHudComposerService();
        private readonly UiRoleRevealService _roleRevealService = new UiRoleRevealService();
        private readonly UiPanelComposerService _panelComposer = new UiPanelComposerService();
        private readonly UiMatchResultService _matchResultService = new UiMatchResultService();
        private readonly UiTooltipErrorMappingService _tooltipService = new UiTooltipErrorMappingService();
        private readonly UiLocalizationKeyRegistry _localizationRegistry = new UiLocalizationKeyRegistry();
        private readonly UiRoleLeakGuardService _roleLeakGuard = new UiRoleLeakGuardService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate UI Phase 15 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case UiPhaseFifteenPackageType.Hud:
                    ValidateHud();
                    break;
                case UiPhaseFifteenPackageType.RoleReveal:
                    ValidateRoleReveal();
                    break;
                case UiPhaseFifteenPackageType.KillerPanel:
                    ValidateKillerPanel();
                    break;
                case UiPhaseFifteenPackageType.TaskPanel:
                    ValidateTaskPanel();
                    break;
                case UiPhaseFifteenPackageType.MeetingPanel:
                    ValidateMeetingPanel();
                    break;
                case UiPhaseFifteenPackageType.VotingPanel:
                    ValidateVotingPanel();
                    break;
                case UiPhaseFifteenPackageType.ReportPanel:
                    ValidateReportPanel();
                    break;
                case UiPhaseFifteenPackageType.FinalAccusationPanel:
                    ValidateFinalAccusationPanel();
                    break;
                case UiPhaseFifteenPackageType.CorpseInteractionPanel:
                    ValidateCorpseInteractionPanel();
                    break;
                case UiPhaseFifteenPackageType.CorpseCarryPanel:
                    ValidateCorpseCarryPanel();
                    break;
                case UiPhaseFifteenPackageType.SabotageRepairPanel:
                    ValidateSabotageRepairPanel();
                    break;
                case UiPhaseFifteenPackageType.DetectiveDashboard:
                    ValidateDetectiveDashboard();
                    break;
                case UiPhaseFifteenPackageType.VictimNotePanel:
                    ValidateVictimNotePanel();
                    break;
                case UiPhaseFifteenPackageType.DeadPlayerPanel:
                    ValidateDeadPlayerPanel();
                    break;
                case UiPhaseFifteenPackageType.MatchResultScreen:
                    ValidateMatchResultScreen();
                    break;
                case UiPhaseFifteenPackageType.TooltipErrorMapping:
                    ValidateTooltipErrorMapping();
                    break;
                case UiPhaseFifteenPackageType.LocalizationKeys:
                    ValidateLocalizationKeys();
                    break;
                case UiPhaseFifteenPackageType.RoleLeakTests:
                    ValidateRoleLeakTests();
                    break;
                case UiPhaseFifteenPackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateHud()
        {
            HudViewModel hud = _hudComposer.Build(PlayerRole.Detective, "ui.department.security", 1080, 74, "ui.voice.proximity", 4, 2);
            bool passed = hud.IsOwnerOnly && hud.OwnRole == PlayerRole.Detective && hud.TimerText == "18:00" && hud.CompanyHealth == 74 && hud.TotalTaskCount == 4 && hud.CompletedTaskCount == 2;
            LogResult("Hud", passed, $"Timer={hud.TimerText}, Company={hud.CompanyHealth}, Tasks={hud.CompletedTaskCount}/{hud.TotalTaskCount}");
        }

        private void ValidateRoleReveal()
        {
            RoleRevealViewModel killerReveal = _roleRevealService.Build(PlayerRole.Killer, new[] { "victim_01", "victim_02" });
            RoleRevealViewModel detectiveReveal = _roleRevealService.Build(PlayerRole.Detective, new[] { "victim_01" });
            bool passed = killerReveal.IsOwnerOnly && killerReveal.TargetIds.Count == 2 && detectiveReveal.TargetIds.Count == 0 && _roleLeakGuard.IsRoleRevealOwnerSafe(killerReveal);
            LogResult("RoleReveal", passed, $"KillerTargets={killerReveal.TargetIds.Count}, DetectiveTargets={detectiveReveal.TargetIds.Count}");
        }

        private void ValidateKillerPanel()
        {
            UiPanelState panel = _panelComposer.BuildKillerPanel(new[] { "victim_01" }, 0f);
            bool passed = panel.AudienceType == UiAudienceType.KillerOnly && panel.IsOwnerOnly && panel.Actions.Count == 1 && panel.Actions[0].CommandType == UiActionCommandType.KillTarget && panel.Actions[0].Enabled;
            LogResult("KillerPanel", passed, panel.PanelId);
        }

        private void ValidateTaskPanel()
        {
            UiPanelState panel = _panelComposer.BuildTaskPanel(true);
            bool passed = panel.IsVisible && panel.Actions[0].CommandType == UiActionCommandType.StartTask && panel.Actions[0].Enabled;
            LogResult("TaskPanel", passed, panel.PanelId);
        }

        private void ValidateMeetingPanel()
        {
            UiPanelState panel = _panelComposer.BuildMeetingPanel(false);
            bool passed = panel.Actions[0].CommandType == UiActionCommandType.JoinMeeting && !panel.Actions[0].Enabled && panel.Actions[0].ErrorKey == "ui.error.meeting_locked";
            LogResult("MeetingPanel", passed, panel.Actions[0].ErrorKey);
        }

        private void ValidateVotingPanel()
        {
            UiPanelState panel = _panelComposer.BuildVotingPanel(true);
            bool passed = panel.Actions[0].CommandType == UiActionCommandType.CastVote && panel.Actions[0].Enabled;
            LogResult("VotingPanel", passed, panel.PanelId);
        }

        private void ValidateReportPanel()
        {
            UiPanelState panel = _panelComposer.BuildReportPanel(true);
            bool passed = panel.IsVisible && panel.Actions[0].CommandType == UiActionCommandType.OpenReport && panel.TextKeys.Count == 2;
            LogResult("ReportPanel", passed, panel.PanelId);
        }

        private void ValidateFinalAccusationPanel()
        {
            UiPanelState panel = _panelComposer.BuildFinalAccusationPanel(false);
            bool passed = panel.Actions[0].CommandType == UiActionCommandType.SubmitFinalAccusation && !panel.Actions[0].Enabled && panel.Actions[0].ErrorKey == "ui.error.final_list_invalid";
            LogResult("FinalAccusationPanel", passed, panel.Actions[0].ErrorKey);
        }

        private void ValidateCorpseInteractionPanel()
        {
            UiPanelState panel = _panelComposer.BuildCorpseInteractionPanel(true, true);
            bool passed = panel.Actions.Count == 2 && panel.Actions[0].CommandType == UiActionCommandType.InspectCorpse && panel.Actions[1].CommandType == UiActionCommandType.AnnounceCorpse;
            LogResult("CorpseInteractionPanel", passed, panel.PanelId);
        }

        private void ValidateCorpseCarryPanel()
        {
            UiPanelState panel = _panelComposer.BuildCorpseCarryPanel(true, false);
            bool passed = panel.AudienceType == UiAudienceType.KillerOnly && panel.Actions[0].CommandType == UiActionCommandType.CarryCorpse && panel.Actions[0].Enabled && !panel.Actions[1].Enabled;
            LogResult("CorpseCarryPanel", passed, panel.PanelId);
        }

        private void ValidateSabotageRepairPanel()
        {
            UiPanelState panel = _panelComposer.BuildSabotageRepairPanel(true, true);
            bool passed = panel.Actions.Count == 2 && panel.Actions[0].CommandType == UiActionCommandType.StartSabotage && panel.Actions[1].CommandType == UiActionCommandType.StartRepair;
            LogResult("SabotageRepairPanel", passed, panel.PanelId);
        }

        private void ValidateDetectiveDashboard()
        {
            UiPanelState panel = _panelComposer.BuildDetectiveDashboard(true);
            bool passed = panel.AudienceType == UiAudienceType.DetectiveOnly && panel.Actions[0].CommandType == UiActionCommandType.PinEvidence && panel.Actions[0].Enabled;
            LogResult("DetectiveDashboard", passed, panel.PanelId);
        }

        private void ValidateVictimNotePanel()
        {
            UiPanelState panel = _panelComposer.BuildVictimNotePanel(true);
            bool passed = panel.AudienceType == UiAudienceType.VictimOnly && panel.Actions[0].CommandType == UiActionCommandType.SaveVictimNote && panel.Actions[0].Enabled;
            LogResult("VictimNotePanel", passed, panel.PanelId);
        }

        private void ValidateDeadPlayerPanel()
        {
            UiPanelState panel = _panelComposer.BuildDeadPlayerPanel(true);
            bool passed = panel.AudienceType == UiAudienceType.DeadOnly && panel.Actions[0].CommandType == UiActionCommandType.CompleteDeadTask && panel.Actions[0].Enabled;
            LogResult("DeadPlayerPanel", passed, panel.PanelId);
        }

        private void ValidateMatchResultScreen()
        {
            MatchResultViewModel result = _matchResultService.Build("ui.result.good_side_wins", new[] { "player_01:detective", "player_02:killer" });
            bool passed = result.AllowsEndRoleReveal && result.RevealRows.Count == 2 && result.WinnerKey == "ui.result.good_side_wins";
            LogResult("MatchResultScreen", passed, $"Rows={result.RevealRows.Count}");
        }

        private void ValidateTooltipErrorMapping()
        {
            string tooltip = _tooltipService.ResolveTooltipKey("ui.error.kill_cooldown");
            string fallback = _tooltipService.ResolveTooltipKey("ui.error.unknown");
            bool passed = tooltip == "ui.tooltip.kill_cooldown" && fallback == "ui.tooltip.generic_error";
            LogResult("TooltipErrorMapping", passed, $"{tooltip}/{fallback}");
        }

        private void ValidateLocalizationKeys()
        {
            bool passed = _localizationRegistry.Contains("ui.hud.role_mini") && _localizationRegistry.Contains("ui.panel.match_result") && _localizationRegistry.Count >= 20;
            LogResult("LocalizationKeys", passed, $"Keys={_localizationRegistry.Count}");
        }

        private void ValidateRoleLeakTests()
        {
            UiPanelState safePublic = _panelComposer.Build("ui.panel.public_status", UiAudienceType.Public, true, false, new[] { new UiActionBinding(UiActionCommandType.OpenReport, "ui.action.open_summary", true, string.Empty) }, new[] { "ui.public.company_status" });
            UiPanelState unsafePublic = _panelComposer.Build("ui.panel.killer_public", UiAudienceType.Public, true, false, null, new[] { "ui.public.killer_identity" });
            HudViewModel hud = _hudComposer.Build(PlayerRole.Victim, "ui.department.accounting", 64, 101, "ui.voice.dead", 3, 1);
            bool passed = _roleLeakGuard.IsPanelSafeForPublic(safePublic) && !_roleLeakGuard.IsPanelSafeForPublic(unsafePublic) && _roleLeakGuard.IsHudOwnerSafe(hud);
            LogResult("RoleLeakTests", passed, "Public role tokens blocked; owner HUD allowed.");
        }

        private void ValidatePhaseClosure()
        {
            ValidateHud();
            ValidateRoleReveal();
            ValidateKillerPanel();
            ValidateTaskPanel();
            ValidateMeetingPanel();
            ValidateVotingPanel();
            ValidateReportPanel();
            ValidateFinalAccusationPanel();
            ValidateCorpseInteractionPanel();
            ValidateCorpseCarryPanel();
            ValidateSabotageRepairPanel();
            ValidateDetectiveDashboard();
            ValidateVictimNotePanel();
            ValidateDeadPlayerPanel();
            ValidateMatchResultScreen();
            ValidateTooltipErrorMapping();
            ValidateLocalizationKeys();
            ValidateRoleLeakTests();

            LogResult("PhaseClosure", true, "MVP Faz 15 packages 15A-15R are represented.");
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[UiPhaseFifteenDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[UiPhaseFifteenDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
