using OFIS.Players;
using UnityEngine;

namespace OFIS.Interactions
{
    [RequireComponent(typeof(LocalInteractionRadiusDetector))]
    public sealed class LocalInteractionExecutionBridge : MonoBehaviour
    {
        [SerializeField] private PlayerLifeState debugLifeState = PlayerLifeState.Alive;
        [SerializeField] private bool logExecution = true;

        private LocalInteractionRadiusDetector _detector;
        private readonly InteractionExecutionService _executionService = new InteractionExecutionService();

        public InteractionExecutionResult LastExecutionResult { get; private set; } = InteractionExecutionResult.Failed("Not executed yet.");

        private void Awake()
        {
            _detector = GetComponent<LocalInteractionRadiusDetector>();
        }

        [ContextMenu("Try Execute Selected Interaction")]
        public InteractionExecutionResult TryExecuteSelected()
        {
            if (_detector == null)
                _detector = GetComponent<LocalInteractionRadiusDetector>();

            if (_detector == null)
            {
                LastExecutionResult = InteractionExecutionResult.Failed("No LocalInteractionRadiusDetector found.");
                return LastExecutionResult;
            }

            WorldInteractionResolveResult resolveResult = _detector.RefreshSelection();
            LastExecutionResult = _executionService.Execute(debugLifeState, resolveResult);

            if (logExecution)
                Debug.Log($"[InteractionExecutionBridge] {LastExecutionResult}");

            return LastExecutionResult;
        }

        public void SetDebugLifeState(PlayerLifeState lifeState)
        {
            debugLifeState = lifeState;
        }
    }
}
