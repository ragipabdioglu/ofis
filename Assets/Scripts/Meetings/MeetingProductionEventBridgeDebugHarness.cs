using OFIS.Company;
using OFIS.Core.Events;
using OFIS.MatchFlow.States;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingProductionEventBridgeDebugHarness : MonoBehaviour
    {
        [Header("Debug Scenario")]
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private MeetingRuntimeDebugScenarioType scenarioType = MeetingRuntimeDebugScenarioType.MeetingEndPipeline;
        [SerializeField] private MatchState matchState = MatchState.Meeting2;
        [SerializeField] private int companyHealth = 100;
        [SerializeField] private float deltaTimeSeconds = 1f;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastActionType;
        [SerializeField] private string lastEventMessage;
        [SerializeField] private bool lastShouldPublishEvent;
        [SerializeField] private bool lastPublishedEvent;
        [SerializeField] private bool lastReceivedEvent;
        [SerializeField] private bool lastRequestedCloseMeeting;
        [SerializeField] private bool lastRequestedWinBranchResolution;
        [SerializeField] private bool lastRequestedMeetingEndPipeline;
        [SerializeField] private int receivedEventCount;

        private MeetingRuntimeProductionBridgeService _bridgeService;
        private MeetingProductionApplyService _applyService;
        private MeetingProductionEventBridgeService _eventBridgeService;
        private GameEventBus _localEventBus;
        private MeetingProductionRuntimeEvent _lastReceivedRuntimeEvent;

        private void Awake()
        {
            _bridgeService = new MeetingRuntimeProductionBridgeService();
            _applyService = new MeetingProductionApplyService();
            _eventBridgeService = new MeetingProductionEventBridgeService();
            _localEventBus = new GameEventBus();
            _localEventBus.Subscribe<MeetingProductionRuntimeEvent>(OnRuntimeEvent);
        }

        private void OnDestroy()
        {
            _localEventBus?.Unsubscribe<MeetingProductionRuntimeEvent>(OnRuntimeEvent);
            _localEventBus?.Clear();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateEventBridge();
        }

        [ContextMenu("Resolve And Publish Selected Scenario")]
        public void ResolveAndPublishSelectedScenario()
        {
            ResetRuntimeState();

            MeetingProductionEventBridgeResult result = ResolveApplyAndPublish(
                scenarioType,
                matchState,
                deltaTimeSeconds,
                companyHealth,
                null);

            Debug.Log($"[MeetingProductionEventBridge] {result}");
        }

        [ContextMenu("Validate Event Bridge")]
        public void ValidateEventBridge()
        {
            ValidateNoEventForNonMeetingPhase();
            ValidatePenaltyEvent();
            ValidateAutoCloseEvent();
            ValidateFinalWinBranchEvent();
            ValidateMeetingEndPipelineEvent();
        }

        private void ValidateNoEventForNonMeetingPhase()
        {
            ResetRuntimeState();

            MeetingProductionEventBridgeResult result = ResolveApplyAndPublish(
                MeetingRuntimeDebugScenarioType.NormalMeetingContinue,
                MatchState.ResolvingMatch,
                1f,
                100,
                null);

            bool passed = !result.ShouldPublishEvent
                && !result.PublishedEvent
                && receivedEventCount == 0;

            LogResult("NoEventForNonMeetingPhase", passed, result);
        }

        private void ValidatePenaltyEvent()
        {
            ResetRuntimeState();
            CompanyHealthService temporaryHealthService = CreateTemporaryHealthService();

            MeetingProductionEventBridgeResult result = ResolveApplyAndPublish(
                MeetingRuntimeDebugScenarioType.MissingPenalty,
                MatchState.Meeting1,
                1f,
                temporaryHealthService.CurrentHealth,
                temporaryHealthService);

            bool passed = result.PublishedEvent
                && lastReceivedEvent
                && receivedEventCount == 1
                && _lastReceivedRuntimeEvent != null
                && _lastReceivedRuntimeEvent.ActionType == MeetingProductionBridgeActionType.ApplyCompanyHealthPenalty
                && _lastReceivedRuntimeEvent.CompanyHealthBefore == 100
                && _lastReceivedRuntimeEvent.CompanyHealthAfter == 90;

            Destroy(temporaryHealthService.gameObject);
            LogResult("PenaltyEvent", passed, result);
        }

        private void ValidateAutoCloseEvent()
        {
            ResetRuntimeState();

            ResolveApplyAndPublish(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100,
                null);

            MeetingProductionEventBridgeResult result = ResolveApplyAndPublish(
                MeetingRuntimeDebugScenarioType.EmptyNormalAutoClose,
                MatchState.Meeting1,
                5f,
                100,
                null);

            bool passed = result.PublishedEvent
                && lastReceivedEvent
                && _lastReceivedRuntimeEvent != null
                && _lastReceivedRuntimeEvent.RequestedCloseMeeting;

            LogResult("AutoCloseEvent", passed, result);
        }

        private void ValidateFinalWinBranchEvent()
        {
            ResetRuntimeState();

            MeetingProductionEventBridgeResult result = ResolveApplyAndPublish(
                MeetingRuntimeDebugScenarioType.EmptyFinalWinBranch,
                MatchState.FinalMeeting,
                1f,
                100,
                null);

            bool passed = result.PublishedEvent
                && lastReceivedEvent
                && _lastReceivedRuntimeEvent != null
                && _lastReceivedRuntimeEvent.RequestedWinBranchResolution;

            LogResult("FinalWinBranchEvent", passed, result);
        }

        private void ValidateMeetingEndPipelineEvent()
        {
            ResetRuntimeState();

            MeetingProductionEventBridgeResult result = ResolveApplyAndPublish(
                MeetingRuntimeDebugScenarioType.MeetingEndPipeline,
                MatchState.Meeting2,
                1f,
                100,
                null);

            bool passed = result.PublishedEvent
                && lastReceivedEvent
                && _lastReceivedRuntimeEvent != null
                && _lastReceivedRuntimeEvent.RequestedMeetingEndPipeline
                && _lastReceivedRuntimeEvent.HasSummaryUiState;

            LogResult("MeetingEndPipelineEvent", passed, result);
        }

        private MeetingProductionEventBridgeResult ResolveApplyAndPublish(
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

            MeetingProductionEventBridgeResult eventResult =
                _eventBridgeService.Publish(_localEventBus, applyResult, Time.realtimeSinceStartup);

            ApplyDebugOutput(eventResult);
            return eventResult;
        }

        private void ResetRuntimeState()
        {
            _bridgeService.Reset();
            receivedEventCount = 0;
            _lastReceivedRuntimeEvent = null;
            lastReceivedEvent = false;
        }

        private void OnRuntimeEvent(MeetingProductionRuntimeEvent runtimeEvent)
        {
            receivedEventCount++;
            _lastReceivedRuntimeEvent = runtimeEvent;
            lastReceivedEvent = true;
        }

        private void ApplyDebugOutput(MeetingProductionEventBridgeResult result)
        {
            lastShouldPublishEvent = result.ShouldPublishEvent;
            lastPublishedEvent = result.PublishedEvent;
            lastEventMessage = result.Message;

            if (result.RuntimeEvent == null)
            {
                lastActionType = MeetingProductionBridgeActionType.None.ToString();
                lastRequestedCloseMeeting = false;
                lastRequestedWinBranchResolution = false;
                lastRequestedMeetingEndPipeline = false;
                return;
            }

            lastActionType = result.RuntimeEvent.ActionType.ToString();
            lastRequestedCloseMeeting = result.RuntimeEvent.RequestedCloseMeeting;
            lastRequestedWinBranchResolution = result.RuntimeEvent.RequestedWinBranchResolution;
            lastRequestedMeetingEndPipeline = result.RuntimeEvent.RequestedMeetingEndPipeline;
        }

        private static CompanyHealthService CreateTemporaryHealthService()
        {
            GameObject temporaryObject = new GameObject("MeetingProductionEventBridgeTempCompanyHealth");
            return temporaryObject.AddComponent<CompanyHealthService>();
        }

        private static void LogResult(
            string testName,
            bool passed,
            MeetingProductionEventBridgeResult result)
        {
            if (passed)
                Debug.Log($"[MeetingProductionEventBridgeValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingProductionEventBridgeValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
