namespace OFIS.Meetings
{
    public sealed class MeetingEmptyStateRuntimeTracker
    {
        private readonly MeetingEmptyStateResolutionService _resolutionService;

        private bool _wasEmptyLastTick;
        private float _emptyElapsedSeconds;

        public MeetingEmptyStateRuntimeTracker()
        {
            _resolutionService = new MeetingEmptyStateResolutionService();
            _wasEmptyLastTick = false;
            _emptyElapsedSeconds = 0f;
        }

        public MeetingEmptyStateRuntimeTracker(MeetingEmptyStateResolutionService resolutionService)
        {
            _resolutionService = resolutionService ?? new MeetingEmptyStateResolutionService();
            _wasEmptyLastTick = false;
            _emptyElapsedSeconds = 0f;
        }

        public float EmptyElapsedSeconds => _emptyElapsedSeconds;

        public void Reset()
        {
            _wasEmptyLastTick = false;
            _emptyElapsedSeconds = 0f;
        }

        public MeetingEmptyStateResolutionResult Tick(
            MeetingRuntimePhaseType phaseType,
            MeetingAttendanceRegistrationResult attendanceResult,
            float deltaTimeSeconds)
        {
            bool isMeetingPhase = phaseType == MeetingRuntimePhaseType.Meeting
                || phaseType == MeetingRuntimePhaseType.FinalMeeting;

            int registeredCount = attendanceResult == null ? 0 : attendanceResult.RegisteredCount;
            bool isEmpty = isMeetingPhase && registeredCount <= 0;

            if (!isEmpty)
            {
                _wasEmptyLastTick = false;
                _emptyElapsedSeconds = 0f;

                return _resolutionService.Evaluate(
                    phaseType,
                    attendanceResult,
                    _emptyElapsedSeconds);
            }

            if (!_wasEmptyLastTick)
            {
                _emptyElapsedSeconds = 0f;
                _wasEmptyLastTick = true;
            }

            if (deltaTimeSeconds > 0f)
                _emptyElapsedSeconds += deltaTimeSeconds;

            return _resolutionService.Evaluate(
                phaseType,
                attendanceResult,
                _emptyElapsedSeconds);
        }
    }
}
