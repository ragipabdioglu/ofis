namespace OFIS.Meetings
{
    public sealed class MeetingParticipationGuardService
    {
        private readonly MeetingVoiceEligibilityService _voiceEligibilityService;

        public MeetingParticipationGuardService()
        {
            _voiceEligibilityService = new MeetingVoiceEligibilityService();
        }

        public MeetingParticipationGuardService(
            MeetingVoiceEligibilityService voiceEligibilityService)
        {
            _voiceEligibilityService = voiceEligibilityService ?? new MeetingVoiceEligibilityService();
        }

        public MeetingParticipationGuardResult Evaluate(
            MeetingAttendanceRegistrationResult attendanceResult,
            MeetingAttendancePlayerSnapshot player)
        {
            if (!player.IsEligible)
            {
                return new MeetingParticipationGuardResult(
                    player.PlayerId,
                    false,
                    false,
                    false,
                    false,
                    "Player is dead, exposed, disconnected, or invalid.");
            }

            bool isLateObserver = Contains(attendanceResult?.LateObserverPlayerIds, player.PlayerId);
            MeetingVoiceEligibilityResult voiceResult =
                _voiceEligibilityService.Evaluate(attendanceResult, player);

            return new MeetingParticipationGuardResult(
                player.PlayerId,
                voiceResult.KeepsVoteRight || voiceResult.CanUseMeetingVoice,
                voiceResult.KeepsVoteRight,
                voiceResult.CanUseMeetingVoice,
                isLateObserver,
                voiceResult.Reason);
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
