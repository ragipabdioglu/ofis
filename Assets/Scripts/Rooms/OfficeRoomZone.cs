using UnityEngine;

namespace OFIS.Rooms
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class OfficeRoomZone : MonoBehaviour
    {
        [SerializeField] private OfficeRoomType roomType = OfficeRoomType.Unknown;
        [SerializeField] private string displayName = "Unknown Room";

        public OfficeRoomType RoomType => roomType;
        public string DisplayName => displayName;

        private void Reset()
        {
            var collider = GetComponent<Collider2D>();

            if (collider != null)
                collider.isTrigger = true;
        }

        private void Awake()
        {
            var collider = GetComponent<Collider2D>();

            if (collider != null && !collider.isTrigger)
                collider.isTrigger = true;
        }
    }
}