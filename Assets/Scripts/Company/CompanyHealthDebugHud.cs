using UnityEngine;

namespace OFIS.Company
{
    public sealed class CompanyHealthDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private CompanyHealthService companyHealthService;
        [SerializeField] private Vector2 screenPosition = new(16f, 540f);
        [SerializeField] private Vector2 boxSize = new(360f, 70f);

        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            if (companyHealthService == null)
                companyHealthService = FindFirstObjectByType<CompanyHealthService>();
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
            if (companyHealthService == null)
                return "Company Health\nService: Missing";

            return
                $"Company Health\n" +
                $"CurrentHealth: {companyHealthService.CurrentHealth}";
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