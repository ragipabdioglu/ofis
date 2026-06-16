using OFIS.MatchFlow.States;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingMatchFlowCommandConsumerDebugHarness : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private bool dryRunOnly = true;
        [SerializeField] private bool allowRuntimeMutation = false;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastTransitionType;
        [SerializeField] private string lastSourceMatchState;
        [SerializeField] private string lastSuggestedNextMatchState;
        [SerializeField] private string lastReason;
        [SerializeField] private bool lastHasTransitionRequest;
        [SerializeField] private bool lastRuntimeMutationApplied;

        private MeetingMatchFlowCommandConsumerService _consumerService;

        private void Awake()
        {
            RebuildService();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateCommandConsumer();
        }

        [ContextMenu("Validate Command Consumer")]
        public void ValidateCommandConsumer()
        {
            RebuildService();

            ValidateNoEventProducesNoTransition();
            ValidateCompanyHealthOnlyKeepsMeeting();
            ValidateCloseMeeting1SuggestsOfficePhase2();
            ValidateCloseMeeting2SuggestsOfficePhase3();
            ValidateFinalWinBranchSuggestsResolving();
            ValidateMeetingEndPipelineShowsSummary();
        }

        private void ValidateNoEventProducesNoTransition()
        {
            MeetingMatchFlowCommandConsumerResult result = _consumerService.Consume(
                MeetingProductionRuntimeListenerState.Empty,
                MatchState.Meeting1);

            bool passed = result.TransitionType == MeetingMatchFlowTransitionType.None
                && !result.HasTransitionRequest
                && result.SuggestedNextMatchState == MatchState.None;

            LogResult("NoEventProducesNoTransition", passed, result);
        }

        private void ValidateCompanyHealthOnlyKeepsMeeting()
        {
            MeetingMatchFlowCommandConsumerResult result = _consumerService.Consume(
                BuildState(
                    MeetingProductionBridgeActionType.ApplyCompanyHealthPenalty,
                    appliedHealth: true,
                    close: false,
                    winBranch: false,
                    pipeline: false,
                    hasSummary: false),
                MatchState.Meeting1);

            bool passed = result.TransitionType == MeetingMatchFlowTransitionType.ApplyCompanyHealthOnly
                && result.SuggestedNextMatchState == MatchState.Meeting1
                && result.HasTransitionRequest;

            LogResult("CompanyHealthOnlyKeepsMeeting", passed, result);
        }

        private void ValidateCloseMeeting1SuggestsOfficePhase2()
        {
            MeetingMatchFlowCommandConsumerResult result = _consumerService.Consume(
                BuildState(
                    MeetingProductionBridgeActionType.CloseMeeting,
                    appliedHealth: false,
                    close: true,
                    winBranch: false,
                    pipeline: false,
                    hasSummary: false),
                MatchState.Meeting1);

            bool passed = result.TransitionType == MeetingMatchFlowTransitionType.CloseNormalMeeting
                && result.SuggestedNextMatchState == MatchState.OfficePhase2
                && result.HasTransitionRequest;

            LogResult("CloseMeeting1SuggestsOfficePhase2", passed, result);
        }

        private void ValidateCloseMeeting2SuggestsOfficePhase3()
        {
            MeetingMatchFlowCommandConsumerResult result = _consumerService.Consume(
                BuildState(
                    MeetingProductionBridgeActionType.CloseMeeting,
                    appliedHealth: false,
                    close: true,
                    winBranch: false,
                    pipeline: false,
                    hasSummary: false),
                MatchState.Meeting2);

            bool passed = result.TransitionType == MeetingMatchFlowTransitionType.CloseNormalMeeting
                && result.SuggestedNextMatchState == MatchState.OfficePhase3
                && result.HasTransitionRequest;

            LogResult("CloseMeeting2SuggestsOfficePhase3", passed, result);
        }

        private void ValidateFinalWinBranchSuggestsResolving()
        {
            MeetingMatchFlowCommandConsumerResult result = _consumerService.Consume(
                BuildState(
                    MeetingProductionBridgeActionType.ResolveFinalMeetingWinBranch,
                    appliedHealth: false,
                    close: false,
                    winBranch: true,
                    pipeline: false,
                    hasSummary: false),
                MatchState.FinalMeeting);

            bool passed = result.TransitionType == MeetingMatchFlowTransitionType.ResolveFinalMeeting
                && result.SuggestedNextMatchState == MatchState.ResolvingMatch
                && result.HasTransitionRequest;

            LogResult("FinalWinBranchSuggestsResolving", passed, result);
        }

        private void ValidateMeetingEndPipelineShowsSummary()
        {
            MeetingMatchFlowCommandConsumerResult result = _consumerService.Consume(
                BuildState(
                    MeetingProductionBridgeActionType.RunMeetingEndPipeline,
                    appliedHealth: false,
                    close: false,
                    winBranch: false,
                    pipeline: true,
                    hasSummary: true),
                MatchState.Meeting2);

            bool passed = result.TransitionType == MeetingMatchFlowTransitionType.ShowMeetingEndSummary
                && result.SuggestedNextMatchState == MatchState.Meeting2
                && result.HasTransitionRequest;

            LogResult("MeetingEndPipelineShowsSummary", passed, result);
        }

        private void RebuildService()
        {
            _consumerService = new MeetingMatchFlowCommandConsumerService(
                new MeetingMatchFlowCommandConsumerConfig(
                    dryRunOnly,
                    allowRuntimeMutation));
        }

        private void ApplyDebugOutput(MeetingMatchFlowCommandConsumerResult result)
        {
            lastTransitionType = result.TransitionType.ToString();
            lastSourceMatchState = result.SourceMatchState.ToString();
            lastSuggestedNextMatchState = result.SuggestedNextMatchState.ToString();
            lastReason = result.Reason;
            lastHasTransitionRequest = result.HasTransitionRequest;
            lastRuntimeMutationApplied = result.RuntimeMutationApplied;
        }

        private static MeetingProductionRuntimeListenerState BuildState(
            MeetingProductionBridgeActionType actionType,
            bool appliedHealth,
            bool close,
            bool winBranch,
            bool pipeline,
            bool hasSummary)
        {
            return new MeetingProductionRuntimeListenerState(
                1,
                true,
                actionType,
                appliedHealth,
                100,
                appliedHealth ? 90 : 100,
                close,
                winBranch,
                pipeline,
                hasSummary,
                "Command consumer debug state.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingMatchFlowCommandConsumerResult result)
        {
            ApplyDebugOutput(result);

            if (passed)
                Debug.Log($"[MeetingMatchFlowCommandConsumerValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingMatchFlowCommandConsumerValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
