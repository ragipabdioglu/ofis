using OFIS.Roles;
using UnityEngine;

namespace OFIS.Kill
{
    public sealed class KillEligibilityDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private KillEligibilityService killEligibilityService;
        [SerializeField] private Vector2 screenPosition = new(16f, 590f);
        [SerializeField] private Vector2 boxSize = new(560f, 215f);

        private readonly PlayerRole[] _mockTargetRoles =
        {
            PlayerRole.Victim,
            PlayerRole.Detective,
            PlayerRole.Killer,
            PlayerRole.Victim
        };

        private readonly bool[] _mockKnownTargetFlags =
        {
            true,
            false,
            false,
            false
        };

        private int _targetIndex;
        private bool _targetIsAlive = true;
        private bool _roomAllowsKill = true;

        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;

        private void Awake()
        {
            if (killEligibilityService == null)
                killEligibilityService = FindAnyObjectByType<KillEligibilityService>();
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
                125f);

            GUI.Label(labelRect, BuildDebugText(), _labelStyle);

            Rect prevButton = new(boxRect.x + 12f, boxRect.y + 150f, 90f, 28f);
            Rect nextButton = new(boxRect.x + 107f, boxRect.y + 150f, 90f, 28f);
            Rect aliveButton = new(boxRect.x + 202f, boxRect.y + 150f, 90f, 28f);
            Rect roomButton = new(boxRect.x + 297f, boxRect.y + 150f, 120f, 28f);
            Rect testButton = new(boxRect.x + 422f, boxRect.y + 150f, 90f, 28f);

            if (GUI.Button(prevButton, "Prev", _buttonStyle))
                PreviousTarget();

            if (GUI.Button(nextButton, "Next", _buttonStyle))
                NextTarget();

            if (GUI.Button(aliveButton, "Alive", _buttonStyle))
                _targetIsAlive = !_targetIsAlive;

            if (GUI.Button(roomButton, "Room Kill", _buttonStyle))
                _roomAllowsKill = !_roomAllowsKill;

            if (GUI.Button(testButton, "Test", _buttonStyle))
                TestKill();
        }

        private string BuildDebugText()
        {
            if (killEligibilityService == null)
                return "Kill Eligibility\nService: Missing";

            PlayerRole targetRole = _mockTargetRoles[_targetIndex];
            bool isKnownTarget = _mockKnownTargetFlags[_targetIndex];

            KillEligibilityResult result = killEligibilityService.CanKill(
                targetRole,
                isKnownTarget,
                _targetIsAlive,
                _roomAllowsKill);

            return
                "Kill Eligibility\n" +
                $"LocalRole: {killEligibilityService.LocalPlayerRole}\n" +
                $"KnownTargets(Debug): {killEligibilityService.KnownTargetCount}\n" +
                $"Selected Mock Target: {_targetIndex + 1}/{_mockTargetRoles.Length}\n" +
                $"TargetRole: {targetRole}\n" +
                $"TargetIsKnownTarget: {isKnownTarget}\n" +
                $"TargetIsAlive: {_targetIsAlive}\n" +
                $"RoomAllowsKill: {_roomAllowsKill}\n" +
                $"CanKill: {result.CanKill}\n" +
                $"Reason: {result.Reason}";
        }

        private void PreviousTarget()
        {
            _targetIndex--;

            if (_targetIndex < 0)
                _targetIndex = _mockTargetRoles.Length - 1;
        }

        private void NextTarget()
        {
            _targetIndex++;

            if (_targetIndex >= _mockTargetRoles.Length)
                _targetIndex = 0;
        }

        private void TestKill()
        {
            if (killEligibilityService == null)
            {
                Debug.LogWarning("[KillEligibilityDebugHud] KillEligibilityService missing.");
                return;
            }

            PlayerRole targetRole = _mockTargetRoles[_targetIndex];
            bool isKnownTarget = _mockKnownTargetFlags[_targetIndex];

            KillEligibilityResult result = killEligibilityService.CanKill(
                targetRole,
                isKnownTarget,
                _targetIsAlive,
                _roomAllowsKill);

            Debug.Log(
                $"[KillEligibility] TargetRole={targetRole}, " +
                $"KnownTarget={isKnownTarget}, " +
                $"Alive={_targetIsAlive}, " +
                $"RoomAllowsKill={_roomAllowsKill}, " +
                $"CanKill={result.CanKill}, " +
                $"Reason={result.Reason}");
        }

        private void EnsureStyles()
        {
            if (_boxStyle != null && _labelStyle != null && _buttonStyle != null)
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

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12
            };
        }
    }
}