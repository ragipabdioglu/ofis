using OFIS.Core.Events;
using OFIS.MatchFlow.States;

namespace OFIS.MatchFlow.Events
{
    public sealed class MatchStateChangedEvent : IGameEvent
    {
        public float CreatedAtRealtime { get; }

        public MatchState PreviousState { get; }
        public MatchState NewState { get; }

        public float MatchTimeSeconds { get; }

        public MatchStateChangedEvent(
            MatchState previousState,
            MatchState newState,
            float matchTimeSeconds,
            float createdAtRealtime)
        {
            PreviousState = previousState;
            NewState = newState;
            MatchTimeSeconds = matchTimeSeconds;
            CreatedAtRealtime = createdAtRealtime;
        }
    }
}