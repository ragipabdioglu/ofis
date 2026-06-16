using OFIS.MatchFlow.States;

namespace OFIS.Meetings
{
    public readonly struct MeetingMatchFlowCommandConsumerResult
    {
        public MeetingMatchFlowTransitionType TransitionType { get; }
        public MatchState SourceMatchState { get; }
        public MatchState SuggestedNextMatchState { get; }
        public bool HasTransitionRequest { get; }
        public bool DryRunOnly { get; }
        public bool RuntimeMutationAllowed { get; }
        public bool RuntimeMutationApplied { get; }
        public string Reason { get; }

        public MeetingMatchFlowCommandConsumerResult(
            MeetingMatchFlowTransitionType transitionType,
            MatchState sourceMatchState,
            MatchState suggestedNextMatchState,
            bool hasTransitionRequest,
            bool dryRunOnly,
            bool runtimeMutationAllowed,
            bool runtimeMutationApplied,
            string reason)
        {
            TransitionType = transitionType;
            SourceMatchState = sourceMatchState;
            SuggestedNextMatchState = suggestedNextMatchState;
            HasTransitionRequest = hasTransitionRequest;
            DryRunOnly = dryRunOnly;
            RuntimeMutationAllowed = runtimeMutationAllowed;
            RuntimeMutationApplied = runtimeMutationApplied;
            Reason = string.IsNullOrWhiteSpace(reason)
                ? "Meeting match flow command consumer resolved."
                : reason;
        }

        public override string ToString()
        {
            return $"Transition={TransitionType}, Source={SourceMatchState}, SuggestedNext={SuggestedNextMatchState}, HasRequest={HasTransitionRequest}, DryRun={DryRunOnly}, MutationAllowed={RuntimeMutationAllowed}, MutationApplied={RuntimeMutationApplied}, Reason={Reason}";
        }
    }
}
