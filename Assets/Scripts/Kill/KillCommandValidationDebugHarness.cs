using OFIS.Core.Ids;
using OFIS.Meetings;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Kill
{
    public sealed class KillCommandValidationDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Debug Config")]
        [SerializeField] private float killRange = 1.75f;
        [SerializeField] private float killCooldownSeconds = 60f;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly KillCommandValidationService _validationService =
            new KillCommandValidationService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateKillCommandRules();
        }

        [ContextMenu("Validate Kill Command Rules")]
        public void ValidateKillCommandRules()
        {
            ValidateAcceptedVictimKill();
            ValidateDetectiveCannotBeKilled();
            ValidateCooldownBlocksSecondKill();
            ValidateMeetingPhaseBlocksKill();
            ValidateMeetingRoomBlocksKill();
            ValidateCarryingCorpseBlocksKill();
            ValidateRangeBlocksKill();
        }

        private void ValidateAcceptedVictimKill()
        {
            KillCommandValidationResult result = _validationService.Validate(BuildValidContext("kill_valid"));
            LogResult("AcceptedVictimKill", result.IsAccepted, result.ToString());
        }

        private void ValidateDetectiveCannotBeKilled()
        {
            KillCommandContext context = BuildValidContext(
                "kill_detective_block",
                targetRole: PlayerRole.Detective,
                targetIsKnownVictim: false);

            KillCommandValidationResult result = _validationService.Validate(context);
            LogResult("DetectiveCannotBeKilled", !result.IsAccepted, result.ToString());
        }

        private void ValidateCooldownBlocksSecondKill()
        {
            KillCooldownState cooldownState = new KillCooldownState();
            PlayerId killerId = new PlayerId("killer_01");
            cooldownState.RecordAcceptedKill(killerId, 100f);

            float lastKillTime;
            cooldownState.TryGetLastAcceptedKillTime(killerId, out lastKillTime);

            KillCommandContext context = BuildValidContext(
                "kill_cooldown_block",
                senderId: killerId,
                serverTimeSeconds: 130f,
                lastAcceptedKillTimeSeconds: lastKillTime);

            KillCommandValidationResult result = _validationService.Validate(context);
            bool passed = !result.IsAccepted && result.RemainingCooldownSeconds > 0f;
            LogResult("CooldownBlocksSecondKill", passed, result.ToString());
        }

        private void ValidateMeetingPhaseBlocksKill()
        {
            KillCommandContext context = BuildValidContext(
                "kill_meeting_phase_block",
                phaseType: MeetingRuntimePhaseType.Meeting);

            KillCommandValidationResult result = _validationService.Validate(context);
            LogResult("MeetingPhaseBlocksKill", !result.IsAccepted, result.ToString());
        }

        private void ValidateMeetingRoomBlocksKill()
        {
            KillCommandContext context = BuildValidContext(
                "kill_meeting_room_block",
                senderRoom: OfficeRoomType.MeetingRoom);

            KillCommandValidationResult result = _validationService.Validate(context);
            LogResult("MeetingRoomBlocksKill", !result.IsAccepted, result.ToString());
        }

        private void ValidateCarryingCorpseBlocksKill()
        {
            KillCommandContext context = BuildValidContext(
                "kill_carrying_block",
                senderIsCarryingCorpse: true);

            KillCommandValidationResult result = _validationService.Validate(context);
            LogResult("CarryingCorpseBlocksKill", !result.IsAccepted, result.ToString());
        }

        private void ValidateRangeBlocksKill()
        {
            KillCommandContext context = BuildValidContext(
                "kill_range_block",
                distanceToTarget: killRange + 0.5f);

            KillCommandValidationResult result = _validationService.Validate(context);
            LogResult("RangeBlocksKill", !result.IsAccepted, result.ToString());
        }

        private KillCommandContext BuildValidContext(
            string commandId,
            PlayerId? senderId = null,
            PlayerId? targetId = null,
            PlayerRole senderRole = PlayerRole.Killer,
            PlayerRole targetRole = PlayerRole.Victim,
            PlayerLifeState targetLifeState = PlayerLifeState.Alive,
            bool targetIsKnownVictim = true,
            float distanceToTarget = 1f,
            float serverTimeSeconds = 100f,
            float lastAcceptedKillTimeSeconds = -1f,
            MeetingRuntimePhaseType phaseType = MeetingRuntimePhaseType.Office,
            OfficeRoomType senderRoom = OfficeRoomType.Hallway,
            bool senderIsCarryingCorpse = false)
        {
            return new KillCommandContext(
                commandId,
                senderId ?? new PlayerId("killer_01"),
                targetId ?? new PlayerId("victim_01"),
                senderRole,
                targetRole,
                targetLifeState,
                targetIsKnownVictim,
                distanceToTarget,
                killRange,
                serverTimeSeconds,
                lastAcceptedKillTimeSeconds,
                killCooldownSeconds,
                phaseType,
                senderRoom,
                senderIsCarryingCorpse);
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[KillCommandValidationDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[KillCommandValidationDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
