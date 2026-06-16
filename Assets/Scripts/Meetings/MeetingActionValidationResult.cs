namespace OFIS.Meetings
{
    public readonly struct MeetingActionValidationResult
    {
        public MeetingActionRequestData Request { get; }
        public bool IsValid { get; }
        public string Message { get; }

        public MeetingActionValidationResult(
            MeetingActionRequestData request,
            bool isValid,
            string message)
        {
            Request = request;
            IsValid = isValid;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting action validation resolved."
                : message;
        }

        public override string ToString()
        {
            return $"Valid={IsValid}, Message={Message}, Request=({Request})";
        }
    }
}
