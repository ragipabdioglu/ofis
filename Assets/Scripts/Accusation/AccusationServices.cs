using System.Collections.Generic;
using OFIS.Roles;
using OFIS.Rules;

namespace OFIS.Accusation
{
    public sealed class OfficialAccusationService
    {
        public AccusationResolutionResult ResolveNormalAccusation(
            string targetPlayerId,
            IReadOnlyList<AccusationPlayerState> players)
        {
            AccusationPlayerState target = Find(players, targetPlayerId);
            if (target.Role == PlayerRole.None || target.IsExposed)
                return new AccusationResolutionResult(AccusationOutcomeType.Invalid, targetPlayerId, false, false, 0, 0, "Invalid accusation target.");

            if (target.Role == PlayerRole.Killer)
            {
                return new AccusationResolutionResult(
                    AccusationOutcomeType.Correct,
                    target.PlayerId,
                    true,
                    true,
                    +4,
                    0,
                    "Correct accusation.");
            }

            return new AccusationResolutionResult(
                AccusationOutcomeType.Wrong,
                target.PlayerId,
                false,
                false,
                -8,
                -1,
                "Wrong accusation.");
        }

        private static AccusationPlayerState Find(IReadOnlyList<AccusationPlayerState> players, string playerId)
        {
            if (players == null)
                return default;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].PlayerId == playerId)
                    return players[i];
            }

            return default;
        }
    }

    public sealed class ExposedKillerRestrictionService
    {
        public bool CanPerform(PlayerActionType actionType, bool isExposedKiller)
        {
            if (!isExposedKiller)
                return true;

            return actionType != PlayerActionType.Kill
                && actionType != PlayerActionType.Sabotage
                && actionType != PlayerActionType.CarryCorpse
                && actionType != PlayerActionType.JoinMeeting;
        }

        public bool CanVote(bool isExposedKiller)
        {
            return !isExposedKiller;
        }

        public bool CanUseMeetingVoice(bool isExposedKiller)
        {
            return !isExposedKiller;
        }
    }

    public sealed class ExposedKillerCleanupService
    {
        public ExposedKillerCleanupResult BuildCleanup()
        {
            return new ExposedKillerCleanupResult(
                true,
                true,
                true,
                true,
                true,
                true);
        }
    }

    public sealed class RemainingKillerCounterService
    {
        public int CountRemaining(IReadOnlyList<AccusationPlayerState> players)
        {
            if (players == null)
                return 0;

            int count = 0;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Role == PlayerRole.Killer && !players[i].IsExposed)
                    count++;
            }

            return count;
        }

        public IReadOnlyList<string> GetRemainingKillerIds(IReadOnlyList<AccusationPlayerState> players)
        {
            List<string> result = new List<string>();
            if (players == null)
                return result;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Role == PlayerRole.Killer && !players[i].IsExposed)
                    result.Add(players[i].PlayerId);
            }

            return result;
        }
    }

    public sealed class FinalAccusationListValidationService
    {
        public FinalAccusationValidationResult Validate(
            IReadOnlyList<string> selectedPlayerIds,
            IReadOnlyList<AccusationPlayerState> players)
        {
            RemainingKillerCounterService counter = new RemainingKillerCounterService();
            int requiredCount = counter.CountRemaining(players);

            if (selectedPlayerIds == null || selectedPlayerIds.Count != requiredCount)
                return new FinalAccusationValidationResult(false, $"Final accusation requires exactly {requiredCount} target(s).");

            HashSet<string> seen = new HashSet<string>();
            for (int i = 0; i < selectedPlayerIds.Count; i++)
            {
                string selected = selectedPlayerIds[i];
                if (string.IsNullOrWhiteSpace(selected) || !seen.Add(selected))
                    return new FinalAccusationValidationResult(false, "Final accusation contains duplicate or empty target.");

                AccusationPlayerState target = Find(players, selected);
                if (target.IsExposed)
                    return new FinalAccusationValidationResult(false, "Final accusation cannot target exposed killer.");
            }

            return new FinalAccusationValidationResult(true, "Final accusation list accepted.");
        }

        private static AccusationPlayerState Find(IReadOnlyList<AccusationPlayerState> players, string playerId)
        {
            if (players == null)
                return default;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].PlayerId == playerId)
                    return players[i];
            }

            return default;
        }
    }

    public sealed class FinalWinResolutionService
    {
        public WinResolutionResult ResolveFinal(
            IReadOnlyList<string> selectedPlayerIds,
            IReadOnlyList<AccusationPlayerState> players)
        {
            FinalAccusationListValidationService validationService = new FinalAccusationListValidationService();
            FinalAccusationValidationResult validation = validationService.Validate(selectedPlayerIds, players);
            if (!validation.IsValid)
                return new WinResolutionResult(MatchWinnerType.Killers, validation.Message);

            RemainingKillerCounterService counter = new RemainingKillerCounterService();
            IReadOnlyList<string> remainingKillers = counter.GetRemainingKillerIds(players);
            return SameSet(selectedPlayerIds, remainingKillers)
                ? new WinResolutionResult(MatchWinnerType.GoodSide, "All remaining killers found.")
                : new WinResolutionResult(MatchWinnerType.Killers, "Final accusation list was wrong.");
        }

        private static bool SameSet(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left == null || right == null || left.Count != right.Count)
                return false;

            HashSet<string> set = new HashSet<string>(left);
            for (int i = 0; i < right.Count; i++)
            {
                if (!set.Contains(right[i]))
                    return false;
            }

            return true;
        }
    }

    public sealed class CompanyBasedWinService
    {
        public WinResolutionResult ResolveIfAllVictimsDead(IReadOnlyList<AccusationPlayerState> players, int companyHealth)
        {
            bool hasLivingVictim = false;
            if (players != null)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].Role == PlayerRole.Victim && players[i].IsAlive)
                        hasLivingVictim = true;
                }
            }

            if (!hasLivingVictim && companyHealth <= 49)
                return new WinResolutionResult(MatchWinnerType.Killers, "All victims dead and company is in loss.");

            return new WinResolutionResult(MatchWinnerType.None, "Company-based win condition not met.");
        }
    }

    public sealed class MatchResultUiStateService
    {
        public MatchResultUiState Build(WinResolutionResult result)
        {
            string headline = result.WinnerType == MatchWinnerType.GoodSide ? "Good Side Wins" : "Killers Win";
            return new MatchResultUiState(result.WinnerType, headline, result.Reason);
        }
    }

    public sealed class EndRoleRevealService
    {
        public IReadOnlyList<EndRoleRevealEntry> BuildReveal(IReadOnlyList<AccusationPlayerState> players)
        {
            List<EndRoleRevealEntry> result = new List<EndRoleRevealEntry>();
            if (players == null)
                return result;

            for (int i = 0; i < players.Count; i++)
                result.Add(new EndRoleRevealEntry(players[i].PlayerId, players[i].Role));

            return result;
        }
    }
}
