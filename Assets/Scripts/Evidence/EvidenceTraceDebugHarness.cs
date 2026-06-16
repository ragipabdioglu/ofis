using OFIS.Core.Ids;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Evidence
{
    public sealed class EvidenceTraceDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly EvidenceTraceRegistry _registry = new EvidenceTraceRegistry();

        private void Start()
        {
            if (validateOnStart)
                ValidateEvidenceTraceCore();
        }

        [ContextMenu("Validate Evidence Trace Core")]
        public void ValidateEvidenceTraceCore()
        {
            ValidateAllMvpTraceTypesExist();
            ValidateTraceRecordAccepted();
            ValidateInvalidTraceRejected();
        }

        private void ValidateAllMvpTraceTypesExist()
        {
            bool passed = (int)EvidenceTraceType.KillSceneTrace > 0
                && (int)EvidenceTraceType.BloodTrace > 0
                && (int)EvidenceTraceType.DragTrace > 0
                && (int)EvidenceTraceType.CarryTrace > 0
                && (int)EvidenceTraceType.DropTrace > 0
                && (int)EvidenceTraceType.HideSpotTrace > 0
                && (int)EvidenceTraceType.SabotageTrace > 0
                && (int)EvidenceTraceType.DoorAnomalyTrace > 0
                && (int)EvidenceTraceType.CameraGapTrace > 0
                && (int)EvidenceTraceType.TaskMismatchTrace > 0
                && (int)EvidenceTraceType.MeetingAttendanceTrace > 0;

            LogResult("AllMvpTraceTypesExist", passed, "MVP trace type enum contains all planned trace types.");
        }

        private void ValidateTraceRecordAccepted()
        {
            EvidenceTraceRecord record = new EvidenceTraceRecord(
                new EvidenceTraceId("trace_8a_record"),
                EvidenceTraceType.CarryTrace,
                "corpse_8a",
                OfficeRoomType.StorageRoom,
                new Vector3(3f, 2f, 0f),
                700f,
                "A corpse was moved through storage.");

            bool passed = _registry.TryRecord(record)
                && _registry.Count == 1
                && _registry.Contains(record.TraceId)
                && _registry.GetBySource("corpse_8a").Count == 1;

            LogResult("TraceRecordAccepted", passed, record.ToString());
        }

        private void ValidateInvalidTraceRejected()
        {
            EvidenceTraceRecord record = new EvidenceTraceRecord(
                default,
                EvidenceTraceType.None,
                "bad_source",
                OfficeRoomType.Unknown,
                default,
                0f,
                "Invalid trace.");

            bool passed = !_registry.TryRecord(record);
            LogResult("InvalidTraceRejected", passed, record.ToString());
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[EvidenceTraceDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[EvidenceTraceDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
