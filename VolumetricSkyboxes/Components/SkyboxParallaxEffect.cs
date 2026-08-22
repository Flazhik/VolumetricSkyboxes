using UnityEngine;
using PortalUtils = VolumetricSkyboxes.Utils.PortalUtils;

namespace VolumetricSkyboxes.Components
{
    [DefaultExecutionOrder(-100)]
    public class SkyboxParallaxEffect : MonoBehaviour
    {
        public float parallaxCoefficient = 1f;
        public Follow[] followers;
        public MovingPlatform[] movingPlatforms;
        
        private CameraController _cameraController;
        private Vector3 _playerPosition;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _cameraController = CameraController.Instance!;
            _playerPosition = _cameraController.transform.position;
            followers = gameObject.GetComponentsInChildren<Follow>(true);
            movingPlatforms = gameObject.GetComponentsInChildren<MovingPlatform>();
        }
        
        public void FixedUpdate()
        {
            AdjustPosition();
            PortalUtils.SyncPortals();
        }

        public void AdjustPosition()
        {
            var currentPlayerPosition = _cameraController.transform.position;
            var playerDelta = currentPlayerPosition - _playerPosition;

            if (playerDelta == Vector3.zero)
                return;

            var parentFactor = 1f - parallaxCoefficient;
            var parentTravelMatrix = Matrix4x4.Translate(playerDelta * parentFactor);
            _transform.position = parentTravelMatrix.MultiplyPoint3x4(_transform.position);
            
            PortalUtils.FixComponentsPositioning(this, parentTravelMatrix);
            _playerPosition = currentPlayerPosition;
        }
    }
}