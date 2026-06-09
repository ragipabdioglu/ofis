using UnityEngine;

namespace OFIS.Spawning
{
    public sealed class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] private PlayerSpawnPointType spawnPointType = PlayerSpawnPointType.Default;
        [SerializeField] private int playerIndex = -1;
        [SerializeField] private string displayName = "Spawn Point";

        public PlayerSpawnPointType SpawnPointType => spawnPointType;
        public int PlayerIndex => playerIndex;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public Vector3 Position => transform.position;

        public bool Matches(int targetPlayerIndex, PlayerSpawnPointType preferredType)
        {
            bool typeMatches = preferredType == PlayerSpawnPointType.None || spawnPointType == preferredType;
            bool indexMatches = playerIndex < 0 || playerIndex == targetPlayerIndex;

            return typeMatches && indexMatches;
        }
    }
}
