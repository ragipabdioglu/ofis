using System.Collections.Generic;
using OFIS.MatchFlow.States;
using UnityEngine;

namespace OFIS.MatchFlow.Config
{
    [CreateAssetMenu(menuName = "OFIS/Match Flow/Match Flow Config")]
    public sealed class MatchFlowConfig : ScriptableObject
    {
        [Header("Match Duration")]
        public float totalMatchDurationSeconds = 18f * 60f;

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
            return new List<MatchTimelineEntry>
            {
                new MatchTimelineEntry
                {
                    state = MatchState.OfficePhase1,
                    startTimeSeconds = 0f,
                    endTimeSeconds = 4f * 60f
                },
                new MatchTimelineEntry
                {
                    state = MatchState.Meeting1,
                    startTimeSeconds = 4f * 60f,
                    endTimeSeconds = 6f * 60f
                },
                new MatchTimelineEntry
                {
                    state = MatchState.OfficePhase2,
                    startTimeSeconds = 6f * 60f,
                    endTimeSeconds = 10f * 60f
                },
                new MatchTimelineEntry
                {
                    state = MatchState.Meeting2,
                    startTimeSeconds = 10f * 60f,
                    endTimeSeconds = 12f * 60f
                },
                new MatchTimelineEntry
                {
                    state = MatchState.OfficePhase3,
                    startTimeSeconds = 12f * 60f,
                    endTimeSeconds = 16f * 60f
                },
                new MatchTimelineEntry
                {
                    state = MatchState.FinalMeeting,
                    startTimeSeconds = 16f * 60f,
                    endTimeSeconds = 18f * 60f
                }
            };
        }

        public List<MatchTimelineEntry> BuildFastTestTimeline()
        {
            var timeline = new List<MatchTimelineEntry>();

            float cursor = 0f;

            AddEntry(timeline, MatchState.OfficePhase1, ref cursor, fastOfficePhaseSeconds);
            AddEntry(timeline, MatchState.Meeting1, ref cursor, fastMeetingSeconds);
            AddEntry(timeline, MatchState.OfficePhase2, ref cursor, fastOfficePhaseSeconds);
            AddEntry(timeline, MatchState.Meeting2, ref cursor, fastMeetingSeconds);
            AddEntry(timeline, MatchState.OfficePhase3, ref cursor, fastOfficePhaseSeconds);
            AddEntry(timeline, MatchState.FinalMeeting, ref cursor, fastMeetingSeconds);

            return timeline;
        }

        public float GetFastTestDurationSeconds()
        {
            return (fastOfficePhaseSeconds * 3f) + (fastMeetingSeconds * 3f);
        }

        private static void AddEntry(
            List<MatchTimelineEntry> timeline,
            MatchState state,
            ref float cursor,
            float durationSeconds)
        {
            timeline.Add(new MatchTimelineEntry
            {
                state = state,
                startTimeSeconds = cursor,
                endTimeSeconds = cursor + durationSeconds
            });

            cursor += durationSeconds;
        }
    }
}