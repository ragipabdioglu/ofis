using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class CorpseCarryState : MonoBehaviour
    {
        public bool IsCarrying => CarriedCorpse != null;
        public CorpsePlaceholder CarriedCorpse { get; private set; }

        public void StartCarrying(CorpsePlaceholder corpse)
        {
            if (corpse == null)
                return;

            CarriedCorpse = corpse;

            Debug.Log($"[CorpseCarryState] Started carrying corpse. Victim={corpse.VictimName}");
        }

        public void DropCarriedCorpse()
        {
            if (CarriedCorpse == null)
                return;

            Debug.Log($"[CorpseCarryState] Dropped corpse. Victim={CarriedCorpse.VictimName}");

            CarriedCorpse = null;
        }
    }
}