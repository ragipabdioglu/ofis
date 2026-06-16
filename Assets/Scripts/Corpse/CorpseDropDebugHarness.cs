using OFIS.Core.Ids;
using OFIS.Players;
using OFIS.Rules;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpseDropDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly CorpseDropService _dropService = new CorpseDropService();
        private readonly CorpseCarryActionGuardService _actionGuard = new CorpseCarryActionGuardService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateCorpseDrop();
        }

        [ContextMenu("Validate Corpse Drop")]
        public void ValidateCorpseDrop()
        {
            ValidateDropMovesCorpseAndClearsCarryState();
            ValidateDropWithoutCarriedCorpseRejects();
            ValidateDropClearsCarryActionBlocks();
        }

        private void ValidateDropMovesCorpseAndClearsCarryState()
        {
            CorpseCarryState carryState = BuildCarryState("corpse_drop_7g_move", out CorpsePlaceholder corpse);
            Vector3 dropPosition = new Vector3(5f, 2f, 0f);

            CorpseDropCommandResult result = _dropService.Drop(
                BuildContext(carryState, dropPosition));

            bool passed = result.Success
                && result.DroppedCorpse == corpse
                && result.CarryStateCleared
                && !carryState.IsCarrying
                && corpse.transform.position == dropPosition;

            Destroy(corpse.gameObject);
            Destroy(carryState.gameObject);
            LogResult("DropMovesCorpseAndClearsCarryState", passed, result.ToString());
        }

        private void ValidateDropWithoutCarriedCorpseRejects()
        {
            GameObject carrier = new GameObject("CorpseDropDebug_EmptyCarrier");
            CorpseCarryState carryState = carrier.AddComponent<CorpseCarryState>();

            CorpseDropCommandResult result = _dropService.Drop(
                BuildContext(carryState, new Vector3(5f, 3f, 0f)));

            bool passed = !result.Success && !carryState.IsCarrying;

            Destroy(carrier);
            LogResult("DropWithoutCarriedCorpseRejects", passed, result.ToString());
        }

        private void ValidateDropClearsCarryActionBlocks()
        {
            CorpseCarryState carryState = BuildCarryState("corpse_drop_7g_blocks", out CorpsePlaceholder corpse);
            PlayerActionRuleResult blockedBeforeDrop =
                _actionGuard.CanPerformWhileCarrying(PlayerActionType.Kill, carryState.IsCarrying);

            CorpseDropCommandResult dropResult = _dropService.Drop(
                BuildContext(carryState, new Vector3(5f, 4f, 0f)));

            PlayerActionRuleResult allowedAfterDrop =
                _actionGuard.CanPerformWhileCarrying(PlayerActionType.Kill, carryState.IsCarrying);

            bool passed = !blockedBeforeDrop.IsAllowed
                && dropResult.Success
                && allowedAfterDrop.IsAllowed;

            Destroy(corpse.gameObject);
            Destroy(carryState.gameObject);
            LogResult("DropClearsCarryActionBlocks", passed, dropResult.ToString());
        }

        private static CorpseDropCommandContext BuildContext(
            CorpseCarryState carryState,
            Vector3 dropPosition)
        {
            return new CorpseDropCommandContext(
                "drop_7g_command",
                new PlayerId("killer_drop_01"),
                PlayerLifeState.Alive,
                carryState,
                dropPosition);
        }

        private static CorpseCarryState BuildCarryState(
            string corpseId,
            out CorpsePlaceholder corpse)
        {
            GameObject carrier = new GameObject("CorpseDropDebug_Carrier");
            CorpseCarryState carryState = carrier.AddComponent<CorpseCarryState>();

            GameObject corpseObject = new GameObject(corpseId);
            corpseObject.AddComponent<BoxCollider2D>().isTrigger = true;
            corpse = corpseObject.AddComponent<CorpsePlaceholder>();
            corpse.Initialize(
                new CorpsePublicState(
                    new CorpseId(corpseId),
                    new PlayerId("victim_drop_01"),
                    "Merve Kaya",
                    new Vector3(2f, 2f, 0f),
                    true));

            carryState.StartCarrying(corpse);
            return carryState;
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[CorpseDropDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpseDropDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
