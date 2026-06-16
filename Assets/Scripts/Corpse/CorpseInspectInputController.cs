using UnityEngine;
using OFIS.Core.Ids;
using OFIS.Rooms;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpseInspectInputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CorpseDetector corpseDetector;
        [SerializeField] private FoundCorpseReportMemory reportMemory;
        [SerializeField] private CorpseOwnerKnowledgeMemory ownerKnowledgeMemory;

        [Header("Debug Owner Context")]
        [SerializeField] private string inspectorPlayerId = "local_player";
        [SerializeField] private OfficeRoomType currentRoom = OfficeRoomType.Unknown;

        [Header("Input")]
        [SerializeField] private KeyCode inspectKey = KeyCode.I;

        private readonly CorpseInspectService _inspectService = new CorpseInspectService();

        private void Awake()
        {
            if (corpseDetector == null)
                corpseDetector = FindAnyObjectByType<CorpseDetector>();

            if (reportMemory == null)
                reportMemory = FindAnyObjectByType<FoundCorpseReportMemory>();

            if (ownerKnowledgeMemory == null)
                ownerKnowledgeMemory = FindAnyObjectByType<CorpseOwnerKnowledgeMemory>();
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
            CorpseInspectResult inspectResult = _inspectService.Inspect(
                new CorpseInspectRequest(
                    $"inspect_{Time.frameCount}",
                    new PlayerId(inspectorPlayerId),
                    corpse,
                    currentRoom,
                    Time.time));

            if (inspectResult.Success && ownerKnowledgeMemory != null)
                ownerKnowledgeMemory.TryRecord(inspectResult.Knowledge);

            if (recorded || inspectResult.Success)
            {
                Debug.Log(
                    $"[CorpseInspect] Accepted: Found corpse. " +
                    $"Victim={corpse.VictimName}, OwnerKnowledge={inspectResult.Success}");
                return;
            }

            Debug.Log($"[CorpseInspect] Already recorded: Victim={corpse.VictimName}");
        }
    }
}
#pragma warning restore 0414
