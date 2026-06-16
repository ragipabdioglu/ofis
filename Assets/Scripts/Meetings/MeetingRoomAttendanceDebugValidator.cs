using System.Collections.Generic;
using System.Linq;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingRoomAttendanceDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private MeetingRoomAttendanceService _attendanceService;

        private void Awake()
        {
            _attendanceService = new MeetingRoomAttendanceService();
        }

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateAttendanceFlow();
        }

        [ContextMenu("Validate Meeting Room Attendance")]
        public void ValidateAttendanceFlow()
        {
            ValidateMeetingStartRegistersOnlyPlayersInMeetingRoom();
            ValidateMissingEligiblePlayersAreTracked();
            ValidateDeadOrDisconnectedPlayersAreIgnored();
            ValidateLateJoinBecomesObserver();
            ValidateParticipantListUsesRegisteredPlayersOnly();
        }

        private void ValidateMeetingStartRegistersOnlyPlayersInMeetingRoom()
        {
            _attendanceService.Reset();

            MeetingAttendanceRegistrationResult result =
                _attendanceService.RegisterMeetingStartAttendance(BuildMixedPlayers());

            bool passed = result.RegisteredCount == 2
                && result.MissingEligibleCount == 2
                && result.IgnoredCount == 2
                && _attendanceService.State.IsRegistrationLocked
                && _attendanceService.IsPlayerAllowedToParticipate("player_meeting_01")
                && _attendanceService.IsPlayerAllowedToParticipate("player_meeting_02")
                && !_attendanceService.IsPlayerAllowedToParticipate("player_office_01");

            LogResult("MeetingStartRegistersOnlyPlayersInMeetingRoom", passed, result);
        }

        private void ValidateMissingEligiblePlayersAreTracked()
        {
            _attendanceService.Reset();

            MeetingAttendanceRegistrationResult result =
                _attendanceService.RegisterMeetingStartAttendance(BuildMixedPlayers());

            int penalty = _attendanceService.CalculateMissingPlayerHealthPenalty(result, 5);

            bool passed = result.HasMissingEligiblePlayers
                && result.MissingEligibleCount == 2
                && penalty == 10;

            LogResult("MissingEligiblePlayersAreTracked", passed, result);
        }

        private void ValidateDeadOrDisconnectedPlayersAreIgnored()
        {
            _attendanceService.Reset();

            MeetingAttendanceRegistrationResult result =
                _attendanceService.RegisterMeetingStartAttendance(BuildMixedPlayers());

            bool passed = result.IgnoredCount == 2
                && !result.IgnoredPlayerIds.Contains("player_meeting_01")
                && !result.IgnoredPlayerIds.Contains("player_office_01");

            LogResult("DeadOrDisconnectedPlayersAreIgnored", passed, result);
        }

        private void ValidateLateJoinBecomesObserver()
        {
            _attendanceService.Reset();

            _attendanceService.RegisterMeetingStartAttendance(BuildMixedPlayers());

            MeetingAttendanceRegistrationResult lateResult =
                _attendanceService.RegisterLateJoinAttempt(new MeetingAttendancePlayerSnapshot(
                    "player_late_01",
                    OfficeRoomType.MeetingRoom,
                    true,
                    true));

            bool passed = lateResult.HasLateObservers
                && _attendanceService.IsPlayerObserver("player_late_01")
                && !_attendanceService.IsPlayerAllowedToParticipate("player_late_01");

            LogResult("LateJoinBecomesObserver", passed, lateResult);
        }

        private void ValidateParticipantListUsesRegisteredPlayersOnly()
        {
            _attendanceService.Reset();

            _attendanceService.RegisterMeetingStartAttendance(BuildMixedPlayers());
            _attendanceService.RegisterLateJoinAttempt(new MeetingAttendancePlayerSnapshot(
                "player_late_01",
                OfficeRoomType.MeetingRoom,
                true,
                true));

            IReadOnlyList<string> participants = _attendanceService.BuildParticipantList();

            bool passed = participants.Count == 2
                && participants.Contains("player_meeting_01")
                && participants.Contains("player_meeting_02")
                && !participants.Contains("player_late_01")
                && !participants.Contains("player_office_01");

            if (passed)
                Debug.Log($"[MeetingRoomAttendanceValidator] PASS ParticipantListUsesRegisteredPlayersOnly: Count={participants.Count}");
            else
                Debug.LogError($"[MeetingRoomAttendanceValidator] FAIL ParticipantListUsesRegisteredPlayersOnly: Count={participants.Count}");
        }

        private static List<MeetingAttendancePlayerSnapshot> BuildMixedPlayers()
        {
            return new List<MeetingAttendancePlayerSnapshot>
            {
                new MeetingAttendancePlayerSnapshot("player_meeting_01", OfficeRoomType.MeetingRoom, true, true, true),
                new MeetingAttendancePlayerSnapshot("player_meeting_02", OfficeRoomType.MeetingRoom, true, true),

                // Project enum does not expose OfficeRoomType.Office.
                // SecurityRoom is used here as a non-meeting room sample.
                new MeetingAttendancePlayerSnapshot("player_office_01", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_security_01", OfficeRoomType.SecurityRoom, true, true),

                new MeetingAttendancePlayerSnapshot("player_dead_01", OfficeRoomType.MeetingRoom, false, true),
                new MeetingAttendancePlayerSnapshot("player_disconnected_01", OfficeRoomType.MeetingRoom, true, false)
            };
        }

        private static void LogResult(string testName, bool passed, MeetingAttendanceRegistrationResult result)
        {
            if (passed)
                Debug.Log($"[MeetingRoomAttendanceValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingRoomAttendanceValidator] FAIL {testName}: {result}");
        }
    }
}
