using OFIS.LocalPlayer;
using OFIS.Rules;
using UnityEngine;

namespace OFIS.Interaction
{
    public readonly struct InteractionContext
    {
        public GameObject Actor { get; }
        public LocalPlayerIdentityBinding IdentityBinding { get; }
        public RoomBasedRuleGuard RuleGuard { get; }

        public InteractionContext(
            GameObject actor,
            LocalPlayerIdentityBinding identityBinding,
            RoomBasedRuleGuard ruleGuard)
        {
            Actor = actor;
            IdentityBinding = identityBinding;
            RuleGuard = ruleGuard;
        }
    }
}