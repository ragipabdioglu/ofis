using OFIS.Core.Ids;
using OFIS.Rooms;

namespace OFIS.Logs
{
    public static class RecordFactory
    {
        public static ServerRecord CreateDoorAccess(string recordId, MatchId matchId, PlayerId actorPlayerId, OfficeRoomType roomType, float serverTimeSeconds, string summary)
        {
            return Create(recordId, matchId, RecordCategory.DoorAccess, RecordVisibility.PublicSafe, actorPlayerId, roomType, serverTimeSeconds, summary, "door_access=critical");
        }

        public static ServerRecord CreateCameraPassage(string recordId, MatchId matchId, PlayerId actorPlayerId, OfficeRoomType roomType, float serverTimeSeconds, string summary)
        {
            return Create(recordId, matchId, RecordCategory.CameraPassage, RecordVisibility.PublicSafe, actorPlayerId, roomType, serverTimeSeconds, summary, "camera_passage=critical");
        }

        public static ServerRecord CreateTaskLifecycle(string recordId, MatchId matchId, PlayerId actorPlayerId, OfficeRoomType roomType, float serverTimeSeconds, TaskLogState state)
        {
            return Create(recordId, matchId, RecordCategory.Task, RecordVisibility.PublicSafe, actorPlayerId, roomType, serverTimeSeconds, $"Task lifecycle state={state}.", $"task_state={state}");
        }

        public static ServerRecord CreateCompany(string recordId, MatchId matchId, OfficeRoomType roomType, float serverTimeSeconds, string summary)
        {
            return Create(recordId, matchId, RecordCategory.Company, RecordVisibility.PublicSafe, new PlayerId("system_company"), roomType, serverTimeSeconds, summary, "company=quality");
        }

        private static ServerRecord Create(string recordId, MatchId matchId, RecordCategory category, RecordVisibility visibility, PlayerId actorPlayerId, OfficeRoomType roomType, float serverTimeSeconds, string summary, string rawPayload)
        {
            return new ServerRecord(recordId, matchId, category, visibility, actorPlayerId, "none", roomType, serverTimeSeconds, summary, rawPayload);
        }
    }
}
