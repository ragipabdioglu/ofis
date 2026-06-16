using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class CorpsePublicSpawnBridgeService : MonoBehaviour
    {
        [SerializeField] private CorpseSpawnService corpseSpawnService;

        private void Awake()
        {
            if (corpseSpawnService == null)
                corpseSpawnService = FindAnyObjectByType<CorpseSpawnService>();
        }

        public CorpsePublicSpawnBridgeResult TrySpawnPublicCorpse(CorpsePublicState publicState)
        {
            if (!publicState.IsPublicWorldObject)
                return CorpsePublicSpawnBridgeResult.Rejected("Corpse state is not public world object.");

            if (string.IsNullOrWhiteSpace(publicState.CorpseId.Value))
                return CorpsePublicSpawnBridgeResult.Rejected("Corpse id is required.");

            if (string.IsNullOrWhiteSpace(publicState.VictimId.Value))
                return CorpsePublicSpawnBridgeResult.Rejected("Victim id is required.");

            if (corpseSpawnService == null)
                return CorpsePublicSpawnBridgeResult.Rejected("CorpseSpawnService missing.");

            CorpsePlaceholder corpse = corpseSpawnService.SpawnCorpse(publicState);
            return CorpsePublicSpawnBridgeResult.Spawned(corpse);
        }
    }
}
