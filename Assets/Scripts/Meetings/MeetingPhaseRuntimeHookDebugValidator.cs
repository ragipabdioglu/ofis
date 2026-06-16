using System.Collections.Generic;
using OFIS.Rooms;
using OFIS.Sabotage;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingPhaseRuntimeHookDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private MeetingSummaryHudStub hudStub;

        private MeetingPhaseRuntimeHookService _runtimeHookService;

        private void Awake()
        {
            _runtimeHookService = new MeetingPhaseRuntimeHookService();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateRuntimeHook();
        }

        [ContextMenu("Validate Meeting Phase Runtime Hook")]
        public void ValidateRuntimeHook()
        {
            ValidateOfficePhaseDoesNotTriggerPipeline();
            ValidateMeetingPhaseBeforeEndDoesNotTriggerPipeline();
            ValidateJoinLockState();
            ValidateMeetingEndTriggersPipelineOnce();
            ValidateFinalMeetingEndTriggersPipeline();
        }

        private void ValidateOfficePhaseDoesNotTriggerPipeline()
        {
            _runtimeHookService.Reset();

            MeetingPhaseRuntimeHookResult result = _runtimeHookService.Tick(
                MeetingRuntimePhaseType.Office,
                240f,
                240f,
                new List<MeetingReportData>(),
                new List<MeetingVoteData>(),
                BuildCulprits("culprit_01"));

            bool passed = !result.HasPipelineResult
                && !result.State.HasActivePhase
                && !result.State.IsMeetingPhase
                && !_runtimeHookService.PipelineTriggered;

            LogResult("OfficePhaseDoesNotTriggerPipeline", passed, result);
        }

        private void ValidateMeetingPhaseBeforeEndDoesNotTriggerPipeline()
        {
            _runtimeHookService.Reset();

            MeetingPhaseRuntimeHookResult result = _runtimeHookService.Tick(
                MeetingRuntimePhaseType.Meeting,
                120f,
                60f,
                BuildSampleReports(1, "culprit_01"),
                BuildCorrectVotes(),
                BuildCulprits("culprit_01"));

            bool passed = !result.HasPipelineResult
                && result.State.HasActivePhase
                && result.State.IsMeetingPhase
                && !result.State.HasEnded
                && !_runtimeHookService.PipelineTriggered;

            LogResult("MeetingPhaseBeforeEndDoesNotTriggerPipeline", passed, result);
        }

        private void ValidateJoinLockState()
        {
            _runtimeHookService.Reset();

            MeetingPhaseRuntimeHookResult result = _runtimeHookService.Tick(
                MeetingRuntimePhaseType.Meeting,
                120f,
                105f,
                BuildSampleReports(1, "culprit_01"),
                BuildCorrectVotes(),
                BuildCulprits("culprit_01"));

            bool passed = !result.HasPipelineResult
                && result.State.IsJoinLocked
                && !result.State.HasEnded
                && !_runtimeHookService.PipelineTriggered;

            LogResult("JoinLockState", passed, result);
        }

        private void ValidateMeetingEndTriggersPipelineOnce()
        {
            _runtimeHookService.Reset();

            MeetingPhaseRuntimeHookResult firstResult = _runtimeHookService.Tick(
                MeetingRuntimePhaseType.Meeting,
                120f,
                120f,
                BuildSampleReports(2, "culprit_01"),
                BuildCorrectVotes(),
                BuildCulprits("culprit_01"));

            MeetingPhaseRuntimeHookResult secondResult = _runtimeHookService.Tick(
                MeetingRuntimePhaseType.Meeting,
                120f,
                130f,
                BuildSampleReports(2, "culprit_01"),
                BuildCorrectVotes(),
                BuildCulprits("culprit_01"));

            bool passed = firstResult.HasPipelineResult
                && firstResult.State.HasEnded
                && firstResult.State.PipelineTriggered
                && firstResult.PipelineResult.IsResolved
                && !secondResult.HasPipelineResult
                && secondResult.State.PipelineTriggered
                && _runtimeHookService.PipelineTriggered;

            if (firstResult.HasPipelineResult)
                PushHudState(firstResult.PipelineResult.SummaryUiState);

            LogResult("MeetingEndTriggersPipelineOnce", passed, firstResult);
        }

        private void ValidateFinalMeetingEndTriggersPipeline()
        {
            _runtimeHookService.Reset();

            MeetingPhaseRuntimeHookResult result = _runtimeHookService.Tick(
                MeetingRuntimePhaseType.FinalMeeting,
                120f,
                120f,
                BuildSampleReports(2, "culprit_01"),
                BuildCorrectVotes(),
                BuildCulprits("culprit_01"));

            bool passed = result.HasPipelineResult
                && result.State.HasEnded
                && result.State.PipelineTriggered
                && result.PipelineResult.IsResolved
                && _runtimeHookService.PipelineTriggered;

            LogResult("FinalMeetingEndTriggersPipeline", passed, result);
        }

        private static List<MeetingReportData> BuildSampleReports(int count, string targetPlayerId)
        {
            List<MeetingReportData> reports = new List<MeetingReportData>();

            for (int i = 0; i < count; i++)
            {
                reports.Add(new MeetingReportData(
                    $"runtime_report_{i + 1}",
                    MeetingReportType.Suspicion,
                    $"player_{i + 1}",
                    targetPlayerId,
                    OfficeRoomType.MeetingRoom,
                    0,
                    0,
                    SabotageObjectiveState.None,
                    "Runtime hook suspicion report."));
            }

            return reports;
        }

        private static List<MeetingVoteData> BuildCorrectVotes()
        {
            return new List<MeetingVoteData>
            {
                new MeetingVoteData("runtime_vote_001", "player_01", "culprit_01", "Reason 1."),
                new MeetingVoteData("runtime_vote_002", "player_02", "culprit_01", "Reason 2."),
                new MeetingVoteData("runtime_vote_003", "player_03", "player_04", "Reason 3.")
            };
        }

        private static HashSet<string> BuildCulprits(params string[] culpritIds)
        {
            return new HashSet<string>(culpritIds);
        }

        private void PushHudState(MeetingSummaryUiState state)
        {
            if (hudStub != null)
                hudStub.SetState(state);
        }

        private static void LogResult(string testName, bool passed, MeetingPhaseRuntimeHookResult result)
        {
            if (passed)
                Debug.Log($"[MeetingPhaseRuntimeHookValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingPhaseRuntimeHookValidator] FAIL {testName}: {result}");
        }
    }
}
