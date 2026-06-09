using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class FoundCorpseReportDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private FoundCorpseReportMemory reportMemory;

        [Header("HUD")]
        [SerializeField] private Vector2 screenPosition = new(600f, 690f);
        [SerializeField] private Vector2 boxSize = new(430f, 135f);

        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            if (reportMemory == null)
                reportMemory = FindAnyObjectByType<FoundCorpseReportMemory>();
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

        private string BuildDebugText()
        {
            if (reportMemory == null)
                return "Found Corpse Report\nMemory: Missing";

            string text = $"Found Corpse Report ({reportMemory.Count})\n";

            if (reportMemory.Count == 0)
            {
                text += "No corpse reported yet.";
                return text;
            }

            for (int i = 0; i < reportMemory.FoundCorpses.Count; i++)
            {
                CorpsePlaceholder corpse = reportMemory.FoundCorpses[i];

                if (corpse == null)
                    continue;

                text += $"{i + 1}. {corpse.VictimName}\n";
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