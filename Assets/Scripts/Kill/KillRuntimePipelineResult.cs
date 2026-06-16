using OFIS.Corpse;

namespace OFIS.Kill
{
    public readonly struct KillRuntimePipelineResult
    {
        public bool Success { get; }
        public KillExecutionResult ExecutionResult { get; }
        public CorpsePublicSpawnBridgeResult SpawnResult { get; }
        public string Message { get; }

        private KillRuntimePipelineResult(
            bool success,
            KillExecutionResult executionResult,
            CorpsePublicSpawnBridgeResult spawnResult,
            string message)
        {
            Success = success;
            ExecutionResult = executionResult;
            SpawnResult = spawnResult;
            Message = message;
        }

        public static KillRuntimePipelineResult Rejected(KillExecutionResult executionResult)
        {
            return new KillRuntimePipelineResult(
                false,
                executionResult,
                default,
                executionResult.Message);
        }

        public static KillRuntimePipelineResult SpawnFailed(
            KillExecutionResult executionResult,
            CorpsePublicSpawnBridgeResult spawnResult)
        {
            return new KillRuntimePipelineResult(
                false,
                executionResult,
                spawnResult,
                spawnResult.Message);
        }

        public static KillRuntimePipelineResult Completed(
            KillExecutionResult executionResult,
            CorpsePublicSpawnBridgeResult spawnResult)
        {
            return new KillRuntimePipelineResult(
                true,
                executionResult,
                spawnResult,
                "Kill runtime pipeline completed.");
        }

        public override string ToString()
        {
            return $"Success={Success}, Execution={ExecutionResult.Success}, Spawn={SpawnResult.Success}, Message={Message}";
        }
    }
}
