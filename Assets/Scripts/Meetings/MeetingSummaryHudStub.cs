using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingSummaryHudStub : MonoBehaviour
    {
        [SerializeField] private bool showOnGui = true;
        [SerializeField] private int x = 20;
        [SerializeField] private int y = 240;
        [SerializeField] private int width = 520;
        [SerializeField] private int height = 140;

        private MeetingSummaryUiState _state;
        private bool _hasState;

        public void SetState(MeetingSummaryUiState state)
        {
            _state = state;
            _hasState = true;
        }

        public void Clear()
        {
            _state = default;
            _hasState = false;
        }

        private void OnGUI()
        {
            if (!showOnGui || !_hasState)
                return;

            GUI.Box(new Rect(x, y, width, height), _state.HeaderText);
            GUI.Label(new Rect(x + 12, y + 28, width - 24, 22), _state.ReportSummaryText);
            GUI.Label(new Rect(x + 12, y + 52, width - 24, 22), _state.VoteSummaryText);
            GUI.Label(new Rect(x + 12, y + 76, width - 24, 22), _state.DeductionSummaryText);
            GUI.Label(new Rect(x + 12, y + 104, width - 24, 22), _state.ActionHintText);
        }
    }
}
