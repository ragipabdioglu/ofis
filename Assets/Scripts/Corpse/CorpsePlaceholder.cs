using UnityEngine;

namespace OFIS.Corpse
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CorpsePlaceholder : MonoBehaviour
    {
        [Header("Corpse Data")]
        [SerializeField] private string corpseId;
        [SerializeField] private string victimPlayerId;
        [SerializeField] private string victimName = "Unknown Victim";
        [SerializeField] private bool isPublicWorldObject = true;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color corpseColor = new(0.45f, 0.05f, 0.05f, 1f);

        public string CorpseId => corpseId;
        public string VictimPlayerId => victimPlayerId;
        public string VictimName => victimName;
        public bool IsPublicWorldObject => isPublicWorldObject;

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

        public void Initialize(CorpsePublicState publicState)
        {
            corpseId = publicState.CorpseId.ToString();
            victimPlayerId = publicState.VictimId.ToString();
            victimName = string.IsNullOrWhiteSpace(publicState.VictimDisplayName)
                ? "Unknown Victim"
                : publicState.VictimDisplayName;
            isPublicWorldObject = publicState.IsPublicWorldObject;

            transform.position = publicState.WorldPosition;
            name = $"Corpse_{victimName}";

            Debug.Log(
                $"[Corpse] Public corpse initialized. " +
                $"CorpseId={corpseId}, Victim={victimName}, Public={isPublicWorldObject}");
        }

        public void SetPublicWorldObject(bool isPublic)
        {
            isPublicWorldObject = isPublic;

            Collider2D corpseCollider = GetComponent<Collider2D>();
            if (corpseCollider != null)
                corpseCollider.enabled = isPublicWorldObject;

            if (spriteRenderer != null)
                spriteRenderer.enabled = isPublicWorldObject;

            Debug.Log($"[Corpse] Public world visibility changed. Victim={victimName}, Public={isPublicWorldObject}");
        }
    }
}
