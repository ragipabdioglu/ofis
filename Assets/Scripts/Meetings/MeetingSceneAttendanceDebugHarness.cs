using System.Collections.Generic;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingSceneAttendanceDebugHarness : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private int lastRegisteredCount;
        [SerializeField] private int lastMissingEligibleCount;
        [SerializeField] private int lastIgnoredCount;
        [SerializeField] private string lastMessage;

        private readonly MeetingRoomAttendanceService _attendanceService =
            new MeetingRoomAttendanceService();

        private readonly MeetingSceneAttendanceSnapshotService _snapshotService =
            new MeetingSceneAttendanceSnapshotService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateSceneAttendance();
        }

        [ContextMenu("Validate Scene Attendance")]
        public void ValidateSceneAttendance()
        {
            ValidateSnapshotBuilder();
            ValidateAttendanceFromSceneSnapshots();
            ValidateExposedPlayerIgnored();
        }

        private void ValidateSnapshotBuilder()
        {
            List<MeetingAttendancePlayerSnapshot> snapshots = new List<MeetingAttendancePlayerSnapshot>
            {
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("p2", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("p3", OfficeRoomType.MeetingRoom, false, true)
            };

            bool passed = snapshots[0].CanRegisterForMeeting
                && snapshots[1].IsEligible
                && !snapshots[1].CanRegisterForMeeting
                && !snapshots[2].IsEligible;

            LogResult(
                "SnapshotBuilder",
                passed,
                new MeetingAttendanceRegistrationResult(
                    new List<string> { "p1" },
                    new List<string> { "p2" },
                    new List<string> { "p3" },
                    new List<string>(),
                    "Snapshot builder validation."));
        }

        private void ValidateAttendanceFromSceneSnapshots()
        {
            _attendanceService.Reset();

            List<MeetingAttendancePlayerSnapshot> snapshots = new List<MeetingAttendancePlayerSnapshot>
            {
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("p2", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("p3", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("p4", OfficeRoomType.MeetingRoom, false, true)
            };

            MeetingAttendanceRegistrationResult result =
                _attendanceService.RegisterMeetingStartAttendance(snapshots);

            bool passed = result.RegisteredCount == 2
                && result.MissingEligibleCount == 1
                && result.IgnoredCount == 1;

            LogResult("AttendanceFromSceneSnapshots", passed, result);
        }

        private void ValidateExposedPlayerIgnored()
        {
            _attendanceService.Reset();

            List<MeetingAttendancePlayerSnapshot> snapshots = new List<MeetingAttendancePlayerSnapshot>
            {
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("exposed_killer", OfficeRoomType.MeetingRoom, true, true, false, true)
            };

            MeetingAttendanceRegistrationResult result =
                _attendanceService.RegisterMeetingStartAttendance(snapshots);

            bool passed = result.RegisteredCount == 1
                && result.IgnoredCount == 1
                && !ContainsPlayerId(result.RegisteredPlayerIds, "exposed_killer");

            LogResult("ExposedPlayerIgnored", passed, result);
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingAttendanceRegistrationResult result)
        {
            lastRegisteredCount = result.RegisteredCount;
            lastMissingEligibleCount = result.MissingEligibleCount;
            lastIgnoredCount = result.IgnoredCount;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingSceneAttendanceValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingSceneAttendanceValidator] FAIL {testName}: {result}");
        }

        private static bool ContainsPlayerId(
            System.Collections.Generic.IReadOnlyList<string> playerIds,
            string playerId)
        {
            if (playerIds == null || string.IsNullOrWhiteSpace(playerId))
                return false;

            for (int i = 0; i < playerIds.Count; i++)
            {
                if (playerIds[i] == playerId)
                    return true;
            }

            return false;
        }
    }
}
#pragma warning restore 0414
