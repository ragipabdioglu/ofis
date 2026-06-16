using OFIS.Core.Ids;
using OFIS.Meetings;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Roles.Identity;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Kill
{
    public sealed class KillExecutionDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly KillExecutionService _executionService =
            new KillExecutionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateKillExecution();
        }

        [ContextMenu("Validate Kill Execution")]
        public void ValidateKillExecution()
        {
            ValidateAcceptedKillCreatesDeadVictimAndCorpse();
            ValidateRejectedKillDoesNotCreateCorpse();
            ValidateAcceptedKillRecordsCooldown();
            ValidateDeadPublicStateBuilder();
        }

        private void ValidateAcceptedKillCreatesDeadVictimAndCorpse()
        {
            KillCooldownState cooldownState = new KillCooldownState();
            KillExecutionRequest request = BuildValidRequest("kill_exec_accept");

            KillExecutionResult result = _executionService.Execute(request, cooldownState);

            bool passed = result.Success
                && result.VictimLifeStateAfterKill == PlayerLifeState.Dead
                && result.CorpseState.IsPublicWorldObject
                && result.CorpseState.VictimId == request.CommandContext.TargetId
                && result.CorpseState.WorldPosition == request.DeathPosition;

            LogResult("AcceptedKillCreatesDeadVictimAndCorpse", passed, result.ToString());
        }

        private void ValidateRejectedKillDoesNotCreateCorpse()
        {
            KillExecutionRequest request = BuildValidRequest(
                "kill_exec_reject_detective",
                targetRole: PlayerRole.Detective,
                targetIsKnownVictim: false);

            KillExecutionResult result = _executionService.Execute(request, new KillCooldownState());
            bool passed = !result.Success
                && result.VictimLifeStateAfterKill == PlayerLifeState.Alive
                && string.IsNullOrWhiteSpace(result.CorpseState.CorpseId.Value);

            LogResult("RejectedKillDoesNotCreateCorpse", passed, result.ToString());
        }

        private void ValidateAcceptedKillRecordsCooldown()
        {
            KillCooldownState cooldownState = new KillCooldownState();
            KillExecutionRequest request = BuildValidRequest("kill_exec_cooldown_record");

            KillExecutionResult result = _executionService.Execute(request, cooldownState);

            float recordedTime;
            bool hasRecordedTime = cooldownState.TryGetLastAcceptedKillTime(
                request.CommandContext.SenderId,
                out recordedTime);

            bool passed = result.Success
                && hasRecordedTime
                && Mathf.Approximately(recordedTime, request.CommandContext.ServerTimeSeconds);

            LogResult("AcceptedKillRecordsCooldown", passed, result.ToString());
        }

        private void ValidateDeadPublicStateBuilder()
        {
            PlayerId victimId = new PlayerId("victim_state_01");
            PlayerPublicIdentity identity = new PlayerPublicIdentity(
                victimId,
                "Merve Kaya",
                OFIS.Roles.Departments.DepartmentType.Accounting,
                "Accounting",
                "Analyst");

            PlayerPublicState aliveState = new PlayerPublicState(
                victimId,
                "Merve Kaya",
                identity,
                PlayerLifeState.Alive);

            PlayerPublicState deadState = _executionService.BuildDeadPublicState(aliveState);

            bool passed = deadState != null
                && deadState.PlayerId == victimId
                && deadState.DisplayName == aliveState.DisplayName
                && deadState.LifeState == PlayerLifeState.Dead;

            LogResult("DeadPublicStateBuilder", passed, deadState != null ? deadState.ToString() : "No state");
        }

        private static KillExecutionRequest BuildValidRequest(
            string commandId,
            PlayerRole targetRole = PlayerRole.Victim,
            bool targetIsKnownVictim = true)
        {
            KillCommandContext context = new KillCommandContext(
                commandId,
                new PlayerId("killer_01"),
                new PlayerId("victim_01"),
                PlayerRole.Killer,
                targetRole,
                PlayerLifeState.Alive,
                targetIsKnownVictim,
                1f,
                1.75f,
                240f,
                -1f,
                60f,
                MeetingRuntimePhaseType.Office,
                OfficeRoomType.Hallway,
                false);

            return new KillExecutionRequest(
                context,
                new CorpseId($"corpse_{commandId}"),
                "Merve Kaya",
                new Vector3(4f, 2f, 0f));
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[KillExecutionDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[KillExecutionDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
