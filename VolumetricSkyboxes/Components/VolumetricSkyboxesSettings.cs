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

        private void Awake()
        {
            overrideSkyboxesToggle.isOn =
                PrefsManager.Instance.GetBoolLocal("cyberGrind.volumetricSkyboxes.arenaLighting.useSkyboxesPanorama",
                    true);
            
            overrideSkyboxesToggle.onValueChanged.AddListener(value =>
            {
                PrefsManager.Instance.SetBoolLocal("cyberGrind.volumetricSkyboxes.arenaLighting.useSkyboxesPanorama",
                    value);
                VolumetricSkyboxesManager.Instance.ChangePanorama();
            });            
            
            overrideGridTexturesToggle.isOn =
                PrefsManager.Instance.GetBoolLocal("cyberGrind.volumetricSkyboxes.useSkyboxesGridTextures",
                    true);
            
            overrideGridTexturesToggle.onValueChanged.AddListener(value =>
            {
                PrefsManager.Instance.SetBoolLocal("cyberGrind.volumetricSkyboxes.useSkyboxesGridTextures",
                    value);
                VolumetricSkyboxesManager.Instance.ChangeGridTextures();
            });
        }
    }
}