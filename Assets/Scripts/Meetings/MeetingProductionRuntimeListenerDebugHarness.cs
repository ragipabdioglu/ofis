using OFIS.Company;
using OFIS.Core.Bootstrap;
using OFIS.Core.Events;
using OFIS.MatchFlow.States;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingProductionRuntimeListenerDebugHarness : MonoBehaviour
    {
        [Header("Runtime Listener")]
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private bool subscribeToGameBootstrapEventBus = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private int lastReceivedEventCount;
        [SerializeField] private string lastActionType;
        [SerializeField] private string lastMessage;
        [SerializeField] private bool lastAppliedCompanyHealthDelta;
        [SerializeField] private bool lastRequestedCloseMeeting;
        [SerializeField] private bool lastRequestedWinBranchResolution;
        [SerializeField] private bool lastRequestedMeetingEndPipeline;
        [SerializeField] private bool lastHasSummaryUiState;
        [SerializeField] private int lastCompanyHealthBefore;
        [SerializeField] private int lastCompanyHealthAfter;

        private MeetingRuntimeProductionBridgeService _bridgeService;
        private MeetingProductionApplyService _applyService;
        private MeetingProductionEventBridgeService _eventBridgeService;
        private MeetingProductionRuntimeListenerService _listenerService;
        private bool _subscribedToGameBootstrap;

        private void Awake()
        {
            _bridgeService = new MeetingRuntimeProductionBridgeService();
            _applyService = new MeetingProductionApplyService();
            _eventBridgeService = new MeetingProductionEventBridgeService();
            _listenerService = new MeetingProductionRuntimeListenerService();

            TrySubscribeToGameBootstrapEventBus();
        }

        private void Start()
        {
            TrySubscribeToGameBootstrapEventBus();

            if (!validateOnStart)
                return;

            ValidateRuntimeListener();
        }

        private void OnDestroy()
        {
            if (_subscribedToGameBootstrap && GameBootstrap.EventBus != null)
                GameBootstrap.EventBus.Unsubscribe<MeetingProductionRuntimeEvent>(OnRuntimeEvent);
        }

        [ContextMenu("Reset Listener")]
        public void ResetListener()
        {
            _listenerService.Reset();
            ApplyDebugOutput(_listenerService.State);
            Debug.Log("[MeetingProductionRuntimeListener] Listener reset.");
        }

        [ContextMenu("Validate Runtime Listener")]
        public void ValidateRuntimeListener()
        {
            ValidatePenaltyEventResponse();
            ValidateAutoCloseEventResponse();
            ValidateFinalWinBranchEventResponse();
            ValidateMeetingEndPipelineEventResponse();
            ValidateMultipleEventCount();
        }

        private void TrySubscribeToGameBootstrapEventBus()
        {
            if (!subscribeToGameBootstrapEventBus)
                return;

            if (GameBootstrap.EventBus == null)
                return;

            if (_subscribedToGameBootstrap)
                return;

            GameBootstrap.EventBus.Subscribe<MeetingProductionRuntimeEvent>(OnRuntimeEvent);
            _subscribedToGameBootstrap = true;
        }

        private void OnRuntimeEvent(MeetingProductionRuntimeEvent runtimeEvent)
        {
            MeetingProductionRuntimeListenerState state =
                _listenerService.Handle(runtimeEvent);

            ApplyDebugOutput(state);
        }

        private void ValidatePenaltyEventResponse()
        {
            ResetListenerForValidation();
            CompanyHealthService temporaryHealthService = CreateTemporaryHealthService();

            MeetingProductionRuntimeListenerState state = PublishScenarioToLocalListener(
                MeetingRuntimeDebugScenarioType.MissingPenalty,
                MatchState.Meeting1,
                1f,
                temporaryHealthService.CurrentHealth,
                temporaryHealthService);

            bool passed = state.ReceivedEventCount == 1
                && state.LastActionType == MeetingProductionBridgeActionType.ApplyCompanyHealthPenalty
                && state.LastAppliedCompanyHealthDelta
                && state.LastCompanyHealthBefore == 100
                && state.LastCompanyHealthAfter == 90;

            Destroy(temporaryHealthService.gameObject);
            LogResult("PenaltyEventResponse", passed, state);
        }

        private void ValidateAutoCloseEventResponse()
        {
            ResetListenerForValidation();

            PublishScenarioToLocalListener(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100,
                null);

            _listenerService.Reset();
            ApplyDebugOutput(_listenerService.State);

            MeetingProductionRuntimeListenerState state = PublishScenarioToLocalListener(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100,
                null);

            bool passed = state.ReceivedEventCount == 1
                && state.LastActionType == MeetingProductionBridgeActionType.CloseMeeting
                && state.LastRequestedCloseMeeting;

            LogResult("AutoCloseEventResponse", passed, state);
        }

        private void ValidateFinalWinBranchEventResponse()
        {
            ResetListenerForValidation();

            MeetingProductionRuntimeListenerState state = PublishScenarioToLocalListener(
                MeetingRuntimeDebugScenarioType.EmptyFinalWinBranch,
                MatchState.FinalMeeting,
                1f,
                100,
                null);

            bool passed = state.ReceivedEventCount == 1
                && state.LastActionType == MeetingProductionBridgeActionType.ResolveFinalMeetingWinBranch
                && state.LastRequestedWinBranchResolution;

            LogResult("FinalWinBranchEventResponse", passed, state);
        }

        private void ValidateMeetingEndPipelineEventResponse()
        {
            ResetListenerForValidation();

            MeetingProductionRuntimeListenerState state = PublishScenarioToLocalListener(
                MeetingRuntimeDebugScenarioType.MeetingEndPipeline,
                MatchState.Meeting2,
                1f,
                100,
                null);

            bool passed = state.ReceivedEventCount == 1
                && state.LastActionType == MeetingProductionBridgeActionType.RunMeetingEndPipeline
                && state.LastRequestedMeetingEndPipeline
                && state.LastHasSummaryUiState;

            LogResult("MeetingEndPipelineEventResponse", passed, state);
        }

        private void ValidateMultipleEventCount()
        {
            ResetListenerForValidation();

            PublishScenarioToLocalListener(
                MeetingRuntimeDebugScenarioType.MissingPenalty,
                MatchState.Meeting1,
                1f,
                100,
                null);

            MeetingProductionRuntimeListenerState state = PublishScenarioToLocalListener(
                MeetingRuntimeDebugScenarioType.MeetingEndPipeline,
                MatchState.Meeting2,
                1f,
                100,
                null);

            bool passed = state.ReceivedEventCount == 2
                && state.LastActionType == MeetingProductionBridgeActionType.RunMeetingEndPipeline;

            LogResult("MultipleEventCount", passed, state);
        }

        private MeetingProductionRuntimeListenerState PublishScenarioToLocalListener(
            MeetingRuntimeDebugScenarioType scenario,
            MatchState sourceMatchState,
            float deltaSeconds,
            int currentHealth,
            CompanyHealthService healthService)
        {
            GameEventBus localEventBus = new GameEventBus();
            localEventBus.Subscribe<MeetingProductionRuntimeEvent>(OnRuntimeEvent);

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

            _eventBridgeService.Publish(
                localEventBus,
                applyResult,
                Time.realtimeSinceStartup);

            localEventBus.Unsubscribe<MeetingProductionRuntimeEvent>(OnRuntimeEvent);
            localEventBus.Clear();

            return _listenerService.State;
        }

        private void ResetListenerForValidation()
        {
            _bridgeService.Reset();
            _listenerService.Reset();
            ApplyDebugOutput(_listenerService.State);
        }

        private void ApplyDebugOutput(MeetingProductionRuntimeListenerState state)
        {
            lastReceivedEventCount = state.ReceivedEventCount;
            lastActionType = state.LastActionType.ToString();
            lastMessage = state.LastMessage;
            lastAppliedCompanyHealthDelta = state.LastAppliedCompanyHealthDelta;
            lastRequestedCloseMeeting = state.LastRequestedCloseMeeting;
            lastRequestedWinBranchResolution = state.LastRequestedWinBranchResolution;
            lastRequestedMeetingEndPipeline = state.LastRequestedMeetingEndPipeline;
            lastHasSummaryUiState = state.LastHasSummaryUiState;
            lastCompanyHealthBefore = state.LastCompanyHealthBefore;
            lastCompanyHealthAfter = state.LastCompanyHealthAfter;
        }

        private static CompanyHealthService CreateTemporaryHealthService()
        {
            GameObject temporaryObject = new GameObject("MeetingRuntimeListenerTempCompanyHealth");
            return temporaryObject.AddComponent<CompanyHealthService>();
        }

        private static void LogResult(
            string testName,
            bool passed,
            MeetingProductionRuntimeListenerState state)
        {
            if (passed)
                Debug.Log($"[MeetingProductionRuntimeListenerValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[MeetingProductionRuntimeListenerValidator] FAIL {testName}: {state}");
        }
    }
}
#pragma warning restore 0414
