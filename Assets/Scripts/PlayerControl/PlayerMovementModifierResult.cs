namespace OFIS.PlayerControl
{
    public readonly struct PlayerMovementModifierResult
    {
        public bool CanMove { get; }
        public float SpeedMultiplier { get; }
        public string Reason { get; }

        public PlayerMovementModifierResult(bool canMove, float speedMultiplier, string reason)
        {
            CanMove = canMove;
            SpeedMultiplier = speedMultiplier < 0f ? 0f : speedMultiplier;
            Reason = reason;
        }

        public override string ToString()
        {
            return $"CanMove={CanMove}, SpeedMultiplier={SpeedMultiplier:0.00}, Reason={Reason}";
        }
    }
}
