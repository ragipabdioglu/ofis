using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingActionMajorityThresholdService
    {
        public MeetingActionMajorityThresholdResult Calculate(
            IReadOnlyList<string> eligibleVoterIds,
            int currentVoteCount)
        {
            int eligibleVoterCount = CountUniqueValidIds(eligibleVoterIds);
            int requiredVotes = CalculateRequiredVotes(eligibleVoterCount);
            bool hasReachedMajority = requiredVotes > 0 && currentVoteCount >= requiredVotes;

            string message = eligibleVoterCount > 0
                ? "Majority threshold calculated."
                : "No eligible voters are available.";

            return new MeetingActionMajorityThresholdResult(
                eligibleVoterCount,
                requiredVotes,
                currentVoteCount,
                hasReachedMajority,
                message);
        }

        public MeetingActionMajorityThresholdResult Calculate(
            MeetingAttendanceRegistrationResult attendanceResult,
            int currentVoteCount)
        {
            return Calculate(attendanceResult?.RegisteredPlayerIds, currentVoteCount);
        }

        public int CalculateRequiredVotes(int eligibleVoterCount)
        {
            if (eligibleVoterCount <= 0)
                return 0;

            return (eligibleVoterCount / 2) + 1;
        }

        private static int CountUniqueValidIds(IReadOnlyList<string> playerIds)
        {
            if (playerIds == null)
                return 0;

            List<string> uniqueIds = new List<string>();

            for (int i = 0; i < playerIds.Count; i++)
            {
                string playerId = playerIds[i];

                if (string.IsNullOrWhiteSpace(playerId))
                    continue;

                if (Contains(uniqueIds, playerId))
                    continue;

                uniqueIds.Add(playerId);
            }

            return uniqueIds.Count;
        }

        private static bool Contains(IReadOnlyList<string> playerIds, string playerId)
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
