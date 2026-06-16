using System.Collections.Generic;

namespace OFIS.Product.Onboarding
{
    public sealed class ProductOnboardingPhaseTwoServices
    {
        public OnboardingGateResult ValidateRoleTutorialScreens(IReadOnlyList<TutorialScreenDefinition> screens)
        {
            if (!HasRole(screens, OnboardingRole.Killer) || !HasRole(screens, OnboardingRole.Victim) || !HasRole(screens, OnboardingRole.Detective))
            {
                return Fail("Role tutorial screens must cover killer, victim, and detective.");
            }

            foreach (var screen in screens)
            {
                var assetResult = ValidateAssetPath(screen.AssetPath);
                if (!assetResult.Passed)
                {
                    return assetResult;
                }

                if (screen.LeaksHiddenRoleData)
                {
                    return Fail("Tutorial screen leaks hidden role data.");
                }
            }

            return Pass("Role tutorial screens are present and role-safe.");
        }

        public OnboardingGateResult ValidateKillerTutorial(TutorialScreenDefinition screen)
        {
            return ValidateConceptCoverage(screen, OnboardingAudience.KillerOnly, "target_list", "kill_cooldown", "corpse_carry", "sabotage");
        }

        public OnboardingGateResult ValidateVictimTutorial(TutorialScreenDefinition screen)
        {
            return ValidateConceptCoverage(screen, OnboardingAudience.VictimOnly, "survival", "indirect_note", "dead_task");
        }

        public OnboardingGateResult ValidateDetectiveTutorial(TutorialScreenDefinition screen)
        {
            return ValidateConceptCoverage(screen, OnboardingAudience.DetectiveOnly, "meeting_actions", "report_reading", "pinning", "contradiction_flag");
        }

        public OnboardingGateResult ValidateMeetingTutorial(TutorialScreenDefinition screen)
        {
            return ValidateConceptCoverage(screen, OnboardingAudience.AllPlayers, "physical_join", "join_lock", "vote_flow");
        }

        public OnboardingGateResult ValidateFinalAccusationTutorial(TutorialScreenDefinition screen)
        {
            return ValidateConceptCoverage(screen, OnboardingAudience.AllPlayers, "remaining_killer_count", "no_duplicate_selection", "final_result");
        }

        public OnboardingGateResult ValidateCompanyHealthExplanation(TutorialScreenDefinition screen)
        {
            return ValidateConceptCoverage(screen, OnboardingAudience.AllPlayers, "profit_loss", "task_effect", "sabotage_effect", "report_quality");
        }

        public OnboardingGateResult ValidateFirstMatchHints(IReadOnlyList<FirstMatchHintDefinition> hints)
        {
            if (hints.Count < 5)
            {
                return Fail("First match hint system needs at least five role-safe hints.");
            }

            var previousPriority = -1;
            foreach (var hint in hints)
            {
                if (!hint.RoleSafe || !hint.Dismissible)
                {
                    return Fail("First match hints must be role-safe and dismissible.");
                }

                var assetResult = ValidateAssetPath(hint.IconPath);
                if (!assetResult.Passed)
                {
                    return assetResult;
                }

                if (hint.Priority <= previousPriority)
                {
                    return Fail("First match hint priorities must be ordered.");
                }

                previousPriority = hint.Priority;
            }

            return Pass("First match hint system is ordered, dismissible, and role-safe.");
        }

        public OnboardingGateResult ValidateTooltipPolish(IReadOnlyList<TooltipDefinition> tooltips)
        {
            if (tooltips.Count < 10)
            {
                return Fail("Tooltip polish requires the core action icon set.");
            }

            foreach (var tooltip in tooltips)
            {
                if (string.IsNullOrWhiteSpace(tooltip.LocalizationKey) || !tooltip.HasErrorCopy)
                {
                    return Fail("Tooltip is missing localization or error copy.");
                }

                var assetResult = ValidateAssetPath(tooltip.IconPath);
                if (!assetResult.Passed)
                {
                    return assetResult;
                }
            }

            return Pass("UI tooltip polish has action icons, localization keys, and error copy.");
        }

        public OnboardingGateResult ValidateAccessibility(AccessibilityProfile profile)
        {
            var paths = new[] { profile.SwatchPath, profile.HighContrastPanelPath, profile.MonoIconPath };
            foreach (var path in paths)
            {
                var assetResult = ValidateAssetPath(path);
                if (!assetResult.Passed)
                {
                    return assetResult;
                }
            }

            if (!profile.ColorBlindAlternative || !profile.UiScaleSupported || !profile.MinimumIconMode)
            {
                return Fail("Accessibility profile must support color-blind alternatives, UI scale, and minimum icons.");
            }

            return Pass("Accessibility alternatives are available.");
        }

        public OnboardingGateResult ValidateMainMenuPolish(MainMenuPolishDefinition menu)
        {
            var paths = new[] { menu.BackgroundPath, menu.LogoPath, menu.ButtonFramePath, menu.LoadingIconPath };
            foreach (var path in paths)
            {
                var assetResult = ValidateAssetPath(path);
                if (!assetResult.Passed)
                {
                    return assetResult;
                }
            }

            return menu.KeyboardNavigation ? Pass("Main menu polish has assets and keyboard navigation.") : Fail("Main menu keyboard navigation is missing.");
        }

