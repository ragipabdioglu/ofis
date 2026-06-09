using OFIS.LocalPlayer;
using OFIS.Roles;
using UnityEngine;

namespace OFIS.Kill
{
    public sealed class KillEligibilityService : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LocalPlayerIdentityBinding localPlayerIdentity;

        [Header("Debug Fallback")]
        [SerializeField] private int debugKnownTargetCount = 2;

        public int KnownTargetCount => debugKnownTargetCount;

        public PlayerRole LocalPlayerRole => localPlayerIdentity == null
            ? default
            : localPlayerIdentity.OwnRole;

        private void Awake()
        {
            if (localPlayerIdentity == null)
                localPlayerIdentity = FindAnyObjectByType<LocalPlayerIdentityBinding>();
        }

        public KillEligibilityResult CanKill(
            PlayerRole targetRole,
            bool targetIsKnownTarget,
            bool targetIsAlive,
            bool roomAllowsKill)
        {
            if (localPlayerIdentity == null)
                return KillEligibilityResult.Rejected("Local player identity missing.");

            if (localPlayerIdentity.OwnRole != PlayerRole.Killer)
                return KillEligibilityResult.Rejected($"Local player is not Killer. LocalRole={localPlayerIdentity.OwnRole}");

            if (!roomAllowsKill)
                return KillEligibilityResult.Rejected("Room rule blocked kill.");

            if (!targetIsAlive)
                return KillEligibilityResult.Rejected("Target is not alive.");

            if (targetRole != PlayerRole.Victim)
                return KillEligibilityResult.Rejected($"Target role is not Victim. TargetRole={targetRole}");

            if (!targetIsKnownTarget)
                return KillEligibilityResult.Rejected("Target is not in killer known target list.");

            return KillEligibilityResult.Allowed();
        }
    }
}