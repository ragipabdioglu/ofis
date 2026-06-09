using UnityEngine;

namespace OFIS.Corpse
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CorpsePlaceholder : MonoBehaviour
    {
        [Header("Corpse Data")]
        [SerializeField] private string victimName = "Unknown Victim";

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color corpseColor = new(0.45f, 0.05f, 0.05f, 1f);

        public string VictimName => victimName;

        private void Reset()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;

            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                spriteRenderer.color = corpseColor;
        }

        public void Initialize(string deadVictimName)
        {
            victimName = string.IsNullOrWhiteSpace(deadVictimName)
                ? "Unknown Victim"
                : deadVictimName;

            name = $"Corpse_{victimName}";

            Debug.Log($"[Corpse] Corpse placeholder initialized. Victim={victimName}");
        }
    }
}