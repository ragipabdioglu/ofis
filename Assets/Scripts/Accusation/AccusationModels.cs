using System.Collections.Generic;
using OFIS.Roles;

namespace OFIS.Accusation
{
    public enum AccusationOutcomeType
    {
        None = 0,
        Correct = 1,
        Wrong = 2,
        Invalid = 3
    }

    public enum MatchWinnerType
    {
        None = 0,
        GoodSide = 1,
        Killers = 2
    }

    public readonly struct AccusationPlayerState
    {
        public string PlayerId { get; }
        public PlayerRole Role { get; }
        public bool IsAlive { get; }
        public bool IsExposed { get; }

        public AccusationPlayerState(string playerId, PlayerRole role, bool isAlive, bool isExposed)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? "unknown_player" : playerId;
            Role = role;
            IsAlive = isAlive;
            IsExposed = isExposed;
        }
    }

    public readonly struct AccusationResolutionResult
    {
        public AccusationOutcomeType OutcomeType { get; }
        public string TargetPlayerId { get; }
        public bool ShouldRevealRole { get; }
        public bool ShouldExposeTarget { get; }
        public int CompanyDelta { get; }
        public int NextInfoActionQualityDelta { get; }
        public string Message { get; }

        public AccusationResolutionResult(
            AccusationOutcomeType outcomeType,
            string targetPlayerId,
            bool shouldRevealRole,
            bool shouldExposeTarget,
            int companyDelta,
            int nextInfoActionQualityDelta,
            string message)
        {
            OutcomeType = outcomeType;
            TargetPlayerId = string.IsNullOrWhiteSpace(targetPlayerId) ? "none" : targetPlayerId;
            ShouldRevealRole = shouldRevealRole;
            ShouldExposeTarget = shouldExposeTarget;
            CompanyDelta = companyDelta;
            NextInfoActionQualityDelta = nextInfoActionQualityDelta;
            Message = string.IsNullOrWhiteSpace(message) ? "No accusation message." : message;
        }
    }

    public readonly struct FinalAccusationValidationResult
    {
        public bool IsValid { get; }
        public string Message { get; }

        public FinalAccusationValidationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = string.IsNullOrWhiteSpace(message) ? "No final accusation message." : message;
        }
    }

    public readonly struct WinResolutionResult
    {
        public MatchWinnerType WinnerType { get; }
        public string Reason { get; }

        public WinResolutionResult(MatchWinnerType winnerType, string reason)
        {
            WinnerType = winnerType;
            Reason = string.IsNullOrWhiteSpace(reason) ? "No win reason." : reason;
        }
    }

    public readonly struct MatchResultUiState
    {
        public MatchWinnerType WinnerType { get; }
        public string Headline { get; }
        public string Detail { get; }

        public MatchResultUiState(MatchWinnerType winnerType, string headline, string detail)
        {
            WinnerType = winnerType;
            Headline = string.IsNullOrWhiteSpace(headline) ? "Match Ended" : headline;
            Detail = string.IsNullOrWhiteSpace(detail) ? "Result resolved." : detail;
        }
    }

    public readonly struct EndRoleRevealEntry
    {
        public string PlayerId { get; }
        public PlayerRole Role { get; }

        public EndRoleRevealEntry(string playerId, PlayerRole role)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? "unknown_player" : playerId;
            Role = role;
        }
    }

    public readonly struct ExposedKillerCleanupResult
    {
        public bool KillDisabled { get; }
        public bool SabotageDisabled { get; }
        public bool CarryDisabled { get; }
        public bool ActiveSabotageNeutralized { get; }
        public bool FutureTracesInvalidated { get; }
        public bool RawLogsPreserved { get; }

        public ExposedKillerCleanupResult(
            bool killDisabled,
            bool sabotageDisabled,
            bool carryDisabled,
            bool activeSabotageNeutralized,
            bool futureTracesInvalidated,
            bool rawLogsPreserved)
        {
            KillDisabled = killDisabled;
            SabotageDisabled = sabotageDisabled;
            CarryDisabled = carryDisabled;
            ActiveSabotageNeutralized = activeSabotageNeutralized;
            FutureTracesInvalidated = futureTracesInvalidated;
            RawLogsPreserved = rawLogsPreserved;
        }
    }
}
