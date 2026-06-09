using UnityEngine;

namespace OFIS.Interactions
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class WorldInteractionCandidateProvider : MonoBehaviour
    {
        [SerializeField] private WorldInteractionType interactionType = WorldInteractionType.Task;
        [SerializeField] private string displayName = "Interaction";
        [SerializeField] private bool isValid = true;

        public WorldInteractionType InteractionType => interactionType;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? interactionType.ToString() : displayName;
        public bool IsValid => isValid;

        private void Reset()
        {
            Collider2D targetCollider = GetComponent<Collider2D>();

            if (targetCollider != null)
                targetCollider.isTrigger = true;
        }

        private void Awake()
        {
            Collider2D targetCollider = GetComponent<Collider2D>();

            if (targetCollider != null && !targetCollider.isTrigger)
                targetCollider.isTrigger = true;
        }

        public WorldInteractionCandidate BuildCandidate(Transform playerTransform)
        {
            float distance = playerTransform == null
                ? 0f
                : Vector2.Distance(playerTransform.position, transform.position);

            return new WorldInteractionCandidate(
                interactionType,
                DisplayName,
                distance,
                isValid);
        }

        public void SetValid(bool value)
        {
            isValid = value;
        }
    }
}
