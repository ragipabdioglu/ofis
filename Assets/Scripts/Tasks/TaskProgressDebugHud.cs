using UnityEngine;

namespace OFIS.Tasks
{
    public sealed class TaskProgressDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private ActiveTaskProgressService activeTaskProgressService;
        [SerializeField] private Vector2 screenPosition = new(16f, 435f);
        [SerializeField] private Vector2 boxSize = new(420f, 150f);

        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            if (activeTaskProgressService == null)
                activeTaskProgressService = FindFirstObjectByType<ActiveTaskProgressService>();
        }

        private void OnGUI()
        {
            if (!showHud)
                return;

            EnsureStyles();

            Rect boxRect = new(
                screenPosition.x,
                screenPosition.y,
                boxSize.x,
                boxSize.y);

            GUI.Box(boxRect, GUIContent.none, _boxStyle);

            string text = BuildDebugText();

            Rect labelRect = new(
                boxRect.x + 12f,
                boxRect.y + 10f,
                boxRect.width - 24f,
                boxRect.height - 20f);

            GUI.Label(labelRect, text, _labelStyle);
        }

        private string BuildDebugText()
        {
            if (activeTaskProgressService == null)
                return "Task Progress\nService: Missing";

            TaskStation activeTask = activeTaskProgressService.ActiveTaskStation;

            if (activeTask == null)
                return
                    "Task Progress\n" +
                    "Active Task: None\n" +
                    "InProgress: False\n" +
                    "Progress: 0%";

            int percent = Mathf.RoundToInt(activeTask.Progress01 * 100f);

            return
                "Task Progress\n" +
                $"Active Task: {activeTask.TaskName}\n" +
                $"Station: {activeTask.StationType}\n" +
                $"InProgress: {activeTask.IsInProgress}\n" +
                $"Completed: {activeTask.IsCompleted}\n" +
                $"LastResult: {activeTask.LastResult}\n" +
                $"Progress: {percent}%";
        }

        private void EnsureStyles()
        {
            if (_boxStyle != null && _labelStyle != null)
                return;

            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal =
                {
                    textColor = Color.white
                },
                wordWrap = true
            };
        }
    }
}