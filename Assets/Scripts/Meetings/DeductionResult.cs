using System.Collections.Generic;

namespace OFIS.Meetings
{
    public readonly struct DeductionResult
    {
        public DeductionOutcomeType OutcomeType { get; }
        public bool IsResolved { get; }
        public bool IsCorrectAccusation { get; }
        public string AccusedPlayerId { get; }
        public int VoteCount { get; }
        public IReadOnlyList<string> TiedPlayerIds { get; }
        public string Message { get; }

        public DeductionResult(
            DeductionOutcomeType outcomeType,
            bool isResolved,
            bool isCorrectAccusation,
            string accusedPlayerId,
            int voteCount,
            IReadOnlyList<string> tiedPlayerIds,
            string message)
        {
            OutcomeType = outcomeType;
            IsResolved = isResolved;
            IsCorrectAccusation = isCorrectAccusation;
            AccusedPlayerId = string.IsNullOrWhiteSpace(accusedPlayerId) ? "none" : accusedPlayerId;
            VoteCount = voteCount < 0 ? 0 : voteCount;
            TiedPlayerIds = tiedPlayerIds ?? new List<string>();
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public override string ToString()
        {
            return $"Outcome={OutcomeType}, Resolved={IsResolved}, Correct={IsCorrectAccusation}, Accused={AccusedPlayerId}, VoteCount={VoteCount}, TieCount={TiedPlayerIds.Count}, Message={Message}";
        }
    }
}
