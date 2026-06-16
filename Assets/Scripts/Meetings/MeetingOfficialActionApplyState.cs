namespace OFIS.Meetings
{
    public sealed class MeetingOfficialActionApplyState
    {
        public string MeetingId { get; private set; }
        public bool HasAppliedOfficialAction { get; private set; }
        public string AppliedProposalId { get; private set; }
        public MeetingActionType AppliedActionType { get; private set; }

        public MeetingOfficialActionApplyState(string meetingId)
        {
            Reset(meetingId);
        }

        public void MarkApplied(MeetingActionProposalData proposal)
        {
            HasAppliedOfficialAction = true;
            AppliedProposalId = proposal.ProposalId;
            AppliedActionType = proposal.Request.ActionType;
        }

        public void Reset(string meetingId)
        {
            MeetingId = string.IsNullOrWhiteSpace(meetingId) ? string.Empty : meetingId;
            HasAppliedOfficialAction = false;
            AppliedProposalId = string.Empty;
            AppliedActionType = MeetingActionType.None;
        }

        public override string ToString()
        {
            return $"MeetingId={MeetingId}, Applied={HasAppliedOfficialAction}, Proposal={AppliedProposalId}, ActionType={AppliedActionType}";
        }
    }
}
