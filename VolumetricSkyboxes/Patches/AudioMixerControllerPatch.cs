using HarmonyLib;
using VolumetricSkyboxes.Components;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(AudioMixerController))]
    public class AudioMixerControllerPatch
    {
        private const string MuteSoundsKey = "cyberGrind.volumetricSkyboxes.muteSounds";
        private const string AllVolume = "skyboxesVolume";
        private const string AllPitch = "skyboxesPitch";
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AudioMixerController), "UpdateSFXVolume")]
        public static void AudioMixerController_UpdateSFXVolume_Postfix(AudioMixerController __instance,
            float ___temporaryDipAmount)
        {
            var bootstrap = VolumetricSkyboxesBootstrap.Instance!;
            if (!VolumetricSkyboxesPlugin.IsCybergrind || bootstrap == null)
                return;
            
            bootstrap.skyboxesSound.SetFloat(AllVolume,
                __instance.CalculateVolume(
                    !PrefsManager.Instance!.GetBoolLocal(MuteSoundsKey)
                    && (!bootstrap.skyboxesSound.GetFloat(AllPitch, out var num) || num != 0.0) 
                        ? __instance.sfxVolume
                        : 0.0f) + ___temporaryDipAmount);
        }
    }
}