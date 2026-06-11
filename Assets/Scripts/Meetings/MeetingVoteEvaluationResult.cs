using System.Collections.Generic;

namespace OFIS.Meetings
{
    public readonly struct MeetingVoteEvaluationResult
    {
        public bool HasVotes { get; }
        public bool HasWinner { get; }
        public bool IsTie { get; }
        public string WinnerPlayerId { get; }
        public int WinnerVoteCount { get; }
        public IReadOnlyList<string> TiedPlayerIds { get; }
        public string Message { get; }

        public MeetingVoteEvaluationResult(
            bool hasVotes,
            bool hasWinner,
            bool isTie,
            string winnerPlayerId,
            int winnerVoteCount,
            IReadOnlyList<string> tiedPlayerIds,
            string message)
        {
            HasVotes = hasVotes;
            HasWinner = hasWinner;
            IsTie = isTie;
            WinnerPlayerId = string.IsNullOrWhiteSpace(winnerPlayerId) ? "none" : winnerPlayerId;
            WinnerVoteCount = winnerVoteCount < 0 ? 0 : winnerVoteCount;
            TiedPlayerIds = tiedPlayerIds ?? new List<string>();
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public override string ToString()
        {
            return $"HasVotes={HasVotes}, HasWinner={HasWinner}, IsTie={IsTie}, Winner={WinnerPlayerId}, WinnerVoteCount={WinnerVoteCount}, TieCount={TiedPlayerIds.Count}, Message={Message}";
        }
    }
}
