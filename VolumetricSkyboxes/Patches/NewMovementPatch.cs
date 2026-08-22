using HarmonyLib;
using ULTRAKILL.Portal;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(NewMovement))]
    public class NewMovementPatch
    {
        private static bool isCybergrind;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NewMovement), "Awake")]
        public static void NewMovement_Awake_Postfix(NewMovement __instance, ref PortalAwarePlayerCollider ___portalAwareCollider)
        {
            if (SceneHelper.CurrentScene != "Endless")
                return;

            ___portalAwareCollider = null;
        }        
    }
}