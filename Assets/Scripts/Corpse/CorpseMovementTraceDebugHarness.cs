using OFIS.Core.Ids;
using OFIS.Players;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpseMovementTraceDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly CorpseMovementTraceService _traceService = new CorpseMovementTraceService();
        private readonly CorpseMovementTraceMemory _traceMemory = new CorpseMovementTraceMemory();
        private readonly CorpseDropService _dropService = new CorpseDropService();
        private readonly CorpseHideService _hideService = new CorpseHideService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateCorpseMovementTraces();
        }

        [ContextMenu("Validate Corpse Movement Traces")]
        public void ValidateCorpseMovementTraces()
        {
            _traceMemory.Clear();
            ValidateCarryTraceEvent();
            ValidateDropTraceEvent();
            ValidateHideTraceEvent();
            ValidateRejectedCommandProducesNoTrace();
        }

        private void ValidateCarryTraceEvent()
        {
            CorpsePlaceholder corpse = BuildCorpse("corpse_trace_7i_carry");
            CorpseCarryCommandResult carryResult = CorpseCarryCommandResult.Accepted(corpse);
            CorpseMovementTraceEvent traceEvent = _traceService.FromCarryStarted(
                new PlayerId("killer_trace_01"),
                carryResult,
                OfficeRoomType.ArchiveRoom,
                500f);

            _traceMemory.Record(traceEvent);
            bool passed = traceEvent.TraceType == CorpseMovementTraceType.CarryStarted
                && traceEvent.CorpseId.ToString() == corpse.CorpseId
                && traceEvent.HiddenUntilCorpseInspect
                && _traceMemory.Count == 1;

            Destroy(corpse.gameObject);
            LogResult("CarryTraceEvent", passed, traceEvent.ToString());
        }

        private void ValidateDropTraceEvent()
        {
            CorpseCarryState carryState = BuildCarryState("corpse_trace_7i_drop", out CorpsePlaceholder corpse);
            Vector3 dropPosition = new Vector3(6f, 2f, 0f);
            CorpseDropCommandResult dropResult = _dropService.Drop(
                new CorpseDropCommandContext(
                    "drop_trace_7i",
                    new PlayerId("killer_trace_01"),
                    PlayerLifeState.Alive,
                    carryState,
                    dropPosition));

            CorpseMovementTraceEvent traceEvent = _traceService.FromDrop(
                new PlayerId("killer_trace_01"),
                dropResult,
                OfficeRoomType.StorageRoom,
                520f);

            _traceMemory.Record(traceEvent);
            bool passed = traceEvent.TraceType == CorpseMovementTraceType.Dropped
                && traceEvent.WorldPosition == dropPosition
                && traceEvent.HiddenUntilCorpseInspect;

            Destroy(corpse.gameObject);
            Destroy(carryState.gameObject);
            LogResult("DropTraceEvent", passed, traceEvent.ToString());
        }

        private void ValidateHideTraceEvent()
        {
            CorpseCarryState carryState = BuildCarryState("corpse_trace_7i_hide", out CorpsePlaceholder corpse);
            CorpseHideSpotState hideSpot = new CorpseHideSpotState(
                "hide_trace_7i",
                OfficeRoomType.StorageRoom,
                new Vector3(7f, 2f, 0f),
                true);
            CorpseHideCommandResult hideResult = _hideService.Hide(
                new CorpseHideCommandContext(
                    "hide_trace_7i",
                    new PlayerId("killer_trace_01"),
                    PlayerLifeState.Alive,
                    carryState,
                    hideSpot));

            CorpseMovementTraceEvent traceEvent = _traceService.FromHide(
                new PlayerId("killer_trace_01"),
                hideResult,
                OfficeRoomType.StorageRoom,
                540f);

            _traceMemory.Record(traceEvent);
            bool passed = traceEvent.TraceType == CorpseMovementTraceType.Hidden
                && traceEvent.WorldPosition == hideSpot.WorldPosition
                && traceEvent.HiddenUntilCorpseInspect;

            Destroy(corpse.gameObject);
            Destroy(carryState.gameObject);
            LogResult("HideTraceEvent", passed, traceEvent.ToString());
        }

        private void ValidateRejectedCommandProducesNoTrace()
        {
            CorpseMovementTraceEvent traceEvent = _traceService.FromDrop(
                new PlayerId("killer_trace_01"),
                CorpseDropCommandResult.Rejected("No carried corpse."),
                OfficeRoomType.Hallway,
                560f);

            bool passed = string.IsNullOrWhiteSpace(traceEvent.TraceId.Value)
                && traceEvent.TraceType == CorpseMovementTraceType.None;

            LogResult("RejectedCommandProducesNoTrace", passed, traceEvent.ToString());
        }

        private static CorpseCarryState BuildCarryState(
            string corpseId,
            out CorpsePlaceholder corpse)
        {
            GameObject carrier = new GameObject("CorpseMovementTraceDebug_Carrier");
            CorpseCarryState carryState = carrier.AddComponent<CorpseCarryState>();
            corpse = BuildCorpse(corpseId);
            carryState.StartCarrying(corpse);
            return carryState;
        }

        private static CorpsePlaceholder BuildCorpse(string corpseId)
        {
            GameObject corpseObject = new GameObject(corpseId);
            corpseObject.AddComponent<BoxCollider2D>().isTrigger = true;
            CorpsePlaceholder corpse = corpseObject.AddComponent<CorpsePlaceholder>();
            corpse.Initialize(
                new CorpsePublicState(
                    new CorpseId(corpseId),
                    new PlayerId("victim_trace_01"),
                    "Merve Kaya",
                    new Vector3(3f, 2f, 0f),
                    true));
            return corpse;
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[CorpseMovementTraceDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpseMovementTraceDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
