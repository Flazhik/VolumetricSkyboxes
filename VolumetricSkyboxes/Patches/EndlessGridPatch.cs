using HarmonyLib;
using UnityEngine;
using VolumetricSkyboxes.Utils;
using static UnityEngine.Object;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(EndlessGrid))]
    public class EndlessGridPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EndlessGrid), "Start")]
        public static void EndlessGrid_Start_Postfix(EndlessGrid __instance)
        {
            foreach (var light in FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                light.cullingMask &= ~RenderingUtils.BackgroundLayersMask;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EndlessGrid), "NextWave")]
        public static void EndlessGrid_NextWave_Postfix(EndlessGrid __instance)
        {
            var events = FindObjectsOfType<OnNextWave>(true);

            foreach (var e in events)
                e.Execute();
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EndlessGrid), "OnTriggerEnter")]
        public static void EndlessGrid_OnTriggerEnter_Postfix(EndlessGrid __instance, Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            __instance.waveNumberText.transform.parent.parent.gameObject.SetActive(
                !PrefsManager.Instance!.GetBoolLocal("cyberGrind.volumetricSkyboxes.disableScorePanel", false));

            var events = FindObjectsOfType<OnCybergrindStart>(true);

            foreach (var e in events)
                e.Execute();
        }
    }
}