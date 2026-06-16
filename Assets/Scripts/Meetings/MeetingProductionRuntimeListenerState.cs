namespace OFIS.Meetings
{
    public readonly struct MeetingProductionRuntimeListenerState
    {
        public int ReceivedEventCount { get; }
        public bool HasRuntimeEvent { get; }
        public MeetingProductionBridgeActionType LastActionType { get; }
        public bool LastAppliedCompanyHealthDelta { get; }
        public int LastCompanyHealthBefore { get; }
        public int LastCompanyHealthAfter { get; }
        public bool LastRequestedCloseMeeting { get; }
        public bool LastRequestedWinBranchResolution { get; }
        public bool LastRequestedMeetingEndPipeline { get; }
        public bool LastHasSummaryUiState { get; }
        public string LastMessage { get; }

        public MeetingProductionRuntimeListenerState(
            int receivedEventCount,
            bool hasRuntimeEvent,
            MeetingProductionBridgeActionType lastActionType,
            bool lastAppliedCompanyHealthDelta,
            int lastCompanyHealthBefore,
            int lastCompanyHealthAfter,
            bool lastRequestedCloseMeeting,
            bool lastRequestedWinBranchResolution,
            bool lastRequestedMeetingEndPipeline,
            bool lastHasSummaryUiState,
            string lastMessage)
        {
            ReceivedEventCount = receivedEventCount < 0 ? 0 : receivedEventCount;
            HasRuntimeEvent = hasRuntimeEvent;
            LastActionType = lastActionType;
            LastAppliedCompanyHealthDelta = lastAppliedCompanyHealthDelta;
            LastCompanyHealthBefore = lastCompanyHealthBefore < 0 ? 0 : lastCompanyHealthBefore;
            LastCompanyHealthAfter = lastCompanyHealthAfter < 0 ? 0 : lastCompanyHealthAfter;
            LastRequestedCloseMeeting = lastRequestedCloseMeeting;
            LastRequestedWinBranchResolution = lastRequestedWinBranchResolution;
            LastRequestedMeetingEndPipeline = lastRequestedMeetingEndPipeline;
            LastHasSummaryUiState = lastHasSummaryUiState;
            LastMessage = string.IsNullOrWhiteSpace(lastMessage)
                ? "No meeting runtime event received."
                : lastMessage;
        }

        public static MeetingProductionRuntimeListenerState Empty =>
            new MeetingProductionRuntimeListenerState(
                0,
                false,
                MeetingProductionBridgeActionType.None,
                false,
                0,
                0,
                false,
                false,
                false,
                false,
                "No meeting runtime event received.");

        public override string ToString()
        {
            return $"Count={ReceivedEventCount}, Action={LastActionType}, Health={LastCompanyHealthBefore}->{LastCompanyHealthAfter}, Close={LastRequestedCloseMeeting}, WinBranch={LastRequestedWinBranchResolution}, Pipeline={LastRequestedMeetingEndPipeline}, HasSummary={LastHasSummaryUiState}, Message={LastMessage}";
        }
    }
}
