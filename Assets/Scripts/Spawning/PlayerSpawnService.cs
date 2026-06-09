using System.Collections.Generic;
using System.Linq;

namespace OFIS.Spawning
{
    public sealed class PlayerSpawnService
    {
        public PlayerSpawnSelectionResult SelectSpawnPoint(
            IReadOnlyList<PlayerSpawnPoint> spawnPoints,
            int playerIndex,
            PlayerSpawnPointType preferredType = PlayerSpawnPointType.Default)
        {
            if (spawnPoints == null || spawnPoints.Count == 0)
                return PlayerSpawnSelectionResult.Failed("No spawn points provided.");

            PlayerSpawnPoint exactMatch = spawnPoints
                .Where(point => point != null)
                .FirstOrDefault(point => point.SpawnPointType == preferredType && point.PlayerIndex == playerIndex);

            if (exactMatch != null)
                return PlayerSpawnSelectionResult.Selected(exactMatch, "Exact type and player index match.");

            PlayerSpawnPoint typeMatch = spawnPoints
                .Where(point => point != null)
                .FirstOrDefault(point => point.SpawnPointType == preferredType && point.PlayerIndex < 0);

            if (typeMatch != null)
                return PlayerSpawnSelectionResult.Selected(typeMatch, "Preferred type fallback match.");

            PlayerSpawnPoint anyDefault = spawnPoints
                .Where(point => point != null)
                .FirstOrDefault(point => point.SpawnPointType == PlayerSpawnPointType.Default);

            if (anyDefault != null)
                return PlayerSpawnSelectionResult.Selected(anyDefault, "Default fallback match.");

            PlayerSpawnPoint anyPoint = spawnPoints.FirstOrDefault(point => point != null);

            if (anyPoint != null)
                return PlayerSpawnSelectionResult.Selected(anyPoint, "Any spawn point fallback match.");

            return PlayerSpawnSelectionResult.Failed("All spawn point references are null.");
        }
    }
}
