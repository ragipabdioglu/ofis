namespace OFIS.Meetings
{
    public readonly struct MeetingOfficialActionEffectResult
    {
        public MeetingActionProposalData Proposal { get; }
        public MeetingOfficialActionEffectType EffectType { get; }
        public bool ShouldApplyEffect { get; }
        public string Message { get; }

        public MeetingOfficialActionEffectResult(
            MeetingActionProposalData proposal,
            MeetingOfficialActionEffectType effectType,
            bool shouldApplyEffect,
            string message)
        {
            Proposal = proposal;
            EffectType = effectType;
            ShouldApplyEffect = shouldApplyEffect;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting official action effect resolved."
                : message;
        }

        public override string ToString()
        {
            return $"Proposal={Proposal.ProposalId}, ActionType={Proposal.Request.ActionType}, EffectType={EffectType}, Apply={ShouldApplyEffect}, Message={Message}";
        }
    }
}
