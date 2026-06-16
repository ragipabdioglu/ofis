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

        public CorpsePlaceholder DropCarriedCorpse()
        {
            if (CarriedCorpse == null)
                return null;

            CorpsePlaceholder droppedCorpse = CarriedCorpse;

            Debug.Log($"[CorpseCarryState] Dropped corpse. Victim={droppedCorpse.VictimName}");

            CarriedCorpse = null;
            return droppedCorpse;
        }
    }
}
