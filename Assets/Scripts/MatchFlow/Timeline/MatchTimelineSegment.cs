using System;
using OFIS.MatchFlow.States;
using UnityEngine;

namespace OFIS.MatchFlow.Timeline
{
    [Serializable]
    public sealed class MatchTimelineSegment
    {
        [SerializeField] private MatchState state;
        [SerializeField] private float durationSeconds;

        public MatchState State => state;
        public float DurationSeconds => durationSeconds;

        public MatchTimelineSegment(MatchState state, float durationSeconds)
        {
            this.state = state;
            this.durationSeconds = Mathf.Max(0f, durationSeconds);
        }
    }
}
