using OFIS.Company;
using OFIS.MatchFlow.States;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingProductionApplyDebugHarness : MonoBehaviour
    {
        [Header("Debug Scenario")]
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private MeetingRuntimeDebugScenarioType scenarioType = MeetingRuntimeDebugScenarioType.MissingPenalty;
        [SerializeField] private MatchState matchState = MatchState.Meeting1;
        [SerializeField] private CompanyHealthService companyHealthService;
        [SerializeField] private int fallbackCompanyHealth = 100;
        [SerializeField] private float deltaTimeSeconds = 1f;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastActionType;
        [SerializeField] private string lastApplyMessage;
        [SerializeField] private bool lastAppliedCompanyHealthDelta;
        [SerializeField] private bool lastRequestedCloseMeeting;
        [SerializeField] private bool lastRequestedWinBranchResolution;
        [SerializeField] private bool lastRequestedMeetingEndPipeline;
        [SerializeField] private bool lastHasSummaryUiState;
        [SerializeField] private int lastCompanyHealthBefore;
        [SerializeField] private int lastCompanyHealthAfter;

        private MeetingRuntimeProductionBridgeService _bridgeService;
        private MeetingProductionApplyService _applyService;

        private void Awake()
        {
            _bridgeService = new MeetingRuntimeProductionBridgeService();
            _applyService = new MeetingProductionApplyService();

            if (companyHealthService == null)
                companyHealthService = FindFirstObjectByType<CompanyHealthService>();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateProductionApply();
        }

        [ContextMenu("Resolve And Apply Selected Scenario")]
        public void ResolveAndApplySelectedScenario()
        {
            _bridgeService.Reset();

            MeetingProductionApplyResult result = ResolveAndApply(
                scenarioType,
                matchState,
                deltaTimeSeconds,
                GetCurrentHealth(),
                companyHealthService);

            Debug.Log($"[MeetingProductionApply] {result}");
        }

        [ContextMenu("Validate Production Apply")]
        public void ValidateProductionApply()
        {
            ValidateApplyCompanyHealthDelta();
            ValidateContinueDoesNotApplyHooks();
            ValidateAutoCloseHook();
            ValidateFinalWinBranchHook();
            ValidateMeetingEndPipelineHook();
        }

        private void ValidateApplyCompanyHealthDelta()
        {
            _bridgeService.Reset();
            CompanyHealthService temporaryHealthService = CreateTemporaryHealthService();

            MeetingProductionApplyResult result = ResolveAndApply(
                MeetingRuntimeDebugScenarioType.MissingPenalty,
                MatchState.Meeting1,
                1f,
                temporaryHealthService.CurrentHealth,
                temporaryHealthService);

            bool passed = result.AppliedCompanyHealthDelta
                && result.CompanyHealthBefore == 100
                && result.CompanyHealthAfter == 90
                && temporaryHealthService.CurrentHealth == 90;

            Destroy(temporaryHealthService.gameObject);
            LogResult("ApplyCompanyHealthDelta", passed, result);
        }

        private void ValidateContinueDoesNotApplyHooks()
        {
            _bridgeService.Reset();

            MeetingProductionApplyResult result = ResolveAndApply(
                MeetingRuntimeDebugScenarioType.NormalMeetingContinue,
                MatchState.Meeting1,
                1f,
                100,
                null);

            bool passed = !result.AppliedCompanyHealthDelta
                && !result.RequestedCloseMeeting
                && !result.RequestedWinBranchResolution
                && !result.RequestedMeetingEndPipeline;

            LogResult("ContinueDoesNotApplyHooks", passed, result);
        }

        private void ValidateAutoCloseHook()
        {
            _bridgeService.Reset();

            ResolveAndApply(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100,
                null);

            MeetingProductionApplyResult result = ResolveAndApply(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100,
                null);

            bool passed = result.RequestedCloseMeeting
                && !result.RequestedWinBranchResolution
                && !result.RequestedMeetingEndPipeline;

            LogResult("AutoCloseHook", passed, result);
        }

        private void ValidateFinalWinBranchHook()
        {
            _bridgeService.Reset();

            MeetingProductionApplyResult result = ResolveAndApply(
                MeetingRuntimeDebugScenarioType.EmptyFinalWinBranch,
                MatchState.FinalMeeting,
                1f,
                100,
                null);

            bool passed = result.RequestedWinBranchResolution
                && !result.RequestedCloseMeeting
                && !result.RequestedMeetingEndPipeline;

            LogResult("FinalWinBranchHook", passed, result);
        }

        private void ValidateMeetingEndPipelineHook()
        {
            _bridgeService.Reset();

            MeetingProductionApplyResult result = ResolveAndApply(
                MeetingRuntimeDebugScenarioType.MeetingEndPipeline,
                MatchState.Meeting2,
                1f,
                100,
                null);

            bool passed = result.RequestedMeetingEndPipeline
                && result.HasSummaryUiState
                && !result.RequestedCloseMeeting
                && !result.RequestedWinBranchResolution;

            LogResult("MeetingEndPipelineHook", passed, result);
        }

        private MeetingProductionApplyResult ResolveAndApply(
            MeetingRuntimeDebugScenarioType scenario,
            MatchState sourceMatchState,
            float deltaSeconds,
            int currentHealth,
            CompanyHealthService healthService)
        {
            MeetingRuntimeDecisionInput scenarioInput =
                MeetingRuntimeDebugScenarioFactory.CreateInput(scenario, deltaSeconds, currentHealth);

            MeetingRuntimeDecisionInput productionInput = new MeetingRuntimeDecisionInput(
                MeetingMatchFlowPhaseAdapter.FromMatchState(sourceMatchState),
                scenarioInput.PhaseDurationSeconds,
                scenarioInput.PhaseElapsedSeconds,
                scenarioInput.DeltaTimeSeconds,
                currentHealth,
                scenarioInput.AttendanceResult,
                scenarioInput.Reports,
                scenarioInput.Votes,
                scenarioInput.CulpritPlayerIds);

            MeetingProductionBridgeResult bridgeResult =
                _bridgeService.Resolve(productionInput);

            MeetingProductionApplyResult applyResult =
                _applyService.Apply(bridgeResult.Command, healthService);

            ApplyDebugOutput(applyResult);
            return applyResult;
        }

        private int GetCurrentHealth()
        {
            if (companyHealthService != null)
                return companyHealthService.CurrentHealth;

            return fallbackCompanyHealth < 0 ? 0 : fallbackCompanyHealth;
        }

        private static CompanyHealthService CreateTemporaryHealthService()
        {
            GameObject temporaryObject = new GameObject("MeetingProductionApplyTempCompanyHealth");
            return temporaryObject.AddComponent<CompanyHealthService>();
        }

        private void ApplyDebugOutput(MeetingProductionApplyResult result)
        {
            lastActionType = result.Command.ActionType.ToString();
            lastApplyMessage = result.Message;
            lastAppliedCompanyHealthDelta = result.AppliedCompanyHealthDelta;
            lastRequestedCloseMeeting = result.RequestedCloseMeeting;
            lastRequestedWinBranchResolution = result.RequestedWinBranchResolution;
            lastRequestedMeetingEndPipeline = result.RequestedMeetingEndPipeline;
            lastHasSummaryUiState = result.HasSummaryUiState;
            lastCompanyHealthBefore = result.CompanyHealthBefore;
            lastCompanyHealthAfter = result.CompanyHealthAfter;
        }

        private static void LogResult(
            string testName,
            bool passed,
            MeetingProductionApplyResult result)
        {
            if (passed)
                Debug.Log($"[MeetingProductionApplyValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingProductionApplyValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
