using UnityEngine;

namespace OFIS.Interaction
{
    [RequireComponent(typeof(PlayerInteractionDetector))]
    public sealed class InteractionDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private Vector2 screenPosition = new(16f, 330f);
        [SerializeField] private Vector2 boxSize = new(360f, 90f);

        private PlayerInteractionDetector _detector;
        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            _detector = GetComponent<PlayerInteractionDetector>();
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

            string targetText = _detector.CurrentTarget == null
                ? "None"
                : $"{_detector.CurrentTarget.DisplayName} ({_detector.CurrentTarget.InteractionType})";

            string text =
                $"Interaction Detector\n" +
                $"CurrentTarget: {targetText}\n" +
                $"Press E to interact";

            Rect labelRect = new(
                boxRect.x + 12f,
                boxRect.y + 10f,
                boxRect.width - 24f,
                boxRect.height - 20f);

            GUI.Label(labelRect, text, _labelStyle);
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