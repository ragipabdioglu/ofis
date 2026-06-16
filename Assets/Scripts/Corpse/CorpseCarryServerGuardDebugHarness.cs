using OFIS.Core.Ids;
using OFIS.PlayerControl;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Rooms;
using OFIS.Rules;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Corpse
{
    public sealed class CorpseCarryServerGuardDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly CorpseCarryServerGuardService _carryGuard =
            new CorpseCarryServerGuardService();
        private readonly CorpseCarryActionGuardService _actionGuard =
            new CorpseCarryActionGuardService();
        private readonly PlayerMovementModifierService _movementService =
            new PlayerMovementModifierService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateCarryServerGuard();
        }

        [ContextMenu("Validate Corpse Carry Server Guard")]
        public void ValidateCarryServerGuard()
        {
            ValidateKillerCanCarryOnePublicCorpse();
            ValidateDetectiveCannotCarry();
            ValidateAlreadyCarryingBlocksSecondCorpse();
            ValidateMeetingRoomBlocksCarry();
            ValidateCarryingBlocksCriticalActions();
            ValidateCarryingSlowsMovement();
        }

        private void ValidateKillerCanCarryOnePublicCorpse()
        {
            CorpsePlaceholder corpse = BuildCorpse("corpse_carry_7f_accept");
            CorpseCarryCommandResult result = _carryGuard.TryStartCarry(BuildContext(corpse));
            bool passed = result.Success && result.CarriedCorpse == corpse;

            Destroy(corpse.gameObject);
            LogResult("KillerCanCarryOnePublicCorpse", passed, result.ToString());
        }

        private void ValidateDetectiveCannotCarry()
        {
            CorpsePlaceholder corpse = BuildCorpse("corpse_carry_7f_detective");
            CorpseCarryCommandResult result = _carryGuard.TryStartCarry(
                BuildContext(corpse, carrierRole: PlayerRole.Detective));

            Destroy(corpse.gameObject);
            LogResult("DetectiveCannotCarry", !result.Success, result.ToString());
        }

        private void ValidateAlreadyCarryingBlocksSecondCorpse()
        {
            CorpsePlaceholder corpse = BuildCorpse("corpse_carry_7f_second");
            CorpseCarryCommandResult result = _carryGuard.TryStartCarry(
                BuildContext(corpse, alreadyCarrying: true));

            Destroy(corpse.gameObject);
            LogResult("AlreadyCarryingBlocksSecondCorpse", !result.Success, result.ToString());
        }

        private void ValidateMeetingRoomBlocksCarry()
        {
            CorpsePlaceholder corpse = BuildCorpse("corpse_carry_7f_meeting_room");
            CorpseCarryCommandResult result = _carryGuard.TryStartCarry(
                BuildContext(corpse, carrierRoom: OfficeRoomType.MeetingRoom));

            Destroy(corpse.gameObject);
            LogResult("MeetingRoomBlocksCarry", !result.Success, result.ToString());
        }

        private void ValidateCarryingBlocksCriticalActions()
        {
            bool passed =
                !_actionGuard.CanPerformWhileCarrying(PlayerActionType.Kill, true).IsAllowed
                && !_actionGuard.CanPerformWhileCarrying(PlayerActionType.DoTask, true).IsAllowed
                && !_actionGuard.CanPerformWhileCarrying(PlayerActionType.Sabotage, true).IsAllowed
                && !_actionGuard.CanPerformWhileCarrying(PlayerActionType.JoinMeeting, true).IsAllowed
                && !_actionGuard.CanPerformWhileCarrying(PlayerActionType.CarryCorpse, true).IsAllowed
                && _actionGuard.CanPerformWhileCarrying(PlayerActionType.ReportFinding, true).IsAllowed;

            LogResult("CarryingBlocksCriticalActions", passed, "Kill/task/sabotage/meeting join blocked while carrying.");
        }

        private void ValidateCarryingSlowsMovement()
        {
            CorpsePlaceholder corpse = BuildCorpse("corpse_carry_7f_slow");
            GameObject carrier = new GameObject("CorpseCarryServerGuard_Carrier");
            CorpseCarryState carryState = carrier.AddComponent<CorpseCarryState>();
            carryState.StartCarrying(corpse);

            PlayerMovementModifierResult result =
                _movementService.Evaluate(PlayerLifeState.Alive, carryState);
            bool passed = result.CanMove && result.SpeedMultiplier > 0f && result.SpeedMultiplier < 1f;

            Destroy(corpse.gameObject);
            Destroy(carrier);
            LogResult("CarryingSlowsMovement", passed, result.ToString());
        }

        private static CorpseCarryCommandContext BuildContext(
            CorpsePlaceholder corpse,
            PlayerRole carrierRole = PlayerRole.Killer,
            bool alreadyCarrying = false,
            OfficeRoomType carrierRoom = OfficeRoomType.Hallway)
        {
            return new CorpseCarryCommandContext(
                "carry_7f_command",
                new PlayerId("killer_carry_01"),
                carrierRole,
                PlayerLifeState.Alive,
                corpse,
                alreadyCarrying,
                carrierRoom,
                true);
        }

        private static CorpsePlaceholder BuildCorpse(string corpseId)
        {
            GameObject corpseObject = new GameObject(corpseId);
            corpseObject.AddComponent<BoxCollider2D>().isTrigger = true;
            CorpsePlaceholder corpse = corpseObject.AddComponent<CorpsePlaceholder>();
            corpse.Initialize(
                new CorpsePublicState(
                    new CorpseId(corpseId),
                    new PlayerId("victim_carry_01"),
                    "Merve Kaya",
                    new Vector3(3f, 2f, 0f),
                    true));
            return corpse;
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[CorpseCarryServerGuardDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CorpseCarryServerGuardDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
