using UnityEngine;

namespace OFIS.Interactions
{
    public sealed class SceneInteractionPlacementDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private float debugDetectorRadius = 1.5f;

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
            detectorCollider.radius = debugDetectorRadius;

            LocalInteractionRadiusDetector detector = detectorObject.AddComponent<LocalInteractionRadiusDetector>();

            WorldInteractionCandidateProvider[] providers = interactionRoot.GetComponentsInChildren<WorldInteractionCandidateProvider>();
            int registeredProviderCount = RegisterProvidersInsideRadius(detector, providers, detectorObject.transform.position, debugDetectorRadius);

            WorldInteractionResolveResult result = detector.RefreshSelection();

            ValidateProviderCount(providers);
            ValidateRegisteredProviderCount(registeredProviderCount);
            ValidateSelectionExists(result);
            ValidateHighestPriorityNearServer(result);
        }

        private static int RegisterProvidersInsideRadius(
            LocalInteractionRadiusDetector detector,
            WorldInteractionCandidateProvider[] providers,
            Vector3 detectorPosition,
            float radius)
        {
            if (detector == null || providers == null)
                return 0;

            int registeredCount = 0;

            foreach (WorldInteractionCandidateProvider provider in providers)
            {
                if (provider == null)
                    continue;

                float distance = Vector2.Distance(detectorPosition, provider.transform.position);

                if (distance > radius)
                    continue;

                detector.RegisterProviderForDebug(provider);
                registeredCount++;
            }

            return registeredCount;
        }

        private static void ValidateProviderCount(WorldInteractionCandidateProvider[] providers)
        {
            bool passed = providers != null && providers.Length >= 7;

            if (passed)
                Debug.Log($"[SceneInteractionPlacementValidator] PASS ProviderCount: Count={providers.Length}");
            else
                Debug.LogError($"[SceneInteractionPlacementValidator] FAIL ProviderCount: Count={(providers == null ? 0 : providers.Length)}");
        }

        private static void ValidateRegisteredProviderCount(int registeredProviderCount)
        {
            bool passed = registeredProviderCount >= 2;

            if (passed)
                Debug.Log($"[SceneInteractionPlacementValidator] PASS RegisteredProviderCount: Count={registeredProviderCount}");
            else
                Debug.LogError($"[SceneInteractionPlacementValidator] FAIL RegisteredProviderCount: Count={registeredProviderCount}");
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
