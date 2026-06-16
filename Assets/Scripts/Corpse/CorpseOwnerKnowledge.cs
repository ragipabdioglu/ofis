using OFIS.Core.Ids;
using OFIS.Rooms;

namespace OFIS.Corpse
{
    public readonly struct CorpseOwnerKnowledge
    {
        public PlayerId OwnerPlayerId { get; }
        public CorpseId CorpseId { get; }
        public PlayerId VictimPlayerId { get; }
        public string VictimDisplayName { get; }
        public OfficeRoomType FoundRoom { get; }
        public float InspectServerTimeSeconds { get; }
        public bool IsOwnerOnly { get; }

        public CorpseOwnerKnowledge(
            PlayerId ownerPlayerId,
            CorpseId corpseId,
            PlayerId victimPlayerId,
            string victimDisplayName,
            OfficeRoomType foundRoom,
            float inspectServerTimeSeconds,
            bool isOwnerOnly)
        {
            OwnerPlayerId = ownerPlayerId;
            CorpseId = corpseId;
            VictimPlayerId = victimPlayerId;
            VictimDisplayName = string.IsNullOrWhiteSpace(victimDisplayName)
                ? "Unknown Victim"
                : victimDisplayName;
            FoundRoom = foundRoom;
            InspectServerTimeSeconds = inspectServerTimeSeconds;
            IsOwnerOnly = isOwnerOnly;
        }

        public override string ToString()
        {
            return $"Owner={OwnerPlayerId}, Corpse={CorpseId}, Victim={VictimDisplayName}, Room={FoundRoom}, OwnerOnly={IsOwnerOnly}";
        }
    }
}
