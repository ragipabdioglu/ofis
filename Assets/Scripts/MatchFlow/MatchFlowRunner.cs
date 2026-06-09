using OFIS.Core.Bootstrap;
using OFIS.MatchFlow.Config;
using UnityEngine;

namespace OFIS.MatchFlow
{
    public sealed class MatchFlowRunner : MonoBehaviour
    {
        [SerializeField] private MatchFlowConfig matchFlowConfig;
        [SerializeField] private bool autoStartMatch = true;
        [SerializeField] private bool autoStartFastTest = false;

        private MatchFlowService _service;

        public MatchFlowService Service => _service;

        private void Start()
        {
            if (matchFlowConfig == null)
            {
                Debug.LogError("[MatchFlowRunner] MatchFlowConfig is missing.");
                return;
            }

            if (GameBootstrap.EventBus == null)
            {
                Debug.LogError("[MatchFlowRunner] GameBootstrap.EventBus is missing. Make sure GameBootstrap exists in the scene.");
                return;
            }

            _service = new MatchFlowService(matchFlowConfig, GameBootstrap.EventBus);

            if (autoStartMatch)
            {
                if (autoStartFastTest)
                    _service.StartFastTestMatch();
                else
                    _service.StartNormalMatch();
            }
        }

        private void Update()
        {
            _service?.Tick(Time.deltaTime);
        }

        public void StartNormalMatch()
        {
            EnsureService();
            _service.StartNormalMatch();
        }

        public void StartFastTestMatch()
        {
            EnsureService();
            _service.StartFastTestMatch();
        }

        public void StopMatch()
        {
            EnsureService();
            _service.StopMatch();
        }

        private void EnsureService()
        {
            if (_service != null)
                return;

            if (matchFlowConfig == null)
            {
                Debug.LogError("[MatchFlowRunner] Cannot create service. MatchFlowConfig is missing.");
                return;
            }

            if (GameBootstrap.EventBus == null)
            {
                Debug.LogError("[MatchFlowRunner] Cannot create service. EventBus is missing.");
                return;
            }

            _service = new MatchFlowService(matchFlowConfig, GameBootstrap.EventBus);
        }
    }
}