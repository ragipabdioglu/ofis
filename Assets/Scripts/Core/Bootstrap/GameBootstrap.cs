using OFIS.Core.Config;
using OFIS.Core.Events;
using OFIS.Core.Debugging;
using UnityEngine;

namespace OFIS.Core.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private OfisCoreConfig coreConfig;

        public static GameBootstrap Instance { get; private set; }
        public static GameEventBus EventBus { get; private set; }
        public static OfisCoreConfig CoreConfig { get; private set; }

        private OfisDebugPanel _debugPanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (coreConfig == null)
            {
                Debug.LogError("[GameBootstrap] Core config is missing. Create OFIS/Core/Core Config asset and assign it.");
                return;
            }

            CoreConfig = coreConfig;
            EventBus = new GameEventBus();

            Debug.Log($"[GameBootstrap] {CoreConfig.projectName} v{CoreConfig.projectVersion} initialized.");
            Debug.Log($"[GameBootstrap] Default player count: {CoreConfig.defaultPlayerCount}");
            Debug.Log($"[GameBootstrap] Default match duration: {CoreConfig.defaultMatchDurationSeconds} seconds");

            SetupDebugPanel();
        }

        private void SetupDebugPanel()
        {
            if (!CoreConfig.enableDebugPanel)
                return;

            _debugPanel = gameObject.AddComponent<OfisDebugPanel>();
            _debugPanel.Initialize(CoreConfig);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                EventBus?.Clear();
                EventBus = null;
                CoreConfig = null;
                Instance = null;
            }
        }
    }
}