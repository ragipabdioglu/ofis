using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class FoundCorpseReportMemory : MonoBehaviour
    {
        private readonly List<CorpsePlaceholder> _foundCorpses = new();

        public IReadOnlyList<CorpsePlaceholder> FoundCorpses => _foundCorpses;
        public int Count => _foundCorpses.Count;

        public bool HasFound(CorpsePlaceholder corpse)
        {
            return corpse != null && _foundCorpses.Contains(corpse);
        }

        public bool TryRecordFoundCorpse(CorpsePlaceholder corpse)
        {
            if (corpse == null)
                return false;

            if (_foundCorpses.Contains(corpse))
                return false;

            _foundCorpses.Add(corpse);

            Debug.Log($"[FoundCorpseReportMemory] Recorded found corpse. Victim={corpse.VictimName}");

            return true;
        }
    }
}