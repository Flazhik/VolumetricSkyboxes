using UnityEngine;
using UnityEngine.UI;

namespace VolumetricSkyboxes.Components
{
    public class SkyboxesLowerPanel : MonoBehaviour
    {
        [SerializeField]
        public Button showVolumetricSkyboxes;
        
        [SerializeField]
        public GameObject buttonsGroup;
        
        [SerializeField]
        public Button settingsBtn;

        public void ShowDiscordInvite() => Application.OpenURL("https://discord.gg/Mc8RhjmbMS");
    }
}