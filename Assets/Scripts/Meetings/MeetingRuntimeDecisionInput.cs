using System.Collections.Generic;

namespace OFIS.Meetings
{
    public readonly struct MeetingRuntimeDecisionInput
    {
        public MeetingRuntimePhaseType PhaseType { get; }
        public float PhaseDurationSeconds { get; }
        public float PhaseElapsedSeconds { get; }
        public float DeltaTimeSeconds { get; }
        public int CurrentCompanyHealth { get; }
        public MeetingAttendanceRegistrationResult AttendanceResult { get; }
        public IReadOnlyList<MeetingReportData> Reports { get; }
        public IReadOnlyList<MeetingVoteData> Votes { get; }
        public IReadOnlyCollection<string> CulpritPlayerIds { get; }

        public MeetingRuntimeDecisionInput(
            MeetingRuntimePhaseType phaseType,
            float phaseDurationSeconds,
            float phaseElapsedSeconds,
            float deltaTimeSeconds,
            int currentCompanyHealth,
            MeetingAttendanceRegistrationResult attendanceResult,
            IReadOnlyList<MeetingReportData> reports,
            IReadOnlyList<MeetingVoteData> votes,
            IReadOnlyCollection<string> culpritPlayerIds)
        {
            PhaseType = phaseType;
            PhaseDurationSeconds = phaseDurationSeconds < 0f ? 0f : phaseDurationSeconds;
            PhaseElapsedSeconds = phaseElapsedSeconds < 0f ? 0f : phaseElapsedSeconds;
            DeltaTimeSeconds = deltaTimeSeconds < 0f ? 0f : deltaTimeSeconds;
            CurrentCompanyHealth = currentCompanyHealth < 0 ? 0 : currentCompanyHealth;
            AttendanceResult = attendanceResult;
            Reports = reports ?? new List<MeetingReportData>();
            Votes = votes ?? new List<MeetingVoteData>();
            CulpritPlayerIds = culpritPlayerIds ?? new HashSet<string>();
        }
    }
}
