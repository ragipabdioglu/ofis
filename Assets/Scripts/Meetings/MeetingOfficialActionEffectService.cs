namespace OFIS.Meetings
{
    public sealed class MeetingOfficialActionEffectService
    {
        public MeetingOfficialActionEffectResult Evaluate(
            MeetingActionProposalData proposal)
        {
            if (string.IsNullOrWhiteSpace(proposal.ProposalId))
            {
                return NoEffect(proposal, "Proposal is missing.");
            }

            if (proposal.Status != MeetingActionProposalStatus.Resolved)
            {
                return NoEffect(proposal, "Proposal is not resolved.");
            }

            if (proposal.Request.ActionType == MeetingActionType.NoAction)
            {
                return NoEffect(proposal, "NoAction resolved with no official effect.");
            }

            if (proposal.Request.ActionType == MeetingActionType.None)
            {
                return NoEffect(proposal, "Action type is invalid.");
            }

            return new MeetingOfficialActionEffectResult(
                proposal,
                MeetingOfficialActionEffectType.ApplyOfficialAction,
                true,
                "Resolved proposal can apply an official action effect.");
        }

        private static MeetingOfficialActionEffectResult NoEffect(
            MeetingActionProposalData proposal,
            string message)
        {
            return new MeetingOfficialActionEffectResult(
                proposal,
                MeetingOfficialActionEffectType.None,
                false,
                message);
        }
    }
}
