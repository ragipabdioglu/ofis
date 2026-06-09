using OFIS.LocalPlayer;
using OFIS.Rules;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OFIS.Interaction
{
    [RequireComponent(typeof(PlayerInteractionDetector))]
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField] private KeyCode legacyInteractKey = KeyCode.E;

        private PlayerInteractionDetector _detector;
        private LocalPlayerIdentityBinding _identityBinding;
        private RoomBasedRuleGuard _ruleGuard;

        private void Awake()
        {
            _detector = GetComponent<PlayerInteractionDetector>();
            _identityBinding = GetComponent<LocalPlayerIdentityBinding>();
            _ruleGuard = GetComponent<RoomBasedRuleGuard>();
        }

        private void Update()
        {
            if (!WasInteractPressed())
                return;

            TryInteract();
        }

        private void TryInteract()
        {
            var target = _detector.CurrentTarget;

            if (target == null)
            {
                Debug.Log("[Interaction] No nearby interactable target.");
                return;
            }

            var context = new InteractionContext(
                gameObject,
                _identityBinding,
                _ruleGuard);

            target.Interact(context);
        }

        private bool WasInteractPressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
                return Keyboard.current.eKey.wasPressedThisFrame;
#endif

            return Input.GetKeyDown(legacyInteractKey);
        }
    }
}