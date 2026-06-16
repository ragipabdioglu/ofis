using OFIS.Core.Ids;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class CorpseMovementTraceService
    {
        public CorpseMovementTraceEvent FromCarryStarted(
            PlayerId actorPlayerId,
            CorpseCarryCommandResult carryResult,
            OfficeRoomType roomType,
            float serverTimeSeconds)
        {
            if (!carryResult.Success || carryResult.CarriedCorpse == null)
                return default;

            return BuildTrace(
                CorpseMovementTraceType.CarryStarted,
                actorPlayerId,
                carryResult.CarriedCorpse,
                roomType,
                carryResult.CarriedCorpse.transform.position,
                serverTimeSeconds);
        }

        public CorpseMovementTraceEvent FromDrop(
            PlayerId actorPlayerId,
            CorpseDropCommandResult dropResult,
            OfficeRoomType roomType,
            float serverTimeSeconds)
        {
            if (!dropResult.Success || dropResult.DroppedCorpse == null)
                return default;

            return BuildTrace(
                CorpseMovementTraceType.Dropped,
                actorPlayerId,
                dropResult.DroppedCorpse,
                roomType,
                dropResult.DropWorldPosition,
                serverTimeSeconds);
        }

        public CorpseMovementTraceEvent FromHide(
            PlayerId actorPlayerId,
            CorpseHideCommandResult hideResult,
            OfficeRoomType roomType,
            float serverTimeSeconds)
        {
            if (!hideResult.Success || hideResult.HiddenCorpse == null)
                return default;

            return BuildTrace(
                CorpseMovementTraceType.Hidden,
                actorPlayerId,
                hideResult.HiddenCorpse,
                roomType,
                hideResult.HiddenCorpse.transform.position,
                serverTimeSeconds);
        }

        private static CorpseMovementTraceEvent BuildTrace(
            CorpseMovementTraceType traceType,
            PlayerId actorPlayerId,
            CorpsePlaceholder corpse,
            OfficeRoomType roomType,
            Vector3 worldPosition,
            float serverTimeSeconds)
        {
            return new CorpseMovementTraceEvent(
                EvidenceTraceId.New(),
                traceType,
                new CorpseId(corpse.CorpseId),
                actorPlayerId,
                corpse.VictimName,
                roomType,
                worldPosition,
                serverTimeSeconds,
                true);
        }
    }
}
