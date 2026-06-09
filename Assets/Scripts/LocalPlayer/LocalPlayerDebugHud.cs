using UnityEngine;

namespace OFIS.LocalPlayer
{
    [RequireComponent(typeof(LocalPlayerIdentityBinding))]
    public sealed class LocalPlayerDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private Vector2 screenPosition = new(16f, 16f);
        [SerializeField] private Vector2 boxSize = new(360f, 170f);

        private LocalPlayerIdentityBinding _binding;
        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            _binding = GetComponent<LocalPlayerIdentityBinding>();
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

            string text = _binding == null
                ? "LocalPlayerDebugHud\nBinding component missing."
                : _binding.GetDebugSummary();

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