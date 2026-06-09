using System.Collections.Generic;
using OFIS.Corpse;
using OFIS.Interactions;
using OFIS.Players;
using UnityEngine;

namespace OFIS.PlayerControl
{
    public sealed class CorpseCarryRestrictionDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private readonly PlayerMovementModifierService _movementModifierService = new PlayerMovementModifierService();
        private readonly InteractionPermissionService _interactionPermissionService = new InteractionPermissionService();
        private readonly LocalInteractionResolver _interactionResolver = new LocalInteractionResolver();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateCarryRestrictionBridge();
        }

        [ContextMenu("Validate Carry Restriction Bridge")]
        public void ValidateCarryRestrictionBridge()
        {
            CorpseCarryState carryState = BuildCarryState();
            WorldInteractionResolveResult interaction = BuildSelectedInteraction();

            ValidateAliveNormalMovement();
            ValidateAliveCarryingMovement(carryState);
            ValidateDeadMovementBlocked(carryState);
            ValidateAliveInteractionAllowed(interaction);
            ValidateDeadInteractionBlocked(interaction);
            ValidateDisconnectedInteractionBlocked(interaction);
        }

        private static CorpseCarryState BuildCarryState()
        {
            GameObject carrier = new GameObject("CarryRestriction_Test_Carrier");
            CorpseCarryState carryState = carrier.AddComponent<CorpseCarryState>();

            GameObject corpseObject = new GameObject("CarryRestriction_Test_Corpse");
            CorpsePlaceholder corpse = corpseObject.AddComponent<CorpsePlaceholder>();
            corpse.Initialize("Debug Victim");

            carryState.StartCarrying(corpse);
            return carryState;
        }

        private WorldInteractionResolveResult BuildSelectedInteraction()
        {
            List<WorldInteractionCandidate> candidates = new List<WorldInteractionCandidate>
            {
                new WorldInteractionCandidate(WorldInteractionType.Task, "Nearby Task", 0.2f, true)
            };

            return _interactionResolver.Resolve(candidates);
        }

        private void ValidateAliveNormalMovement()
        {
            PlayerMovementModifierResult result = _movementModifierService.Evaluate(PlayerLifeState.Alive, null);
            bool passed = result.CanMove && Mathf.Approximately(result.SpeedMultiplier, 1f);

            LogMovementResult("AliveNormalMovement", passed, result);
        }

        private void ValidateAliveCarryingMovement(CorpseCarryState carryState)
        {
            PlayerMovementModifierResult result = _movementModifierService.Evaluate(PlayerLifeState.Alive, carryState);
            bool passed = result.CanMove && result.SpeedMultiplier > 0f && result.SpeedMultiplier < 1f;

            LogMovementResult("AliveCarryingMovementSlow", passed, result);
        }

        private void ValidateDeadMovementBlocked(CorpseCarryState carryState)
        {
            PlayerMovementModifierResult result = _movementModifierService.Evaluate(PlayerLifeState.Dead, carryState);
            bool passed = !result.CanMove && Mathf.Approximately(result.SpeedMultiplier, 0f);

            LogMovementResult("DeadMovementBlocked", passed, result);
        }

        private void ValidateAliveInteractionAllowed(WorldInteractionResolveResult interaction)
        {
            InteractionPermissionResult result = _interactionPermissionService.Evaluate(PlayerLifeState.Alive, interaction);
            bool passed = result.CanInteract;

            LogInteractionResult("AliveInteractionAllowed", passed, result);
        }

        private void ValidateDeadInteractionBlocked(WorldInteractionResolveResult interaction)
        {
            InteractionPermissionResult result = _interactionPermissionService.Evaluate(PlayerLifeState.Dead, interaction);
            bool passed = !result.CanInteract;

            LogInteractionResult("DeadInteractionBlocked", passed, result);
        }

        private void ValidateDisconnectedInteractionBlocked(WorldInteractionResolveResult interaction)
        {
            InteractionPermissionResult result = _interactionPermissionService.Evaluate(PlayerLifeState.Disconnected, interaction);
            bool passed = !result.CanInteract;

            LogInteractionResult("DisconnectedInteractionBlocked", passed, result);
        }

        private static void LogMovementResult(string testName, bool passed, PlayerMovementModifierResult result)
        {
            if (passed)
                Debug.Log($"[CarryRestrictionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[CarryRestrictionValidator] FAIL {testName}: {result}");
        }

        private static void LogInteractionResult(string testName, bool passed, InteractionPermissionResult result)
        {
            if (passed)
                Debug.Log($"[CarryRestrictionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[CarryRestrictionValidator] FAIL {testName}: {result}");
        }
    }
}
