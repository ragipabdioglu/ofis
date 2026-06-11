using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingVoteEvaluationService
    {
        public MeetingVoteEvaluationResult Evaluate(IReadOnlyList<MeetingVoteData> votes)
        {
            if (votes == null || votes.Count == 0)
            {
                return new MeetingVoteEvaluationResult(
                    false,
                    false,
                    false,
                    "none",
                    0,
                    new List<string>(),
                    "No votes submitted.");
            }

            Dictionary<string, int> countsByTarget = new Dictionary<string, int>();

            for (int i = 0; i < votes.Count; i++)
            {
                string targetPlayerId = votes[i].TargetPlayerId;

                if (string.IsNullOrWhiteSpace(targetPlayerId) || targetPlayerId == "unknown_target")
                    continue;

                if (!countsByTarget.ContainsKey(targetPlayerId))
                    countsByTarget[targetPlayerId] = 0;

                countsByTarget[targetPlayerId]++;
            }

            if (countsByTarget.Count == 0)
            {
                return new MeetingVoteEvaluationResult(
                    false,
                    false,
                    false,
                    "none",
                    0,
                    new List<string>(),
                    "No valid vote targets.");
            }

            int highestVoteCount = 0;
            List<string> leaders = new List<string>();

            foreach (KeyValuePair<string, int> entry in countsByTarget)
            {
                if (entry.Value > highestVoteCount)
                {
                    highestVoteCount = entry.Value;
                    leaders.Clear();
                    leaders.Add(entry.Key);
                }
                else if (entry.Value == highestVoteCount)
                {
                    leaders.Add(entry.Key);
                }
            }

            if (leaders.Count == 1)
            {
                return new MeetingVoteEvaluationResult(
                    true,
                    true,
                    false,
                    leaders[0],
                    highestVoteCount,
                    new List<string>(),
                    "Vote winner selected.");
            }

            return new MeetingVoteEvaluationResult(
                true,
                false,
                true,
                "none",
                highestVoteCount,
                leaders,
                "Vote result is tied.");
        }
    }
}
