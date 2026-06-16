namespace OFIS.Corpse
{
    public readonly struct CorpsePublicSpawnBridgeResult
    {
        public bool Success { get; }
        public CorpsePlaceholder Corpse { get; }
        public string Message { get; }

        private CorpsePublicSpawnBridgeResult(
            bool success,
            CorpsePlaceholder corpse,
            string message)
        {
            Success = success;
            Corpse = corpse;
            Message = message;
        }

        public static CorpsePublicSpawnBridgeResult Spawned(CorpsePlaceholder corpse)
        {
            return new CorpsePublicSpawnBridgeResult(
                true,
                corpse,
                "Public corpse spawned.");
        }

        public static CorpsePublicSpawnBridgeResult Rejected(string message)
        {
            return new CorpsePublicSpawnBridgeResult(
                false,
                null,
                message);
        }

        public override string ToString()
        {
            string corpseName = Corpse == null ? "none" : Corpse.VictimName;
            return $"Success={Success}, Corpse={corpseName}, Message={Message}";
        }
    }
}
