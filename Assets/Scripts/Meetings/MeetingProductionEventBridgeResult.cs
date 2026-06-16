namespace OFIS.Meetings
{
    public readonly struct MeetingProductionEventBridgeResult
    {
        public bool ShouldPublishEvent { get; }
        public bool PublishedEvent { get; }
        public MeetingProductionRuntimeEvent RuntimeEvent { get; }
        public string Message { get; }

        public MeetingProductionEventBridgeResult(
            bool shouldPublishEvent,
            bool publishedEvent,
            MeetingProductionRuntimeEvent runtimeEvent,
            string message)
        {
            ShouldPublishEvent = shouldPublishEvent;
            PublishedEvent = publishedEvent;
            RuntimeEvent = runtimeEvent;
            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting production event bridge completed."
                : message;
        }

        public override string ToString()
        {
            return $"ShouldPublish={ShouldPublishEvent}, Published={PublishedEvent}, Event=[{RuntimeEvent}], Message={Message}";
        }
    }
}
