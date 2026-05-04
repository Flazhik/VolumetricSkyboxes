using HarmonyLib;
using UnityEngine;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public class PostProcessV2HandlerPatch
    {
        private static Camera m_backgroundCam;
        private static Camera BackgroundCam => m_backgroundCam ? m_backgroundCam
            : CameraController.Instance
                ?.cam
                ?.transform
                .Find("Background Camera")
                ?.GetComponent<Camera>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PostProcessV2_Handler), "SetupRTs")]
        [HarmonyPriority(401)]
        public static void PostProcessV2_Handler_SetupRTs_Postfix(
            PostProcessV2_Handler __instance,
            RenderBuffer[] ___buffers,
            RenderTexture ___depthBuffer,
            Material ___screenNormal,
            ref bool ___reinitializeTextures
            )
        {
            if (BackgroundCam == null)
                return;
            
            BackgroundCam.SetTargetBuffers(___buffers, ___depthBuffer.depthBuffer);
            ___screenNormal.SetTexture("_DepthBuffer", ___depthBuffer);
        }        
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PostProcessV2_Handler), "OnPreRenderCallback")]
        [HarmonyPriority(402)]
        public static void PostProcessV2_Handler_OnPreRenderCallback_Postfix(
            PostProcessV2_Handler __instance,
            RenderBuffer[] ___buffers,
            RenderTexture ___depthBuffer
            )
        {
            if (BackgroundCam == null)
                return;
            BackgroundCam.SetTargetBuffers(___buffers, ___depthBuffer.depthBuffer);
        }
    }
}