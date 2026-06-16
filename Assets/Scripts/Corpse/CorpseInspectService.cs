using OFIS.Core.Ids;

namespace OFIS.Corpse
{
    public sealed class CorpseInspectService
    {
        public CorpseInspectResult Inspect(CorpseInspectRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.InspectId))
                return CorpseInspectResult.Rejected("Inspect id is required.");

            if (string.IsNullOrWhiteSpace(request.InspectorPlayerId.Value))
                return CorpseInspectResult.Rejected("Inspector player id is required.");

            if (request.Corpse == null)
                return CorpseInspectResult.Rejected("Corpse is required.");

            if (string.IsNullOrWhiteSpace(request.Corpse.CorpseId))
                return CorpseInspectResult.Rejected("Corpse id is missing.");

            if (string.IsNullOrWhiteSpace(request.Corpse.VictimPlayerId))
                return CorpseInspectResult.Rejected("Victim player id is missing.");

            CorpseOwnerKnowledge knowledge = new CorpseOwnerKnowledge(
                request.InspectorPlayerId,
                new CorpseId(request.Corpse.CorpseId),
                new PlayerId(request.Corpse.VictimPlayerId),
                request.Corpse.VictimName,
                request.RoomType,
                request.ServerTimeSeconds,
                true);

            return CorpseInspectResult.Accepted(knowledge);
        }
    }
}
