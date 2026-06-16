using OFIS.Core.Ids;
using OFIS.Rooms;

namespace OFIS.Corpse
{
    public readonly struct CorpseInspectRequest
    {
        public string InspectId { get; }
        public PlayerId InspectorPlayerId { get; }
        public CorpsePlaceholder Corpse { get; }
        public OfficeRoomType RoomType { get; }
        public float ServerTimeSeconds { get; }

        public CorpseInspectRequest(
            string inspectId,
            PlayerId inspectorPlayerId,
            CorpsePlaceholder corpse,
            OfficeRoomType roomType,
            float serverTimeSeconds)
        {
            InspectId = string.IsNullOrWhiteSpace(inspectId) ? "unknown_inspect" : inspectId;
            InspectorPlayerId = inspectorPlayerId;
            Corpse = corpse;
            RoomType = roomType;
            ServerTimeSeconds = serverTimeSeconds;
        }
    }
}
