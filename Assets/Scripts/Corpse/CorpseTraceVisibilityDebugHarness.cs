using System.Collections.Generic;
using OFIS.Core.Ids;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpseTraceVisibilityDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly CorpseTraceVisibilityService _visibilityService = new CorpseTraceVisibilityService();
        private readonly CorpseOwnerTraceViewService _ownerTraceViewService = new CorpseOwnerTraceViewService();

        private void Start()
        {
            if (validateOnStart)
                ValidateTraceVisibility();
        }

        [ContextMenu("Validate Trace Visibility")]
        public void ValidateTraceVisibility()
        {
            ValidateHiddenBeforeInspect();
            ValidateInspectorOnlyAfterInspect();
            ValidatePublicAfterAnnouncement();
        }

        private void ValidateHiddenBeforeInspect()
        {
            CorpseMovementTraceEvent traceEvent = BuildTrace();
            CorpseTraceVisibilityResult result = _visibilityService.ResolveForViewer(
                traceEvent,
                new PlayerId("detective_trace_viewer"),
                default,
                false);

            LogResult("HiddenBeforeInspect", !result.CanView && result.VisibilityType == CorpseTraceVisibilityType.Hidden, result.Message);
        }

        private void ValidateInspectorOnlyAfterInspect()
        {
            CorpseMovementTraceEvent traceEvent = BuildTrace();
            CorpseOwnerKnowledge knowledge = BuildKnowledge("detective_trace_owner");
            IReadOnlyList<CorpseTraceVisibilityResult> view = _ownerTraceViewService.BuildOwnerView(
                new[] { traceEvent },
                new PlayerId("detective_trace_owner"),
                knowledge,
                false);

            bool passed = view.Count == 1 && view[0].VisibilityType == CorpseTraceVisibilityType.InspectorOnly;
            LogResult("InspectorOnlyAfterInspect", passed, view.Count == 1 ? view[0].Message : "No visible trace.");
        }

        private void ValidatePublicAfterAnnouncement()
        {
            CorpseMovementTraceEvent traceEvent = BuildTrace();
            CorpseTraceVisibilityResult result = _visibilityService.ResolveForViewer(
                traceEvent,
                new PlayerId("player_any"),
                default,
                true);

            LogResult("PublicAfterAnnouncement", result.CanView && result.VisibilityType == CorpseTraceVisibilityType.Public, result.Message);
        }

        public static CorpseMovementTraceEvent BuildTrace()
        {
            return new CorpseMovementTraceEvent(
                new EvidenceTraceId("trace_visibility_7j"),
                CorpseMovementTraceType.Dropped,
                new CorpseId("corpse_visibility_7j"),
                new PlayerId("killer_visibility_7j"),
                "Merve Kaya",
                OfficeRoomType.StorageRoom,
                new Vector3(1f, 1f, 0f),
                600f,
                true);
        }

        public static CorpseOwnerKnowledge BuildKnowledge(string ownerId)
        {
            return new CorpseOwnerKnowledge(
                new PlayerId(ownerId),
                new CorpseId("corpse_visibility_7j"),
                new PlayerId("victim_visibility_7j"),
                "Merve Kaya",
                OfficeRoomType.StorageRoom,
                620f,
                true);
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;
            if (passed)
                Debug.Log($"[CorpseTraceVisibilityDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpseTraceVisibilityDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
