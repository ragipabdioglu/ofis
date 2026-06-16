using OFIS.Meetings;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Rooms;

namespace OFIS.Kill
{
    public sealed class KillCommandValidationService
    {
        public KillCommandValidationResult Validate(KillCommandContext context)
        {
            if (string.IsNullOrWhiteSpace(context.CommandId))
                return KillCommandValidationResult.Rejected("Command id is required.");

            if (string.IsNullOrWhiteSpace(context.SenderId.Value))
                return KillCommandValidationResult.Rejected("Sender id is required.");

            if (string.IsNullOrWhiteSpace(context.TargetId.Value))
                return KillCommandValidationResult.Rejected("Target id is required.");

            if (context.SenderId == context.TargetId)
                return KillCommandValidationResult.Rejected("Killer cannot target self.");

            if (context.SenderRole != PlayerRole.Killer)
                return KillCommandValidationResult.Rejected($"Sender is not Killer. SenderRole={context.SenderRole}");

            if (context.TargetRole != PlayerRole.Victim)
                return KillCommandValidationResult.Rejected($"Target is not Victim. TargetRole={context.TargetRole}");

            if (!context.TargetIsKnownVictim)
                return KillCommandValidationResult.Rejected("Target is not in killer known victim target list.");

            if (context.TargetLifeState != PlayerLifeState.Alive)
                return KillCommandValidationResult.Rejected($"Target is not alive. TargetLifeState={context.TargetLifeState}");

            if (context.DistanceToTarget < 0f)
                return KillCommandValidationResult.Rejected("Distance cannot be negative.");

            if (context.MaxKillRange <= 0f)
                return KillCommandValidationResult.Rejected("Max kill range must be greater than zero.");

            if (context.DistanceToTarget > context.MaxKillRange)
                return KillCommandValidationResult.Rejected(
                    $"Target out of range. Distance={context.DistanceToTarget:0.00}, Range={context.MaxKillRange:0.00}");

            float cooldownLeft = GetRemainingCooldownSeconds(context);
            if (cooldownLeft > 0f)
                return KillCommandValidationResult.Rejected("Kill cooldown is active.", cooldownLeft);

            if (context.PhaseType == MeetingRuntimePhaseType.Meeting
                || context.PhaseType == MeetingRuntimePhaseType.FinalMeeting)
                return KillCommandValidationResult.Rejected($"Kill blocked during meeting phase. Phase={context.PhaseType}");

            if (context.SenderRoom == OfficeRoomType.MeetingRoom)
                return KillCommandValidationResult.Rejected("Kill blocked inside meeting room.");

            if (context.SenderIsCarryingCorpse)
                return KillCommandValidationResult.Rejected("Kill blocked while carrying corpse.");

            return KillCommandValidationResult.Accepted();
        }

        private static float GetRemainingCooldownSeconds(KillCommandContext context)
        {
            if (context.CooldownSeconds <= 0f)
                return 0f;

            if (context.LastAcceptedKillTimeSeconds < 0f)
                return 0f;

            float elapsed = context.ServerTimeSeconds - context.LastAcceptedKillTimeSeconds;
            float remaining = context.CooldownSeconds - elapsed;
            return remaining > 0f ? remaining : 0f;
        }
    }
}
