using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OFIS.Corpse
{
    public sealed class CorpseCarryInputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CorpseDetector corpseDetector;
        [SerializeField] private CorpseCarryEligibilityService carryEligibilityService;
        [SerializeField] private CorpseCarryState carryState;

        [Header("Input")]
        [SerializeField] private KeyCode carryKey = KeyCode.C;

        [Header("Debug Room Rule")]
        [SerializeField] private bool roomAllowsCarry = true;

        public bool RoomAllowsCarry => roomAllowsCarry;

        private void Awake()
        {
            if (corpseDetector == null)
                corpseDetector = FindAnyObjectByType<CorpseDetector>();

            if (carryEligibilityService == null)
                carryEligibilityService = FindAnyObjectByType<CorpseCarryEligibilityService>();

            if (carryState == null)
                carryState = FindAnyObjectByType<CorpseCarryState>();
        }

        private void Update()
        {
            if (!WasCarryKeyPressed())
                return;

            ToggleCarry();
        }

        private bool WasCarryKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(carryKey);
#endif
        }

        public void ToggleCarry()
        {
            if (carryState == null)
            {
                Debug.LogWarning("[CorpseCarryInput] Failed: CorpseCarryState missing.");
                return;
            }

            if (carryState.IsCarrying)
            {
                carryState.DropCarriedCorpse();
                return;
            }

            TryStartCarry();
        }

        private void TryStartCarry()
        {
            if (corpseDetector == null)
            {
                Debug.LogWarning("[CorpseCarryInput] Failed: CorpseDetector missing.");
                return;
            }

            if (carryEligibilityService == null)
            {
                Debug.LogWarning("[CorpseCarryInput] Failed: CorpseCarryEligibilityService missing.");
                return;
            }

            CorpsePlaceholder corpse = corpseDetector.CurrentCorpse;

            CorpseCarryEligibilityResult result = carryEligibilityService.CanCarryCorpse(
                corpse,
                roomAllowsCarry,
                carryState != null && carryState.IsCarrying);

            if (!result.CanCarry)
            {
                Debug.Log($"[CorpseCarryInput] Blocked: Reason={result.Reason}");
                return;
            }

            carryState.StartCarrying(corpse);
        }

        public void ToggleRoomAllowsCarryForDebug()
        {
            roomAllowsCarry = !roomAllowsCarry;
            Debug.Log($"[CorpseCarryInput] RoomAllowsCarry={roomAllowsCarry}");
        }
    }
}