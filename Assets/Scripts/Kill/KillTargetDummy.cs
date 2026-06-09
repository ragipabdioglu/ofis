using OFIS.Roles;
using UnityEngine;

namespace OFIS.Kill
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class KillTargetDummy : MonoBehaviour
    {
        [Header("Dummy Identity")]
        [SerializeField] private string displayName = "Mock Target";
        [SerializeField] private PlayerRole role = PlayerRole.Victim;
        [SerializeField] private bool isKnownTarget = true;
        [SerializeField] private bool isAlive = true;

        [Header("Visual State")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color aliveColor = Color.white;
        [SerializeField] private Color deadColor = Color.gray;

        public string DisplayName => displayName;
        public PlayerRole Role => role;
        public bool IsKnownTarget => isKnownTarget;
        public bool IsAlive => isAlive;

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

            RefreshVisualState();
        }

        public void MarkDead()
        {
            if (!isAlive)
                return;

            isAlive = false;
            RefreshVisualState();

            Debug.Log($"[KillTargetDummy] {displayName} marked dead.");
        }

        public void ReviveForDebug()
        {
            isAlive = true;
            RefreshVisualState();

            Debug.Log($"[KillTargetDummy] {displayName} revived for debug.");
        }

        private void RefreshVisualState()
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.color = isAlive ? aliveColor : deadColor;
        }
    }
}