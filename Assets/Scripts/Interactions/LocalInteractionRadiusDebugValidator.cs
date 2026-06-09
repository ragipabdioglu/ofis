using UnityEngine;

namespace OFIS.Interactions
{
    public sealed class LocalInteractionRadiusDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool buildOnStart = true;
        [SerializeField] private float radius = 2.5f;

        private LocalInteractionRadiusDetector _detector;

        private void Start()
        {
            if (!buildOnStart)
                return;

            BuildAndValidate();
        }

        [ContextMenu("Build And Validate Interaction Radius")]
        public void BuildAndValidate()
        {
            GameObject player = new GameObject("Interaction_Test_Player");
            player.transform.position = Vector3.zero;

            CircleCollider2D radiusCollider = player.AddComponent<CircleCollider2D>();
            radiusCollider.isTrigger = true;
            radiusCollider.radius = radius;

            Rigidbody2D playerBody = player.AddComponent<Rigidbody2D>();
            playerBody.bodyType = RigidbodyType2D.Kinematic;
            playerBody.gravityScale = 0f;

            _detector = player.AddComponent<LocalInteractionRadiusDetector>();
            player.AddComponent<LocalInteractionPromptDebugHud>();

            CreateCandidate("Nearest Door Panel", WorldInteractionType.DoorPanel, new Vector2(0.25f, 0f));
            CreateCandidate("Nearby Task", WorldInteractionType.Task, new Vector2(0.50f, 0f));
            CreateCandidate("Farther Corpse", WorldInteractionType.CorpseInspectOrCarry, new Vector2(1.50f, 0f));

            Physics2D.SyncTransforms();
            RegisterCandidatesInRadius();

            WorldInteractionResolveResult result = _detector.RefreshSelection();
            bool passed = result.HasSelection && result.SelectedCandidate.Type == WorldInteractionType.CorpseInspectOrCarry;

            if (passed)
                Debug.Log($"[InteractionRadiusValidator] PASS Priority selection: {result}");
            else
                Debug.LogError($"[InteractionRadiusValidator] FAIL Priority selection: {result}");

            Debug.Log($"[InteractionRadiusValidator] Prompt={_detector.GetPromptText()}");
        }

        private static void CreateCandidate(string displayName, WorldInteractionType type, Vector2 position)
        {
            GameObject candidate = new GameObject($"Interaction_{displayName}");
            candidate.transform.position = position;

            CircleCollider2D candidateCollider = candidate.AddComponent<CircleCollider2D>();
            candidateCollider.isTrigger = true;
            candidateCollider.radius = 0.25f;

            Rigidbody2D body = candidate.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;

            WorldInteractionCandidateProvider provider = candidate.AddComponent<WorldInteractionCandidateProvider>();
            provider.Configure(type, displayName, true);
        }

        private void RegisterCandidatesInRadius()
        {
            WorldInteractionCandidateProvider[] providers = FindObjectsByType<WorldInteractionCandidateProvider>(FindObjectsSortMode.None);

            foreach (WorldInteractionCandidateProvider provider in providers)
            {
                if (provider == null)
                    continue;

                float distance = Vector2.Distance(_detector.transform.position, provider.transform.position);

                if (distance > radius)
                    continue;

                _detector.RegisterProviderForDebug(provider);
            }
        }
    }
}
