using OFIS.Corpse;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OFIS.Kill
{
    public sealed class DebugKillInputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerKillTargetDetector detector;
        [SerializeField] private KillEligibilityService killEligibilityService;
        [SerializeField] private NearbyKillTargetDebugHud nearbyKillTargetDebugHud;
        [SerializeField] private CorpseSpawnService corpseSpawnService;

        [Header("Input")]
        [SerializeField] private KeyCode killKey = KeyCode.K;

        [Header("Debug Room Rule Fallback")]
        [SerializeField] private bool roomAllowsKill = true;

        [Header("Debug Behavior")]
        [SerializeField] private bool markTargetDeadOnAcceptedKill = true;
        [SerializeField] private bool spawnCorpseOnAcceptedKill = true;

        private void Awake()
        {
            if (detector == null)
                detector = FindAnyObjectByType<PlayerKillTargetDetector>();

            if (killEligibilityService == null)
                killEligibilityService = FindAnyObjectByType<KillEligibilityService>();

            if (nearbyKillTargetDebugHud == null)
                nearbyKillTargetDebugHud = FindAnyObjectByType<NearbyKillTargetDebugHud>();

            if (corpseSpawnService == null)
                corpseSpawnService = FindAnyObjectByType<CorpseSpawnService>();
        }

        private void Update()
        {
            if (!WasKillKeyPressed())
                return;

            TryDebugKill();
        }

        private bool WasKillKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(killKey);
#endif
        }

        public void TryDebugKill()
        {
            if (detector == null)
            {
                Debug.LogWarning("[DebugKillInput] Failed: PlayerKillTargetDetector missing.");
                return;
            }

            if (killEligibilityService == null)
            {
                Debug.LogWarning("[DebugKillInput] Failed: KillEligibilityService missing.");
                return;
            }

            KillTargetDummy target = detector.CurrentTarget;

            if (target == null)
            {
                Debug.Log("[DebugKillInput] Failed: No nearby target.");
                return;
            }

            bool effectiveRoomAllowsKill = nearbyKillTargetDebugHud == null
                ? roomAllowsKill
                : nearbyKillTargetDebugHud.RoomAllowsKill;

            KillEligibilityResult result = killEligibilityService.CanKill(
                target.Role,
                target.IsKnownTarget,
                target.IsAlive,
                effectiveRoomAllowsKill);

            if (!result.CanKill)
            {
                Debug.Log(
                    $"[DebugKillInput] Blocked: Target={target.DisplayName}, " +
                    $"Role={target.Role}, Known={target.IsKnownTarget}, Alive={target.IsAlive}, " +
                    $"RoomAllowsKill={effectiveRoomAllowsKill}, Reason={result.Reason}");
                return;
            }

            Vector3 deathPosition = target.transform.position;

            Debug.Log(
                $"[DebugKillInput] Accepted: Target={target.DisplayName}, " +
                $"Role={target.Role}, Known={target.IsKnownTarget}, Alive={target.IsAlive}, " +
                $"RoomAllowsKill={effectiveRoomAllowsKill}.");

            if (markTargetDeadOnAcceptedKill)
                target.MarkDead();

            if (spawnCorpseOnAcceptedKill)
            {
                if (corpseSpawnService == null)
                {
                    Debug.LogWarning("[DebugKillInput] Corpse spawn skipped: CorpseSpawnService missing.");
                    return;
                }

                corpseSpawnService.SpawnCorpse(target.DisplayName, deathPosition);
            }
        }
    }
}