using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Interactions
{
    public sealed class LocalInteractionDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly LocalInteractionResolver _resolver = new();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateResolver();
        }

        [ContextMenu("Validate Interaction Resolver")]
        public void ValidateResolver()
        {
            ValidateHighestPriorityWins();
            ValidateDistanceTieBreaker();
            ValidateInvalidCandidatesIgnored();
            ValidateNoCandidates();
        }

        private void ValidateHighestPriorityWins()
        {
            List<WorldInteractionCandidate> candidates = new()
            {
                new WorldInteractionCandidate(WorldInteractionType.Task, "Nearby Task", 0.25f),
                new WorldInteractionCandidate(WorldInteractionType.DoorPanel, "Nearest Door Panel", 0.10f),
                new WorldInteractionCandidate(WorldInteractionType.CorpseInspectOrCarry, "Farther Corpse", 1.10f),
                new WorldInteractionCandidate(WorldInteractionType.VictimNote, "Victim Note", 0.40f)
            };

            WorldInteractionResolveResult result = _resolver.Resolve(candidates);
            bool passed = result.HasSelection && result.SelectedCandidate.Type == WorldInteractionType.CorpseInspectOrCarry;

            LogResult("HighestPriorityWins", passed, result);
        }

        private void ValidateDistanceTieBreaker()
        {
            List<WorldInteractionCandidate> candidates = new()
            {
                new WorldInteractionCandidate(WorldInteractionType.Task, "Far Task", 1.25f),
                new WorldInteractionCandidate(WorldInteractionType.Task, "Near Task", 0.25f)
            };

            WorldInteractionResolveResult result = _resolver.Resolve(candidates);
            bool passed = result.HasSelection && result.SelectedCandidate.DisplayName == "Near Task";

            LogResult("DistanceTieBreaker", passed, result);
        }

        private void ValidateInvalidCandidatesIgnored()
        {
            List<WorldInteractionCandidate> candidates = new()
            {
                new WorldInteractionCandidate(WorldInteractionType.CorpseInspectOrCarry, "Invalid Corpse", 0.10f, false),
                new WorldInteractionCandidate(WorldInteractionType.Task, "Valid Task", 0.80f, true)
            };

            WorldInteractionResolveResult result = _resolver.Resolve(candidates);
            bool passed = result.HasSelection && result.SelectedCandidate.Type == WorldInteractionType.Task;

            LogResult("InvalidCandidatesIgnored", passed, result);
        }

        private void ValidateNoCandidates()
        {
            WorldInteractionResolveResult result = _resolver.Resolve(new List<WorldInteractionCandidate>());
            bool passed = !result.HasSelection;

            LogResult("NoCandidates", passed, result);
        }

        private static void LogResult(string testName, bool passed, WorldInteractionResolveResult result)
        {
            if (passed)
                Debug.Log($"[InteractionResolverValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[InteractionResolverValidator] FAIL {testName}: {result}");
        }
    }
}
