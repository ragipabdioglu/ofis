namespace OFIS.Meetings
{
    public readonly struct MeetingVoiceEligibilityResult
    {
        public string PlayerId { get; }
        public bool CanUseMeetingVoice { get; }
        public bool KeepsVoteRight { get; }
        public string Reason { get; }

        public MeetingVoiceEligibilityResult(
            string playerId,
            bool canUseMeetingVoice,
            bool keepsVoteRight,
            string reason)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? string.Empty : playerId;
            CanUseMeetingVoice = canUseMeetingVoice;
            KeepsVoteRight = keepsVoteRight;
            Reason = string.IsNullOrWhiteSpace(reason)
                ? "Meeting voice eligibility resolved."
                : reason;
        }

        public override string ToString()
        {
            return $"Player={PlayerId}, Voice={CanUseMeetingVoice}, Vote={KeepsVoteRight}, Reason={Reason}";
        }
    }
}
