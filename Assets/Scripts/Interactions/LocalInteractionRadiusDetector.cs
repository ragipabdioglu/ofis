using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OFIS.Interactions
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class LocalInteractionRadiusDetector : MonoBehaviour
    {
        private readonly List<WorldInteractionCandidateProvider> _activeProviders = new();
        private readonly LocalInteractionResolver _resolver = new();

        public WorldInteractionResolveResult CurrentResolveResult { get; private set; } = WorldInteractionResolveResult.None("Not evaluated yet.");
        public bool HasSelection => CurrentResolveResult.HasSelection;
        public WorldInteractionCandidate SelectedCandidate => CurrentResolveResult.SelectedCandidate;
        public IReadOnlyList<WorldInteractionCandidateProvider> ActiveProviders => _activeProviders;

        private void Reset()
        {
            Collider2D radiusCollider = GetComponent<Collider2D>();

            if (radiusCollider != null)
                radiusCollider.isTrigger = true;
        }

        private void Awake()
        {
            Collider2D radiusCollider = GetComponent<Collider2D>();

            if (radiusCollider != null && !radiusCollider.isTrigger)
                radiusCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            WorldInteractionCandidateProvider provider = other.GetComponent<WorldInteractionCandidateProvider>();

            if (provider == null)
                return;

            if (!_activeProviders.Contains(provider))
                _activeProviders.Add(provider);

            RefreshSelection();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            WorldInteractionCandidateProvider provider = other.GetComponent<WorldInteractionCandidateProvider>();

            if (provider == null)
                return;

            if (_activeProviders.Contains(provider))
                _activeProviders.Remove(provider);

            RefreshSelection();
        }

        public WorldInteractionResolveResult RefreshSelection()
        {
            List<WorldInteractionCandidate> candidates = _activeProviders
                .Where(provider => provider != null)
                .Select(provider => provider.BuildCandidate(transform))
                .ToList();

            CurrentResolveResult = _resolver.Resolve(candidates);
            return CurrentResolveResult;
        }

        public string GetPromptText()
        {
            RefreshSelection();

            if (!CurrentResolveResult.HasSelection)
                return "No interaction";

            return $"Interact: {CurrentResolveResult.SelectedCandidate.DisplayName}";
        }
    }
}
