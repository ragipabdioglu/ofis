using OFIS.Core.Ids;
using OFIS.Players;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpseHideDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly CorpseHideService _hideService = new CorpseHideService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateCorpseHide();
        }

        [ContextMenu("Validate Corpse Hide")]
        public void ValidateCorpseHide()
        {
            ValidateCarriedCorpseCanBeHidden();
            ValidateHideRequiresCarriedCorpse();
            ValidateOccupiedHideSpotRejects();
            ValidateInactiveHideSpotRejects();
        }

        private void ValidateCarriedCorpseCanBeHidden()
        {
            CorpseCarryState carryState = BuildCarryState("corpse_hide_7h_accept", out CorpsePlaceholder corpse);
            CorpseHideSpotState hideSpot = BuildHideSpot("hide_spot_7h_accept", true);

            CorpseHideCommandResult result =
                _hideService.Hide(BuildContext(carryState, hideSpot));

            bool passed = result.Success
                && result.HiddenCorpse == corpse
                && result.CarryStateCleared
                && !carryState.IsCarrying
                && !corpse.IsPublicWorldObject
                && hideSpot.HiddenCorpse == corpse
                && corpse.transform.position == hideSpot.WorldPosition;

            Destroy(corpse.gameObject);
            Destroy(carryState.gameObject);
            LogResult("CarriedCorpseCanBeHidden", passed, result.ToString());
        }

        private void ValidateHideRequiresCarriedCorpse()
        {
            GameObject carrier = new GameObject("CorpseHideDebug_EmptyCarrier");
            CorpseCarryState carryState = carrier.AddComponent<CorpseCarryState>();
            CorpseHideCommandResult result =
                _hideService.Hide(BuildContext(carryState, BuildHideSpot("hide_spot_7h_empty", true)));

            Destroy(carrier);
            LogResult("HideRequiresCarriedCorpse", !result.Success, result.ToString());
        }

        private void ValidateOccupiedHideSpotRejects()
        {
            CorpseCarryState carryState = BuildCarryState("corpse_hide_7h_occupied", out CorpsePlaceholder corpse);
            CorpseHideSpotState hideSpot = BuildHideSpot("hide_spot_7h_occupied", true);
            hideSpot.StoreCorpse(BuildDetachedCorpse("corpse_hide_7h_existing"));

            CorpseHideCommandResult result =
                _hideService.Hide(BuildContext(carryState, hideSpot));

            Destroy(corpse.gameObject);
            Destroy(hideSpot.HiddenCorpse.gameObject);
            Destroy(carryState.gameObject);
            LogResult("OccupiedHideSpotRejects", !result.Success, result.ToString());
        }

        private void ValidateInactiveHideSpotRejects()
        {
            CorpseCarryState carryState = BuildCarryState("corpse_hide_7h_inactive", out CorpsePlaceholder corpse);
            CorpseHideCommandResult result =
                _hideService.Hide(BuildContext(carryState, BuildHideSpot("hide_spot_7h_inactive", false)));

            Destroy(corpse.gameObject);
            Destroy(carryState.gameObject);
            LogResult("InactiveHideSpotRejects", !result.Success, result.ToString());
        }

        private static CorpseHideCommandContext BuildContext(
            CorpseCarryState carryState,
            CorpseHideSpotState hideSpot)
        {
            return new CorpseHideCommandContext(
                "hide_7h_command",
                new PlayerId("killer_hide_01"),
                PlayerLifeState.Alive,
                carryState,
                hideSpot);
        }

        private static CorpseHideSpotState BuildHideSpot(string hideSpotId, bool isActive)
        {
            return new CorpseHideSpotState(
                hideSpotId,
                OfficeRoomType.StorageRoom,
                new Vector3(8f, 2f, 0f),
                isActive);
        }

        private static CorpseCarryState BuildCarryState(
            string corpseId,
            out CorpsePlaceholder corpse)
        {
            GameObject carrier = new GameObject("CorpseHideDebug_Carrier");
            CorpseCarryState carryState = carrier.AddComponent<CorpseCarryState>();
            corpse = BuildDetachedCorpse(corpseId);
            carryState.StartCarrying(corpse);
            return carryState;
        }

        private static CorpsePlaceholder BuildDetachedCorpse(string corpseId)
        {
            GameObject corpseObject = new GameObject(corpseId);
            corpseObject.AddComponent<BoxCollider2D>().isTrigger = true;
            CorpsePlaceholder corpse = corpseObject.AddComponent<CorpsePlaceholder>();
            corpse.Initialize(
                new CorpsePublicState(
                    new CorpseId(corpseId),
                    new PlayerId("victim_hide_01"),
                    "Merve Kaya",
                    new Vector3(2f, 2f, 0f),
                    true));
            return corpse;
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[CorpseHideDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpseHideDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
