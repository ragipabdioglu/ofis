namespace OFIS.Corpse
{
    public readonly struct CorpseInspectResult
    {
        public bool Success { get; }
        public CorpseOwnerKnowledge Knowledge { get; }
        public string Message { get; }

        private CorpseInspectResult(
            bool success,
            CorpseOwnerKnowledge knowledge,
            string message)
        {
            Success = success;
            Knowledge = knowledge;
            Message = message;
        }

        public static CorpseInspectResult Accepted(CorpseOwnerKnowledge knowledge)
        {
            return new CorpseInspectResult(
                true,
                knowledge,
                "Corpse inspected. Owner-only knowledge created.");
        }

        public static CorpseInspectResult Rejected(string message)
        {
            return new CorpseInspectResult(
                false,
                default,
                message);
        }

        public override string ToString()
        {
            return $"Success={Success}, Knowledge={Knowledge}, Message={Message}";
        }
    }
}
