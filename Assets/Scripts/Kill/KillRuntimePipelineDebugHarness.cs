using OFIS.Core.Ids;
using OFIS.Corpse;
using OFIS.Meetings;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Kill
{
    public sealed class KillRuntimePipelineDebugHarness : MonoBehaviour
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

            ValidateKillRuntimePipeline();
        }

        [ContextMenu("Validate Kill Runtime Pipeline")]
        public void ValidateKillRuntimePipeline()
        {
            ValidateAcceptedKillCompletesPipeline();
            ValidateRejectedKillDoesNotSpawnCorpse();
        }

        private void ValidateAcceptedKillCompletesPipeline()
        {
            KillRuntimePipelineService pipelineService =
                new KillRuntimePipelineService(spawnBridgeService);
            KillCooldownState cooldownState = new KillCooldownState();
            KillExecutionRequest request = BuildRequest("kill_pipeline_accept", PlayerRole.Victim, true);

            KillRuntimePipelineResult result =
                pipelineService.ExecuteKill(request, cooldownState);

            bool hasCooldown = cooldownState.TryGetLastAcceptedKillTime(
                request.CommandContext.SenderId,
                out float recordedTime);

            bool passed = result.Success
                && result.ExecutionResult.Success
                && result.ExecutionResult.VictimLifeStateAfterKill == PlayerLifeState.Dead
                && result.SpawnResult.Success
                && result.SpawnResult.Corpse != null
                && result.SpawnResult.Corpse.CorpseId == request.CorpseId.ToString()
                && hasCooldown
                && Mathf.Approximately(recordedTime, request.CommandContext.ServerTimeSeconds);

            if (result.SpawnResult.Corpse != null)
                Destroy(result.SpawnResult.Corpse.gameObject);

            LogResult("AcceptedKillCompletesPipeline", passed, result.ToString());
        }

        private void ValidateRejectedKillDoesNotSpawnCorpse()
        {
            KillRuntimePipelineService pipelineService =
                new KillRuntimePipelineService(spawnBridgeService);
            KillExecutionRequest request = BuildRequest("kill_pipeline_reject", PlayerRole.Detective, false);

            KillRuntimePipelineResult result =
                pipelineService.ExecuteKill(request, new KillCooldownState());

            bool passed = !result.Success
                && !result.ExecutionResult.Success
                && result.SpawnResult.Corpse == null;

            LogResult("RejectedKillDoesNotSpawnCorpse", passed, result.ToString());
        }

        private static KillExecutionRequest BuildRequest(
            string commandId,
            PlayerRole targetRole,
            bool targetIsKnownVictim)
        {
            KillCommandContext context = new KillCommandContext(
                commandId,
                new PlayerId("killer_pipeline_01"),
                new PlayerId("victim_pipeline_01"),
                PlayerRole.Killer,
                targetRole,
                PlayerLifeState.Alive,
                targetIsKnownVictim,
                1f,
                1.75f,
                360f,
                -1f,
                60f,
                MeetingRuntimePhaseType.Office,
                OfficeRoomType.Hallway,
                false);

            return new KillExecutionRequest(
                context,
                new CorpseId($"corpse_{commandId}"),
                "Merve Kaya",
                new Vector3(7f, 2f, 0f));
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[KillRuntimePipelineDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[KillRuntimePipelineDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
