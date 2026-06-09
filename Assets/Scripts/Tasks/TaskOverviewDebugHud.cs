using System.Linq;
using UnityEngine;

namespace OFIS.Tasks
{
    public sealed class TaskOverviewDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private Vector2 screenPosition = new(455f, 16f);
        [SerializeField] private Vector2 boxSize = new(430f, 180f);
        [SerializeField] private float refreshInterval = 0.5f;

        private TaskStation[] _taskStations = new TaskStation[0];
        private float _nextRefreshTime;

        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            RefreshTaskStations();
        }

        private void Update()
        {
            if (Time.time < _nextRefreshTime)
                return;

            RefreshTaskStations();
            _nextRefreshTime = Time.time + refreshInterval;
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

            Rect labelRect = new(
                boxRect.x + 12f,
                boxRect.y + 10f,
                boxRect.width - 24f,
                boxRect.height - 20f);

            GUI.Label(labelRect, BuildDebugText(), _labelStyle);
        }

        private void RefreshTaskStations()
        {
            _taskStations = FindObjectsByType<TaskStation>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None)
                .OrderBy(station => station.TaskName)
                .ToArray();
        }

        private string BuildDebugText()
        {
            if (_taskStations == null || _taskStations.Length == 0)
                return "Task Overview\nNo TaskStations found.";

            string text = $"Task Overview ({_taskStations.Length})\n";

            foreach (TaskStation station in _taskStations)
            {
                if (station == null)
                    continue;

                text +=
                    $"{station.TaskName} | " +
                    $"{station.StationType} | " +
                    $"{station.DebugStatus} | " +
                    $"Last={station.LastResult}\n";
            }

            return text;
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
                fontSize = 13,
                normal =
                {
                    textColor = Color.white
                },
                wordWrap = true
            };
        }
    }
}