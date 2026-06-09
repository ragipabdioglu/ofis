using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OFIS.Rooms
{
    public sealed class PlayerRoomTracker : MonoBehaviour
    {
        private readonly List<OfficeRoomZone> _activeZones = new();

        public OfficeRoomType CurrentRoomType { get; private set; } = OfficeRoomType.None;
        public string CurrentRoomDisplayName { get; private set; } = "None";

        private void OnTriggerEnter2D(Collider2D other)
        {
            var zone = other.GetComponent<OfficeRoomZone>();

            if (zone == null)
                return;

            if (!_activeZones.Contains(zone))
                _activeZones.Add(zone);

            RefreshCurrentRoom();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var zone = other.GetComponent<OfficeRoomZone>();

            if (zone == null)
                return;

            if (_activeZones.Contains(zone))
                _activeZones.Remove(zone);

            RefreshCurrentRoom();
        }

        private void RefreshCurrentRoom()
        {
            var previousRoom = CurrentRoomType;

            if (_activeZones.Count == 0)
            {
                CurrentRoomType = OfficeRoomType.None;
                CurrentRoomDisplayName = "None";
            }
            else
            {
                var selectedZone = _activeZones.Last();

                CurrentRoomType = selectedZone.RoomType;
                CurrentRoomDisplayName = selectedZone.DisplayName;
            }

            if (previousRoom != CurrentRoomType)
            {
                Debug.Log($"[RoomTracker] Current room changed: {previousRoom} -> {CurrentRoomType} ({CurrentRoomDisplayName})");
            }
        }
    }
}