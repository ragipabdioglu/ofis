using System.Collections.Generic;

namespace OFIS.Tasks
{
    public sealed class PlayerTaskAssignmentService
    {
        public PlayerTaskAssignmentResult AssignTasks(string playerId, IEnumerable<OfficeTaskDefinition> definitions)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return PlayerTaskAssignmentResult.Failed("Player id is missing.");

            if (definitions == null)
                return PlayerTaskAssignmentResult.Failed("Task definitions are missing.");

            List<OfficeTaskRuntimeState> runtimeTasks = new List<OfficeTaskRuntimeState>();

            foreach (OfficeTaskDefinition definition in definitions)
            {
                if (string.IsNullOrWhiteSpace(definition.TaskId))
                    continue;

                runtimeTasks.Add(new OfficeTaskRuntimeState(definition, playerId, OfficeTaskState.Available));
            }

            if (runtimeTasks.Count == 0)
                return PlayerTaskAssignmentResult.Failed("No valid task definitions found.");

            PlayerTaskList taskList = new PlayerTaskList(playerId, runtimeTasks);

            return new PlayerTaskAssignmentResult(
                true,
                taskList,
                "Tasks assigned.");
        }
    }
}
