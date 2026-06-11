namespace OFIS.Meetings
{
    public readonly struct MeetingVoteSubmitResult
    {
        public bool Success { get; }
        public MeetingVoteData Vote { get; }
        public string Message { get; }

        public MeetingVoteSubmitResult(bool success, MeetingVoteData vote, string message)
        {
            Success = success;
            Vote = vote;
            Message = string.IsNullOrWhiteSpace(message) ? "No message." : message;
        }

        public static MeetingVoteSubmitResult Failed(MeetingVoteData vote, string message)
        {
            return new MeetingVoteSubmitResult(false, vote, message);
        }

        public override string ToString()
        {
            return $"Success={Success}, Vote={Vote}, Message={Message}";
        }
    }
}
