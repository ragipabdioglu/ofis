using UnityEngine;

namespace OFIS.Interactions
{
    public sealed class SceneInteractionPlacementDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateSceneInteractionPlacement();
        }

        [ContextMenu("Validate Scene Interaction Placement")]
        public void ValidateSceneInteractionPlacement()
        {
            GameObject interactionRoot = new GameObject("SceneInteractionPlacement_TestRoot");
            SceneInteractionPlacementDebugBuilder builder = interactionRoot.AddComponent<SceneInteractionPlacementDebugBuilder>();
            builder.BuildInteractions();

            GameObject detectorObject = new GameObject("SceneInteractionPlacement_TestDetector");
            detectorObject.transform.position = new Vector3(4.6f, 4.7f, 0f);

            CircleCollider2D detectorCollider = detectorObject.AddComponent<CircleCollider2D>();
            detectorCollider.isTrigger = true;
            detectorCollider.radius = 1.5f;

            LocalInteractionRadiusDetector detector = detectorObject.AddComponent<LocalInteractionRadiusDetector>();

            WorldInteractionCandidateProvider[] providers = interactionRoot.GetComponentsInChildren<WorldInteractionCandidateProvider>();

            foreach (WorldInteractionCandidateProvider provider in providers)
                detector.RegisterProviderForDebug(provider);

            WorldInteractionResolveResult result = detector.RefreshSelection();

            ValidateProviderCount(providers);
            ValidateSelectionExists(result);
            ValidateHighestPriorityNearServer(result);
        }

        private static void ValidateProviderCount(WorldInteractionCandidateProvider[] providers)
        {
            bool passed = providers != null && providers.Length >= 7;

            if (passed)
                Debug.Log($"[SceneInteractionPlacementValidator] PASS ProviderCount: Count={providers.Length}");
            else
                Debug.LogError($"[SceneInteractionPlacementValidator] FAIL ProviderCount: Count={(providers == null ? 0 : providers.Length)}");
        }

        private static void ValidateSelectionExists(WorldInteractionResolveResult result)
        {
            bool passed = result.HasSelection;

            if (passed)
                Debug.Log($"[SceneInteractionPlacementValidator] PASS SelectionExists: {result}");
            else
                Debug.LogError($"[SceneInteractionPlacementValidator] FAIL SelectionExists: {result}");
        }

        private static void ValidateHighestPriorityNearServer(WorldInteractionResolveResult result)
        {
            bool passed = result.HasSelection && result.SelectedCandidate.Type == WorldInteractionType.SabotageRepair;

            if (passed)
                Debug.Log($"[SceneInteractionPlacementValidator] PASS HighestPriorityNearServer: {result.SelectedCandidate}");
            else
                Debug.LogError($"[SceneInteractionPlacementValidator] FAIL HighestPriorityNearServer: {result}");
        }
    }
}
