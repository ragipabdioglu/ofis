using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OFIS.Interaction
{
    public sealed class PlayerInteractionDetector : MonoBehaviour
    {
        private readonly List<IInteractable> _nearbyInteractables = new();

        public IInteractable CurrentTarget { get; private set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var interactable = other.GetComponent<IInteractable>();

            if (interactable == null)
                return;

            if (!_nearbyInteractables.Contains(interactable))
                _nearbyInteractables.Add(interactable);

            RefreshTarget();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var interactable = other.GetComponent<IInteractable>();

            if (interactable == null)
                return;

            if (_nearbyInteractables.Contains(interactable))
                _nearbyInteractables.Remove(interactable);

            RefreshTarget();
        }

        private void Update()
        {
            RefreshTarget();
        }

        private void RefreshTarget()
        {
            CurrentTarget = _nearbyInteractables
                .Where(item => item != null && item.CanInteract)
                .OrderBy(item =>
                {
                    var component = item as Component;

                    if (component == null)
                        return float.MaxValue;

                    return Vector2.Distance(transform.position, component.transform.position);
                })
                .FirstOrDefault();
        }
    }
}