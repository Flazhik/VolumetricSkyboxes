using System.Collections.Generic;
using System.Linq;
using ULTRAKILL.Portal;
using UnityEngine;
using PortalUtils = VolumetricSkyboxes.Utils.PortalUtils;

namespace VolumetricSkyboxes.Components
{
    [RequireComponent(typeof(Camera))]
    public class BackgroundCamera : MonoBehaviour, IPortalTraveller
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
            _manager = VolumetricSkyboxesManager.Instance!;
            PortalManagerV2.Instance!.AddPlayer(this);

            _manager.OnSkyboxesListChanged += UpdateFogSource;
            UpdateFogSource(_manager.Skyboxes);
        }

        private void OnDestroy()
        {
            _manager.OnSkyboxesListChanged -= UpdateFogSource;
        }
        
        /*
         * It's not the camera that moves on portal traversal, it's the skybox itself
         */
        public bool? OnTravel(PortalTravelDetails details)
        {
            var skybox = ULTRAKILL.Portal.PortalUtils.GetPortalObject(details.enterHandle).GetComponentInParent<SkyboxParallaxEffect>();
            if (skybox == null)
            {
                Debug.LogWarning("No parent skybox found for portal");
                return false;
            }

            var skyboxTransform = skybox.transform;
            var position = skyboxTransform.position;
            var inverseTravelMatrix = details.enterToExit.inverse;
            var skyboxRotation = skyboxTransform.rotation;

            skyboxTransform.rotation = Quaternion.LookRotation(
                inverseTravelMatrix.MultiplyVector(skyboxRotation * Vector3.forward).normalized,
                inverseTravelMatrix.MultiplyVector(skyboxRotation * Vector3.up).normalized);
            skyboxTransform.position = inverseTravelMatrix.MultiplyPoint3x4(position);

            PortalUtils.FixComponentsPositioning(skybox, inverseTravelMatrix);
            PortalUtils.SyncPortals();
            return true;
        }

        public void OnTeleportBlocked(PortalTravelDetails details) {}

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

        public int id => GetInstanceID();

        public PortalTravellerType travellerType => PortalTravellerType.OTHER;

        public Vector3 travellerPosition => transform.position;

        public Vector3 travellerVelocity => new();
    }
}