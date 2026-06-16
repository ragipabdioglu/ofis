using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Sabotage
{
    public readonly struct SabotageDeviceDefinition
    {
        public string DeviceId { get; }
        public SabotageType SabotageType { get; }
        public OfficeRoomType RoomType { get; }
        public Vector3 WorldPosition { get; }
        public float InteractionRange { get; }

        public SabotageDeviceDefinition(
            string deviceId,
            SabotageType sabotageType,
            OfficeRoomType roomType,
            Vector3 worldPosition,
            float interactionRange)
        {
            DeviceId = string.IsNullOrWhiteSpace(deviceId) ? "unknown_device" : deviceId;
            SabotageType = sabotageType;
            RoomType = roomType;
            WorldPosition = worldPosition;
            InteractionRange = interactionRange <= 0f ? 1.5f : interactionRange;
        }

        public override string ToString()
        {
            return $"Device={DeviceId}, Type={SabotageType}, Room={RoomType}, Range={InteractionRange:0.##}";
        }
    }
}
