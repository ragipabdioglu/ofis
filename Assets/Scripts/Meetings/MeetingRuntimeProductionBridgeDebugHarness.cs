using OFIS.MatchFlow.States;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingRuntimeProductionBridgeDebugHarness : MonoBehaviour
    {
        [Header("Debug Scenario")]
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private MeetingRuntimeDebugScenarioType scenarioType = MeetingRuntimeDebugScenarioType.NormalMeetingContinue;
        [SerializeField] private MatchState matchState = MatchState.Meeting1;
        [SerializeField] private int companyHealth = 100;
        [SerializeField] private float deltaTimeSeconds = 1f;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastActionType;
        [SerializeField] private string lastRuntimeDecisionType;
        [SerializeField] private string lastPhaseType;
        [SerializeField] private string lastSummary;
        [SerializeField] private bool lastHasCommand;
        [SerializeField] private bool lastTerminalCommand;
        [SerializeField] private bool lastApplyCompanyHealthDelta;
        [SerializeField] private bool lastCloseMeeting;
        [SerializeField] private bool lastResolveWinBranch;
        [SerializeField] private bool lastRunMeetingEndPipeline;
        [SerializeField] private int lastCompanyHealthDelta;

        private MeetingRuntimeProductionBridgeService _productionBridgeService;

        private void Awake()
        {
            _productionBridgeService = new MeetingRuntimeProductionBridgeService();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateProductionBridge();
        }

        [ContextMenu("Resolve Selected Scenario")]
        public void ResolveSelectedScenario()
        {
            _productionBridgeService.Reset();

            MeetingProductionBridgeResult result = ResolveScenario(
                scenarioType,
                matchState,
                deltaTimeSeconds,
                companyHealth);

            Debug.Log($"[MeetingRuntimeProductionBridge] {result}");
        }

        [ContextMenu("Reset Production Bridge")]
        public void ResetProductionBridge()
        {
            _productionBridgeService.Reset();
            lastActionType = string.Empty;
            lastRuntimeDecisionType = string.Empty;
            lastPhaseType = string.Empty;
            lastSummary = "Production bridge reset.";
            lastHasCommand = false;
            lastTerminalCommand = false;
            lastApplyCompanyHealthDelta = false;
            lastCloseMeeting = false;
            lastResolveWinBranch = false;
            lastRunMeetingEndPipeline = false;
            lastCompanyHealthDelta = 0;

            Debug.Log("[MeetingRuntimeProductionBridge] Bridge state reset.");
        }

        [ContextMenu("Validate Production Bridge")]
        public void ValidateProductionBridge()
        {
            ValidateMatchStateAdapter();
            ValidateContinueCommand();
            ValidateMissingPenaltyCommand();
            ValidateAutoCloseCommand();
            ValidateFinalWinBranchCommand();
            ValidateMeetingEndPipelineCommand();
        }

        private void ValidateMatchStateAdapter()
        {
            bool passed = MeetingMatchFlowPhaseAdapter.FromMatchState(MatchState.Meeting1) == MeetingRuntimePhaseType.Meeting
                && MeetingMatchFlowPhaseAdapter.FromMatchState(MatchState.Meeting2) == MeetingRuntimePhaseType.Meeting
                && MeetingMatchFlowPhaseAdapter.FromMatchState(MatchState.FinalMeeting) == MeetingRuntimePhaseType.FinalMeeting
                && MeetingMatchFlowPhaseAdapter.FromMatchState(MatchState.OfficePhase1) == MeetingRuntimePhaseType.Office
                && MeetingMatchFlowPhaseAdapter.FromMatchState(MatchState.ResolvingMatch) == MeetingRuntimePhaseType.None;

            LogSimpleResult("MatchStateAdapter", passed);
        }

        private void ValidateContinueCommand()
        {
            _productionBridgeService.Reset();

            MeetingProductionBridgeResult result = ResolveScenario(
                MeetingRuntimeDebugScenarioType.NormalMeetingContinue,
                MatchState.Meeting1,
                1f,
                100);

            bool passed = result.Command.ActionType == MeetingProductionBridgeActionType.ContinueMeeting
                && result.Command.ShouldContinueMeeting
                && !result.IsTerminalCommand
                && result.HasCommand;

            LogResult("ContinueCommand", passed, result);
        }

        private void ValidateMissingPenaltyCommand()
        {
            _productionBridgeService.Reset();

            MeetingProductionBridgeResult result = ResolveScenario(
                MeetingRuntimeDebugScenarioType.MissingPenalty,
                MatchState.Meeting1,
                1f,
                100);

            bool passed = result.Command.ActionType == MeetingProductionBridgeActionType.ApplyCompanyHealthPenalty
                && result.Command.ShouldApplyCompanyHealthDelta
                && result.Command.CompanyHealthDelta == -10
                && !result.IsTerminalCommand;

            LogResult("MissingPenaltyCommand", passed, result);
        }

        private void ValidateAutoCloseCommand()
        {
            _productionBridgeService.Reset();

            ResolveScenario(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100);

            MeetingProductionBridgeResult result = ResolveScenario(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100);

            bool passed = result.Command.ActionType == MeetingProductionBridgeActionType.CloseMeeting
                && result.Command.ShouldCloseMeeting
                && result.IsTerminalCommand;

            LogResult("AutoCloseCommand", passed, result);
        }

        private void ValidateFinalWinBranchCommand()
        {
            _productionBridgeService.Reset();

            MeetingProductionBridgeResult result = ResolveScenario(
                MeetingRuntimeDebugScenarioType.EmptyFinalWinBranch,
                MatchState.FinalMeeting,
                1f,
                100);

            bool passed = result.Command.ActionType == MeetingProductionBridgeActionType.ResolveFinalMeetingWinBranch
                && result.Command.ShouldResolveWinBranch
                && result.IsTerminalCommand;

            LogResult("FinalWinBranchCommand", passed, result);
        }

        private void ValidateMeetingEndPipelineCommand()
        {
            _productionBridgeService.Reset();

            MeetingProductionBridgeResult result = ResolveScenario(
                MeetingRuntimeDebugScenarioType.MeetingEndPipeline,
                MatchState.Meeting2,
                1f,
                100);

            bool passed = result.Command.ActionType == MeetingProductionBridgeActionType.RunMeetingEndPipeline
                && result.Command.ShouldRunMeetingEndPipeline
                && result.Command.HasSummaryUiState
                && result.IsTerminalCommand;

            LogResult("MeetingEndPipelineCommand", passed, result);
        }

        private MeetingProductionBridgeResult ResolveScenario(
            MeetingRuntimeDebugScenarioType scenario,
            MatchState sourceMatchState,
            float deltaSeconds,
            int health)
        {
            MeetingRuntimeDecisionInput scenarioInput =
                MeetingRuntimeDebugScenarioFactory.CreateInput(scenario, deltaSeconds, health);

            MeetingRuntimeDecisionInput productionInput = new MeetingRuntimeDecisionInput(
                MeetingMatchFlowPhaseAdapter.FromMatchState(sourceMatchState),
                scenarioInput.PhaseDurationSeconds,
                scenarioInput.PhaseElapsedSeconds,
                scenarioInput.DeltaTimeSeconds,
                scenarioInput.CurrentCompanyHealth,
                scenarioInput.AttendanceResult,
                scenarioInput.Reports,
                scenarioInput.Votes,
                scenarioInput.CulpritPlayerIds);

            MeetingProductionBridgeResult result =
                _productionBridgeService.Resolve(productionInput);

            ApplyDebugOutput(result);
            return result;
        }

        private void ApplyDebugOutput(MeetingProductionBridgeResult result)
        {
            lastActionType = result.Command.ActionType.ToString();
            lastRuntimeDecisionType = result.RuntimeDecisionResult.DecisionType.ToString();
            lastPhaseType = result.RuntimeDecisionResult.PhaseType.ToString();
            lastSummary = result.Message;
            lastHasCommand = result.HasCommand;
            lastTerminalCommand = result.IsTerminalCommand;
            lastApplyCompanyHealthDelta = result.Command.ShouldApplyCompanyHealthDelta;
            lastCloseMeeting = result.Command.ShouldCloseMeeting;
            lastResolveWinBranch = result.Command.ShouldResolveWinBranch;
            lastRunMeetingEndPipeline = result.Command.ShouldRunMeetingEndPipeline;
            lastCompanyHealthDelta = result.Command.CompanyHealthDelta;
        }

        private static void LogSimpleResult(string testName, bool passed)
        {
            if (passed)
                Debug.Log($"[MeetingRuntimeProductionBridgeValidator] PASS {testName}");
            else
                Debug.LogError($"[MeetingRuntimeProductionBridgeValidator] FAIL {testName}");
        }

        private static void LogResult(
            string testName,
            bool passed,
            MeetingProductionBridgeResult result)
        {
            if (passed)
                Debug.Log($"[MeetingRuntimeProductionBridgeValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingRuntimeProductionBridgeValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
