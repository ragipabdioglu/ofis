using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingRuntimeSceneBridgeDebugHarness : MonoBehaviour
    {
        [Header("Debug Scenario")]
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private MeetingRuntimeDebugScenarioType scenarioType = MeetingRuntimeDebugScenarioType.NormalMeetingContinue;
        [SerializeField] private int companyHealth = 100;
        [SerializeField] private float deltaTimeSeconds = 1f;
        [SerializeField] private bool logEveryUpdate = false;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastDecisionType;
        [SerializeField] private string lastPhaseType;
        [SerializeField] private string lastSummary;
        [SerializeField] private bool lastTerminalDecision;
        [SerializeField] private bool lastApplyHealthPenalty;
        [SerializeField] private bool lastCloseMeeting;
        [SerializeField] private bool lastResolveWinBranch;
        [SerializeField] private bool lastRunMeetingEndPipeline;
        [SerializeField] private int lastCompanyHealthBefore;
        [SerializeField] private int lastCompanyHealthAfter;

        private MeetingRuntimeSceneBridgeService _sceneBridgeService;

        private void Awake()
        {
            _sceneBridgeService = new MeetingRuntimeSceneBridgeService();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateSceneBridge();
        }

        private void Update()
        {
            if (!logEveryUpdate)
                return;

            ResolveSelectedScenario();
        }

        [ContextMenu("Resolve Selected Scenario")]
        public void ResolveSelectedScenario()
        {
            MeetingRuntimeDecisionInput input = MeetingRuntimeDebugScenarioFactory.CreateInput(
                scenarioType,
                deltaTimeSeconds,
                companyHealth);

            MeetingRuntimeSceneBridgeState state = _sceneBridgeService.ResolveSceneState(
                scenarioType,
                input);

            ApplyDebugOutput(state);
            Debug.Log($"[MeetingRuntimeSceneBridge] {state}");
        }

        [ContextMenu("Reset Bridge State")]
        public void ResetBridgeState()
        {
            _sceneBridgeService.Reset();
            lastDecisionType = string.Empty;
            lastPhaseType = string.Empty;
            lastSummary = "Bridge reset.";
            lastTerminalDecision = false;
            lastApplyHealthPenalty = false;
            lastCloseMeeting = false;
            lastResolveWinBranch = false;
            lastRunMeetingEndPipeline = false;
            lastCompanyHealthBefore = companyHealth;
            lastCompanyHealthAfter = companyHealth;

            Debug.Log("[MeetingRuntimeSceneBridge] Bridge state reset.");
        }

        [ContextMenu("Validate Scene Bridge")]
        public void ValidateSceneBridge()
        {
            ValidateNormalContinueScenario();
            ValidateMissingPenaltyScenario();
            ValidateEmptyNormalAutoCloseScenario();
            ValidateEmptyFinalWinBranchScenario();
            ValidateMeetingEndPipelineScenario();
        }

        private void ValidateNormalContinueScenario()
        {
            _sceneBridgeService.Reset();

            MeetingRuntimeSceneBridgeState state = ResolveScenario(
                MeetingRuntimeDebugScenarioType.NormalMeetingContinue,
                deltaTimeSeconds: 1f,
                health: 100);

            bool passed = state.DecisionType == MeetingRuntimeDecisionType.ContinueMeeting
                && !state.IsTerminalDecision
                && !state.ShouldApplyHealthPenalty
                && !state.ShouldCloseMeeting
                && !state.ShouldResolveWinBranch
                && !state.ShouldRunMeetingEndPipeline;

            LogResult("NormalContinueScenario", passed, state);
        }

        private void ValidateMissingPenaltyScenario()
        {
            _sceneBridgeService.Reset();

            MeetingRuntimeSceneBridgeState state = ResolveScenario(
                MeetingRuntimeDebugScenarioType.MissingPenalty,
                deltaTimeSeconds: 1f,
                health: 100);

            bool passed = state.DecisionType == MeetingRuntimeDecisionType.ApplyMissingPlayerPenalty
                && state.ShouldApplyHealthPenalty
                && state.CompanyHealthBefore == 100
                && state.CompanyHealthAfter == 90
                && !state.IsTerminalDecision;

            LogResult("MissingPenaltyScenario", passed, state);
        }

        private void ValidateEmptyNormalAutoCloseScenario()
        {
            _sceneBridgeService.Reset();

            ResolveScenario(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                deltaTimeSeconds: 5f,
                health: 100);

            MeetingRuntimeSceneBridgeState state = ResolveScenario(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                deltaTimeSeconds: 5f,
                health: 100);

            bool passed = state.DecisionType == MeetingRuntimeDecisionType.AutoCloseMeeting
                && state.ShouldCloseMeeting
                && state.IsTerminalDecision;

            LogResult("EmptyNormalAutoCloseScenario", passed, state);
        }

        private void ValidateEmptyFinalWinBranchScenario()
        {
            _sceneBridgeService.Reset();

            MeetingRuntimeSceneBridgeState state = ResolveScenario(
                MeetingRuntimeDebugScenarioType.EmptyFinalWinBranch,
                deltaTimeSeconds: 1f,
                health: 100);

            bool passed = state.DecisionType == MeetingRuntimeDecisionType.ResolveFinalMeetingWinBranch
                && state.ShouldResolveWinBranch
                && state.IsTerminalDecision;

            LogResult("EmptyFinalWinBranchScenario", passed, state);
        }

        private void ValidateMeetingEndPipelineScenario()
        {
            _sceneBridgeService.Reset();

            MeetingRuntimeSceneBridgeState state = ResolveScenario(
                MeetingRuntimeDebugScenarioType.MeetingEndPipeline,
                deltaTimeSeconds: 1f,
                health: 100);

            bool passed = state.DecisionType == MeetingRuntimeDecisionType.RunMeetingEndPipeline
                && state.ShouldRunMeetingEndPipeline
                && state.IsTerminalDecision;

            LogResult("MeetingEndPipelineScenario", passed, state);
        }

        private MeetingRuntimeSceneBridgeState ResolveScenario(
            MeetingRuntimeDebugScenarioType scenario,
            float deltaTimeSeconds,
            int health)
        {
            MeetingRuntimeDecisionInput input = MeetingRuntimeDebugScenarioFactory.CreateInput(
                scenario,
                deltaTimeSeconds,
                health);

            MeetingRuntimeSceneBridgeState state = _sceneBridgeService.ResolveSceneState(
                scenario,
                input);

            ApplyDebugOutput(state);
            return state;
        }

        private void ApplyDebugOutput(MeetingRuntimeSceneBridgeState state)
        {
            lastDecisionType = state.DecisionType.ToString();
            lastPhaseType = state.PhaseType.ToString();
            lastSummary = state.Summary;
            lastTerminalDecision = state.IsTerminalDecision;
            lastApplyHealthPenalty = state.ShouldApplyHealthPenalty;
            lastCloseMeeting = state.ShouldCloseMeeting;
            lastResolveWinBranch = state.ShouldResolveWinBranch;
            lastRunMeetingEndPipeline = state.ShouldRunMeetingEndPipeline;
            lastCompanyHealthBefore = state.CompanyHealthBefore;
            lastCompanyHealthAfter = state.CompanyHealthAfter;
        }

        private static void LogResult(string testName, bool passed, MeetingRuntimeSceneBridgeState state)
        {
            if (passed)
                Debug.Log($"[MeetingRuntimeSceneBridgeValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[MeetingRuntimeSceneBridgeValidator] FAIL {testName}: {state}");
        }
    }
}
