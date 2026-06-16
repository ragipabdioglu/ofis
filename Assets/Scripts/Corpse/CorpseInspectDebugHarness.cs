using OFIS.Core.Ids;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpseInspectDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly CorpseInspectService _inspectService = new CorpseInspectService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateCorpseInspect();
        }

        [ContextMenu("Validate Corpse Inspect")]
        public void ValidateCorpseInspect()
        {
            ValidateInspectCreatesOwnerOnlyKnowledge();
            ValidateKnowledgeMemoryDoesNotDuplicate();
            ValidateMissingCorpseRejects();
        }

        private void ValidateInspectCreatesOwnerOnlyKnowledge()
        {
            CorpsePlaceholder corpse = BuildCorpse();
            CorpseInspectRequest request = new CorpseInspectRequest(
                "inspect_7e_owner",
                new PlayerId("detective_01"),
                corpse,
                OfficeRoomType.ArchiveRoom,
                420f);

            CorpseInspectResult result = _inspectService.Inspect(request);

            bool passed = result.Success
                && result.Knowledge.IsOwnerOnly
                && result.Knowledge.OwnerPlayerId == request.InspectorPlayerId
                && result.Knowledge.CorpseId.ToString() == corpse.CorpseId
                && result.Knowledge.VictimDisplayName == corpse.VictimName;

            Destroy(corpse.gameObject);
            LogResult("InspectCreatesOwnerOnlyKnowledge", passed, result.ToString());
        }

        private void ValidateKnowledgeMemoryDoesNotDuplicate()
        {
            CorpsePlaceholder corpse = BuildCorpse();
            CorpseInspectResult result = _inspectService.Inspect(
                new CorpseInspectRequest(
                    "inspect_7e_memory",
                    new PlayerId("detective_02"),
                    corpse,
                    OfficeRoomType.SecurityRoom,
                    440f));

            CorpseOwnerKnowledgeMemory memory =
                new GameObject("CorpseInspectDebug_Memory").AddComponent<CorpseOwnerKnowledgeMemory>();

            bool firstRecorded = memory.TryRecord(result.Knowledge);
            bool secondRecorded = memory.TryRecord(result.Knowledge);
            bool passed = result.Success && firstRecorded && !secondRecorded && memory.Count == 1;

            Destroy(corpse.gameObject);
            Destroy(memory.gameObject);
            LogResult("KnowledgeMemoryDoesNotDuplicate", passed, result.ToString());
        }

        private void ValidateMissingCorpseRejects()
        {
            CorpseInspectResult result = _inspectService.Inspect(
                new CorpseInspectRequest(
                    "inspect_7e_missing",
                    new PlayerId("detective_03"),
                    null,
                    OfficeRoomType.Hallway,
                    450f));

            LogResult("MissingCorpseRejects", !result.Success, result.ToString());
        }

        private static CorpsePlaceholder BuildCorpse()
        {
            GameObject corpseObject = new GameObject("CorpseInspectDebug_Corpse");
            corpseObject.AddComponent<BoxCollider2D>().isTrigger = true;
            CorpsePlaceholder corpse = corpseObject.AddComponent<CorpsePlaceholder>();
            corpse.Initialize(
                new CorpsePublicState(
                    new CorpseId("corpse_inspect_7e"),
                    new PlayerId("victim_inspect_7e"),
                    "Merve Kaya",
                    new Vector3(2f, 2f, 0f),
                    true));
            return corpse;
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[CorpseInspectDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpseInspectDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
