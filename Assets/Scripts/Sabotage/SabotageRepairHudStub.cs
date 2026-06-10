using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Sabotage
{
    public sealed class SabotageRepairHudStub : MonoBehaviour
    {
        [SerializeField] private bool showUi = true;
        [SerializeField] private Vector2 screenPosition = new Vector2(20f, 120f);
        [SerializeField] private Vector2 size = new Vector2(340f, 100f);
        [SerializeField] private SabotageObjectiveState debugInitialState = SabotageObjectiveState.Active;

        private readonly SabotageRepairUiStateService _uiStateService = new SabotageRepairUiStateService();
        private SabotageObjectiveRuntimeState _sabotageState;

        public SabotageRepairUiState CurrentState { get; private set; }

        private void Awake()
        {
            _sabotageState = new SabotageObjectiveRuntimeState(BuildDebugDefinition(), debugInitialState);
            RefreshState();
        }

        private void Update()
        {
            RefreshState();
        }

        [ContextMenu("Refresh Sabotage Repair HUD State")]
        public SabotageRepairUiState RefreshState()
        {
            CurrentState = _uiStateService.Build(_sabotageState);
            return CurrentState;
        }

        public void SetSabotageState(SabotageObjectiveRuntimeState sabotageState)
        {
            _sabotageState = sabotageState;
            RefreshState();
        }

        private void OnGUI()
        {
            if (!showUi)
                return;

            GUI.Box(new Rect(screenPosition.x, screenPosition.y, size.x, size.y), CurrentState.HeaderText);
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 30f, size.x - 20f, 22f), $"State: {CurrentState.State}");
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 55f, size.x - 20f, 22f), $"Status: {CurrentState.StatusText}");
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 78f, size.x - 20f, 22f), $"Action: {CurrentState.ActionHintText}");
        }

        private static SabotageObjectiveDefinition BuildDebugDefinition()
        {
            return new SabotageObjectiveDefinition(
                "sabotage_server_room",
                "Server room sabotage",
                OfficeRoomType.ServerRoom,
                5f);
        }
    }
}
