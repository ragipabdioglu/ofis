namespace OFIS.Kill
{
    public readonly struct KillCommandValidationResult
    {
        public bool IsAccepted { get; }
        public string Reason { get; }
        public float RemainingCooldownSeconds { get; }

        private KillCommandValidationResult(
            bool isAccepted,
            string reason,
            float remainingCooldownSeconds)
        {
            IsAccepted = isAccepted;
            Reason = reason;
            RemainingCooldownSeconds = remainingCooldownSeconds;
        }

        public static KillCommandValidationResult Accepted()
        {
            return new KillCommandValidationResult(true, "Accepted", 0f);
        }

        public static KillCommandValidationResult Rejected(
            string reason,
            float remainingCooldownSeconds = 0f)
        {
            return new KillCommandValidationResult(false, reason, remainingCooldownSeconds);
        }

        public override string ToString()
        {
            return $"Accepted={IsAccepted}, CooldownLeft={RemainingCooldownSeconds:0.00}, Reason={Reason}";
        }
    }
}
