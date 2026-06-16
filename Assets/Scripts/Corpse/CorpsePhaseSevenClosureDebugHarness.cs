using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpsePhaseSevenClosureDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private void Start()
        {
            if (validateOnStart)
                ValidateClosure();
        }

        [ContextMenu("Validate Phase Seven Closure")]
        public void ValidateClosure()
        {
            bool hasKillPipeline = FindAnyObjectByType<OFIS.Kill.KillRuntimePipelineDebugHarness>() != null;
            bool hasInspect = FindAnyObjectByType<CorpseInspectDebugHarness>() != null;
            bool hasCarry = FindAnyObjectByType<CorpseCarryServerGuardDebugHarness>() != null;
            bool hasDrop = FindAnyObjectByType<CorpseDropDebugHarness>() != null;
            bool hasHide = FindAnyObjectByType<CorpseHideDebugHarness>() != null;
            bool hasTrace = FindAnyObjectByType<CorpseMovementTraceDebugHarness>() != null;
            bool hasVisibility = FindAnyObjectByType<CorpseTraceVisibilityDebugHarness>() != null;
            bool hasAnnouncement = FindAnyObjectByType<CorpseAnnouncementDebugHarness>() != null;

            bool passed = hasKillPipeline
                && hasInspect
                && hasCarry
                && hasDrop
                && hasHide
                && hasTrace
                && hasVisibility
                && hasAnnouncement;

            LogResult("PhaseSevenClosure", passed, "MVP Faz 7 validators present.");
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;
            if (passed)
                Debug.Log($"[CorpsePhaseSevenClosureDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpsePhaseSevenClosureDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
