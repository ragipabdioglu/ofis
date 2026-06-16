using OFIS.Corpse;
using OFIS.Players;

namespace OFIS.Kill
{
    public sealed class KillExecutionService
    {
        private readonly KillCommandValidationService _validationService;

        public KillExecutionService()
            : this(new KillCommandValidationService())
        {
        }

        public KillExecutionService(KillCommandValidationService validationService)
        {
            _validationService = validationService ?? new KillCommandValidationService();
        }

        public KillExecutionResult Execute(
            KillExecutionRequest request,
            KillCooldownState cooldownState)
        {
            KillCommandValidationResult validationResult =
                _validationService.Validate(request.CommandContext);

            if (!validationResult.IsAccepted)
                return KillExecutionResult.Rejected(validationResult);

            CorpsePublicState corpseState = new CorpsePublicState(
                request.CorpseId,
                request.CommandContext.TargetId,
                request.VictimDisplayName,
                request.DeathPosition,
                true);

            if (cooldownState != null)
                cooldownState.RecordAcceptedKill(
                    request.CommandContext.SenderId,
                    request.CommandContext.ServerTimeSeconds);

            return KillExecutionResult.Accepted(corpseState);
        }

        public PlayerPublicState BuildDeadPublicState(PlayerPublicState currentState)
        {
            if (currentState == null)
                return null;

            return new PlayerPublicState(
                currentState.PlayerId,
                currentState.DisplayName,
                currentState.PublicIdentity,
                PlayerLifeState.Dead);
        }
    }
}
