using System.Collections.Generic;
using System.Linq;

namespace OFIS.Interactions
{
    public sealed class LocalInteractionResolver
    {
        public WorldInteractionResolveResult Resolve(IReadOnlyList<WorldInteractionCandidate> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                return WorldInteractionResolveResult.None("No candidates.");

            List<WorldInteractionCandidate> validCandidates = candidates
                .Where(candidate => candidate.IsValid && candidate.Type != WorldInteractionType.None)
                .ToList();

            if (validCandidates.Count == 0)
                return WorldInteractionResolveResult.None("No valid candidates.");

            WorldInteractionCandidate selected = validCandidates
                .OrderByDescending(candidate => candidate.Priority)
                .ThenBy(candidate => candidate.Distance)
                .First();

            return WorldInteractionResolveResult.Selected(
                selected,
                "Highest priority wins. Distance is used only as a tie-breaker.");
        }
    }
}
