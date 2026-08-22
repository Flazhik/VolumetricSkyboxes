using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static VolumetricSkyboxes.Utils.ReflectionUtils;

namespace VolumetricSkyboxes.Components
{
    [ConfigureSingleton(SingletonFlags.NoAutoInstance)]
    public class VolumetricSkyboxesBootstrap : MonoSingleton<VolumetricSkyboxesBootstrap>
    {
        [SerializeField]
        public GameObject browsePanel;
        
        [SerializeField]
        public VolumetricSkyboxesBrowser skyboxesBrowser;
        
        [SerializeField]
        public ArenaLightingManager arenaLightingManager;
        
        [SerializeField]
        public VolumetricSkyboxesCredits credits;
        
        [SerializeField]
        public GameObject settingsPanel;
        
        [SerializeField]
        public GameObject lowerPanel;
        
        [SerializeField]
        public GameObject portalsHack;
        
        [SerializeField]
        public AudioMixer skyboxesSound;
        
        [SerializeField]
        public AudioMixerGroup skyboxesMixerGroup;

        protected void Awake()
        {
            Instantiate();
        }

        public void Instantiate()
        {
            var customTextures = FindObjectOfType<CustomTextures>(true);
            var gridWrapper = customTextures.GetPrivate<GameObject>("gridWrapper");
            var skyboxesButton = GetShopButton("skyboxBtn");
            var defaultGridWrapper = customTextures.GetPrivate<GameObject>("gridWrapper");

            var window = gridWrapper.transform.parent;

            var themes = FindObjectOfType<CyberGrindSettingsNavigator>().transform.Find("Logic/Themes");
            skyboxesBrowser.transform.parent = themes;
            arenaLightingManager.transform.parent = themes;

            browsePanel.transform.SetParent(window.transform, false);
            lowerPanel.transform.SetParent(window.transform, false);
            settingsPanel.transform.SetParent(window.transform, false);

            var panel = lowerPanel.GetComponent<SkyboxesLowerPanel>();
            browsePanel.gameObject.SetActive(false);
            lowerPanel.SetActive(false);
            settingsPanel.SetActive(false);

            foreach (var buttonName in new [] {"gridBtn", "emissionBtn", "fogBtn"})
            {
                var button = GetShopButton(buttonName);
                button.toDeactivate = button.toDeactivate.Concat(new [] { browsePanel, lowerPanel, settingsPanel }).ToArray();
            }

            skyboxesButton.toActivate = skyboxesButton.toActivate.Concat(new [] { lowerPanel }).ToArray();

            panel.showVolumetricSkyboxes.onClick.AddListener(() =>
            {
                var enableVolumetric = !browsePanel.activeSelf;
                browsePanel.SetActive(enableVolumetric);
                panel.buttonsGroup.SetActive(enableVolumetric);
                defaultGridWrapper.SetActive(!enableVolumetric && !settingsPanel.activeSelf);
                settingsPanel.SetActive(false);
            });

            panel.settingsBtn.onClick.AddListener(() =>
            {
                browsePanel.SetActive(false);
                defaultGridWrapper.SetActive(false);
                settingsPanel.SetActive(true);
            });

            /*
             * Portal renderer goes nuts when all the portals in a scene get removed
             * We'll just provide it with two 3x3 portals placed somewhere in the stratosphere to prevent that
             */
            Instantiate(portalsHack);
            return;

            ShopButton GetShopButton(string fieldName) =>
                customTextures.GetPrivate<Button>(fieldName).gameObject.GetComponent<ShopButton>();
        }
    }
}