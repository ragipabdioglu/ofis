namespace OFIS.Meetings
{
    public readonly struct MeetingActionProposalData
    {
        public string ProposalId { get; }
        public string MeetingId { get; }
        public MeetingActionRequestData Request { get; }
        public MeetingActionProposalStatus Status { get; }

        public MeetingActionProposalData(
            string proposalId,
            string meetingId,
            MeetingActionRequestData request,
            MeetingActionProposalStatus status)
        {
            ProposalId = string.IsNullOrWhiteSpace(proposalId) ? string.Empty : proposalId;
            MeetingId = string.IsNullOrWhiteSpace(meetingId) ? string.Empty : meetingId;
            Request = request;
            Status = status;
        }

        public MeetingActionProposalData WithStatus(MeetingActionProposalStatus status)
        {
            return new MeetingActionProposalData(ProposalId, MeetingId, Request, status);
        }

        public override string ToString()
        {
            return $"ProposalId={ProposalId}, MeetingId={MeetingId}, Status={Status}, Request=({Request})";
        }
    }
}
