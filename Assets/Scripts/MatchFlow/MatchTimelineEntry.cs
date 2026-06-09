using System;
using OFIS.MatchFlow.States;

namespace OFIS.MatchFlow
{
    [Serializable]
    public sealed class MatchTimelineEntry
    {
        public MatchState state;
        public float startTimeSeconds;
        public float endTimeSeconds;

        public float DurationSeconds => endTimeSeconds - startTimeSeconds;

        public bool ContainsTime(float matchTimeSeconds)
        {
            return matchTimeSeconds >= startTimeSeconds && matchTimeSeconds < endTimeSeconds;
        }
    }
}