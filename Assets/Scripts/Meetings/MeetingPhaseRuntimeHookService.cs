using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingPhaseRuntimeHookService
    {
        private readonly MeetingEndPipelineService _meetingEndPipelineService;
        private bool _pipelineTriggered;

        public MeetingPhaseRuntimeHookService()
        {
            _meetingEndPipelineService = new MeetingEndPipelineService();
        }

        public MeetingPhaseRuntimeHookService(MeetingEndPipelineService meetingEndPipelineService)
        {
            _meetingEndPipelineService = meetingEndPipelineService ?? new MeetingEndPipelineService();
        }

        public bool PipelineTriggered => _pipelineTriggered;

        public void Reset()
        {
            _pipelineTriggered = false;
        }

        public MeetingPhaseRuntimeHookResult Tick(
            MeetingRuntimePhaseType phaseType,
            float durationSeconds,
            float elapsedSeconds,
            IReadOnlyList<MeetingReportData> reports,
            IReadOnlyList<MeetingVoteData> votes,
            IReadOnlyCollection<string> culpritPlayerIds,
            float joinLockThresholdSeconds = 20f)
        {
            bool isMeetingPhase = phaseType == MeetingRuntimePhaseType.Meeting
                || phaseType == MeetingRuntimePhaseType.FinalMeeting;

            if (!isMeetingPhase)
            {
                MeetingPhaseRuntimeHookState nonMeetingState = new MeetingPhaseRuntimeHookState(
                    phaseType,
                    durationSeconds,
                    elapsedSeconds,
                    joinLockThresholdSeconds,
                    false,
                    _pipelineTriggered,
                    "Current phase is not a meeting phase.");

                return new MeetingPhaseRuntimeHookResult(
                    nonMeetingState,
                    false,
                    default,
                    "No meeting pipeline run.");
            }

            MeetingPhaseRuntimeHookState state = new MeetingPhaseRuntimeHookState(
                phaseType,
                durationSeconds,
                elapsedSeconds,
                joinLockThresholdSeconds,
                true,
                _pipelineTriggered,
                "Meeting phase tick evaluated.");

            if (!state.HasEnded)
            {
                return new MeetingPhaseRuntimeHookResult(
                    state,
                    false,
                    default,
                    "Meeting phase is still running.");
            }

            if (_pipelineTriggered)
            {
                MeetingPhaseRuntimeHookState alreadyTriggeredState = new MeetingPhaseRuntimeHookState(
                    phaseType,
                    durationSeconds,
                    elapsedSeconds,
                    joinLockThresholdSeconds,
                    true,
                    true,
                    "Meeting phase already ended and pipeline was already triggered.");

                return new MeetingPhaseRuntimeHookResult(
                    alreadyTriggeredState,
                    false,
                    default,
                    "Pipeline already triggered.");
            }

            _pipelineTriggered = true;

            MeetingEndPipelineResult pipelineResult = _meetingEndPipelineService.Run(
                reports ?? new List<MeetingReportData>(),
                votes ?? new List<MeetingVoteData>(),
                culpritPlayerIds ?? new HashSet<string>());

            MeetingPhaseRuntimeHookState triggeredState = new MeetingPhaseRuntimeHookState(
                phaseType,
                durationSeconds,
                elapsedSeconds,
                joinLockThresholdSeconds,
                true,
                true,
                "Meeting phase ended and pipeline was triggered.");

            return new MeetingPhaseRuntimeHookResult(
                triggeredState,
                true,
                pipelineResult,
                "Meeting end pipeline triggered by runtime hook.");
        }
    }
}
