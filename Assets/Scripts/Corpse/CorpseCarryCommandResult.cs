namespace OFIS.Corpse
{
    public readonly struct CorpseCarryCommandResult
    {
        public bool Success { get; }
        public CorpsePlaceholder CarriedCorpse { get; }
        public string Message { get; }

        private CorpseCarryCommandResult(
            bool success,
            CorpsePlaceholder carriedCorpse,
            string message)
        {
            Success = success;
            CarriedCorpse = carriedCorpse;
            Message = message;
        }

        public static CorpseCarryCommandResult Accepted(CorpsePlaceholder corpse)
        {
            return new CorpseCarryCommandResult(
                true,
                corpse,
                "Corpse carry accepted.");
        }

        public static CorpseCarryCommandResult Rejected(string message)
        {
            return new CorpseCarryCommandResult(
                false,
                null,
                message);
        }

        public override string ToString()
        {
            string corpseName = CarriedCorpse == null ? "none" : CarriedCorpse.VictimName;
            return $"Success={Success}, Corpse={corpseName}, Message={Message}";
        }
    }
}
