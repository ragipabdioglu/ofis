using UnityEngine;

namespace OFIS.Spawning
{
    public readonly struct PlayerSpawnSelectionResult
    {
        public bool Success { get; }
        public PlayerSpawnPoint SpawnPoint { get; }
        public Vector3 Position { get; }
        public string Reason { get; }

        private PlayerSpawnSelectionResult(bool success, PlayerSpawnPoint spawnPoint, Vector3 position, string reason)
        {
            Success = success;
            SpawnPoint = spawnPoint;
            Position = position;
            Reason = reason;
        }

        public static PlayerSpawnSelectionResult Selected(PlayerSpawnPoint spawnPoint, string reason)
        {
            return new PlayerSpawnSelectionResult(true, spawnPoint, spawnPoint.Position, reason);
        }

        public static PlayerSpawnSelectionResult Failed(string reason)
        {
            return new PlayerSpawnSelectionResult(false, null, Vector3.zero, reason);
        }

        public override string ToString()
        {
            return Success
                ? $"Selected={SpawnPoint.DisplayName}, Position={Position}, Reason={Reason}"
                : $"Failed. Reason={Reason}";
        }
    }
}
