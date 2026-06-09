using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class CorpseCarryDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private CorpseDetector corpseDetector;
        [SerializeField] private CorpseCarryState carryState;
        [SerializeField] private CorpseCarryInputController carryInputController;

        [Header("HUD")]
        [SerializeField] private Vector2 screenPosition = new(1045f, 565f);
        [SerializeField] private Vector2 boxSize = new(430f, 160f);

        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            if (corpseDetector == null)
                corpseDetector = FindAnyObjectByType<CorpseDetector>();

            if (carryState == null)
                carryState = FindAnyObjectByType<CorpseCarryState>();

            if (carryInputController == null)
                carryInputController = FindAnyObjectByType<CorpseCarryInputController>();
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
                boxRect.height - 55f);

            GUI.Label(labelRect, BuildDebugText(), _labelStyle);

            Rect buttonRect = new(
                boxRect.x + 12f,
                boxRect.y + boxRect.height - 38f,
                190f,
                26f);

            if (GUI.Button(buttonRect, "Toggle RoomAllowsCarry"))
            {
                if (carryInputController != null)
                    carryInputController.ToggleRoomAllowsCarryForDebug();
            }
        }

        private string BuildDebugText()
        {
            string nearbyCorpse = corpseDetector != null && corpseDetector.CurrentCorpse != null
                ? corpseDetector.CurrentCorpse.VictimName
                : "None";

            string carriedCorpse = carryState != null && carryState.CarriedCorpse != null
                ? carryState.CarriedCorpse.VictimName
                : "None";

            bool isCarrying = carryState != null && carryState.IsCarrying;
            bool roomAllowsCarry = carryInputController == null || carryInputController.RoomAllowsCarry;

            return
                "Corpse Carry Debug\n" +
                $"NearbyCorpse: {nearbyCorpse}\n" +
                $"IsCarrying: {isCarrying}\n" +
                $"CarriedCorpse: {carriedCorpse}\n" +
                $"RoomAllowsCarry: {roomAllowsCarry}\n" +
                "Input: C";
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