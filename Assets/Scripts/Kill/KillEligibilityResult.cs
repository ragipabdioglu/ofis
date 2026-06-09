namespace OFIS.Kill
{
    public readonly struct KillEligibilityResult
    {
        public bool CanKill { get; }
        public string Reason { get; }

        public KillEligibilityResult(bool canKill, string reason)
        {
            CanKill = canKill;
            Reason = reason;
        }

        public static KillEligibilityResult Allowed()
        {
            return new KillEligibilityResult(true, "Allowed");
        }

        public static KillEligibilityResult Rejected(string reason)
        {
            return new KillEligibilityResult(false, reason);
        }
    }
}