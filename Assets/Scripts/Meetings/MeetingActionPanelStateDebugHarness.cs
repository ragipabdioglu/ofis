using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionPanelStateDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastMeetingId;
        [SerializeField] private MeetingRuntimePhaseType lastPhaseType;
        [SerializeField] private bool lastIsOpen;
        [SerializeField] private bool lastCanSelectAction;
        [SerializeField] private bool lastShouldShowTargetPicker;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionPanelStateService _service =
            new MeetingActionPanelStateService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateActionPanelState();
        }

        [ContextMenu("Validate Meeting Action Panel State")]
        public void ValidateActionPanelState()
        {
            ValidateMeetingPhaseOpensPanel();
            ValidateFinalMeetingPhaseOpensPanel();
            ValidateOfficePhaseClosesPanel();
            ValidateEndedMeetingClosesPanel();
            ValidateAppliedOfficialActionClosesPanel();
            ValidateInactivePhaseClosesPanel();
        }

        private void ValidateMeetingPhaseOpensPanel()
        {
            MeetingActionPanelState state = _service.BuildState(
                "meeting_panel_001",
                MeetingRuntimePhaseType.Meeting,
                true,
                false,
                false);

            bool passed = state.IsOpen && state.CanSelectAction && state.ShouldShowTargetPicker;
            LogResult("MeetingPhaseOpensPanel", passed, state);
        }

        private void ValidateFinalMeetingPhaseOpensPanel()
        {
            MeetingActionPanelState state = _service.BuildState(
                "meeting_panel_002",
                MeetingRuntimePhaseType.FinalMeeting,
                true,
                false,
                false);

            bool passed = state.IsOpen && state.CanSelectAction;
            LogResult("FinalMeetingPhaseOpensPanel", passed, state);
        }

        private void ValidateOfficePhaseClosesPanel()
        {
            MeetingActionPanelState state = _service.BuildState(
                "meeting_panel_003",
                MeetingRuntimePhaseType.Office,
                true,
                false,
                false);

            LogResult("OfficePhaseClosesPanel", !state.IsOpen, state);
        }

        private void ValidateEndedMeetingClosesPanel()
        {
            MeetingActionPanelState state = _service.BuildState(
                "meeting_panel_004",
                MeetingRuntimePhaseType.Meeting,
                true,
                true,
                false);

            LogResult("EndedMeetingClosesPanel", !state.IsOpen, state);
        }

        private void ValidateAppliedOfficialActionClosesPanel()
        {
            MeetingActionPanelState state = _service.BuildState(
                "meeting_panel_005",
                MeetingRuntimePhaseType.Meeting,
                true,
                false,
                true);

            LogResult("AppliedOfficialActionClosesPanel", !state.IsOpen, state);
        }

        private void ValidateInactivePhaseClosesPanel()
        {
            MeetingActionPanelState state = _service.BuildState(
                "meeting_panel_006",
                MeetingRuntimePhaseType.Meeting,
                false,
                false,
                false);

            LogResult("InactivePhaseClosesPanel", !state.IsOpen, state);
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionPanelState state)
        {
            lastMeetingId = state.MeetingId;
            lastPhaseType = state.PhaseType;
            lastIsOpen = state.IsOpen;
            lastCanSelectAction = state.CanSelectAction;
            lastShouldShowTargetPicker = state.ShouldShowTargetPicker;
            lastMessage = state.Message;

            if (passed)
                Debug.Log($"[MeetingActionPanelStateValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[MeetingActionPanelStateValidator] FAIL {testName}: {state}");
        }
    }
}
#pragma warning restore 0414
