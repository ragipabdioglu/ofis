using System.Collections.Generic;
using OFIS.PlayerControl;
using OFIS.Players;
using UnityEngine;

namespace OFIS.Spawning
{
    public sealed class PlayerSpawnAndRestrictionDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly PlayerSpawnService _spawnService = new PlayerSpawnService();
        private readonly PlayerControlRestrictionService _restrictionService = new PlayerControlRestrictionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateSpawnAndRestrictions();
        }

        [ContextMenu("Validate Spawn And Restrictions")]
        public void ValidateSpawnAndRestrictions()
        {
            List<PlayerSpawnPoint> spawnPoints = BuildSpawnPoints();

            ValidateExactSpawn(spawnPoints);
            ValidateFallbackSpawn(spawnPoints);
            ValidateAliveRestrictions();
            ValidateDeadRestrictions();
            ValidateDisconnectedRestrictions();
        }

        private static List<PlayerSpawnPoint> BuildSpawnPoints()
        {
            List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();

            spawnPoints.Add(CreateSpawnPoint("Spawn_Player_0", PlayerSpawnPointType.Default, 0, new Vector3(-2f, 0f, 0f)));
            spawnPoints.Add(CreateSpawnPoint("Spawn_Player_1", PlayerSpawnPointType.Default, 1, new Vector3(2f, 0f, 0f)));
            spawnPoints.Add(CreateSpawnPoint("Spawn_Spectator", PlayerSpawnPointType.Spectator, -1, new Vector3(0f, 4f, 0f)));

            return spawnPoints;
        }

        private static PlayerSpawnPoint CreateSpawnPoint(string objectName, PlayerSpawnPointType type, int playerIndex, Vector3 position)
        {
            GameObject spawnObject = new GameObject(objectName);
            spawnObject.transform.position = position;

            PlayerSpawnPoint spawnPoint = spawnObject.AddComponent<PlayerSpawnPoint>();
            spawnPoint.ConfigureForDebug(type, playerIndex, objectName);

            return spawnPoint;
        }

        private void ValidateExactSpawn(IReadOnlyList<PlayerSpawnPoint> spawnPoints)
        {
            PlayerSpawnSelectionResult result = _spawnService.SelectSpawnPoint(spawnPoints, 1, PlayerSpawnPointType.Default);
            bool passed = result.Success && result.SpawnPoint != null && result.SpawnPoint.PlayerIndex == 1;

            LogSpawnResult("ExactSpawn", passed, result);
        }

        private void ValidateFallbackSpawn(IReadOnlyList<PlayerSpawnPoint> spawnPoints)
        {
            PlayerSpawnSelectionResult result = _spawnService.SelectSpawnPoint(spawnPoints, 99, PlayerSpawnPointType.Spectator);
            bool passed = result.Success && result.SpawnPoint != null && result.SpawnPoint.SpawnPointType == PlayerSpawnPointType.Spectator;

            LogSpawnResult("FallbackSpawn", passed, result);
        }

        private void ValidateAliveRestrictions()
        {
            PlayerControlRestrictionResult result = _restrictionService.Evaluate(PlayerLifeState.Alive);
            bool passed = result.CanMove && result.CanInteract && result.CanUseMeetingVote && !result.IsSpectatorLike;

            LogRestrictionResult("AliveRestrictions", passed, result);
        }

        private void ValidateDeadRestrictions()
        {
            PlayerControlRestrictionResult result = _restrictionService.Evaluate(PlayerLifeState.Dead);
            bool passed = !result.CanMove && !result.CanInteract && !result.CanUseMeetingVote && result.IsSpectatorLike;

            LogRestrictionResult("DeadRestrictions", passed, result);
        }

        private void ValidateDisconnectedRestrictions()
        {
            PlayerControlRestrictionResult result = _restrictionService.Evaluate(PlayerLifeState.Disconnected);
            bool passed = !result.CanMove && !result.CanInteract && !result.CanUseMeetingVote && !result.IsSpectatorLike;

            LogRestrictionResult("DisconnectedRestrictions", passed, result);
        }

        private static void LogSpawnResult(string testName, bool passed, PlayerSpawnSelectionResult result)
        {
            if (passed)
                Debug.Log($"[SpawnRestrictionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[SpawnRestrictionValidator] FAIL {testName}: {result}");
        }

        private static void LogRestrictionResult(string testName, bool passed, PlayerControlRestrictionResult result)
        {
            if (passed)
                Debug.Log($"[SpawnRestrictionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[SpawnRestrictionValidator] FAIL {testName}: {result}");
        }
    }
}
