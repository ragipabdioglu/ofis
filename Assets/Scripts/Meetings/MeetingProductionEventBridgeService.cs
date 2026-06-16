using OFIS.Core.Events;

namespace OFIS.Meetings
{
    public sealed class MeetingProductionEventBridgeService
    {
        public MeetingProductionEventBridgeResult Publish(
            GameEventBus eventBus,
            MeetingProductionApplyResult applyResult,
            float createdAtRealtime)
        {
            bool shouldPublish = ShouldPublish(applyResult);

            if (!shouldPublish)
            {
                return new MeetingProductionEventBridgeResult(
                    shouldPublishEvent: false,
                    publishedEvent: false,
                    runtimeEvent: null,
                    message: "No production runtime event was required.");
            }

            MeetingProductionRuntimeEvent runtimeEvent =
                new MeetingProductionRuntimeEvent(applyResult, createdAtRealtime);

            if (eventBus == null)
            {
                return new MeetingProductionEventBridgeResult(
                    shouldPublishEvent: true,
                    publishedEvent: false,
                    runtimeEvent: runtimeEvent,
                    message: "Runtime event was created but GameEventBus was missing.");
            }

            eventBus.Publish(runtimeEvent);

            return new MeetingProductionEventBridgeResult(
                shouldPublishEvent: true,
                publishedEvent: true,
                runtimeEvent: runtimeEvent,
                message: "Meeting production runtime event published.");
        }

        private static bool ShouldPublish(MeetingProductionApplyResult applyResult)
        {
            return applyResult.Command.ActionType != MeetingProductionBridgeActionType.None;
        }
    }
}
