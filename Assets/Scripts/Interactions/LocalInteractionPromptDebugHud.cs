using UnityEngine;

namespace OFIS.Interactions
{
    public sealed class LocalInteractionPromptDebugHud : MonoBehaviour
    {
        [SerializeField] private LocalInteractionRadiusDetector detector;
        [SerializeField] private bool showHud = true;
        [SerializeField] private Vector2 screenPosition = new(20f, 300f);
        [SerializeField] private Vector2 size = new(420f, 70f);

        private void Awake()
        {
            if (detector == null)
                detector = GetComponent<LocalInteractionRadiusDetector>();
        }

        private void OnGUI()
        {
            if (!showHud || detector == null)
                return;

            GUI.Box(new Rect(screenPosition.x, screenPosition.y, size.x, size.y), "Interaction Prompt");
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 28f, size.x - 20f, 24f), detector.GetPromptText());
        }
    }
}
