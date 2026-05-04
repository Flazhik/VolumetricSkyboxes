using UnityEngine;

namespace VolumetricSkyboxes.Components
{
    public class SkyboxParallaxEffect : MonoBehaviour
    {
        public float parallaxCoefficient = 1f;
        private CameraController _cameraController;
        private ParallaxEffect[] _parallaxEffects;
        
        private Vector3 _playerPosition;

        private void Awake()
        {
            _playerPosition = CameraController.Instance!.transform.position;
            _cameraController = MonoSingleton<CameraController>.Instance;

            _parallaxEffects = GetComponentsInChildren<ParallaxEffect>(true);
        }

        private void Update()
        {
            var playerDelta = _cameraController.transform.position - _playerPosition;
            transform.position += playerDelta * (1 - parallaxCoefficient);
            
            foreach (var child in _parallaxEffects)
            {
                if (child.transform == transform)
                    continue;

                var relativeCoefficient = parallaxCoefficient - child.parallaxCoefficient;
                child.transform.localPosition += playerDelta * relativeCoefficient;
            }
            
            _playerPosition = _cameraController.transform.position;
        }
    }
}