        public OnboardingGateResult ValidateLobbyReadyUx(LobbyReadyUxDefinition lobby)
        {
            var paths = new[] { lobby.ReadyIconPath, lobby.NotReadyIconPath, lobby.PlayerSlotFramePath, lobby.HostIconPath, lobby.PingIconPath };
            foreach (var path in paths)
            {
                var assetResult = ValidateAssetPath(path);
                if (!assetResult.Passed)
                {
                    return assetResult;
                }
            }

            return lobby.ReadyStateReadable ? Pass("Lobby ready UX is readable.") : Fail("Lobby ready state is not readable.");
        }

        public OnboardingGateResult ValidateMatchResultDetails(MatchResultDetailDefinition result)
        {
            var paths = new List<string>
            {
                result.GoodWinBannerPath,
                result.KillerWinBannerPath,
                result.SummaryPanelPath,
                result.RoleRevealCardPath
            };
            paths.AddRange(result.CompanyStateIcons);

            foreach (var path in paths)
            {
                var assetResult = ValidateAssetPath(path);
                if (!assetResult.Passed)
                {
                    return assetResult;
                }
            }

            return result.ExplainsWinCondition ? Pass("Match result detail screen explains win condition.") : Fail("Match result detail screen does not explain win condition.");
        }

        public OnboardingGateResult ValidatePhaseClosure(
            IReadOnlyList<TutorialScreenDefinition> screens,
            IReadOnlyList<FirstMatchHintDefinition> hints,
            IReadOnlyList<TooltipDefinition> tooltips,
            AccessibilityProfile accessibility,
            MainMenuPolishDefinition menu,
            LobbyReadyUxDefinition lobby,
            MatchResultDetailDefinition result)
        {
            var gates = new[]
            {
                ValidateRoleTutorialScreens(screens),
                ValidateKillerTutorial(FindRole(screens, OnboardingRole.Killer)),
                ValidateVictimTutorial(FindRole(screens, OnboardingRole.Victim)),
                ValidateDetectiveTutorial(FindRole(screens, OnboardingRole.Detective)),
                ValidateMeetingTutorial(FindRole(screens, OnboardingRole.Meeting)),
                ValidateFinalAccusationTutorial(FindRole(screens, OnboardingRole.FinalAccusation)),
                ValidateCompanyHealthExplanation(FindRole(screens, OnboardingRole.Company)),
                ValidateFirstMatchHints(hints),
                ValidateTooltipPolish(tooltips),
                ValidateAccessibility(accessibility),
                ValidateMainMenuPolish(menu),
                ValidateLobbyReadyUx(lobby),
                ValidateMatchResultDetails(result)
            };

            foreach (var gate in gates)
            {
                if (!gate.Passed)
                {
                    return gate;
                }
            }

            return Pass("Product Phase 2 closure passed: onboarding teaches the core loop, UI actions are explainable, and tutorial data is role-safe.");
        }

        private static OnboardingGateResult ValidateConceptCoverage(TutorialScreenDefinition screen, OnboardingAudience expectedAudience, params string[] concepts)
        {
            var assetResult = ValidateAssetPath(screen.AssetPath);
            if (!assetResult.Passed)
            {
                return assetResult;
            }

            if (screen.Audience != expectedAudience || screen.LeaksHiddenRoleData)
            {
                return Fail("Tutorial screen audience is wrong or leaks hidden role data.");
            }

            foreach (var concept in concepts)
            {
                if (!ContainsConcept(screen.CoveredConcepts, concept))
                {
                    return Fail($"Tutorial is missing concept: {concept}.");
                }
            }

            return Pass($"{screen.Role} tutorial covers required concepts.");
        }

        private static OnboardingGateResult ValidateAssetPath(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return Fail("Asset path is empty.");
            }

            if (!assetPath.StartsWith("Assets/assets-img/product-phase-2/") || !assetPath.EndsWith(".png"))
            {
                return Fail($"Asset path is outside the Product Phase 2 asset folder: {assetPath}.");
            }

            return Pass("Asset path is valid.");
        }

        private static TutorialScreenDefinition FindRole(IReadOnlyList<TutorialScreenDefinition> screens, OnboardingRole role)
        {
            for (var i = 0; i < screens.Count; i++)
            {
                if (screens[i].Role == role)
                {
                    return screens[i];
                }
            }

            return new TutorialScreenDefinition(role, OnboardingAudience.AllPlayers, string.Empty, string.Empty, true, new string[0]);
        }

        private static bool HasRole(IReadOnlyList<TutorialScreenDefinition> screens, OnboardingRole role)
        {
            for (var i = 0; i < screens.Count; i++)
            {
                if (screens[i].Role == role)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsConcept(IReadOnlyList<string> concepts, string expected)
        {
            for (var i = 0; i < concepts.Count; i++)
            {
                if (concepts[i] == expected)
                {
                    return true;
                }
            }

            return false;
        }

        private static OnboardingGateResult Pass(string message)
        {
            return new OnboardingGateResult(true, message);
        }

        private static OnboardingGateResult Fail(string message)
        {
            return new OnboardingGateResult(false, message);
        }
    }
}
