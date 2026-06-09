using System.Collections.Generic;
using OFIS.MatchFlow.States;
using OFIS.MatchFlow.Timeline;
using UnityEngine;

namespace OFIS.MatchFlow.Config
{
    [CreateAssetMenu(menuName = "OFIS/Match Flow/Match Flow Config")]
    public sealed class MatchFlowConfig : ScriptableObject
    {
        [Header("Match Duration")]
        public float totalMatchDurationSeconds = MatchTimelineFactory.ProductionTotalDurationSeconds;

        [Header("Match End")]
        public float resolvingMatchSeconds = 2f;

        [Header("Meeting Warning")]
        public float meetingAnnouncementSeconds = 30f;
        public float meetingRedWarningSeconds = 10f;
        public float meetingJoinLockLastSeconds = 20f;

        [Header("Setup Durations")]
        public float roleAssignmentSeconds = 2f;
        public float characterAssignmentSeconds = 2f;

        [Header("Fast Test Timeline")]
        public float fastOfficePhaseSeconds = 10f;
        public float fastMeetingSeconds = 6f;

        public List<MatchTimelineEntry> BuildDefaultTimeline()
        {
            return BuildEntries(MatchTimelineFactory.CreateProductionTimeline());
        }

        public List<MatchTimelineEntry> BuildFastTestTimeline()
        {
            return new List<MatchTimelineEntry>
            {
                CreateEntry(MatchState.OfficePhase1, 0f, fastOfficePhaseSeconds),
                CreateEntry(MatchState.Meeting1, fastOfficePhaseSeconds, fastOfficePhaseSeconds + fastMeetingSeconds),
                CreateEntry(MatchState.OfficePhase2, fastOfficePhaseSeconds + fastMeetingSeconds, (fastOfficePhaseSeconds * 2f) + fastMeetingSeconds),
                CreateEntry(MatchState.Meeting2, (fastOfficePhaseSeconds * 2f) + fastMeetingSeconds, (fastOfficePhaseSeconds * 2f) + (fastMeetingSeconds * 2f)),
                CreateEntry(MatchState.OfficePhase3, (fastOfficePhaseSeconds * 2f) + (fastMeetingSeconds * 2f), (fastOfficePhaseSeconds * 3f) + (fastMeetingSeconds * 2f)),
                CreateEntry(MatchState.FinalMeeting, (fastOfficePhaseSeconds * 3f) + (fastMeetingSeconds * 2f), GetFastTestDurationSeconds())
            };
        }

        public float GetDefaultTimelineDurationSeconds()
        {
            return MatchTimelineFactory.CreateProductionTimeline().TotalDurationSeconds;
        }

        public float GetFastTestDurationSeconds()
        {
            return (fastOfficePhaseSeconds * 3f) + (fastMeetingSeconds * 3f);
        }

        private static List<MatchTimelineEntry> BuildEntries(MatchTimelineDefinition definition)
        {
            List<MatchTimelineEntry> entries = new();
            float cursor = 0f;

            for (int i = 0; i < definition.Segments.Count; i++)
            {
                MatchTimelineSegment segment = definition.Segments[i];
                entries.Add(CreateEntry(segment.State, cursor, cursor + segment.DurationSeconds));
                cursor += segment.DurationSeconds;
            }

            return entries;
        }

        private static MatchTimelineEntry CreateEntry(MatchState state, float startTimeSeconds, float endTimeSeconds)
        {
            return new MatchTimelineEntry
            {
                state = state,
                startTimeSeconds = startTimeSeconds,
                endTimeSeconds = endTimeSeconds
            };
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            totalMatchDurationSeconds = MatchTimelineFactory.ProductionTotalDurationSeconds;
            resolvingMatchSeconds = Mathf.Max(0f, resolvingMatchSeconds);
        }
#endif
    }
}
