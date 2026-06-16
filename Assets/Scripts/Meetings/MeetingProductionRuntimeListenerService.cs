namespace OFIS.Meetings
{
    public sealed class MeetingProductionRuntimeListenerService
    {
        private MeetingProductionRuntimeListenerState _state;

        public MeetingProductionRuntimeListenerService()
        {
            _state = MeetingProductionRuntimeListenerState.Empty;
        }

        public MeetingProductionRuntimeListenerState State => _state;

        public void Reset()
        {
            _state = MeetingProductionRuntimeListenerState.Empty;
        }

        public MeetingProductionRuntimeListenerState Handle(
            MeetingProductionRuntimeEvent runtimeEvent)
        {
            if (runtimeEvent == null)
                return _state;

            _state = new MeetingProductionRuntimeListenerState(
                _state.ReceivedEventCount + 1,
                true,
                runtimeEvent.ActionType,
                runtimeEvent.AppliedCompanyHealthDelta,
                runtimeEvent.CompanyHealthBefore,
                runtimeEvent.CompanyHealthAfter,
                runtimeEvent.RequestedCloseMeeting,
                runtimeEvent.RequestedWinBranchResolution,
                runtimeEvent.RequestedMeetingEndPipeline,
                runtimeEvent.HasSummaryUiState,
                runtimeEvent.Message);

            return _state;
        }
    }
}
