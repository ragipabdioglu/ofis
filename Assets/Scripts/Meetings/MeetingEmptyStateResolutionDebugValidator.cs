using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingEmptyStateResolutionDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateEmptyStateResolution();
        }

        [ContextMenu("Validate Meeting Empty State Resolution")]
        public void ValidateEmptyStateResolution()
        {
            ValidateNonMeetingPhaseDoesNothing();
            ValidateNormalMeetingWithParticipantsContinues();
            ValidateEmptyNormalMeetingWaitsBeforeAutoClose();
            ValidateEmptyNormalMeetingAutoClosesAfterDelay();
            ValidateEmptyFinalMeetingTriggersWinBranch();
            ValidateRuntimeTrackerAccumulatesEmptyTime();
            ValidateRuntimeTrackerResetsWhenParticipantAppears();
        }

        private void ValidateNonMeetingPhaseDoesNothing()
        {
            MeetingEmptyStateResolutionService service = new MeetingEmptyStateResolutionService();

            MeetingEmptyStateResolutionResult result = service.Evaluate(
                MeetingRuntimePhaseType.Office,
                BuildEmptyAttendance(),
                20f);

            bool passed = result.ResolutionType == MeetingEmptyStateResolutionType.None
                && !result.IsResolved
                && !result.ShouldCloseMeeting
                && !result.ShouldResolveWinBranch;

            LogResult("NonMeetingPhaseDoesNothing", passed, result);
        }

        private void ValidateNormalMeetingWithParticipantsContinues()
        {
            MeetingEmptyStateResolutionService service = new MeetingEmptyStateResolutionService();

            MeetingEmptyStateResolutionResult result = service.Evaluate(
                MeetingRuntimePhaseType.Meeting,
                BuildAttendance(registeredCount: 2, missingCount: 0, lateObserverCount: 0),
                20f);

            bool passed = result.ResolutionType == MeetingEmptyStateResolutionType.ContinueMeeting
                && !result.IsEmpty
                && !result.IsResolved
                && !result.ShouldCloseMeeting;

            LogResult("NormalMeetingWithParticipantsContinues", passed, result);
        }

        private void ValidateEmptyNormalMeetingWaitsBeforeAutoClose()
        {
            MeetingEmptyStateResolutionService service = new MeetingEmptyStateResolutionService();

            MeetingEmptyStateResolutionResult result = service.Evaluate(
                MeetingRuntimePhaseType.Meeting,
                BuildEmptyAttendance(),
                5f);

            bool passed = result.ResolutionType == MeetingEmptyStateResolutionType.ContinueMeeting
                && result.IsEmpty
                && !result.IsResolved
                && !result.ShouldCloseMeeting;

            LogResult("EmptyNormalMeetingWaitsBeforeAutoClose", passed, result);
        }

        private void ValidateEmptyNormalMeetingAutoClosesAfterDelay()
        {
            MeetingEmptyStateResolutionService service = new MeetingEmptyStateResolutionService();

            MeetingEmptyStateResolutionResult result = service.Evaluate(
                MeetingRuntimePhaseType.Meeting,
                BuildEmptyAttendance(),
                10f);

            bool passed = result.ResolutionType == MeetingEmptyStateResolutionType.AutoCloseNormalMeeting
                && result.IsEmpty
                && result.IsResolved
                && result.ShouldCloseMeeting
                && !result.ShouldResolveWinBranch;

            LogResult("EmptyNormalMeetingAutoClosesAfterDelay", passed, result);
        }

        private void ValidateEmptyFinalMeetingTriggersWinBranch()
        {
            MeetingEmptyStateResolutionService service = new MeetingEmptyStateResolutionService();

            MeetingEmptyStateResolutionResult result = service.Evaluate(
                MeetingRuntimePhaseType.FinalMeeting,
                BuildEmptyAttendance(),
                0f);

            bool passed = result.ResolutionType == MeetingEmptyStateResolutionType.ResolveFinalMeetingWinBranch
                && result.IsEmpty
                && result.IsResolved
                && !result.ShouldCloseMeeting
                && result.ShouldResolveWinBranch;

            LogResult("EmptyFinalMeetingTriggersWinBranch", passed, result);
        }

        private void ValidateRuntimeTrackerAccumulatesEmptyTime()
        {
            MeetingEmptyStateRuntimeTracker tracker = new MeetingEmptyStateRuntimeTracker();

            MeetingEmptyStateResolutionResult first = tracker.Tick(
                MeetingRuntimePhaseType.Meeting,
                BuildEmptyAttendance(),
                3f);

            MeetingEmptyStateResolutionResult second = tracker.Tick(
                MeetingRuntimePhaseType.Meeting,
                BuildEmptyAttendance(),
                4f);

            MeetingEmptyStateResolutionResult third = tracker.Tick(
                MeetingRuntimePhaseType.Meeting,
                BuildEmptyAttendance(),
                3f);

            bool passed = !first.IsResolved
                && !second.IsResolved
                && third.IsResolved
                && third.ShouldCloseMeeting
                && third.EmptyElapsedSeconds >= 10f;

            LogResult("RuntimeTrackerAccumulatesEmptyTime", passed, third);
        }

        private void ValidateRuntimeTrackerResetsWhenParticipantAppears()
        {
            MeetingEmptyStateRuntimeTracker tracker = new MeetingEmptyStateRuntimeTracker();

            tracker.Tick(MeetingRuntimePhaseType.Meeting, BuildEmptyAttendance(), 7f);

            MeetingEmptyStateResolutionResult withParticipant = tracker.Tick(
                MeetingRuntimePhaseType.Meeting,
                BuildAttendance(registeredCount: 1, missingCount: 0, lateObserverCount: 0),
                1f);

            MeetingEmptyStateResolutionResult emptyAgain = tracker.Tick(
                MeetingRuntimePhaseType.Meeting,
                BuildEmptyAttendance(),
                2f);

            bool passed = !withParticipant.IsEmpty
                && withParticipant.EmptyElapsedSeconds == 0f
                && emptyAgain.IsEmpty
                && emptyAgain.EmptyElapsedSeconds == 2f
                && !emptyAgain.IsResolved;

            LogResult("RuntimeTrackerResetsWhenParticipantAppears", passed, emptyAgain);
        }

        private static MeetingAttendanceRegistrationResult BuildEmptyAttendance()
        {
            return BuildAttendance(registeredCount: 0, missingCount: 0, lateObserverCount: 0);
        }

        private static MeetingAttendanceRegistrationResult BuildAttendance(
            int registeredCount,
            int missingCount,
            int lateObserverCount)
        {
            List<string> registered = new List<string>();
            List<string> missing = new List<string>();
            List<string> lateObservers = new List<string>();

            for (int i = 0; i < registeredCount; i++)
                registered.Add($"registered_{i + 1}");

            for (int i = 0; i < missingCount; i++)
                missing.Add($"missing_{i + 1}");

            for (int i = 0; i < lateObserverCount; i++)
                lateObservers.Add($"observer_{i + 1}");

            return new MeetingAttendanceRegistrationResult(
                registered,
                missing,
                lateObservers,
                new List<string>(),
                "Debug attendance fixture.");
        }

        private static void LogResult(string testName, bool passed, MeetingEmptyStateResolutionResult result)
        {
            if (passed)
                Debug.Log($"[MeetingEmptyStateResolutionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingEmptyStateResolutionValidator] FAIL {testName}: {result}");
        }
    }
}
