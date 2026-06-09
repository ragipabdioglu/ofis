using UnityEngine;

namespace OFIS.Rooms
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class OfficeWall : MonoBehaviour
    {
        [SerializeField] private string wallName = "Wall";

        public string WallName => wallName;

        private void Reset()
        {
            SetupCollider();
        }

        private void Awake()
        {
            SetupCollider();
        }

        private void SetupCollider()
        {
            var collider = GetComponent<BoxCollider2D>();

            if (collider == null)
                return;

            collider.isTrigger = false;
        }
    }
}