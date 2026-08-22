using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VolumetricSkyboxes.UI;

namespace VolumetricSkyboxes.Components
{
    public class ArenaLightingManager : MonoBehaviour
    {
        [SerializeField]
        public Image colorImage;
        
        [SerializeField]
        private Slider redSlider;
        
        [SerializeField]
        private Slider greenSlider;
        
        [SerializeField]
        private Slider blueSlider;
        
        [SerializeField]
        private Slider intensitySlider;

        [SerializeField]
        private ControllerPointerToggle overrideLightingToggle;
        
        private List<Tuple<Light, float>> _lights;
        private OutdoorLightMaster _olm;
        private PrefsManager _prefsManager;
        private VolumetricSkyboxesManager _manager;
        private VolumetricSkyboxContainer _lightingSource;

        private float intensityAmount;
        private float redAmount;
        private float greenAmount;
        private float blueAmount;
        
        private bool skyboxesOverrideLighting;
        private bool skyboxesOverridePanorama;
        
        private void Awake()
        {
            _olm = OutdoorLightMaster.Instance;
            _prefsManager =  PrefsManager.Instance;
            _manager = VolumetricSkyboxesManager.Instance;
            _lights = _olm!.gameObject.GetComponentsInChildren<Light>()
                .Where(light => light.type == LightType.Directional)
                .Select(light => new Tuple<Light, float>(light, light.intensity))
                .ToList();

            intensityAmount = _prefsManager.GetFloatLocal("cyberGrind.volumetricSkyboxes.arenaLighting.intensity", 1);
            redAmount = _prefsManager.GetFloatLocal("cyberGrind.volumetricSkyboxes.arenaLighting.r", 1);
            greenAmount = _prefsManager.GetFloatLocal("cyberGrind.volumetricSkyboxes.arenaLighting.g", 1);
            blueAmount = _prefsManager.GetFloatLocal("cyberGrind.volumetricSkyboxes.arenaLighting.b", 1);
            skyboxesOverrideLighting = _prefsManager.GetBoolLocal("cyberGrind.volumetricSkyboxes.arenaLighting.useSkyboxesLighting", true);
        }

        private void Start()
        {
            redSlider.value = redAmount;
            greenSlider.value = greenAmount;
            blueSlider.value = blueAmount;
            intensitySlider.value = intensityAmount;
            overrideLightingToggle.isOn = skyboxesOverrideLighting;
            overrideLightingToggle.onValueChanged.AddListener(SetUseSkyboxLighting);
            _manager.OnSkyboxesListChanged += SkyboxesListChanged;
            SkyboxesListChanged(_manager.Skyboxes);
            UpdateLightingSettings();
        }

        private void OnDestroy()
        {
            _manager.OnSkyboxesListChanged -= SkyboxesListChanged;
        }

        public void SetRed(float amount)
        {
            redAmount = amount;
            UpdateLightingSettings();
        }
        
        public void SetGreen(float amount)
        {
            greenAmount = amount;
            UpdateLightingSettings();
        }
        
        public void SetBlue(float amount)
        {
            blueAmount = amount;
            UpdateLightingSettings();
        }
        
        public void SetIntensity(float amount)
        {
            intensityAmount = amount;
            UpdateLightingSettings();
        }

        public void SetUseSkyboxLighting(bool value)
        {
            skyboxesOverrideLighting = value;
            UpdateLightingSettings();
        }
        
        private void UpdateLightingSettings()
        {
            Color lightsColor;
            float lightsIntensity;

            if (skyboxesOverrideLighting && _lightingSource != null)
            {
                lightsColor = _lightingSource.Data.lightingColor;
                lightsIntensity = _lightingSource.Data.lightingIntensity;
            }
            else
            {
                lightsColor = new Color(redAmount, greenAmount, blueAmount);
                lightsIntensity = intensityAmount;
            }
            
            foreach (var (light, originalIntensity) in _lights)
            {
                light.color = lightsColor;
                light.intensity = originalIntensity * lightsIntensity;
                colorImage.color = lightsColor;
            }

            _prefsManager.SetFloatLocal("cyberGrind.volumetricSkyboxes.arenaLighting.intensity", intensityAmount);
            _prefsManager.SetFloatLocal("cyberGrind.volumetricSkyboxes.arenaLighting.r", redAmount);
            _prefsManager.SetFloatLocal("cyberGrind.volumetricSkyboxes.arenaLighting.g", greenAmount);
            _prefsManager.SetFloatLocal("cyberGrind.volumetricSkyboxes.arenaLighting.b", blueAmount);
            _prefsManager.SetBoolLocal("cyberGrind.volumetricSkyboxes.arenaLighting.useSkyboxesLighting", skyboxesOverrideLighting);
        }

        private void SkyboxesListChanged(ICollection<VolumetricSkyboxContainer> containers)
        {
            _lightingSource = containers.LastOrDefault(t => t.Data.customLighting);
            UpdateLightingSettings();
        }
    }
}