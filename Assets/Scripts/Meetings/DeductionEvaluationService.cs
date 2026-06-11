using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class DeductionEvaluationService
    {
        public DeductionResult Evaluate(
            MeetingVoteEvaluationResult voteResult,
            IReadOnlyCollection<string> killerPlayerIds)
        {
            if (!voteResult.HasVotes)
            {
                return new DeductionResult(
                    DeductionOutcomeType.NoVotes,
                    false,
                    false,
                    "none",
                    0,
                    new List<string>(),
                    "No vote result to evaluate.");
            }

            if (voteResult.IsTie)
            {
                return new DeductionResult(
                    DeductionOutcomeType.Tie,
                    false,
                    false,
                    "none",
                    voteResult.WinnerVoteCount,
                    voteResult.TiedPlayerIds,
                    "Vote result is tied.");
            }

            if (!voteResult.HasWinner || string.IsNullOrWhiteSpace(voteResult.WinnerPlayerId) || voteResult.WinnerPlayerId == "none")
            {
                return new DeductionResult(
                    DeductionOutcomeType.InvalidTarget,
                    false,
                    false,
                    "none",
                    voteResult.WinnerVoteCount,
                    new List<string>(),
                    "Vote winner is invalid.");
            }

            bool isKiller = IsKiller(voteResult.WinnerPlayerId, killerPlayerIds);

            if (isKiller)
            {
                return new DeductionResult(
                    DeductionOutcomeType.CorrectAccusation,
                    true,
                    true,
                    voteResult.WinnerPlayerId,
                    voteResult.WinnerVoteCount,
                    new List<string>(),
                    "Correct accusation.");
            }

            return new DeductionResult(
                DeductionOutcomeType.WrongAccusation,
                true,
                false,
                voteResult.WinnerPlayerId,
                voteResult.WinnerVoteCount,
                new List<string>(),
                "Wrong accusation.");
        }

        private static bool IsKiller(string playerId, IReadOnlyCollection<string> killerPlayerIds)
        {
            if (string.IsNullOrWhiteSpace(playerId) || killerPlayerIds == null)
                return false;

            foreach (string killerPlayerId in killerPlayerIds)
            {
                if (killerPlayerId == playerId)
                    return true;
            }

            return false;
        }
    }
}
