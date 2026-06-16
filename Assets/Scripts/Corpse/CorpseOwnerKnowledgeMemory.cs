using System.Collections.Generic;
using OFIS.Core.Ids;
using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class CorpseOwnerKnowledgeMemory : MonoBehaviour
    {
        private readonly List<CorpseOwnerKnowledge> _knowledge = new List<CorpseOwnerKnowledge>();

        public IReadOnlyList<CorpseOwnerKnowledge> Knowledge => _knowledge;
        public int Count => _knowledge.Count;

        public bool HasKnowledge(PlayerId ownerPlayerId, CorpseId corpseId)
        {
            for (int i = 0; i < _knowledge.Count; i++)
            {
                if (_knowledge[i].OwnerPlayerId == ownerPlayerId
                    && _knowledge[i].CorpseId == corpseId)
                    return true;
            }

            return false;
        }

        public bool TryRecord(CorpseOwnerKnowledge knowledge)
        {
            if (!knowledge.IsOwnerOnly)
                return false;

            if (HasKnowledge(knowledge.OwnerPlayerId, knowledge.CorpseId))
                return false;

            _knowledge.Add(knowledge);
            Debug.Log($"[CorpseOwnerKnowledgeMemory] Recorded owner-only corpse knowledge. {knowledge}");
            return true;
        }
    }
}
