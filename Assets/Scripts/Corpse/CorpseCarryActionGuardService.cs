using OFIS.Rules;

namespace OFIS.Corpse
{
    public sealed class CorpseCarryActionGuardService
    {
        public PlayerActionRuleResult CanPerformWhileCarrying(
            PlayerActionType actionType,
            bool isCarryingCorpse)
        {
            if (!isCarryingCorpse)
                return PlayerActionRuleResult.Allow();

            switch (actionType)
            {
                case PlayerActionType.Kill:
                    return PlayerActionRuleResult.Deny("Kill blocked while carrying corpse.");

                case PlayerActionType.DoTask:
                    return PlayerActionRuleResult.Deny("Task blocked while carrying corpse.");

                case PlayerActionType.Sabotage:
                    return PlayerActionRuleResult.Deny("Sabotage blocked while carrying corpse.");

                case PlayerActionType.JoinMeeting:
                    return PlayerActionRuleResult.Deny("Meeting join blocked while carrying corpse.");

                case PlayerActionType.CarryCorpse:
                    return PlayerActionRuleResult.Deny("Cannot carry more than one corpse.");

                default:
                    return PlayerActionRuleResult.Allow();
            }
        }
    }
}
