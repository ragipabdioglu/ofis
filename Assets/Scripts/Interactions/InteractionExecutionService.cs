using OFIS.Players;
using OFIS.Sabotage;
using OFIS.Tasks;

namespace OFIS.Interactions
{
    public sealed class InteractionExecutionService
    {
        private readonly InteractionPermissionService _permissionService = new InteractionPermissionService();
        private readonly OfficeTaskCompletionService _taskCompletionService = new OfficeTaskCompletionService();
        private readonly SabotageRepairService _sabotageRepairService = new SabotageRepairService();

        public InteractionExecutionResult Execute(PlayerLifeState lifeState, WorldInteractionResolveResult resolveResult)
        {
            InteractionPermissionResult permission = _permissionService.Evaluate(lifeState, resolveResult);

            if (!permission.CanInteract)
                return InteractionExecutionResult.Failed(permission.Reason);

            WorldInteractionCandidate selected = resolveResult.SelectedCandidate;

            switch (selected.Type)
            {
                case WorldInteractionType.CorpseInspectOrCarry:
                    return Success(selected, "CorpseInspectOrCarry", "Corpse interaction execute stub selected.");

                case WorldInteractionType.MeetingJoin:
                    return Success(selected, "MeetingJoin", "Meeting join execute stub selected.");

                case WorldInteractionType.SabotageRepair:
                    return Success(selected, "SabotageRepair", "Sabotage repair execute stub selected.");

                case WorldInteractionType.Task:
                    return Success(selected, "Task", "Task execute stub selected.");

                case WorldInteractionType.Sabotage:
                    return Success(selected, "Sabotage", "Sabotage execute stub selected.");

                case WorldInteractionType.VictimNote:
                    return Success(selected, "VictimNote", "Victim note execute stub selected.");

                case WorldInteractionType.DoorPanel:
                    return Success(selected, "DoorPanel", "Door panel execute stub selected.");

                default:
                    return InteractionExecutionResult.Failed("Selected interaction type is not executable.");
            }
        }

        public InteractionExecutionResult ExecuteTask(
            PlayerLifeState lifeState,
            WorldInteractionResolveResult resolveResult,
            OfficeTaskRuntimeState taskState,
            string playerId)
        {
            InteractionPermissionResult permission = _permissionService.Evaluate(lifeState, resolveResult);

            if (!permission.CanInteract)
                return InteractionExecutionResult.Failed(permission.Reason);

            WorldInteractionCandidate selected = resolveResult.SelectedCandidate;

            if (selected.Type != WorldInteractionType.Task)
                return InteractionExecutionResult.Failed("Selected interaction is not a task.");

            OfficeTaskCompletionResult completionResult = _taskCompletionService.CompleteTask(taskState, playerId);

            return new InteractionExecutionResult(
                completionResult.Success,
                selected.Type,
                selected.DisplayName,
                "TaskCompletion",
                completionResult.Message);
        }

        public InteractionExecutionResult ExecuteSabotageRepair(
            PlayerLifeState lifeState,
            WorldInteractionResolveResult resolveResult,
            SabotageObjectiveRuntimeState sabotageState)
        {
            InteractionPermissionResult permission = _permissionService.Evaluate(lifeState, resolveResult);

            if (!permission.CanInteract)
                return InteractionExecutionResult.Failed(permission.Reason);

            WorldInteractionCandidate selected = resolveResult.SelectedCandidate;

            if (selected.Type != WorldInteractionType.SabotageRepair)
                return InteractionExecutionResult.Failed("Selected interaction is not a sabotage repair.");

            SabotageRepairResult repairResult = _sabotageRepairService.Repair(sabotageState);

            return new InteractionExecutionResult(
                repairResult.Success,
                selected.Type,
                selected.DisplayName,
                "SabotageRepair",
                repairResult.Message);
        }

        private static InteractionExecutionResult Success(WorldInteractionCandidate selected, string actionKey, string message)
        {
            return new InteractionExecutionResult(
                true,
                selected.Type,
                selected.DisplayName,
                actionKey,
                message);
        }
    }
}
