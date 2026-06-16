using System.Collections.Generic;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Meetings
{
    public sealed class MeetingMissingPlayerPenaltyDebugValidator : MonoBehaviour
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

            ValidatePenaltyBridge();
        }

        [ContextMenu("Validate Meeting Missing Player Penalty")]
        public void ValidatePenaltyBridge()
        {
            ValidateNoMissingPlayersProducesNoPenalty();
            ValidateMissingPlayersProducePenalty();
            ValidatePenaltyIsCapped();
            ValidateHealthDoesNotGoBelowZero();
            ValidateNoRegisteredPlayersConfigCanBlockPenalty();
        }

        private void ValidateNoMissingPlayersProducesNoPenalty()
        {
            _attendanceService.Reset();

            MeetingAttendanceRegistrationResult attendanceResult =
                _attendanceService.RegisterMeetingStartAttendance(BuildAllPlayersInMeetingRoom());

            MeetingCompanyHealthPenaltyBridgeService bridgeService =
                new MeetingCompanyHealthPenaltyBridgeService();

            MeetingCompanyHealthPenaltyBridgeResult result =
                bridgeService.BuildBridgeResult(attendanceResult, 100);

            bool passed = !result.PenaltyResult.ShouldApplyPenalty
                && result.PenaltyResult.AppliedPenaltyAmount == 0
                && result.CompanyHealthBefore == 100
                && result.CompanyHealthAfter == 100
                && !result.ChangedHealth;

            LogResult("NoMissingPlayersProducesNoPenalty", passed, result);
        }

        private void ValidateMissingPlayersProducePenalty()
        {
            _attendanceService.Reset();

            MeetingAttendanceRegistrationResult attendanceResult =
                _attendanceService.RegisterMeetingStartAttendance(BuildMixedAttendancePlayers());

            MeetingMissingPlayerPenaltyConfig config =
                new MeetingMissingPlayerPenaltyConfig(5, 25);

            MeetingCompanyHealthPenaltyBridgeService bridgeService =
                new MeetingCompanyHealthPenaltyBridgeService(
                    new MeetingMissingPlayerPenaltyService(config));

            MeetingCompanyHealthPenaltyBridgeResult result =
                bridgeService.BuildBridgeResult(attendanceResult, 100);

            bool passed = result.PenaltyResult.ShouldApplyPenalty
                && result.PenaltyResult.MissingEligiblePlayerCount == 2
                && result.PenaltyResult.RawPenaltyAmount == 10
                && result.PenaltyResult.AppliedPenaltyAmount == 10
                && result.CompanyHealthAfter == 90
                && result.ChangedHealth;

            LogResult("MissingPlayersProducePenalty", passed, result);
        }

        private void ValidatePenaltyIsCapped()
        {
            _attendanceService.Reset();

            MeetingAttendanceRegistrationResult attendanceResult =
                _attendanceService.RegisterMeetingStartAttendance(BuildManyMissingPlayers());

            MeetingMissingPlayerPenaltyConfig config =
                new MeetingMissingPlayerPenaltyConfig(10, 25);

            MeetingCompanyHealthPenaltyBridgeService bridgeService =
                new MeetingCompanyHealthPenaltyBridgeService(
                    new MeetingMissingPlayerPenaltyService(config));

            MeetingCompanyHealthPenaltyBridgeResult result =
                bridgeService.BuildBridgeResult(attendanceResult, 100);

            bool passed = result.PenaltyResult.ShouldApplyPenalty
                && result.PenaltyResult.RawPenaltyAmount == 40
                && result.PenaltyResult.AppliedPenaltyAmount == 25
                && result.PenaltyResult.WasCapped
                && result.CompanyHealthAfter == 75;

            LogResult("PenaltyIsCapped", passed, result);
        }

        private void ValidateHealthDoesNotGoBelowZero()
        {
            _attendanceService.Reset();

            MeetingAttendanceRegistrationResult attendanceResult =
                _attendanceService.RegisterMeetingStartAttendance(BuildManyMissingPlayers());

            MeetingMissingPlayerPenaltyConfig config =
                new MeetingMissingPlayerPenaltyConfig(10, 100);

            MeetingCompanyHealthPenaltyBridgeService bridgeService =
                new MeetingCompanyHealthPenaltyBridgeService(
                    new MeetingMissingPlayerPenaltyService(config));

            MeetingCompanyHealthPenaltyBridgeResult result =
                bridgeService.BuildBridgeResult(attendanceResult, 15);

            bool passed = result.PenaltyResult.ShouldApplyPenalty
                && result.CompanyHealthBefore == 15
                && result.CompanyHealthAfter == 0
                && result.AppliedDelta == 15
                && result.ChangedHealth;

            LogResult("HealthDoesNotGoBelowZero", passed, result);
        }

        private void ValidateNoRegisteredPlayersConfigCanBlockPenalty()
        {
            _attendanceService.Reset();

            MeetingAttendanceRegistrationResult attendanceResult =
                _attendanceService.RegisterMeetingStartAttendance(BuildOnlyMissingPlayers());

            MeetingMissingPlayerPenaltyConfig config =
                new MeetingMissingPlayerPenaltyConfig(
                    penaltyPerMissingPlayer: 5,
                    maxPenaltyPerMeeting: 25,
                    applyPenaltyWhenNoRegisteredPlayers: false);

            MeetingCompanyHealthPenaltyBridgeService bridgeService =
                new MeetingCompanyHealthPenaltyBridgeService(
                    new MeetingMissingPlayerPenaltyService(config));

            MeetingCompanyHealthPenaltyBridgeResult result =
                bridgeService.BuildBridgeResult(attendanceResult, 100);

            bool passed = !result.PenaltyResult.ShouldApplyPenalty
                && result.PenaltyResult.AppliedPenaltyAmount == 0
                && result.CompanyHealthAfter == 100
                && !result.ChangedHealth;

            LogResult("NoRegisteredPlayersConfigCanBlockPenalty", passed, result);
        }

        private static List<MeetingAttendancePlayerSnapshot> BuildAllPlayersInMeetingRoom()
        {
            return new List<MeetingAttendancePlayerSnapshot>
            {
                new MeetingAttendancePlayerSnapshot("player_01", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_02", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_03", OfficeRoomType.MeetingRoom, true, true)
            };
        }

        private static List<MeetingAttendancePlayerSnapshot> BuildMixedAttendancePlayers()
        {
            return new List<MeetingAttendancePlayerSnapshot>
            {
                new MeetingAttendancePlayerSnapshot("player_meeting_01", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_meeting_02", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_missing_01", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_missing_02", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_dead_01", OfficeRoomType.MeetingRoom, false, true)
            };
        }

        private static List<MeetingAttendancePlayerSnapshot> BuildManyMissingPlayers()
        {
            return new List<MeetingAttendancePlayerSnapshot>
            {
                new MeetingAttendancePlayerSnapshot("player_meeting_01", OfficeRoomType.MeetingRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_missing_01", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_missing_02", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_missing_03", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_missing_04", OfficeRoomType.SecurityRoom, true, true)
            };
        }

        private static List<MeetingAttendancePlayerSnapshot> BuildOnlyMissingPlayers()
        {
            return new List<MeetingAttendancePlayerSnapshot>
            {
                new MeetingAttendancePlayerSnapshot("player_missing_01", OfficeRoomType.SecurityRoom, true, true),
                new MeetingAttendancePlayerSnapshot("player_missing_02", OfficeRoomType.SecurityRoom, true, true)
            };
        }

        private static void LogResult(string testName, bool passed, MeetingCompanyHealthPenaltyBridgeResult result)
        {
            if (passed)
                Debug.Log($"[MeetingMissingPlayerPenaltyValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingMissingPlayerPenaltyValidator] FAIL {testName}: {result}");
        }
    }
}
