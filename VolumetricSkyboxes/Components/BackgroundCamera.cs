using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumetricSkyboxes.Components
{
    [RequireComponent(typeof(Camera))]
    public class BackgroundCamera : MonoBehaviour
    {
        public Camera camera;

        private bool _origFogState;
        private Color _origFogColor;
        private float _origFogMin;
        private float _origFogMax;

        private VolumetricSkyboxContainer _fogSource;
        private VolumetricSkyboxesManager _manager;

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        private void Start()
        {
            _manager = VolumetricSkyboxesManager.Instance;
            _manager.OnSkyboxesListChanged += UpdateFogSource;
            UpdateFogSource(_manager.Skyboxes);
        }

        private void OnDestroy()
        {
            _manager.OnSkyboxesListChanged -= UpdateFogSource;
        }

        public void SetCurrentEnvironmentalFog(bool fogDisabled, Color fogColor, float fogStartDistance, float fogEndDistance)
        {
            _origFogState = !fogDisabled;
            _origFogColor = fogColor;
            _origFogMin = fogStartDistance;
            _origFogMax = fogEndDistance;
        }

        private void OnPreCull()
        {
            RenderSettings.fog = _fogSource != null && _fogSource.Data.customFog;
            
            if (!RenderSettings.fog)
                return;
            
            RenderSettings.fogColor = _fogSource.Data.fogColor;
            RenderSettings.fogStartDistance = _fogSource.Data.fogMinimum;
            RenderSettings.fogEndDistance = _fogSource.Data.fogMaximum;
        }

        private void OnPostRender()
        {
            RenderSettings.fog = _origFogState;
            RenderSettings.fogColor = _origFogColor;
            RenderSettings.fogStartDistance = _origFogMin;
            RenderSettings.fogEndDistance = _origFogMax;
        }

        private void UpdateFogSource(ICollection<VolumetricSkyboxContainer> containers)
        {
            _fogSource = containers.LastOrDefault(t => t.Data.customFog);
        }
    }
}