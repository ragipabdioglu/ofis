using UnityEngine;

namespace OFIS.Kill
{
    public sealed class NearbyKillTargetDebugHud : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        [SerializeField] private PlayerKillTargetDetector detector;
        [SerializeField] private KillEligibilityService killEligibilityService;

        [Header("Debug Room Rule")]
        [SerializeField] private bool roomAllowsKill = true;
        
        public bool RoomAllowsKill => roomAllowsKill;

        [Header("HUD")]
        [SerializeField] private Vector2 screenPosition = new(600f, 590f);
        [SerializeField] private Vector2 boxSize = new(460f, 175f);

        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;

        private void Awake()
        {
            if (detector == null)
                detector = FindAnyObjectByType<PlayerKillTargetDetector>();

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
                115f);

            GUI.Label(labelRect, BuildDebugText(), _labelStyle);

            Rect roomButton = new(boxRect.x + 12f, boxRect.y + 132f, 130f, 28f);
            Rect testButton = new(boxRect.x + 150f, boxRect.y + 132f, 120f, 28f);

            if (GUI.Button(roomButton, "Toggle Room", _buttonStyle))
                roomAllowsKill = !roomAllowsKill;

            if (GUI.Button(testButton, "Test Target", _buttonStyle))
                TestCurrentTarget();
        }

        private string BuildDebugText()
        {
            if (detector == null)
                return "Nearby Kill Target\nDetector: Missing";

            if (killEligibilityService == null)
                return "Nearby Kill Target\nKillEligibilityService: Missing";

            KillTargetDummy target = detector.CurrentTarget;

            if (target == null)
            {
                return
                    "Nearby Kill Target\n" +
                    "CurrentTarget: None\n" +
                    $"RoomAllowsKill: {roomAllowsKill}\n" +
                    "CanKill: False\n" +
                    "Reason: No nearby target";
            }

            KillEligibilityResult result = killEligibilityService.CanKill(
                target.Role,
                target.IsKnownTarget,
                target.IsAlive,
                roomAllowsKill);

            return
                "Nearby Kill Target\n" +
                $"CurrentTarget: {target.DisplayName}\n" +
                $"TargetRole: {target.Role}\n" +
                $"KnownTarget: {target.IsKnownTarget}\n" +
                $"Alive: {target.IsAlive}\n" +
                $"RoomAllowsKill: {roomAllowsKill}\n" +
                $"CanKill: {result.CanKill}\n" +
                $"Reason: {result.Reason}";
        }

        private void TestCurrentTarget()
        {
            if (detector == null || killEligibilityService == null)
            {
                Debug.LogWarning("[NearbyKillTargetDebugHud] Missing detector or eligibility service.");
                return;
            }

            KillTargetDummy target = detector.CurrentTarget;

            if (target == null)
            {
                Debug.Log("[NearbyKillTarget] No nearby target.");
                return;
            }

            KillEligibilityResult result = killEligibilityService.CanKill(
                target.Role,
                target.IsKnownTarget,
                target.IsAlive,
                roomAllowsKill);

            Debug.Log(
                $"[NearbyKillTarget] Target={target.DisplayName}, " +
                $"Role={target.Role}, Known={target.IsKnownTarget}, Alive={target.IsAlive}, " +
                $"RoomAllowsKill={roomAllowsKill}, CanKill={result.CanKill}, Reason={result.Reason}");
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