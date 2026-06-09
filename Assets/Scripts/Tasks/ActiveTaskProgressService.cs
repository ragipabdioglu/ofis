using UnityEngine;

namespace OFIS.Tasks
{
    public sealed class ActiveTaskProgressService : MonoBehaviour
    {
        public TaskStation ActiveTaskStation { get; private set; }

        public bool HasActiveTask => ActiveTaskStation != null && ActiveTaskStation.IsInProgress;

        public void SetActiveTask(TaskStation taskStation)
        {
            ActiveTaskStation = taskStation;

            if (taskStation != null)
                Debug.Log($"[ActiveTaskProgress] Active task set: {taskStation.TaskName}");
        }

        public void ClearActiveTask(TaskStation taskStation)
        {
            if (ActiveTaskStation != taskStation)
                return;

            Debug.Log($"[ActiveTaskProgress] Active task cleared: {taskStation.TaskName}");
            ActiveTaskStation = null;
        }
    }
}