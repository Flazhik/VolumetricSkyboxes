using HarmonyLib;
using UnityEngine;
using VolumetricSkyboxes.Components;
using VolumetricSkyboxes.Utils;
using static UnityEngine.Object;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(CustomFogController))]
    public class CustomFogControllerPatch
    {
        private static BackgroundCamera _backgroundCam;

        private static void NotifyBackgroundCamera(bool disabled, bool levelStarted, float r, float g, float b, float min, float max)
        {
            if (!levelStarted)
                return;
            
            if (_backgroundCam == null)
                _backgroundCam = FindObjectOfType<BackgroundCamera>();
            
            if (_backgroundCam == null)
                return;
            
            _backgroundCam.SetCurrentEnvironmentalFog(disabled, new Color(r, g, b), min, max);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomFogController), "SetState")]
        public static void CustomFogController_SetState_Postfix(
            CustomFogController __instance,
            float ___redAmount,
            float ___greenAmount,
            float ___blueAmount,
            float ___startDistance,
            float ___endDistance,
            bool ___levelStarted
        )
        {
            NotifyBackgroundCamera(GetDisabled(__instance), ___levelStarted, ___redAmount, ___greenAmount, ___blueAmount, ___startDistance, ___endDistance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomFogController), "UpdateColor")]
        public static void CustomFogController_UpdateColor_Postfix(
            CustomFogController __instance,
            float ___redAmount,
            float ___greenAmount,
            float ___blueAmount,
            float ___startDistance,
            float ___endDistance,
            bool ___levelStarted
            )
        {
            NotifyBackgroundCamera(GetDisabled(__instance), ___levelStarted, ___redAmount, ___greenAmount, ___blueAmount, ___startDistance, ___endDistance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomFogController), "SetFogStartDistance")]
        public static void CustomFogController_SetFogStartDistance_Postfix(
            CustomFogController __instance,
            float ___redAmount,
            float ___greenAmount,
            float ___blueAmount,
            float ___startDistance,
            float ___endDistance,
            bool ___levelStarted
        )
        {
            NotifyBackgroundCamera(GetDisabled(__instance), ___levelStarted, ___redAmount, ___greenAmount, ___blueAmount, ___startDistance, ___endDistance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomFogController), "SetFogEndDistance")]
        public static void CustomFogController_SetFogEndDistance_Postfix(
            CustomFogController __instance,
            float ___redAmount,
            float ___greenAmount,
            float ___blueAmount,
            float ___startDistance,
            float ___endDistance,
            bool ___levelStarted
        )
        {
            NotifyBackgroundCamera(GetDisabled(__instance), ___levelStarted, ___redAmount, ___greenAmount, ___blueAmount, ___startDistance, ___endDistance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomFogController), "SetPreset")]
        public static void CustomFogController_SetPreset_Postfix(
            CustomFogController __instance,
            float ___redAmount,
            float ___greenAmount,
            float ___blueAmount,
            float ___startDistance,
            float ___endDistance,
            bool ___levelStarted
        )
        {
            NotifyBackgroundCamera(GetDisabled(__instance), ___levelStarted, ___redAmount, ___greenAmount, ___blueAmount, ___startDistance, ___endDistance);
        }        
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomFogController), "ResetValues")]
        public static void CustomFogController_ResetValues_Postfix(
            CustomFogController __instance,
            float ___redAmount,
            float ___greenAmount,
            float ___blueAmount,
            float ___startDistance,
            float ___endDistance,
            bool ___levelStarted
        )
        {
            NotifyBackgroundCamera(GetDisabled(__instance), ___levelStarted, ___redAmount, ___greenAmount, ___blueAmount, ___startDistance, ___endDistance);
        }
        
        private static bool GetDisabled(CustomFogController instance) =>
            instance.GetPrivateProperty<bool>("fogDisabled");
    }
}