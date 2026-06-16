using OFIS.Corpse;
using OFIS.Players;

namespace OFIS.Kill
{
    public readonly struct KillExecutionResult
    {
        public bool Success { get; }
        public KillCommandValidationResult ValidationResult { get; }
        public PlayerLifeState VictimLifeStateAfterKill { get; }
        public CorpsePublicState CorpseState { get; }
        public string Message { get; }

        private KillExecutionResult(
            bool success,
            KillCommandValidationResult validationResult,
            PlayerLifeState victimLifeStateAfterKill,
            CorpsePublicState corpseState,
            string message)
        {
            Success = success;
            ValidationResult = validationResult;
            VictimLifeStateAfterKill = victimLifeStateAfterKill;
            CorpseState = corpseState;
            Message = message;
        }

        public static KillExecutionResult Rejected(KillCommandValidationResult validationResult)
        {
            return new KillExecutionResult(
                false,
                validationResult,
                PlayerLifeState.Alive,
                default,
                validationResult.Reason);
        }

        public static KillExecutionResult Accepted(CorpsePublicState corpseState)
        {
            return new KillExecutionResult(
                true,
                KillCommandValidationResult.Accepted(),
                PlayerLifeState.Dead,
                corpseState,
                "Kill accepted. Victim marked dead and public corpse state created.");
        }

        public override string ToString()
        {
            return $"Success={Success}, VictimLife={VictimLifeStateAfterKill}, Corpse={CorpseState.CorpseId}, Message={Message}";
        }
    }
}
