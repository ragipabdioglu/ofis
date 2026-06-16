namespace OFIS.Meetings
{
    public sealed class MeetingVoiceEligibilityService
    {
        public MeetingVoiceEligibilityResult Evaluate(
            MeetingAttendanceRegistrationResult attendanceResult,
            MeetingAttendancePlayerSnapshot player)
        {
            if (attendanceResult == null)
            {
                return new MeetingVoiceEligibilityResult(
                    player.PlayerId,
                    false,
                    false,
                    "Attendance result is missing.");
            }

            bool isRegistered = Contains(attendanceResult.RegisteredPlayerIds, player.PlayerId);
            bool isLateObserver = Contains(attendanceResult.LateObserverPlayerIds, player.PlayerId);

            if (!player.IsEligible)
            {
                return new MeetingVoiceEligibilityResult(
                    player.PlayerId,
                    false,
                    false,
                    "Player is not eligible for meeting participation.");
            }

            if (isLateObserver)
            {
                return new MeetingVoiceEligibilityResult(
                    player.PlayerId,
                    false,
                    false,
                    "Late observers cannot use meeting voice or vote.");
            }

            if (!isRegistered)
            {
                return new MeetingVoiceEligibilityResult(
                    player.PlayerId,
                    false,
                    false,
                    "Player is not a registered meeting participant.");
            }

            if (!player.IsInMeetingRoom)
            {
                return new MeetingVoiceEligibilityResult(
                    player.PlayerId,
                    false,
                    true,
                    "Registered participant left the meeting room; vote right remains.");
            }

            return new MeetingVoiceEligibilityResult(
                player.PlayerId,
                true,
                true,
                "Registered participant is inside meeting room.");
        }

        private static bool Contains(
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
