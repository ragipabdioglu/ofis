using OFIS.Players;
using UnityEngine;

namespace OFIS.Interactions
{
    [RequireComponent(typeof(LocalInteractionRadiusDetector))]
    public sealed class LocalInteractionUiStub : MonoBehaviour
    {
        [SerializeField] private bool showUi = true;
        [SerializeField] private PlayerLifeState debugLifeState = PlayerLifeState.Alive;
        [SerializeField] private Vector2 screenPosition = new Vector2(20f, 380f);
        [SerializeField] private Vector2 size = new Vector2(500f, 110f);

        private LocalInteractionRadiusDetector _detector;
        private LocalInteractionExecutionBridge _executionBridge;
        private readonly InteractionUiStateService _uiStateService = new InteractionUiStateService();

        public InteractionUiState CurrentState { get; private set; } = new InteractionUiState(
            false,
            false,
            WorldInteractionType.None,
            "No interaction",
            "Not evaluated yet.",
            "No action yet");

        private void Awake()
        {
            _detector = GetComponent<LocalInteractionRadiusDetector>();
            _executionBridge = GetComponent<LocalInteractionExecutionBridge>();
        }

        private void Update()
        {
            RefreshState();
        }

        [ContextMenu("Refresh Interaction UI State")]
        public InteractionUiState RefreshState()
        {
            if (_detector == null)
                _detector = GetComponent<LocalInteractionRadiusDetector>();

            if (_executionBridge == null)
                _executionBridge = GetComponent<LocalInteractionExecutionBridge>();

            if (_detector == null)
            {
                CurrentState = new InteractionUiState(
                    false,
                    false,
                    WorldInteractionType.None,
                    "No detector",
                    "Missing LocalInteractionRadiusDetector.",
                    "No action yet");

                return CurrentState;
            }

            WorldInteractionResolveResult resolveResult = _detector.RefreshSelection();
            InteractionExecutionResult lastExecution = _executionBridge != null
                ? _executionBridge.LastExecutionResult
                : InteractionExecutionResult.Failed("No execution bridge attached.");

            CurrentState = _uiStateService.Build(debugLifeState, resolveResult, lastExecution);
            return CurrentState;
        }

        public void SetDebugLifeState(PlayerLifeState lifeState)
        {
            debugLifeState = lifeState;
        }

        private void OnGUI()
        {
            if (!showUi)
                return;

            GUI.Box(new Rect(screenPosition.x, screenPosition.y, size.x, size.y), "Interaction UI Stub");
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 25f, size.x - 20f, 22f), $"Prompt: {CurrentState.PromptText}");
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 50f, size.x - 20f, 22f), $"Status: {CurrentState.StatusText}");
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 75f, size.x - 20f, 22f), $"Last Action: {CurrentState.LastActionText}");
        }
    }
}
