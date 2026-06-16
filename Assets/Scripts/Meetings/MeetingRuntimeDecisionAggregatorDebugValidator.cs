using System.Collections.Generic;
using OFIS.Rooms;
using OFIS.Sabotage;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingRuntimeDecisionAggregatorDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateAggregator();
        }

        [ContextMenu("Validate Meeting Runtime Decision Aggregator")]
        public void ValidateAggregator()
        {
            ValidateNonMeetingPhaseReturnsNone();
            ValidateMissingPenaltyIsReturnedFirstAndOnlyOnce();
            ValidateEmptyNormalMeetingAutoCloseDecision();
            ValidateEmptyFinalMeetingWinBranchDecision();
            ValidateMeetingEndPipelineDecision();
            ValidateNormalMeetingContinuesBeforeTerminalDecision();
        }

        private void ValidateNonMeetingPhaseReturnsNone()
        {
            MeetingRuntimeDecisionAggregatorService aggregator =
                new MeetingRuntimeDecisionAggregatorService();

            MeetingRuntimeDecisionResult result = aggregator.Resolve(
                BuildInput(
                    MeetingRuntimePhaseType.Office,
                    120f,
                    0f,
                    1f,
                    100,
                    BuildAttendance(1, 0, 0),
                    BuildReports(),
                    BuildVotes(),
                    BuildCulprits()));

            bool passed = result.DecisionType == MeetingRuntimeDecisionType.None
                && !result.IsTerminalDecision
                && !result.ShouldContinueMeeting;

            LogResult("NonMeetingPhaseReturnsNone", passed, result);
        }

        private void ValidateMissingPenaltyIsReturnedFirstAndOnlyOnce()
        {
            MeetingRuntimeDecisionAggregatorService aggregator =
                new MeetingRuntimeDecisionAggregatorService();

            MeetingRuntimeDecisionInput input = BuildInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                10f,
                1f,
                100,
                BuildAttendance(2, 2, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits());

            MeetingRuntimeDecisionResult first = aggregator.Resolve(input);
            MeetingRuntimeDecisionResult second = aggregator.Resolve(input);

            bool passed = first.DecisionType == MeetingRuntimeDecisionType.ApplyMissingPlayerPenalty
                && first.ShouldApplyHealthPenalty
                && first.HasHealthPenaltyBridgeResult
                && first.HealthPenaltyBridgeResult.CompanyHealthAfter == 90
                && second.DecisionType == MeetingRuntimeDecisionType.ContinueMeeting
                && !second.ShouldApplyHealthPenalty;

            LogResult("MissingPenaltyIsReturnedFirstAndOnlyOnce", passed, second);
        }

        private void ValidateEmptyNormalMeetingAutoCloseDecision()
        {
            MeetingRuntimeDecisionAggregatorConfig config =
                new MeetingRuntimeDecisionAggregatorConfig(
                    evaluateMissingPlayerPenalty: false,
                    evaluateEmptyStateResolution: true,
                    evaluateMeetingEndPipeline: false,
                    joinLockThresholdSeconds: 20f);

            MeetingRuntimeDecisionAggregatorService aggregator =
                new MeetingRuntimeDecisionAggregatorService(
                    config,
                    new MeetingPhaseRuntimeHookService(),
                    new MeetingCompanyHealthPenaltyBridgeService(),
                    new MeetingEmptyStateRuntimeTracker());

            MeetingAttendanceRegistrationResult emptyAttendance = BuildAttendance(0, 0, 0);

            aggregator.Resolve(BuildInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                1f,
                5f,
                100,
                emptyAttendance,
                BuildReports(),
                BuildVotes(),
                BuildCulprits()));

            MeetingRuntimeDecisionResult result = aggregator.Resolve(BuildInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                6f,
                5f,
                100,
                emptyAttendance,
                BuildReports(),
                BuildVotes(),
                BuildCulprits()));

            bool passed = result.DecisionType == MeetingRuntimeDecisionType.AutoCloseMeeting
                && result.ShouldCloseMeeting
                && result.IsTerminalDecision
                && result.HasEmptyStateResolutionResult;

            LogResult("EmptyNormalMeetingAutoCloseDecision", passed, result);
        }

        private void ValidateEmptyFinalMeetingWinBranchDecision()
        {
            MeetingRuntimeDecisionAggregatorConfig config =
                new MeetingRuntimeDecisionAggregatorConfig(
                    evaluateMissingPlayerPenalty: false,
                    evaluateEmptyStateResolution: true,
                    evaluateMeetingEndPipeline: false,
                    joinLockThresholdSeconds: 20f);

            MeetingRuntimeDecisionAggregatorService aggregator =
                new MeetingRuntimeDecisionAggregatorService(
                    config,
                    new MeetingPhaseRuntimeHookService(),
                    new MeetingCompanyHealthPenaltyBridgeService(),
                    new MeetingEmptyStateRuntimeTracker());

            MeetingRuntimeDecisionResult result = aggregator.Resolve(BuildInput(
                MeetingRuntimePhaseType.FinalMeeting,
                120f,
                1f,
                1f,
                100,
                BuildAttendance(0, 0, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits()));

            bool passed = result.DecisionType == MeetingRuntimeDecisionType.ResolveFinalMeetingWinBranch
                && result.ShouldResolveWinBranch
                && result.IsTerminalDecision
                && result.HasEmptyStateResolutionResult;

            LogResult("EmptyFinalMeetingWinBranchDecision", passed, result);
        }

        private void ValidateMeetingEndPipelineDecision()
        {
            MeetingRuntimeDecisionAggregatorConfig config =
                new MeetingRuntimeDecisionAggregatorConfig(
                    evaluateMissingPlayerPenalty: false,
                    evaluateEmptyStateResolution: false,
                    evaluateMeetingEndPipeline: true,
                    joinLockThresholdSeconds: 20f);

            MeetingRuntimeDecisionAggregatorService aggregator =
                new MeetingRuntimeDecisionAggregatorService(
                    config,
                    new MeetingPhaseRuntimeHookService(),
                    new MeetingCompanyHealthPenaltyBridgeService(),
                    new MeetingEmptyStateRuntimeTracker());

            MeetingRuntimeDecisionResult result = aggregator.Resolve(BuildInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                120f,
                1f,
                100,
                BuildAttendance(2, 0, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits()));

            bool passed = result.DecisionType == MeetingRuntimeDecisionType.RunMeetingEndPipeline
                && result.ShouldRunMeetingEndPipeline
                && result.IsTerminalDecision
                && result.HasRuntimeHookResult
                && result.RuntimeHookResult.HasPipelineResult;

            LogResult("MeetingEndPipelineDecision", passed, result);
        }

        private void ValidateNormalMeetingContinuesBeforeTerminalDecision()
        {
            MeetingRuntimeDecisionAggregatorConfig config =
                new MeetingRuntimeDecisionAggregatorConfig(
                    evaluateMissingPlayerPenalty: true,
                    evaluateEmptyStateResolution: true,
                    evaluateMeetingEndPipeline: true,
                    joinLockThresholdSeconds: 20f);

            MeetingRuntimeDecisionAggregatorService aggregator =
                new MeetingRuntimeDecisionAggregatorService(
                    config,
                    new MeetingPhaseRuntimeHookService(),
                    new MeetingCompanyHealthPenaltyBridgeService(),
                    new MeetingEmptyStateRuntimeTracker());

            MeetingRuntimeDecisionResult result = aggregator.Resolve(BuildInput(
                MeetingRuntimePhaseType.Meeting,
                120f,
                30f,
                1f,
                100,
                BuildAttendance(2, 0, 0),
                BuildReports(),
                BuildVotes(),
                BuildCulprits()));

            bool passed = result.DecisionType == MeetingRuntimeDecisionType.ContinueMeeting
                && result.ShouldContinueMeeting
                && !result.IsTerminalDecision
                && !result.ShouldApplyHealthPenalty
                && !result.ShouldCloseMeeting
                && !result.ShouldResolveWinBranch
                && !result.ShouldRunMeetingEndPipeline;

            LogResult("NormalMeetingContinuesBeforeTerminalDecision", passed, result);
        }

        private static MeetingRuntimeDecisionInput BuildInput(
            MeetingRuntimePhaseType phaseType,
            float phaseDurationSeconds,
            float phaseElapsedSeconds,
            float deltaTimeSeconds,
            int companyHealth,
            MeetingAttendanceRegistrationResult attendanceResult,
            IReadOnlyList<MeetingReportData> reports,
            IReadOnlyList<MeetingVoteData> votes,
            IReadOnlyCollection<string> culpritPlayerIds)
        {
            return new MeetingRuntimeDecisionInput(
                phaseType,
                phaseDurationSeconds,
                phaseElapsedSeconds,
                deltaTimeSeconds,
                companyHealth,
                attendanceResult,
                reports,
                votes,
                culpritPlayerIds);
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
                "Aggregator debug attendance fixture.");
        }

        private static List<MeetingReportData> BuildReports()
        {
            return new List<MeetingReportData>
            {
                new MeetingReportData(
                    "aggregator_report_001",
                    MeetingReportType.Suspicion,
                    "registered_1",
                    "culprit_01",
                    OfficeRoomType.MeetingRoom,
                    0,
                    0,
                    SabotageObjectiveState.None,
                    "Aggregator suspicion report.")
            };
        }

        private static List<MeetingVoteData> BuildVotes()
        {
            return new List<MeetingVoteData>
            {
                new MeetingVoteData("aggregator_vote_001", "registered_1", "culprit_01", "Reason 1."),
                new MeetingVoteData("aggregator_vote_002", "registered_2", "culprit_01", "Reason 2.")
            };
        }

        private static HashSet<string> BuildCulprits()
        {
            return new HashSet<string> { "culprit_01" };
        }

        private static void LogResult(string testName, bool passed, MeetingRuntimeDecisionResult result)
        {
            if (passed)
                Debug.Log($"[MeetingRuntimeDecisionAggregatorValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingRuntimeDecisionAggregatorValidator] FAIL {testName}: {result}");
        }
    }
}
