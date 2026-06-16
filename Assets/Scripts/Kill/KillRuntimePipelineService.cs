using OFIS.Corpse;

namespace OFIS.Kill
{
    public sealed class KillRuntimePipelineService
    {
        private readonly KillExecutionService _executionService;
        private readonly CorpsePublicSpawnBridgeService _spawnBridgeService;

        public KillRuntimePipelineService(CorpsePublicSpawnBridgeService spawnBridgeService)
            : this(new KillExecutionService(), spawnBridgeService)
        {
        }

        public KillRuntimePipelineService(
            KillExecutionService executionService,
            CorpsePublicSpawnBridgeService spawnBridgeService)
        {
            _executionService = executionService ?? new KillExecutionService();
            _spawnBridgeService = spawnBridgeService;
        }

        public KillRuntimePipelineResult ExecuteKill(
            KillExecutionRequest request,
            KillCooldownState cooldownState)
        {
            KillExecutionResult executionResult =
                _executionService.Execute(request, cooldownState);

            if (!executionResult.Success)
                return KillRuntimePipelineResult.Rejected(executionResult);

            if (_spawnBridgeService == null)
            {
                return KillRuntimePipelineResult.SpawnFailed(
                    executionResult,
                    CorpsePublicSpawnBridgeResult.Rejected("CorpsePublicSpawnBridgeService missing."));
            }

            CorpsePublicSpawnBridgeResult spawnResult =
                _spawnBridgeService.TrySpawnPublicCorpse(executionResult.CorpseState);

            if (!spawnResult.Success)
                return KillRuntimePipelineResult.SpawnFailed(executionResult, spawnResult);

            return KillRuntimePipelineResult.Completed(executionResult, spawnResult);
        }
    }
}
