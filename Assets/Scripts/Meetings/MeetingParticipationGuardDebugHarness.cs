using System.Collections.Generic;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingParticipationGuardDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastPlayerId;
        [SerializeField] private bool lastCanAttendMeeting;
        [SerializeField] private bool lastCanVote;
        [SerializeField] private bool lastCanUseMeetingVoice;
        [SerializeField] private bool lastIsLateObserver;
        [SerializeField] private string lastReason;

        private readonly MeetingParticipationGuardService _service =
            new MeetingParticipationGuardService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateParticipationGuard();
        }

        [ContextMenu("Validate Participation Guard")]
        public void ValidateParticipationGuard()
        {
            ValidateRegisteredParticipant();
            ValidateParticipantOutsideKeepsVoteOnly();
            ValidateLateObserverBlocked();
            ValidateDisconnectedBlocked();
            ValidateExposedBlocked();
        }

        private void ValidateRegisteredParticipant()
        {
            MeetingParticipationGuardResult result = _service.Evaluate(
                BuildAttendance(new[] { "p1" }, new string[0]),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true));

            bool passed = result.CanAttendMeeting && result.CanVote && result.CanUseMeetingVoice;
            LogResult("RegisteredParticipant", passed, result);
        }

        private void ValidateParticipantOutsideKeepsVoteOnly()
        {
            MeetingParticipationGuardResult result = _service.Evaluate(
                BuildAttendance(new[] { "p1" }, new string[0]),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.SecurityRoom, true, true));

            bool passed = result.CanAttendMeeting && result.CanVote && !result.CanUseMeetingVoice;
            LogResult("ParticipantOutsideKeepsVoteOnly", passed, result);
        }

        private void ValidateLateObserverBlocked()
        {
            MeetingParticipationGuardResult result = _service.Evaluate(
                BuildAttendance(new string[0], new[] { "p1" }),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true));

            bool passed = !result.CanAttendMeeting
                && !result.CanVote
                && !result.CanUseMeetingVoice
                && result.IsLateObserver;

            LogResult("LateObserverBlocked", passed, result);
        }

        private void ValidateDisconnectedBlocked()
        {
            MeetingParticipationGuardResult result = _service.Evaluate(
                BuildAttendance(new[] { "p1" }, new string[0]),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, false));

            bool passed = !result.CanAttendMeeting && !result.CanVote && !result.CanUseMeetingVoice;
            LogResult("DisconnectedBlocked", passed, result);
        }

        private void ValidateExposedBlocked()
        {
            MeetingParticipationGuardResult result = _service.Evaluate(
                BuildAttendance(new[] { "p1" }, new string[0]),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true, false, true));

            bool passed = !result.CanAttendMeeting && !result.CanVote && !result.CanUseMeetingVoice;
            LogResult("ExposedBlocked", passed, result);
        }

        private static MeetingAttendanceRegistrationResult BuildAttendance(
            IEnumerable<string> registered,
            IEnumerable<string> lateObservers)
        {
            return new MeetingAttendanceRegistrationResult(
                registered,
                new List<string>(),
                lateObservers,
                new List<string>(),
                "Participation guard debug attendance.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingParticipationGuardResult result)
        {
            lastPlayerId = result.PlayerId;
            lastCanAttendMeeting = result.CanAttendMeeting;
            lastCanVote = result.CanVote;
            lastCanUseMeetingVoice = result.CanUseMeetingVoice;
            lastIsLateObserver = result.IsLateObserver;
            lastReason = result.Reason;

            if (passed)
                Debug.Log($"[MeetingParticipationGuardValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingParticipationGuardValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
