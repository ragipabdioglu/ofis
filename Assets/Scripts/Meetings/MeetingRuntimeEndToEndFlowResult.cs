using OFIS.MatchFlow.States;

namespace OFIS.Meetings
{
    public readonly struct MeetingRuntimeEndToEndFlowResult
    {
        public MeetingProductionBridgeResult BridgeResult { get; }
        public MeetingProductionApplyResult ApplyResult { get; }
        public MeetingProductionEventBridgeResult EventBridgeResult { get; }
        public MeetingProductionRuntimeListenerState ListenerState { get; }
        public MeetingMatchFlowCommandConsumerResult ConsumerResult { get; }
        public bool CompletedFlow { get; }
        public string Message { get; }

        public MeetingRuntimeEndToEndFlowResult(
            MeetingProductionBridgeResult bridgeResult,
            MeetingProductionApplyResult applyResult,
            MeetingProductionEventBridgeResult eventBridgeResult,
            MeetingProductionRuntimeListenerState listenerState,
            MeetingMatchFlowCommandConsumerResult consumerResult,
            bool completedFlow,
            string message)
        {
            BridgeResult = bridgeResult;
            ApplyResult = applyResult;
            EventBridgeResult = eventBridgeResult;
            ListenerState = listenerState;
            ConsumerResult = consumerResult;
            CompletedFlow = completedFlow;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting runtime end-to-end flow completed."
                : message;
        }

        public override string ToString()
        {
            return $"Completed={CompletedFlow}, Action={BridgeResult.Command.ActionType}, Listener={ListenerState.LastActionType}, Transition={ConsumerResult.TransitionType}, Next={ConsumerResult.SuggestedNextMatchState}, Message={Message}";
        }
    }
}
