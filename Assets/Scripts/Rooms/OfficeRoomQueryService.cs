using UnityEngine;

namespace OFIS.Rooms
{
    public sealed class OfficeRoomQueryService
    {
        public OfficeRoomQueryResult QueryRoomAtPosition(Vector2 position, OfficeRoomZone[] zones)
        {
            if (zones == null || zones.Length == 0)
                return OfficeRoomQueryResult.None("No room zones provided.");

            for (int i = zones.Length - 1; i >= 0; i--)
            {
                OfficeRoomZone zone = zones[i];

                if (zone == null)
                    continue;

                Collider2D collider = zone.GetComponent<Collider2D>();

                if (collider == null)
                    continue;

                if (!collider.OverlapPoint(position))
                    continue;

                return new OfficeRoomQueryResult(
                    true,
                    zone.RoomType,
                    zone.DisplayName,
                    "Position overlaps room zone.");
            }

            return OfficeRoomQueryResult.None("Position does not overlap any room zone.");
        }
    }
}
