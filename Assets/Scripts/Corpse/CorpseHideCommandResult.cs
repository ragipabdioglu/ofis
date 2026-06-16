namespace OFIS.Corpse
{
    public readonly struct CorpseHideCommandResult
    {
        public bool Success { get; }
        public CorpsePlaceholder HiddenCorpse { get; }
        public string HideSpotId { get; }
        public bool CarryStateCleared { get; }
        public bool CorpseHiddenFromPublicWorld { get; }
        public string Message { get; }

        private CorpseHideCommandResult(
            bool success,
            CorpsePlaceholder hiddenCorpse,
            string hideSpotId,
            bool carryStateCleared,
            bool corpseHiddenFromPublicWorld,
            string message)
        {
            Success = success;
            HiddenCorpse = hiddenCorpse;
            HideSpotId = hideSpotId;
            CarryStateCleared = carryStateCleared;
            CorpseHiddenFromPublicWorld = corpseHiddenFromPublicWorld;
            Message = message;
        }

        public static CorpseHideCommandResult Hidden(
            CorpsePlaceholder corpse,
            string hideSpotId,
            bool carryStateCleared)
        {
            return new CorpseHideCommandResult(
                true,
                corpse,
                hideSpotId,
                carryStateCleared,
                true,
                "Corpse hidden.");
        }

        public static CorpseHideCommandResult Rejected(string message)
        {
            return new CorpseHideCommandResult(
                false,
                null,
                string.Empty,
                false,
                false,
                message);
        }

        public override string ToString()
        {
            string corpseName = HiddenCorpse == null ? "none" : HiddenCorpse.VictimName;
            return $"Success={Success}, Corpse={corpseName}, HideSpot={HideSpotId}, Cleared={CarryStateCleared}, Hidden={CorpseHiddenFromPublicWorld}, Message={Message}";
        }
    }
}
