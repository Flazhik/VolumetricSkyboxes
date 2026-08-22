using HarmonyLib;
using UnityEngine;
using VolumetricSkyboxes.Utils;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(SpaceSkybox))]
    public class SpaceSkyboxPatch
    {
        private static bool isCybergrind;
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpaceSkybox), "OnEnable")]
        public static void SpaceSkybox_OnEnable_Prefix(SpaceSkybox __instance)
        {
            isCybergrind = SceneHelper.CurrentScene == "Endless";
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpaceSkybox), "UpdateCamera")]
        public static bool SpaceSkybox_UpdateCamera_Prefix(
            SpaceSkybox __instance,
            Camera cam,
            ref Camera ___playerCam,
            ref Camera ___fakeCam,
            CameraController ___cc,
            RenderTexture ___skybox)
        {
            if (!isCybergrind)
                return true;
            if (!__instance.isActiveAndEnabled)
                return false;
            if (Application.isPlaying)
                ___playerCam = ___cc.cam;
            if (cam != null)
                ___playerCam = cam;
            if (___playerCam == null)
                return false;
            ___fakeCam.transform.rotation = ___playerCam.transform.rotation;
            ___fakeCam.cullingMask = RenderingUtils.BackgroundLayersMask;
            ___fakeCam.fieldOfView = ___playerCam.fieldOfView;
            ___fakeCam.targetTexture = ___skybox;
            ___fakeCam.Render();

            return false;
        }
    }
}