using OFIS.Company;
using OFIS.Core.Events;
using OFIS.MatchFlow.States;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingRuntimeEndToEndFlowDebugHarness : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastActionType;
        [SerializeField] private string lastTransitionType;
        [SerializeField] private string lastSuggestedNextState;
        [SerializeField] private bool lastCompletedFlow;
        [SerializeField] private bool lastPublishedEvent;
        [SerializeField] private int lastListenerEventCount;

        private MeetingRuntimeProductionBridgeService _bridgeService;
        private MeetingProductionApplyService _applyService;
        private MeetingProductionEventBridgeService _eventBridgeService;
        private MeetingProductionRuntimeListenerService _listenerService;
        private MeetingMatchFlowCommandConsumerService _consumerService;

        private void Awake()
        {
            _bridgeService = new MeetingRuntimeProductionBridgeService();
            _applyService = new MeetingProductionApplyService();
            _eventBridgeService = new MeetingProductionEventBridgeService();
            _listenerService = new MeetingProductionRuntimeListenerService();
            _consumerService = new MeetingMatchFlowCommandConsumerService();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateEndToEndFlow();
        }

        [ContextMenu("Validate End To End Flow")]
        public void ValidateEndToEndFlow()
        {
            ValidatePenaltyFlow();
            ValidateAutoCloseFlow();
            ValidateFinalWinBranchFlow();
            ValidateMeetingEndPipelineFlow();
        }

        private void ValidatePenaltyFlow()
        {
            CompanyHealthService temporaryHealthService = CreateTemporaryHealthService();

            MeetingRuntimeEndToEndFlowResult result = RunScenario(
                MeetingRuntimeDebugScenarioType.MissingPenalty,
                MatchState.Meeting1,
                1f,
                temporaryHealthService.CurrentHealth,
                temporaryHealthService);

            bool passed = result.CompletedFlow
                && result.BridgeResult.Command.ActionType == MeetingProductionBridgeActionType.ApplyCompanyHealthPenalty
                && result.ApplyResult.AppliedCompanyHealthDelta
                && result.ListenerState.LastAppliedCompanyHealthDelta
                && result.ConsumerResult.TransitionType == MeetingMatchFlowTransitionType.ApplyCompanyHealthOnly;

            Destroy(temporaryHealthService.gameObject);
            LogResult("PenaltyFlow", passed, result);
        }

        private void ValidateAutoCloseFlow()
        {
            ResetServices();

            RunScenario(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100,
                null,
                resetBeforeRun: false);

            MeetingRuntimeEndToEndFlowResult result = RunScenario(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100,
                null,
                resetBeforeRun: false);

            bool passed = result.CompletedFlow
                && result.BridgeResult.Command.ActionType == MeetingProductionBridgeActionType.CloseMeeting
                && result.ConsumerResult.TransitionType == MeetingMatchFlowTransitionType.CloseNormalMeeting
                && result.ConsumerResult.SuggestedNextMatchState == MatchState.OfficePhase2;

            LogResult("AutoCloseFlow", passed, result);
        }

        private void ValidateFinalWinBranchFlow()
        {
            MeetingRuntimeEndToEndFlowResult result = RunScenario(
                MeetingRuntimeDebugScenarioType.EmptyFinalWinBranch,
                MatchState.FinalMeeting,
                1f,
                100,
                null);

            bool passed = result.CompletedFlow
                && result.BridgeResult.Command.ActionType == MeetingProductionBridgeActionType.ResolveFinalMeetingWinBranch
                && result.ConsumerResult.TransitionType == MeetingMatchFlowTransitionType.ResolveFinalMeeting
                && result.ConsumerResult.SuggestedNextMatchState == MatchState.ResolvingMatch;

            LogResult("FinalWinBranchFlow", passed, result);
        }

        private void ValidateMeetingEndPipelineFlow()
        {
            MeetingRuntimeEndToEndFlowResult result = RunScenario(
                MeetingRuntimeDebugScenarioType.MeetingEndPipeline,
                MatchState.Meeting2,
                1f,
                100,
                null);

            bool passed = result.CompletedFlow
                && result.BridgeResult.Command.ActionType == MeetingProductionBridgeActionType.RunMeetingEndPipeline
                && result.ApplyResult.HasSummaryUiState
                && result.ListenerState.LastHasSummaryUiState
                && result.ConsumerResult.TransitionType == MeetingMatchFlowTransitionType.ShowMeetingEndSummary;

            LogResult("MeetingEndPipelineFlow", passed, result);
        }

        private MeetingRuntimeEndToEndFlowResult RunScenario(
            MeetingRuntimeDebugScenarioType scenario,
            MatchState sourceMatchState,
            float deltaSeconds,
            int currentHealth,
            CompanyHealthService healthService,
            bool resetBeforeRun = true)
        {
            if (resetBeforeRun)
                ResetServices();

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

            MeetingProductionBridgeResult bridgeResult = _bridgeService.Resolve(productionInput);
            MeetingProductionApplyResult applyResult = _applyService.Apply(bridgeResult.Command, healthService);
            MeetingProductionEventBridgeResult eventResult = _eventBridgeService.Publish(
                localEventBus,
                applyResult,
                Time.realtimeSinceStartup);

            MeetingProductionRuntimeListenerState listenerState = _listenerService.State;
            MeetingMatchFlowCommandConsumerResult consumerResult = _consumerService.Consume(
                listenerState,
                sourceMatchState);

            localEventBus.Unsubscribe<MeetingProductionRuntimeEvent>(OnRuntimeEvent);
            localEventBus.Clear();

            bool completedFlow = bridgeResult.HasCommand
                && eventResult.PublishedEvent
                && listenerState.HasRuntimeEvent
                && listenerState.ReceivedEventCount > 0;

            MeetingRuntimeEndToEndFlowResult result = new MeetingRuntimeEndToEndFlowResult(
                bridgeResult,
                applyResult,
                eventResult,
                listenerState,
                consumerResult,
                completedFlow,
                "Meeting runtime end-to-end debug flow resolved.");

            ApplyDebugOutput(result);
            return result;
        }

        private void OnRuntimeEvent(MeetingProductionRuntimeEvent runtimeEvent)
        {
            _listenerService.Handle(runtimeEvent);
        }

        private void ResetServices()
        {
            _bridgeService.Reset();
            _listenerService.Reset();
        }

        private void ApplyDebugOutput(MeetingRuntimeEndToEndFlowResult result)
        {
            lastActionType = result.BridgeResult.Command.ActionType.ToString();
            lastTransitionType = result.ConsumerResult.TransitionType.ToString();
            lastSuggestedNextState = result.ConsumerResult.SuggestedNextMatchState.ToString();
            lastCompletedFlow = result.CompletedFlow;
            lastPublishedEvent = result.EventBridgeResult.PublishedEvent;
            lastListenerEventCount = result.ListenerState.ReceivedEventCount;
        }

        private static CompanyHealthService CreateTemporaryHealthService()
        {
            GameObject temporaryObject = new GameObject("MeetingEndToEndFlowTempCompanyHealth");
            return temporaryObject.AddComponent<CompanyHealthService>();
        }

        private static void LogResult(
            string testName,
            bool passed,
            MeetingRuntimeEndToEndFlowResult result)
        {
            if (passed)
                Debug.Log($"[MeetingRuntimeEndToEndFlowValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingRuntimeEndToEndFlowValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
