using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class NearbyCorpseDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private CorpseDetector corpseDetector;

        [Header("HUD")]
        [SerializeField] private Vector2 screenPosition = new(600f, 565f);
        [SerializeField] private Vector2 boxSize = new(430f, 115f);

        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            if (corpseDetector == null)
                corpseDetector = FindAnyObjectByType<CorpseDetector>();
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
            if (corpseDetector == null)
                return "Nearby Corpse\nDetector: Missing";

            CorpsePlaceholder corpse = corpseDetector.CurrentCorpse;

            if (corpse == null)
            {
                return
                    "Nearby Corpse\n" +
                    "CurrentCorpse: None";
            }

            return
                "Nearby Corpse\n" +
                $"CurrentCorpse: {corpse.name}\n" +
                $"VictimName: {corpse.VictimName}";
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