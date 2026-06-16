namespace OFIS.Meetings
{
    public sealed class MeetingOfficialActionApplyService
    {
        private readonly MeetingOfficialActionEffectService _effectService;

        public MeetingOfficialActionApplyService()
        {
            _effectService = new MeetingOfficialActionEffectService();
        }

        public MeetingOfficialActionApplyService(
            MeetingOfficialActionEffectService effectService)
        {
            _effectService = effectService ?? new MeetingOfficialActionEffectService();
        }

        public MeetingOfficialActionApplyResult TryApply(
            MeetingOfficialActionApplyState state,
            MeetingActionProposalData proposal)
        {
            MeetingOfficialActionEffectResult effectResult = _effectService.Evaluate(proposal);

            if (state == null)
            {
                return new MeetingOfficialActionApplyResult(
                    false,
                    effectResult,
                    proposal.MeetingId,
                    false,
                    "Official action apply state is missing.");
            }

            if (!effectResult.ShouldApplyEffect)
            {
                return new MeetingOfficialActionApplyResult(
                    false,
                    effectResult,
                    state.MeetingId,
                    state.HasAppliedOfficialAction,
                    effectResult.Message);
            }

            if (!string.IsNullOrWhiteSpace(state.MeetingId)
                && !string.IsNullOrWhiteSpace(proposal.MeetingId)
                && state.MeetingId != proposal.MeetingId)
            {
                return new MeetingOfficialActionApplyResult(
                    false,
                    effectResult,
                    state.MeetingId,
                    state.HasAppliedOfficialAction,
                    "Proposal belongs to a different meeting.");
            }

            if (state.HasAppliedOfficialAction)
            {
                return new MeetingOfficialActionApplyResult(
                    false,
                    effectResult,
                    state.MeetingId,
                    true,
                    "Meeting already has an applied official action.");
            }

            state.MarkApplied(proposal);

            return new MeetingOfficialActionApplyResult(
                true,
                effectResult,
                state.MeetingId,
                true,
                "Official action applied for meeting.");
        }
    }
}
