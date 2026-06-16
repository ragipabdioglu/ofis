namespace OFIS.Meetings
{
    public readonly struct MeetingActionVoteSubmitResult
    {
        public bool Success { get; }
        public MeetingActionVoteData Vote { get; }
        public string Message { get; }

        public MeetingActionVoteSubmitResult(
            bool success,
            MeetingActionVoteData vote,
            string message)
        {
            Success = success;
            Vote = vote;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action vote submit resolved."
                : message;
        }

        public static MeetingActionVoteSubmitResult Failed(
            MeetingActionVoteData vote,
            string message)
        {
            return new MeetingActionVoteSubmitResult(false, vote, message);
        }

        public override string ToString()
        {
            return $"Success={Success}, Vote=({Vote}), Message={Message}";
        }
    }
}
