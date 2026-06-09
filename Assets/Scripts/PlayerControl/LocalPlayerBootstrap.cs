using UnityEngine;

namespace OFIS.PlayerControl
{
    public sealed class LocalPlayerBootstrap : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogWarning("[LocalPlayerBootstrap] Main camera not found.");
                return;
            }

            var follow = mainCamera.GetComponent<CameraFollow2D>();

            if (follow == null)
                follow = mainCamera.gameObject.AddComponent<CameraFollow2D>();

            follow.SetTarget(transform);

            Debug.Log("[LocalPlayerBootstrap] Camera follow target assigned.");
        }
    }
}