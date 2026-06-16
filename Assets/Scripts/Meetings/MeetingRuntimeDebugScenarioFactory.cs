using System.Collections.Generic;
using OFIS.Rooms;
using OFIS.Sabotage;

namespace OFIS.Meetings
{
    public static class MeetingRuntimeDebugScenarioFactory
    {
        public static MeetingRuntimeDecisionInput CreateInput(
            MeetingRuntimeDebugScenarioType scenarioType,
            float deltaTimeSeconds,
            int currentCompanyHealth)
        {
            switch (scenarioType)
            {
                case MeetingRuntimeDebugScenarioType.MissingPenalty:
                    return CreateMissingPenaltyInput(deltaTimeSeconds, currentCompanyHealth);

                case MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose:
                    return CreateEmptyNormalInput(deltaTimeSeconds, currentCompanyHealth);

                case MeetingRuntimeDebugScenarioType.EmptyFinalWinBranch:
                    return CreateEmptyFinalInput(deltaTimeSeconds, currentCompanyHealth);

                case MeetingRuntimeDebugScenarioType.MeetingEndPipeline:
                    return CreateMeetingEndPipelineInput(deltaTimeSeconds, currentCompanyHealth);

                case MeetingRuntimeDebugScenarioType.NormalMeetingContinue:
                default:
                    return CreateNormalContinueInput(deltaTimeSeconds, currentCompanyHealth);
            }
        }

        private static MeetingRuntimeDecisionInput CreateNormalContinueInput(float deltaTimeSeconds, int currentCompanyHealth)
        {
            return new MeetingRuntimeDecisionInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                30f,
                deltaTimeSeconds,
                currentCompanyHealth,
                BuildAttendance(2, 0, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits());
        }

        private static MeetingRuntimeDecisionInput CreateMissingPenaltyInput(float deltaTimeSeconds, int currentCompanyHealth)
        {
            return new MeetingRuntimeDecisionInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                30f,
                deltaTimeSeconds,
                currentCompanyHealth,
                BuildAttendance(2, 2, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits());
        }

        private static MeetingRuntimeDecisionInput CreateEmptyNormalInput(float deltaTimeSeconds, int currentCompanyHealth)
        {
            return new MeetingRuntimeDecisionInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                30f,
                deltaTimeSeconds,
                currentCompanyHealth,
                BuildAttendance(0, 0, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits());
        }

        private static MeetingRuntimeDecisionInput CreateEmptyFinalInput(float deltaTimeSeconds, int currentCompanyHealth)
        {
            return new MeetingRuntimeDecisionInput(
                MeetingRuntimePhaseType.FinalMeeting,
                120f,
                30f,
                deltaTimeSeconds,
                currentCompanyHealth,
                BuildAttendance(0, 0, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits());
        }

        private static MeetingRuntimeDecisionInput CreateMeetingEndPipelineInput(float deltaTimeSeconds, int currentCompanyHealth)
        {
            return new MeetingRuntimeDecisionInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                120f,
                deltaTimeSeconds,
                currentCompanyHealth,
                BuildAttendance(2, 0, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits());
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
                "Scene bridge debug attendance fixture.");
        }

        private static List<MeetingReportData> BuildReports()
        {
            return new List<MeetingReportData>
            {
                new MeetingReportData(
                    "scene_bridge_report_001",
                    MeetingReportType.Suspicion,
                    "registered_1",
                    "culprit_01",
                    OfficeRoomType.MeetingRoom,
                    0,
                    0,
                    SabotageObjectiveState.None,
                    "Scene bridge suspicion report.")
            };
        }

        private static List<MeetingVoteData> BuildVotes()
        {
            return new List<MeetingVoteData>
            {
                new MeetingVoteData("scene_bridge_vote_001", "registered_1", "culprit_01", "Reason 1."),
                new MeetingVoteData("scene_bridge_vote_002", "registered_2", "culprit_01", "Reason 2.")
            };
        }

        private static HashSet<string> BuildCulprits()
        {
            return new HashSet<string> { "culprit_01" };
        }
    }
}
