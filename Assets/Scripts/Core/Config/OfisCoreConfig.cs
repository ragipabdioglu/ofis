using UnityEngine;

namespace OFIS.Core.Config
{
    [CreateAssetMenu(menuName = "OFIS/Core/Core Config")]
    public sealed class OfisCoreConfig : ScriptableObject
    {
        [Header("Project")]
        public string projectName = "OFIS";
        public string projectVersion = "0.0.1";

        [Header("Debug")]
        public bool enableDebugPanel = true;
        public KeyCode debugPanelKey = KeyCode.F1;

        [Header("Match Defaults")]
        public int defaultPlayerCount = 8;
        public float defaultMatchDurationSeconds = 18f * 60f;
    }
}