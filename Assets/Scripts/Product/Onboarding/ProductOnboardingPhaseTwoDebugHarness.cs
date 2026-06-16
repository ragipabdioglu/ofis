using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Product.Onboarding
{
    public sealed class ProductOnboardingPhaseTwoDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private ProductOnboardingPhaseTwoPackageType packageType = ProductOnboardingPhaseTwoPackageType.PhaseClosure;

        private readonly ProductOnboardingPhaseTwoServices services = new ProductOnboardingPhaseTwoServices();

        private void Start()
        {
            if (validateOnStart)
            {
                ValidatePackage();
            }
        }

        [ContextMenu("Validate Product Phase 2 Package")]
        public void ValidatePackage()
        {
            var screens = CreateTutorialScreens();
            switch (packageType)
            {
                case ProductOnboardingPhaseTwoPackageType.RoleTutorialScreens:
                    LogResult("RoleTutorialScreens", services.ValidateRoleTutorialScreens(screens));
                    break;
                case ProductOnboardingPhaseTwoPackageType.KillerTutorial:
                    LogResult("KillerTutorial", services.ValidateKillerTutorial(screens[0]));
                    break;
                case ProductOnboardingPhaseTwoPackageType.VictimTutorial:
                    LogResult("VictimTutorial", services.ValidateVictimTutorial(screens[1]));
                    break;
                case ProductOnboardingPhaseTwoPackageType.DetectiveTutorial:
                    LogResult("DetectiveTutorial", services.ValidateDetectiveTutorial(screens[2]));
                    break;
                case ProductOnboardingPhaseTwoPackageType.MeetingTutorial:
                    LogResult("MeetingTutorial", services.ValidateMeetingTutorial(screens[3]));
                    break;
                case ProductOnboardingPhaseTwoPackageType.FinalAccusationTutorial:
                    LogResult("FinalAccusationTutorial", services.ValidateFinalAccusationTutorial(screens[4]));
                    break;
                case ProductOnboardingPhaseTwoPackageType.CompanyHealthExplanation:
                    LogResult("CompanyHealthExplanation", services.ValidateCompanyHealthExplanation(screens[5]));
                    break;
                case ProductOnboardingPhaseTwoPackageType.FirstMatchHintSystem:
                    LogResult("FirstMatchHintSystem", services.ValidateFirstMatchHints(CreateHints()));
                    break;
                case ProductOnboardingPhaseTwoPackageType.UiTooltipPolish:
                    LogResult("UiTooltipPolish", services.ValidateTooltipPolish(CreateTooltips()));
                    break;
                case ProductOnboardingPhaseTwoPackageType.AccessibilitySupport:
                    LogResult("AccessibilitySupport", services.ValidateAccessibility(CreateAccessibility()));
                    break;
                case ProductOnboardingPhaseTwoPackageType.MainMenuPolish:
                    LogResult("MainMenuPolish", services.ValidateMainMenuPolish(CreateMainMenu()));
                    break;
                case ProductOnboardingPhaseTwoPackageType.LobbyReadyUx:
                    LogResult("LobbyReadyUx", services.ValidateLobbyReadyUx(CreateLobbyReadyUx()));
                    break;
                case ProductOnboardingPhaseTwoPackageType.MatchResultDetailScreen:
                    LogResult("MatchResultDetailScreen", services.ValidateMatchResultDetails(CreateResultDetails()));
                    break;
                case ProductOnboardingPhaseTwoPackageType.PhaseClosure:
                    LogResult("PhaseClosure", services.ValidatePhaseClosure(
                        screens,
                        CreateHints(),
                        CreateTooltips(),
                        CreateAccessibility(),
                        CreateMainMenu(),
                        CreateLobbyReadyUx(),
                        CreateResultDetails()));
                    break;
            }
        }

        private static IReadOnlyList<TutorialScreenDefinition> CreateTutorialScreens()
        {
            return new[]
            {
                new TutorialScreenDefinition(OnboardingRole.Killer, OnboardingAudience.KillerOnly, "tutorial.killer.title", "Assets/assets-img/product-phase-2/tutorial/tutorial_killer.png", false, new[] { "target_list", "kill_cooldown", "corpse_carry", "sabotage" }),
                new TutorialScreenDefinition(OnboardingRole.Victim, OnboardingAudience.VictimOnly, "tutorial.victim.title", "Assets/assets-img/product-phase-2/tutorial/tutorial_victim.png", false, new[] { "survival", "indirect_note", "dead_task" }),
                new TutorialScreenDefinition(OnboardingRole.Detective, OnboardingAudience.DetectiveOnly, "tutorial.detective.title", "Assets/assets-img/product-phase-2/tutorial/tutorial_detective.png", false, new[] { "meeting_actions", "report_reading", "pinning", "contradiction_flag" }),
                new TutorialScreenDefinition(OnboardingRole.Meeting, OnboardingAudience.AllPlayers, "tutorial.meeting.title", "Assets/assets-img/product-phase-2/tutorial/tutorial_meeting.png", false, new[] { "physical_join", "join_lock", "vote_flow" }),
                new TutorialScreenDefinition(OnboardingRole.FinalAccusation, OnboardingAudience.AllPlayers, "tutorial.final.title", "Assets/assets-img/product-phase-2/tutorial/tutorial_final_accusation.png", false, new[] { "remaining_killer_count", "no_duplicate_selection", "final_result" }),
                new TutorialScreenDefinition(OnboardingRole.Company, OnboardingAudience.AllPlayers, "tutorial.company.title", "Assets/assets-img/product-phase-2/icons/ui/ui_company_health.png", false, new[] { "profit_loss", "task_effect", "sabotage_effect", "report_quality" })
            };
        }

        private static IReadOnlyList<FirstMatchHintDefinition> CreateHints()
        {
            return new[]
            {
                new FirstMatchHintDefinition("hint.move_to_tasks", "Assets/assets-img/product-phase-2/icons/ui/ui_interaction.png", true, true, 1),
                new FirstMatchHintDefinition("hint.watch_timer", "Assets/assets-img/product-phase-2/icons/ui/ui_timer_warning.png", true, true, 2),
                new FirstMatchHintDefinition("hint.company_health", "Assets/assets-img/product-phase-2/icons/ui/ui_company_health.png", true, true, 3),
                new FirstMatchHintDefinition("hint.voice_status", "Assets/assets-img/product-phase-2/icons/ui/ui_voice_on.png", true, true, 4),
                new FirstMatchHintDefinition("hint.meeting_vote", "Assets/assets-img/product-phase-2/icons/actions/action_vote.png", true, true, 5)
            };
        }

        private static IReadOnlyList<TooltipDefinition> CreateTooltips()
        {
            return new[]
            {
                new TooltipDefinition("kill", "Assets/assets-img/product-phase-2/icons/actions/action_kill.png", "tooltip.kill", true),
                new TooltipDefinition("carry_corpse", "Assets/assets-img/product-phase-2/icons/actions/action_carry_corpse.png", "tooltip.carry_corpse", true),
                new TooltipDefinition("drop_corpse", "Assets/assets-img/product-phase-2/icons/actions/action_drop_corpse.png", "tooltip.drop_corpse", true),
                new TooltipDefinition("hide_corpse", "Assets/assets-img/product-phase-2/icons/actions/action_hide_corpse.png", "tooltip.hide_corpse", true),
                new TooltipDefinition("sabotage", "Assets/assets-img/product-phase-2/icons/actions/action_sabotage.png", "tooltip.sabotage", true),
                new TooltipDefinition("repair", "Assets/assets-img/product-phase-2/icons/actions/action_repair.png", "tooltip.repair", true),
                new TooltipDefinition("inspect_corpse", "Assets/assets-img/product-phase-2/icons/actions/action_inspect_corpse.png", "tooltip.inspect_corpse", true),
                new TooltipDefinition("announce_report", "Assets/assets-img/product-phase-2/icons/actions/action_announce_report.png", "tooltip.announce_report", true),
                new TooltipDefinition("vote", "Assets/assets-img/product-phase-2/icons/actions/action_vote.png", "tooltip.vote", true),
                new TooltipDefinition("pin_evidence", "Assets/assets-img/product-phase-2/icons/actions/action_pin_evidence.png", "tooltip.pin_evidence", true),
                new TooltipDefinition("contradiction_flag", "Assets/assets-img/product-phase-2/icons/actions/action_contradiction_flag.png", "tooltip.contradiction_flag", true),
                new TooltipDefinition("final_accusation", "Assets/assets-img/product-phase-2/icons/actions/action_final_accusation.png", "tooltip.final_accusation", true)
            };
        }

        private static AccessibilityProfile CreateAccessibility()
        {
            return new AccessibilityProfile(
                "Assets/assets-img/product-phase-2/accessibility/colorblind_role_swatches.png",
                "Assets/assets-img/product-phase-2/accessibility/high_contrast_panel.png",
                "Assets/assets-img/product-phase-2/accessibility/mono_icon_template.png",
                true,
                true,
                true);
        }

        private static MainMenuPolishDefinition CreateMainMenu()
        {
            return new MainMenuPolishDefinition(
                "Assets/assets-img/product-phase-2/menu/ofis_menu_background.png",
                "Assets/assets-img/product-phase-2/menu/ofis_logo_title.png",
                "Assets/assets-img/product-phase-2/menu/ui_button_frame.png",
                "Assets/assets-img/product-phase-2/menu/loading_spinner.png",
                true);
        }

        private static LobbyReadyUxDefinition CreateLobbyReadyUx()
        {
            return new LobbyReadyUxDefinition(
                "Assets/assets-img/product-phase-2/lobby/lobby_ready.png",
                "Assets/assets-img/product-phase-2/lobby/lobby_not_ready.png",
                "Assets/assets-img/product-phase-2/lobby/lobby_player_slot_frame.png",
                "Assets/assets-img/product-phase-2/lobby/lobby_host_crown.png",
                "Assets/assets-img/product-phase-2/lobby/lobby_ping_quality.png",
                true);
        }

        private static MatchResultDetailDefinition CreateResultDetails()
        {
            return new MatchResultDetailDefinition(
                "Assets/assets-img/product-phase-2/result/result_good_win_banner.png",
                "Assets/assets-img/product-phase-2/result/result_killer_win_banner.png",
                "Assets/assets-img/product-phase-2/result/result_summary_panel_frame.png",
                "Assets/assets-img/product-phase-2/result/result_role_reveal_card.png",
                new[]
                {
                    "Assets/assets-img/product-phase-2/result/result_company_profit.png",
                    "Assets/assets-img/product-phase-2/result/result_company_neutral.png",
                    "Assets/assets-img/product-phase-2/result/result_company_loss.png"
                },
                true);
        }

        private static void LogResult(string gateName, OnboardingGateResult result)
        {
            if (result.Passed)
            {
                Debug.Log($"[ProductOnboardingPhaseTwoDebugHarness] PASS {gateName}: {result.Message}");
                return;
            }

            Debug.LogError($"[ProductOnboardingPhaseTwoDebugHarness] FAIL {gateName}: {result.Message}");
        }
    }
}
