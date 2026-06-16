using OFIS.Core.Ids;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Evidence
{
    public sealed class EvidenceTraceAgeDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly EvidenceTraceAgeService _ageService = new EvidenceTraceAgeService();

        private void Start()
        {
            if (validateOnStart)
                ValidateTraceAge();
        }

        [ContextMenu("Validate Evidence Trace Age")]
        public void ValidateTraceAge()
        {
            ValidateFreshTrace();
            ValidateOldTrace();
            ValidateVeryOldTrace();
            ValidateFutureTraceClampsToFresh();
        }

        private void ValidateFreshTrace()
        {
            EvidenceTraceAgeResult result = _ageService.CalculateAge(BuildRecord(100f), 160f);
            LogResult("FreshTrace", result.Category == EvidenceTraceAgeCategory.Fresh && Mathf.Approximately(result.AgeSeconds, 60f), result.ToString());
        }

        private void ValidateOldTrace()
        {
            EvidenceTraceAgeResult result = _ageService.CalculateAge(BuildRecord(100f), 261f);
            LogResult("OldTrace", result.Category == EvidenceTraceAgeCategory.Old && Mathf.Approximately(result.AgeSeconds, 161f), result.ToString());
        }

        private void ValidateVeryOldTrace()
        {
            EvidenceTraceAgeResult result = _ageService.CalculateAge(BuildRecord(100f), 281f);
            LogResult("VeryOldTrace", result.Category == EvidenceTraceAgeCategory.VeryOld && Mathf.Approximately(result.AgeSeconds, 181f), result.ToString());
        }

        private void ValidateFutureTraceClampsToFresh()
        {
            EvidenceTraceAgeResult result = _ageService.CalculateAge(BuildRecord(200f), 100f);
            LogResult("FutureTraceClampsToFresh", result.Category == EvidenceTraceAgeCategory.Fresh && Mathf.Approximately(result.AgeSeconds, 0f), result.ToString());
        }

        private static EvidenceTraceRecord BuildRecord(float serverTimeSeconds)
        {
            return new EvidenceTraceRecord(
                new EvidenceTraceId("trace_8b_age"),
                EvidenceTraceType.BloodTrace,
                "corpse_8b",
                OfficeRoomType.ArchiveRoom,
                new Vector3(2f, 2f, 0f),
                serverTimeSeconds,
                "Blood trace age test.");
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[EvidenceTraceAgeDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[EvidenceTraceAgeDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
