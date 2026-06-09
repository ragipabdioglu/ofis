using OFIS.Core.Events;
using OFIS.MatchFlow.States;

namespace OFIS.MatchFlow.Events
{
    public sealed class MatchTimerTickEvent : IGameEvent
    {
        public float CreatedAtRealtime { get; }

        public MatchState CurrentState { get; }

        public float MatchTimeSeconds { get; }
        public float MatchRemainingSeconds { get; }

        public float CurrentStateElapsedSeconds { get; }
        public float CurrentStateRemainingSeconds { get; }

        public MatchTimerTickEvent(
            MatchState currentState,
            float matchTimeSeconds,
            float matchRemainingSeconds,
            float currentStateElapsedSeconds,
            float currentStateRemainingSeconds,
            float createdAtRealtime)
        {
            CurrentState = currentState;
            MatchTimeSeconds = matchTimeSeconds;
            MatchRemainingSeconds = matchRemainingSeconds;
            CurrentStateElapsedSeconds = currentStateElapsedSeconds;
            CurrentStateRemainingSeconds = currentStateRemainingSeconds;
            CreatedAtRealtime = createdAtRealtime;
        }
    }
}