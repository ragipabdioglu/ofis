using OFIS.Core.Ids;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpsePublicSpawnBridgeDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private CorpsePublicSpawnBridgeService spawnBridgeService;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private void Awake()
        {
            if (spawnBridgeService == null)
                spawnBridgeService = FindAnyObjectByType<CorpsePublicSpawnBridgeService>();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidatePublicSpawnBridge();
        }

        [ContextMenu("Validate Public Corpse Spawn Bridge")]
        public void ValidatePublicSpawnBridge()
        {
            ValidateAcceptedPublicCorpseSpawnsWorldObject();
            ValidateNonPublicCorpseDoesNotSpawn();
        }

        private void ValidateAcceptedPublicCorpseSpawnsWorldObject()
        {
            if (spawnBridgeService == null)
            {
                LogResult("AcceptedPublicCorpseSpawnsWorldObject", false, "CorpsePublicSpawnBridgeService missing.");
                return;
            }

            CorpsePublicState state = new CorpsePublicState(
                new CorpseId("corpse_spawn_bridge_01"),
                new PlayerId("victim_spawn_bridge_01"),
                "Merve Kaya",
                new Vector3(6f, 2f, 0f),
                true);

            CorpsePublicSpawnBridgeResult result = spawnBridgeService.TrySpawnPublicCorpse(state);
            bool passed = result.Success
                && result.Corpse != null
                && result.Corpse.CorpseId == state.CorpseId.ToString()
                && result.Corpse.VictimPlayerId == state.VictimId.ToString()
                && result.Corpse.VictimName == state.VictimDisplayName
                && result.Corpse.IsPublicWorldObject
                && result.Corpse.transform.position == state.WorldPosition;

            if (result.Corpse != null)
                Destroy(result.Corpse.gameObject);

            LogResult("AcceptedPublicCorpseSpawnsWorldObject", passed, result.ToString());
        }

        private void ValidateNonPublicCorpseDoesNotSpawn()
        {
            if (spawnBridgeService == null)
            {
                LogResult("NonPublicCorpseDoesNotSpawn", false, "CorpsePublicSpawnBridgeService missing.");
                return;
            }

            CorpsePublicState state = new CorpsePublicState(
                new CorpseId("corpse_spawn_bridge_hidden"),
                new PlayerId("victim_spawn_bridge_hidden"),
                "Hidden Victim",
                new Vector3(6f, 3f, 0f),
                false);

            CorpsePublicSpawnBridgeResult result = spawnBridgeService.TrySpawnPublicCorpse(state);
            bool passed = !result.Success && result.Corpse == null;

            LogResult("NonPublicCorpseDoesNotSpawn", passed, result.ToString());
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[CorpsePublicSpawnBridgeDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpsePublicSpawnBridgeDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
