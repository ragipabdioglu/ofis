namespace OFIS.PlayerControl
{
    public readonly struct PlayerControlRestrictionResult
    {
        public bool CanMove { get; }
        public bool CanInteract { get; }
        public bool CanUseMeetingVote { get; }
        public bool IsSpectatorLike { get; }
        public string Reason { get; }

        public PlayerControlRestrictionResult(
            bool canMove,
            bool canInteract,
            bool canUseMeetingVote,
            bool isSpectatorLike,
            string reason)
        {
            CanMove = canMove;
            CanInteract = canInteract;
            CanUseMeetingVote = canUseMeetingVote;
            IsSpectatorLike = isSpectatorLike;
            Reason = reason;
        }

        public override string ToString()
        {
            return $"CanMove={CanMove}, CanInteract={CanInteract}, CanUseMeetingVote={CanUseMeetingVote}, IsSpectatorLike={IsSpectatorLike}, Reason={Reason}";
        }
    }
}
