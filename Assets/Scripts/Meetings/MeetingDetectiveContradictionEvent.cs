namespace OFIS.Meetings
{
    public readonly struct MeetingDetectiveContradictionEvent
    {
        public string EventId { get; }
        public string MeetingId { get; }
        public string DetectivePlayerId { get; }
        public string ReportId { get; }
        public string ProposalId { get; }
        public MeetingDetectiveContradictionFlagType FlagType { get; }
        public string Message { get; }

        public MeetingDetectiveContradictionEvent(
            string eventId,
            string meetingId,
            string detectivePlayerId,
            string reportId,
            string proposalId,
            MeetingDetectiveContradictionFlagType flagType,
            string message)
        {
            EventId = string.IsNullOrWhiteSpace(eventId) ? string.Empty : eventId;
            MeetingId = string.IsNullOrWhiteSpace(meetingId) ? string.Empty : meetingId;
            DetectivePlayerId = string.IsNullOrWhiteSpace(detectivePlayerId)
                ? string.Empty
                : detectivePlayerId;
            ReportId = string.IsNullOrWhiteSpace(reportId) ? string.Empty : reportId;
            ProposalId = string.IsNullOrWhiteSpace(proposalId) ? string.Empty : proposalId;
            FlagType = flagType;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Detective contradiction flag raised."
                : message;
        }

        public override string ToString()
        {
            return $"EventId={EventId}, MeetingId={MeetingId}, Detective={DetectivePlayerId}, Report={ReportId}, Proposal={ProposalId}, Flag={FlagType}, Message={Message}";
        }
    }
}
