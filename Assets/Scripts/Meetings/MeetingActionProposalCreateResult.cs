namespace OFIS.Meetings
{
    public readonly struct MeetingActionProposalCreateResult
    {
        public bool Success { get; }
        public MeetingActionProposalData Proposal { get; }
        public string Message { get; }

        public MeetingActionProposalCreateResult(
            bool success,
            MeetingActionProposalData proposal,
            string message)
        {
            Success = success;
            Proposal = proposal;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action proposal create resolved."
                : message;
        }

        public static MeetingActionProposalCreateResult Failed(
            MeetingActionProposalData proposal,
            string message)
        {
            return new MeetingActionProposalCreateResult(false, proposal, message);
        }

        public override string ToString()
        {
            return $"Success={Success}, Proposal=({Proposal}), Message={Message}";
        }
    }
}
