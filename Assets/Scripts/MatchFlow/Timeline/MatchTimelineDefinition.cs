using System;
using System.Collections.Generic;
using OFIS.MatchFlow.States;

namespace OFIS.MatchFlow.Timeline
{
    public sealed class MatchTimelineDefinition
    {
        private readonly MatchTimelineSegment[] _segments;

        public IReadOnlyList<MatchTimelineSegment> Segments => _segments;
        public float TotalDurationSeconds { get; }

        public MatchTimelineDefinition(params MatchTimelineSegment[] segments)
        {
            _segments = segments ?? Array.Empty<MatchTimelineSegment>();

            float totalDuration = 0f;
            for (int i = 0; i < _segments.Length; i++)
            {
                if (_segments[i] == null)
                    continue;

                totalDuration += _segments[i].DurationSeconds;
            }

            TotalDurationSeconds = totalDuration;
        }

        public MatchState GetStateAtTime(float elapsedSeconds)
        {
            if (_segments.Length == 0)
                return MatchState.None;

            float clampedElapsed = Math.Max(0f, elapsedSeconds);
            float cursor = 0f;

            for (int i = 0; i < _segments.Length; i++)
            {
                MatchTimelineSegment segment = _segments[i];
                if (segment == null)
                    continue;

                float segmentEnd = cursor + segment.DurationSeconds;
                if (clampedElapsed < segmentEnd)
                    return segment.State;

                cursor = segmentEnd;
            }

            return MatchState.MatchEnded;
        }

        public float GetRemainingSecondsInCurrentState(float elapsedSeconds)
        {
            if (_segments.Length == 0)
                return 0f;

            float clampedElapsed = Math.Max(0f, elapsedSeconds);
            float cursor = 0f;

            for (int i = 0; i < _segments.Length; i++)
            {
                MatchTimelineSegment segment = _segments[i];
                if (segment == null)
                    continue;

                float segmentEnd = cursor + segment.DurationSeconds;
                if (clampedElapsed < segmentEnd)
                    return segmentEnd - clampedElapsed;

                cursor = segmentEnd;
            }

            return 0f;
        }
    }
}
