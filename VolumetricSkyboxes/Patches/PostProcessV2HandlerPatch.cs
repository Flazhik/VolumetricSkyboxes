using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public class PostProcessV2HandlerPatch
    {
        private static Camera m_backgroundCam;

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
            if (GetBackgroundCamera() == null)
                return;
            
            GetBackgroundCamera().SetTargetBuffers(___buffers, ___depthBuffer.depthBuffer);
            ___screenNormal.SetTexture("_DepthBuffer", ___depthBuffer);
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PostProcessV2_Handler), "OnPreRenderCallback")]
        public static IEnumerable<CodeInstruction> PostProcessV2_Handler_OnPreRenderCallback_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inst = new List<CodeInstruction>(instructions);

            var equality = AccessTools.Method(typeof(Object), "op_Equality", new[] { typeof(Object), typeof(Object) });
            var portalCamField = AccessTools.Field(
                typeof(VolumetricSkyboxesPlugin),
                nameof(VolumetricSkyboxesPlugin.PortalCamera));
            
            Label lab = default;
            foreach (var code in inst)
            {
                if (code.opcode != Brtrue && code.opcode != Brtrue_S)
                    continue;
                
                lab = (Label)code.operand;
                break;
            }

            inst.InsertRange(0, new[]
            {
                new CodeInstruction(Ldarg_1),
                new CodeInstruction(Ldsfld, portalCamField),
                new CodeInstruction(Call, equality),
                new CodeInstruction(Brtrue, lab)
            });

            return inst;
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
            if (GetBackgroundCamera() == null)
                return;
            GetBackgroundCamera().SetTargetBuffers(___buffers, ___depthBuffer.depthBuffer);
        }

        private static Camera GetBackgroundCamera()
        {
            if (m_backgroundCam != null)
                return m_backgroundCam;
            
            m_backgroundCam = CameraController.Instance
                ?.cam
                ?.transform
                .Find("Background Camera")
                ?.GetComponent<Camera>();
            
            return m_backgroundCam;
        }
    }
}