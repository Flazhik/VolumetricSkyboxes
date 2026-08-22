using System;
using UnityEngine;
using VolumetricSkyboxes.UI;

namespace VolumetricSkyboxes.Components
{
    public class VolumetricSkyboxesSettings: MonoBehaviour
    {
        [SerializeField]
        public ControllerPointerToggle overrideSkyboxesToggle;
        
        [SerializeField]
        public ControllerPointerToggle overrideGridTexturesToggle;
        
        [SerializeField]
        public ControllerPointerToggle muteSoundsToggle;
        
        [SerializeField]
        public ControllerPointerToggle disableScorePanelToggle;

        private void Awake()
        {
            RegisterToggle("cyberGrind.volumetricSkyboxes.arenaLighting.useSkyboxesPanorama", true,
                overrideSkyboxesToggle, () => VolumetricSkyboxesManager.Instance.ChangePanorama());

            RegisterToggle("cyberGrind.volumetricSkyboxes.useSkyboxesGridTextures", true,
                overrideGridTexturesToggle, () => VolumetricSkyboxesManager.Instance.ChangeGridTextures());
            
            RegisterToggle("cyberGrind.volumetricSkyboxes.muteSounds", false, muteSoundsToggle);
            RegisterToggle("cyberGrind.volumetricSkyboxes.disableScorePanel", false, disableScorePanelToggle);
        }

        private static void RegisterToggle(string key, bool fallback, ControllerPointerToggle toggle, Action postAction = null)
        {
            toggle.isOn = PrefsManager.Instance!.GetBoolLocal(key, fallback);
            toggle.onValueChanged.AddListener(value => PrefsManager.Instance.SetBoolLocal(key, value));
            postAction?.Invoke();
        }
    }
}