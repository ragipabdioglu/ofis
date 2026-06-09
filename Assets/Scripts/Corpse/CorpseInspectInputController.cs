using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OFIS.Corpse
{
    public sealed class CorpseInspectInputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CorpseDetector corpseDetector;
        [SerializeField] private FoundCorpseReportMemory reportMemory;

        [Header("Input")]
        [SerializeField] private KeyCode inspectKey = KeyCode.I;

        private void Awake()
        {
            if (corpseDetector == null)
                corpseDetector = FindAnyObjectByType<CorpseDetector>();

            if (reportMemory == null)
                reportMemory = FindAnyObjectByType<FoundCorpseReportMemory>();
        }

        private void Update()
        {
            if (!WasInspectKeyPressed())
                return;

            TryInspectCorpse();
        }

        private bool WasInspectKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(inspectKey);
#endif
        }

        public void TryInspectCorpse()
        {
            if (corpseDetector == null)
            {
                Debug.LogWarning("[CorpseInspect] Failed: CorpseDetector missing.");
                return;
            }

            if (reportMemory == null)
            {
                Debug.LogWarning("[CorpseInspect] Failed: FoundCorpseReportMemory missing.");
                return;
            }

            CorpsePlaceholder corpse = corpseDetector.CurrentCorpse;

            if (corpse == null)
            {
                Debug.Log("[CorpseInspect] Failed: No nearby corpse.");
                return;
            }

            bool recorded = reportMemory.TryRecordFoundCorpse(corpse);

            if (recorded)
            {
                Debug.Log($"[CorpseInspect] Accepted: Found corpse. Victim={corpse.VictimName}");
                return;
            }

            Debug.Log($"[CorpseInspect] Already recorded: Victim={corpse.VictimName}");
        }
    }
}