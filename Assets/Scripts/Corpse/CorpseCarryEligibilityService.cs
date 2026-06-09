using OFIS.LocalPlayer;
using OFIS.Roles;
using UnityEngine;

namespace OFIS.Corpse
{
    public readonly struct CorpseCarryEligibilityResult
    {
        public bool CanCarry { get; }
        public string Reason { get; }

        private CorpseCarryEligibilityResult(bool canCarry, string reason)
        {
            CanCarry = canCarry;
            Reason = reason;
        }

        public static CorpseCarryEligibilityResult Allowed()
        {
            return new CorpseCarryEligibilityResult(true, "Allowed");
        }

        public static CorpseCarryEligibilityResult Rejected(string reason)
        {
            return new CorpseCarryEligibilityResult(false, reason);
        }
    }

    public sealed class CorpseCarryEligibilityService : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LocalPlayerIdentityBinding localPlayerIdentity;

        private void Awake()
        {
            if (localPlayerIdentity == null)
                localPlayerIdentity = FindAnyObjectByType<LocalPlayerIdentityBinding>();
        }

        public CorpseCarryEligibilityResult CanCarryCorpse(
            CorpsePlaceholder corpse,
            bool roomAllowsCarry,
            bool alreadyCarrying)
        {
            if (localPlayerIdentity == null)
                return CorpseCarryEligibilityResult.Rejected("Local player identity missing.");

            if (localPlayerIdentity.OwnRole != PlayerRole.Killer)
                return CorpseCarryEligibilityResult.Rejected($"Local player is not Killer. LocalRole={localPlayerIdentity.OwnRole}");

            if (corpse == null)
                return CorpseCarryEligibilityResult.Rejected("No nearby corpse.");

            if (!roomAllowsCarry)
                return CorpseCarryEligibilityResult.Rejected("Room rule blocked corpse carry.");

            if (alreadyCarrying)
                return CorpseCarryEligibilityResult.Rejected("Already carrying a corpse.");

            return CorpseCarryEligibilityResult.Allowed();
        }
    }
}