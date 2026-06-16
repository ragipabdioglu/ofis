using OFIS.Core.Ids;
using UnityEngine;

namespace OFIS.Corpse
{
    public readonly struct CorpsePublicState
    {
        public CorpseId CorpseId { get; }
        public PlayerId VictimId { get; }
        public string VictimDisplayName { get; }
        public Vector3 WorldPosition { get; }
        public bool IsPublicWorldObject { get; }

        public CorpsePublicState(
            CorpseId corpseId,
            PlayerId victimId,
            string victimDisplayName,
            Vector3 worldPosition,
            bool isPublicWorldObject)
        {
            CorpseId = corpseId;
            VictimId = victimId;
            VictimDisplayName = victimDisplayName;
            WorldPosition = worldPosition;
            IsPublicWorldObject = isPublicWorldObject;
        }

        public override string ToString()
        {
            return $"Corpse={CorpseId}, Victim={VictimDisplayName}, Public={IsPublicWorldObject}, Position={WorldPosition}";
        }
    }
}
