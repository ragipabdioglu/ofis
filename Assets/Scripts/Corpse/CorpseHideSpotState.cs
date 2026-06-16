using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class CorpseHideSpotState
    {
        public string HideSpotId { get; }
        public OfficeRoomType RoomType { get; }
        public Vector3 WorldPosition { get; }
        public bool IsActive { get; private set; }
        public CorpsePlaceholder HiddenCorpse { get; private set; }
        public bool HasHiddenCorpse => HiddenCorpse != null;

        public CorpseHideSpotState(
            string hideSpotId,
            OfficeRoomType roomType,
            Vector3 worldPosition,
            bool isActive)
        {
            HideSpotId = string.IsNullOrWhiteSpace(hideSpotId) ? "unknown_hide_spot" : hideSpotId;
            RoomType = roomType;
            WorldPosition = worldPosition;
            IsActive = isActive;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void StoreCorpse(CorpsePlaceholder corpse)
        {
            HiddenCorpse = corpse;
        }

        public override string ToString()
        {
            string corpseName = HiddenCorpse == null ? "none" : HiddenCorpse.VictimName;
            return $"HideSpot={HideSpotId}, Room={RoomType}, Active={IsActive}, Corpse={corpseName}";
        }
    }
}
