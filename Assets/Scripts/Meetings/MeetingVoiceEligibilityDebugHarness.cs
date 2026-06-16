using System.Collections.Generic;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingVoiceEligibilityDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastPlayerId;
        [SerializeField] private bool lastCanUseMeetingVoice;
        [SerializeField] private bool lastKeepsVoteRight;
        [SerializeField] private string lastReason;

        private readonly MeetingVoiceEligibilityService _service =
            new MeetingVoiceEligibilityService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateVoiceEligibility();
        }

        [ContextMenu("Validate Voice Eligibility")]
        public void ValidateVoiceEligibility()
        {
            ValidateRegisteredInsideGetsVoice();
            ValidateRegisteredOutsideLosesVoiceKeepsVote();
            ValidateLateObserverGetsNoVoiceNoVote();
            ValidateDeadPlayerGetsNoVoiceNoVote();
            ValidateExposedPlayerGetsNoVoiceNoVote();
        }

        private void ValidateRegisteredInsideGetsVoice()
        {
            MeetingVoiceEligibilityResult result = _service.Evaluate(
                BuildAttendance(new[] { "p1" }, new string[0]),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true));

            bool passed = result.CanUseMeetingVoice && result.KeepsVoteRight;
            LogResult("RegisteredInsideGetsVoice", passed, result);
        }

        private void ValidateRegisteredOutsideLosesVoiceKeepsVote()
        {
            MeetingVoiceEligibilityResult result = _service.Evaluate(
                BuildAttendance(new[] { "p1" }, new string[0]),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.SecurityRoom, true, true));

            bool passed = !result.CanUseMeetingVoice && result.KeepsVoteRight;
            LogResult("RegisteredOutsideLosesVoiceKeepsVote", passed, result);
        }

        private void ValidateLateObserverGetsNoVoiceNoVote()
        {
            MeetingVoiceEligibilityResult result = _service.Evaluate(
                BuildAttendance(new string[0], new[] { "p1" }),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true));

            bool passed = !result.CanUseMeetingVoice && !result.KeepsVoteRight;
            LogResult("LateObserverGetsNoVoiceNoVote", passed, result);
        }

        private void ValidateDeadPlayerGetsNoVoiceNoVote()
        {
            MeetingVoiceEligibilityResult result = _service.Evaluate(
                BuildAttendance(new[] { "p1" }, new string[0]),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, false, true));

            bool passed = !result.CanUseMeetingVoice && !result.KeepsVoteRight;
            LogResult("DeadPlayerGetsNoVoiceNoVote", passed, result);
        }

        private void ValidateExposedPlayerGetsNoVoiceNoVote()
        {
            MeetingVoiceEligibilityResult result = _service.Evaluate(
                BuildAttendance(new[] { "p1" }, new string[0]),
                new MeetingAttendancePlayerSnapshot("p1", OfficeRoomType.MeetingRoom, true, true, false, true));

            bool passed = !result.CanUseMeetingVoice && !result.KeepsVoteRight;
            LogResult("ExposedPlayerGetsNoVoiceNoVote", passed, result);
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
                "Voice eligibility debug attendance.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingVoiceEligibilityResult result)
        {
            lastPlayerId = result.PlayerId;
            lastCanUseMeetingVoice = result.CanUseMeetingVoice;
            lastKeepsVoteRight = result.KeepsVoteRight;
            lastReason = result.Reason;

            if (passed)
                Debug.Log($"[MeetingVoiceEligibilityValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingVoiceEligibilityValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
