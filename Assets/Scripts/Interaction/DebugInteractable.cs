using UnityEngine;

namespace OFIS.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class DebugInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string displayName = "Debug Interactable";
        [SerializeField] private InteractionType interactionType = InteractionType.DebugObject;
        [SerializeField] private bool canInteract = true;

        public string DisplayName => displayName;
        public InteractionType InteractionType => interactionType;
        public bool CanInteract => canInteract;

        private void Reset()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void Awake()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        public void Interact(InteractionContext context)
        {
            string actorName = context.IdentityBinding == null
                ? context.Actor.name
                : context.IdentityBinding.DisplayName;

            Debug.Log($"[Interaction] {actorName} interacted with {displayName} ({interactionType}).");
        }
    }
}