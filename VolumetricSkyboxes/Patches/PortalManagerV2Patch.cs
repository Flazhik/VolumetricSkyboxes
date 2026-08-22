using System.Collections.Generic;
using HarmonyLib;
using ULTRAKILL.Portal;
using UnityEngine;
using VolumetricSkyboxes.Components;
using static System.Reflection.Emit.OpCodes;
using PortalUtils = VolumetricSkyboxes.Utils.PortalUtils;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(PortalManagerV2))]
    public class PortalManagerV2Patch
    {
        private delegate bool CheckPlayerTraversalsDelegate(PortalManagerV2 instance, out (IPortalTraveller traveller, PortalTravelDetails details) result);
        private delegate void TraverseAndCallBackDelegate(PortalManagerV2 instance, in IPortalTraveller traveller, in PortalTravelDetails details);
        private delegate void UpdatePortalAwareRenderersDelegate(PortalManagerV2 instance);
        
        private static readonly CheckPlayerTraversalsDelegate CheckPlayerTraversals = AccessTools.MethodDelegate<CheckPlayerTraversalsDelegate>(
            AccessTools.Method(typeof(PortalManagerV2), "CheckPlayerTraversals"));
        private static readonly TraverseAndCallBackDelegate TraverseAndCallBack = AccessTools.MethodDelegate<TraverseAndCallBackDelegate>(
            AccessTools.Method(typeof(PortalManagerV2), "TraverseAndCallBack"));
        private static readonly UpdatePortalAwareRenderersDelegate UpdatePortalAwareRenderers = AccessTools.MethodDelegate<UpdatePortalAwareRenderersDelegate>(
            AccessTools.Method(typeof(PortalManagerV2), "UpdatePortalAwareRenderers"));

        private static Camera m_backgroundCam;
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PortalManagerV2), "AddTraveller")]
        public static bool PortalManagerV2_AddTraveller_Prefix(PortalManagerV2 __instance)
        {
            return !VolumetricSkyboxesPlugin.IsCybergrind;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PortalManagerV2), "Update")]
        public static bool PortalManagerV2_Update_Prefix(PortalManagerV2 __instance)
        {
            if (!VolumetricSkyboxesPlugin.IsCybergrind || GetBackgroundCamera() == null)
                return true;

            __instance.mainCamera = GetBackgroundCamera();
            return true;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PortalManagerV2), "LateUpdate")]
        public static bool PortalManagerV2_LateUpdate_Prefix(
            PortalManagerV2 __instance,
            IPortalTraveller ___playerTraveller,
            ref Dictionary<int, Vector3> ___lastTravellerPositions,
            bool ___initialized,
            List<Portal> ___portalComponents)
        {
            if (!VolumetricSkyboxesPlugin.IsCybergrind || VolumetricSkyboxesManager.Instance == null)
                return true;

            VolumetricSkyboxesManager.Instance!.UpdateParallax();

            (IPortalTraveller traveller, PortalTravelDetails details) result;
            if (___playerTraveller is BackgroundCamera)
            {
                var traversed = CheckPlayerTraversals(__instance, out result);
                if (traversed)
                {
                    TraverseAndCallBack(__instance, result.traveller, result.details);
                    ___lastTravellerPositions[___playerTraveller.id] = ___playerTraveller.travellerPosition;
                }
            }

            UpdatePortalAwareRenderers(__instance);
            if (!___initialized)
                return false;

            PortalUtils.SyncPortals();
            __instance.render.Setup(__instance.Scene, __instance.mainCamera, __instance.portalCamera);
            return false;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PortalManagerV2), "FixedUpdate")]
        public static void PortalManagerV2_FixedUpdate_Postfix(PortalManagerV2 __instance)
        {
            if (!VolumetricSkyboxesPlugin.IsCybergrind)
                return;
            
            PortalUtils.SyncPortals();
        }        
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PortalManagerV2), "OnPreRenderCallback")]
        static IEnumerable<CodeInstruction> PortalManagerV2_OnPreRenderCallback_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var mainCamField = AccessTools.Field(typeof(PostProcessV2_Handler), "mainCam");
            var portalCameraField = AccessTools.Field(typeof(VolumetricSkyboxesPlugin), "PortalCamera");

            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new CodeMatch(Ldloc_0),
                    new CodeMatch(Ldfld, mainCamField))
                .RemoveInstructions(2)
                .Insert(
                    new CodeInstruction(Ldsfld, portalCameraField))
                .InstructionEnumeration();
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