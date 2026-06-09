namespace OFIS.Interactions
{
    public readonly struct WorldInteractionResolveResult
    {
        public bool HasSelection { get; }
        public WorldInteractionCandidate SelectedCandidate { get; }
        public string Reason { get; }

        private WorldInteractionResolveResult(bool hasSelection, WorldInteractionCandidate selectedCandidate, string reason)
        {
            HasSelection = hasSelection;
            SelectedCandidate = selectedCandidate;
            Reason = reason;
        }

        public static WorldInteractionResolveResult Selected(WorldInteractionCandidate candidate, string reason)
        {
            return new WorldInteractionResolveResult(true, candidate, reason);
        }

        public static WorldInteractionResolveResult None(string reason)
        {
            return new WorldInteractionResolveResult(false, default, reason);
        }

        public override string ToString()
        {
            return HasSelection
                ? $"Selected: {SelectedCandidate}. Reason={Reason}"
                : $"No selection. Reason={Reason}";
        }
    }
}
