namespace OFIS.Meetings
{
    public sealed class MeetingEmptyStateResolutionService
    {
        private readonly MeetingEmptyStateResolutionConfig _config;

        public MeetingEmptyStateResolutionService()
        {
            _config = MeetingEmptyStateResolutionConfig.Default;
        }

        public MeetingEmptyStateResolutionService(MeetingEmptyStateResolutionConfig config)
        {
            _config = config;
        }

        public MeetingEmptyStateResolutionConfig Config => _config;

        public MeetingEmptyStateResolutionResult Evaluate(
            MeetingRuntimePhaseType phaseType,
            MeetingAttendanceRegistrationResult attendanceResult,
            float emptyElapsedSeconds)
        {
            bool isMeetingPhase = phaseType == MeetingRuntimePhaseType.Meeting
                || phaseType == MeetingRuntimePhaseType.FinalMeeting;

            if (!isMeetingPhase)
            {
                return new MeetingEmptyStateResolutionResult(
                    phaseType,
                    MeetingEmptyStateResolutionType.None,
                    registeredPlayerCount: 0,
                    lateObserverCount: 0,
                    emptyElapsedSeconds: emptyElapsedSeconds,
                    requiredDelaySeconds: _config.NormalMeetingAutoCloseDelaySeconds,
                    isEmpty: false,
                    shouldCloseMeeting: false,
                    shouldResolveWinBranch: false,
                    isResolved: false,
                    reason: "Current phase is not a meeting phase.");
            }

            int registeredCount = attendanceResult == null ? 0 : attendanceResult.RegisteredCount;
            int lateObserverCount = attendanceResult == null ? 0 : attendanceResult.LateObserverCount;
            bool isEmpty = registeredCount <= 0;

            if (!isEmpty)
            {
                return new MeetingEmptyStateResolutionResult(
                    phaseType,
                    MeetingEmptyStateResolutionType.ContinueMeeting,
                    registeredCount,
                    lateObserverCount,
                    emptyElapsedSeconds,
                    _config.NormalMeetingAutoCloseDelaySeconds,
                    isEmpty: false,
                    shouldCloseMeeting: false,
                    shouldResolveWinBranch: false,
                    isResolved: false,
                    reason: "Meeting has registered participants.");
            }

            if (phaseType == MeetingRuntimePhaseType.FinalMeeting)
            {
                if (!_config.ResolveFinalMeetingWhenEmpty)
                {
                    return new MeetingEmptyStateResolutionResult(
                        phaseType,
                        MeetingEmptyStateResolutionType.ContinueMeeting,
                        registeredCount,
                        lateObserverCount,
                        emptyElapsedSeconds,
                        _config.NormalMeetingAutoCloseDelaySeconds,
                        isEmpty: true,
                        shouldCloseMeeting: false,
                        shouldResolveWinBranch: false,
                        isResolved: false,
                        reason: "Final meeting is empty but config blocks win resolution.");
                }

                return new MeetingEmptyStateResolutionResult(
                    phaseType,
                    MeetingEmptyStateResolutionType.ResolveFinalMeetingWinBranch,
                    registeredCount,
                    lateObserverCount,
                    emptyElapsedSeconds,
                    _config.NormalMeetingAutoCloseDelaySeconds,
                    isEmpty: true,
                    shouldCloseMeeting: false,
                    shouldResolveWinBranch: true,
                    isResolved: true,
                    reason: "Final meeting is empty. Win resolution branch should be triggered.");
            }

            if (!_config.AutoCloseNormalMeetingWhenEmpty)
            {
                return new MeetingEmptyStateResolutionResult(
                    phaseType,
                    MeetingEmptyStateResolutionType.ContinueMeeting,
                    registeredCount,
                    lateObserverCount,
                    emptyElapsedSeconds,
                    _config.NormalMeetingAutoCloseDelaySeconds,
                    isEmpty: true,
                    shouldCloseMeeting: false,
                    shouldResolveWinBranch: false,
                    isResolved: false,
                    reason: "Normal meeting is empty but auto close is disabled.");
            }

            bool hasDelayPassed = emptyElapsedSeconds >= _config.NormalMeetingAutoCloseDelaySeconds;

            if (!hasDelayPassed)
            {
                return new MeetingEmptyStateResolutionResult(
                    phaseType,
                    MeetingEmptyStateResolutionType.ContinueMeeting,
                    registeredCount,
                    lateObserverCount,
                    emptyElapsedSeconds,
                    _config.NormalMeetingAutoCloseDelaySeconds,
                    isEmpty: true,
                    shouldCloseMeeting: false,
                    shouldResolveWinBranch: false,
                    isResolved: false,
                    reason: "Normal meeting is empty but auto close delay has not passed yet.");
            }

            return new MeetingEmptyStateResolutionResult(
                phaseType,
                MeetingEmptyStateResolutionType.AutoCloseNormalMeeting,
                registeredCount,
                lateObserverCount,
                emptyElapsedSeconds,
                _config.NormalMeetingAutoCloseDelaySeconds,
                isEmpty: true,
                shouldCloseMeeting: true,
                shouldResolveWinBranch: false,
                isResolved: true,
                reason: "Normal meeting is empty and auto close delay has passed.");
        }
    }
}
