namespace OFIS.Rules
{
    public readonly struct PlayerActionRuleResult
    {
        public bool IsAllowed { get; }
        public string Reason { get; }

        private PlayerActionRuleResult(bool isAllowed, string reason)
        {
            IsAllowed = isAllowed;
            Reason = reason;
        }

        public static PlayerActionRuleResult Allow()
        {
            return new PlayerActionRuleResult(true, "Allowed");
        }

        public static PlayerActionRuleResult Deny(string reason)
        {
            return new PlayerActionRuleResult(false, reason);
        }
    }
}