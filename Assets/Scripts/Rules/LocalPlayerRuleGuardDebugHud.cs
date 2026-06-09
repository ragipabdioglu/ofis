using UnityEngine;

namespace OFIS.Rules
{
    [RequireComponent(typeof(RoomBasedRuleGuard))]
    public sealed class LocalPlayerRuleGuardDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private Vector2 screenPosition = new(16f, 200f);
        [SerializeField] private Vector2 boxSize = new(360f, 115f);

        private RoomBasedRuleGuard _ruleGuard;
        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            _ruleGuard = GetComponent<RoomBasedRuleGuard>();
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
            if (_ruleGuard == null)
                return "RuleGuard missing.";

            var kill = _ruleGuard.CanPerform(PlayerActionType.Kill);
            var carry = _ruleGuard.CanPerform(PlayerActionType.CarryCorpse);
            var hide = _ruleGuard.CanPerform(PlayerActionType.HideCorpse);

            return
                $"Room Rule Guard\n" +
                $"KillAllowed: {kill.IsAllowed} ({kill.Reason})\n" +
                $"CarryCorpseAllowed: {carry.IsAllowed} ({carry.Reason})\n" +
                $"HideCorpseAllowed: {hide.IsAllowed} ({hide.Reason})";
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