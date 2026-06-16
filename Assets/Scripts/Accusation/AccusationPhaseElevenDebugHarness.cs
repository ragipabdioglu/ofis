using System.Collections.Generic;
using OFIS.Roles;
using OFIS.Rules;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Accusation
{
    public sealed class AccusationPhaseElevenDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private AccusationPhaseElevenPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly OfficialAccusationService _officialAccusationService = new OfficialAccusationService();
        private readonly ExposedKillerRestrictionService _restrictionService = new ExposedKillerRestrictionService();
        private readonly ExposedKillerCleanupService _cleanupService = new ExposedKillerCleanupService();
        private readonly RemainingKillerCounterService _remainingKillerCounter = new RemainingKillerCounterService();
        private readonly FinalAccusationListValidationService _finalListValidation = new FinalAccusationListValidationService();
        private readonly FinalWinResolutionService _finalWinResolution = new FinalWinResolutionService();
        private readonly CompanyBasedWinService _companyBasedWinService = new CompanyBasedWinService();
        private readonly MatchResultUiStateService _matchResultUiStateService = new MatchResultUiStateService();
        private readonly EndRoleRevealService _endRoleRevealService = new EndRoleRevealService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Accusation Phase 11 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case AccusationPhaseElevenPackageType.NormalOfficialAccusation:
                    ValidateNormalOfficialAccusation();
                    break;
                case AccusationPhaseElevenPackageType.CorrectAccusation:
                    ValidateCorrectAccusation();
                    break;
                case AccusationPhaseElevenPackageType.WrongAccusation:
                    ValidateWrongAccusation();
                    break;
                case AccusationPhaseElevenPackageType.ExposedKillerRestrictions:
                    ValidateExposedKillerRestrictions();
                    break;
                case AccusationPhaseElevenPackageType.ExposedKillerCleanup:
                    ValidateExposedKillerCleanup();
                    break;
                case AccusationPhaseElevenPackageType.RemainingKillerCount:
                    ValidateRemainingKillerCount();
                    break;
                case AccusationPhaseElevenPackageType.FinalAccusationListValidation:
                    ValidateFinalAccusationListValidation();
                    break;
                case AccusationPhaseElevenPackageType.FinalWinResolve:
                    ValidateFinalWinResolve();
                    break;
                case AccusationPhaseElevenPackageType.AllVictimsDeadCompanyWin:
                    ValidateAllVictimsDeadCompanyWin();
                    break;
                case AccusationPhaseElevenPackageType.MatchResultUi:
                    ValidateMatchResultUi();
                    break;
                case AccusationPhaseElevenPackageType.EndRoleReveal:
                    ValidateEndRoleReveal();
                    break;
                case AccusationPhaseElevenPackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateNormalOfficialAccusation()
        {
            AccusationResolutionResult result = _officialAccusationService.ResolveNormalAccusation("killer_01", BuildPlayers());
            LogResult("NormalOfficialAccusation", result.OutcomeType == AccusationOutcomeType.Correct, result.Message);
        }

        private void ValidateCorrectAccusation()
        {
            AccusationResolutionResult result = _officialAccusationService.ResolveNormalAccusation("killer_01", BuildPlayers());
            bool passed = result.OutcomeType == AccusationOutcomeType.Correct
                && result.ShouldRevealRole
                && result.ShouldExposeTarget
                && result.CompanyDelta == 4;

            LogResult("CorrectAccusation", passed, result.Message);
        }

        private void ValidateWrongAccusation()
        {
            AccusationResolutionResult result = _officialAccusationService.ResolveNormalAccusation("detective_01", BuildPlayers());
            bool passed = result.OutcomeType == AccusationOutcomeType.Wrong
                && !result.ShouldRevealRole
                && result.CompanyDelta == -8
                && result.NextInfoActionQualityDelta == -1;

            LogResult("WrongAccusation", passed, result.Message);
        }

        private void ValidateExposedKillerRestrictions()
        {
            bool passed = !_restrictionService.CanPerform(PlayerActionType.Kill, true)
                && !_restrictionService.CanPerform(PlayerActionType.Sabotage, true)
                && !_restrictionService.CanPerform(PlayerActionType.CarryCorpse, true)
                && !_restrictionService.CanVote(true)
                && !_restrictionService.CanUseMeetingVoice(true);

            LogResult("ExposedKillerRestrictions", passed, "Exposed killer action restrictions applied.");
        }

        private void ValidateExposedKillerCleanup()
        {
            ExposedKillerCleanupResult result = _cleanupService.BuildCleanup();
            bool passed = result.KillDisabled
                && result.SabotageDisabled
                && result.CarryDisabled
                && result.ActiveSabotageNeutralized
                && result.FutureTracesInvalidated
                && result.RawLogsPreserved;

            LogResult("ExposedKillerCleanup", passed, "Cleanup keeps raw logs and blocks future effects.");
        }

        private void ValidateRemainingKillerCount()
        {
            int count = _remainingKillerCounter.CountRemaining(BuildPlayers());
            LogResult("RemainingKillerCount", count == 2, $"Remaining={count}");
        }

        private void ValidateFinalAccusationListValidation()
        {
            FinalAccusationValidationResult valid = _finalListValidation.Validate(new[] { "killer_01", "killer_02" }, BuildPlayers());
            FinalAccusationValidationResult duplicate = _finalListValidation.Validate(new[] { "killer_01", "killer_01" }, BuildPlayers());
            FinalAccusationValidationResult exposed = _finalListValidation.Validate(new[] { "killer_01" }, BuildPlayersWithExposedKiller());

            LogResult("FinalAccusationListValidation", valid.IsValid && !duplicate.IsValid && !exposed.IsValid, valid.Message);
        }

        private void ValidateFinalWinResolve()
        {
            WinResolutionResult good = _finalWinResolution.ResolveFinal(new[] { "killer_02", "killer_01" }, BuildPlayers());
            WinResolutionResult killers = _finalWinResolution.ResolveFinal(new[] { "killer_01", "detective_01" }, BuildPlayers());
            bool passed = good.WinnerType == MatchWinnerType.GoodSide && killers.WinnerType == MatchWinnerType.Killers;

            LogResult("FinalWinResolve", passed, $"{good.Reason} / {killers.Reason}");
        }

        private void ValidateAllVictimsDeadCompanyWin()
        {
            WinResolutionResult result = _companyBasedWinService.ResolveIfAllVictimsDead(BuildPlayersWithDeadVictims(), 35);
            LogResult("AllVictimsDeadCompanyWin", result.WinnerType == MatchWinnerType.Killers, result.Reason);
        }

        private void ValidateMatchResultUi()
        {
            MatchResultUiState state = _matchResultUiStateService.Build(new WinResolutionResult(MatchWinnerType.GoodSide, "All killers found."));
            LogResult("MatchResultUi", state.WinnerType == MatchWinnerType.GoodSide && state.Headline.Contains("Good"), state.Detail);
        }

        private void ValidateEndRoleReveal()
        {
            var reveal = _endRoleRevealService.BuildReveal(BuildPlayers());
            LogResult("EndRoleReveal", reveal.Count == 6 && reveal[0].Role == PlayerRole.Killer, $"RevealCount={reveal.Count}");
        }

        private void ValidatePhaseClosure()
        {
            ValidateNormalOfficialAccusation();
            ValidateCorrectAccusation();
            ValidateWrongAccusation();
            ValidateExposedKillerRestrictions();
            ValidateExposedKillerCleanup();
            ValidateRemainingKillerCount();
            ValidateFinalAccusationListValidation();
            ValidateFinalWinResolve();
            ValidateAllVictimsDeadCompanyWin();
            ValidateMatchResultUi();
            ValidateEndRoleReveal();

            LogResult("PhaseClosure", true, "MVP Faz 11 packages 11A-11K are represented.");
        }

        private static IReadOnlyList<AccusationPlayerState> BuildPlayers()
        {
            return new List<AccusationPlayerState>
            {
                new AccusationPlayerState("killer_01", PlayerRole.Killer, true, false),
                new AccusationPlayerState("killer_02", PlayerRole.Killer, true, false),
                new AccusationPlayerState("victim_01", PlayerRole.Victim, true, false),
                new AccusationPlayerState("victim_02", PlayerRole.Victim, true, false),
                new AccusationPlayerState("detective_01", PlayerRole.Detective, true, false),
                new AccusationPlayerState("detective_02", PlayerRole.Detective, true, false)
            };
        }

        private static IReadOnlyList<AccusationPlayerState> BuildPlayersWithExposedKiller()
        {
            return new List<AccusationPlayerState>
            {
                new AccusationPlayerState("killer_01", PlayerRole.Killer, true, true),
                new AccusationPlayerState("killer_02", PlayerRole.Killer, true, false),
                new AccusationPlayerState("detective_01", PlayerRole.Detective, true, false)
            };
        }

        private static IReadOnlyList<AccusationPlayerState> BuildPlayersWithDeadVictims()
        {
            return new List<AccusationPlayerState>
            {
                new AccusationPlayerState("killer_01", PlayerRole.Killer, true, false),
                new AccusationPlayerState("victim_01", PlayerRole.Victim, false, false),
                new AccusationPlayerState("victim_02", PlayerRole.Victim, false, false),
                new AccusationPlayerState("detective_01", PlayerRole.Detective, true, false)
            };
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[AccusationPhaseElevenDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[AccusationPhaseElevenDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
