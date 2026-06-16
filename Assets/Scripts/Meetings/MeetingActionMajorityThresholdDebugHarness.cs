using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionMajorityThresholdDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private int lastEligibleVoterCount;
        [SerializeField] private int lastRequiredVotes;
        [SerializeField] private int lastCurrentVoteCount;
        [SerializeField] private bool lastHasReachedMajority;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionMajorityThresholdService _service =
            new MeetingActionMajorityThresholdService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMajorityThreshold();
        }

        [ContextMenu("Validate Meeting Action Majority Threshold")]
        public void ValidateMajorityThreshold()
        {
            ValidateSingleEligibleVoterRequiresOneVote();
            ValidateTwoEligibleVotersRequireTwoVotes();
            ValidateThreeEligibleVotersRequireTwoVotes();
            ValidateFourEligibleVotersRequireThreeVotes();
            ValidateAttendanceUsesRegisteredPlayersOnly();
            ValidateEmptyEligibleVotersCannotReachMajority();
            ValidateDuplicateEligibleVotersCountOnce();
        }

        private void ValidateSingleEligibleVoterRequiresOneVote()
        {
            MeetingActionMajorityThresholdResult result = _service.Calculate(
                new[] { "player_01" },
                1);

            bool passed = result.EligibleVoterCount == 1
                && result.RequiredVotes == 1
                && result.HasReachedMajority;

            LogResult("SingleEligibleVoterRequiresOneVote", passed, result);
        }

        private void ValidateTwoEligibleVotersRequireTwoVotes()
        {
            MeetingActionMajorityThresholdResult result = _service.Calculate(
                new[] { "player_01", "player_02" },
                1);

            bool passed = result.EligibleVoterCount == 2
                && result.RequiredVotes == 2
                && !result.HasReachedMajority;

            LogResult("TwoEligibleVotersRequireTwoVotes", passed, result);
        }

        private void ValidateThreeEligibleVotersRequireTwoVotes()
        {
            MeetingActionMajorityThresholdResult result = _service.Calculate(
                new[] { "player_01", "player_02", "player_03" },
                2);

            bool passed = result.EligibleVoterCount == 3
                && result.RequiredVotes == 2
                && result.HasReachedMajority;

            LogResult("ThreeEligibleVotersRequireTwoVotes", passed, result);
        }

        private void ValidateFourEligibleVotersRequireThreeVotes()
        {
            MeetingActionMajorityThresholdResult result = _service.Calculate(
                new[] { "player_01", "player_02", "player_03", "player_04" },
                2);

            bool passed = result.EligibleVoterCount == 4
                && result.RequiredVotes == 3
                && !result.HasReachedMajority;

            LogResult("FourEligibleVotersRequireThreeVotes", passed, result);
        }

        private void ValidateAttendanceUsesRegisteredPlayersOnly()
        {
            MeetingAttendanceRegistrationResult attendance = new MeetingAttendanceRegistrationResult(
                new[] { "player_01", "player_02", "player_03" },
                new[] { "player_missing" },
                new[] { "player_late" },
                new[] { "player_ignored" },
                "Majority threshold debug attendance.");

            MeetingActionMajorityThresholdResult result = _service.Calculate(attendance, 2);

            bool passed = result.EligibleVoterCount == 3
                && result.RequiredVotes == 2
                && result.HasReachedMajority;

            LogResult("AttendanceUsesRegisteredPlayersOnly", passed, result);
        }

        private void ValidateEmptyEligibleVotersCannotReachMajority()
        {
            MeetingActionMajorityThresholdResult result = _service.Calculate(
                new string[0],
                3);

            bool passed = result.EligibleVoterCount == 0
                && result.RequiredVotes == 0
                && !result.HasReachedMajority;

            LogResult("EmptyEligibleVotersCannotReachMajority", passed, result);
        }

        private void ValidateDuplicateEligibleVotersCountOnce()
        {
            MeetingActionMajorityThresholdResult result = _service.Calculate(
                new List<string> { "player_01", "player_01", "player_02", string.Empty },
                2);

            bool passed = result.EligibleVoterCount == 2
                && result.RequiredVotes == 2
                && result.HasReachedMajority;

            LogResult("DuplicateEligibleVotersCountOnce", passed, result);
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionMajorityThresholdResult result)
        {
            lastEligibleVoterCount = result.EligibleVoterCount;
            lastRequiredVotes = result.RequiredVotes;
            lastCurrentVoteCount = result.CurrentVoteCount;
            lastHasReachedMajority = result.HasReachedMajority;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionMajorityThresholdValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionMajorityThresholdValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
