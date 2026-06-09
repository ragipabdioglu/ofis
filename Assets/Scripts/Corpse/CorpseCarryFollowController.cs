using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class CorpseCarryFollowController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CorpseCarryState carryState;
        [SerializeField] private Transform carrierTransform;

        [Header("Follow Settings")]
        [SerializeField] private Vector3 carryOffset = new(0f, -0.65f, 0f);
        [SerializeField] private bool snapInstantly = true;
        [SerializeField] private float followSpeed = 18f;

        [Header("Debug")]
        [SerializeField] private bool logStateChanges = true;

        private CorpsePlaceholder _lastCarriedCorpse;

        public bool HasCarrier => carrierTransform != null;
        public Vector3 CarryOffset => carryOffset;
        public bool IsFollowing => carryState != null && carryState.IsCarrying && carryState.CarriedCorpse != null && carrierTransform != null;

        private void Awake()
        {
            if (carryState == null)
                carryState = FindAnyObjectByType<CorpseCarryState>();

            if (carrierTransform == null)
                carrierTransform = transform;
        }

        private void LateUpdate()
        {
            UpdateFollowState();
        }

        private void UpdateFollowState()
        {
            if (carryState == null)
            {
                if (_lastCarriedCorpse != null)
                    ClearLastCarriedCorpse();

                return;
            }

            CorpsePlaceholder carriedCorpse = carryState.CarriedCorpse;

            if (carriedCorpse == null)
            {
                if (_lastCarriedCorpse != null)
                    ClearLastCarriedCorpse();

                return;
            }

            if (carrierTransform == null)
            {
                Debug.LogWarning("[CorpseCarryFollow] Carrier transform missing. Corpse follow skipped.");
                return;
            }

            if (_lastCarriedCorpse != carriedCorpse)
            {
                _lastCarriedCorpse = carriedCorpse;

                if (logStateChanges)
                    Debug.Log($"[CorpseCarryFollow] Started following carrier. Victim={carriedCorpse.VictimName}");
            }

            Vector3 targetPosition = carrierTransform.position + carryOffset;

            if (snapInstantly)
            {
                carriedCorpse.transform.position = targetPosition;
                return;
            }

            carriedCorpse.transform.position = Vector3.Lerp(
                carriedCorpse.transform.position,
                targetPosition,
                followSpeed * Time.deltaTime);
        }

        private void ClearLastCarriedCorpse()
        {
            if (logStateChanges)
                Debug.Log($"[CorpseCarryFollow] Stopped following carrier. Victim={_lastCarriedCorpse.VictimName}");

            _lastCarriedCorpse = null;
        }
    }
}
