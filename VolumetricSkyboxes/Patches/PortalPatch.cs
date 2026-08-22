using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ULTRAKILL.Portal;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public static class PortalPatch
    {
        /*
         * Null portals can suck my balls
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PortalManagerV2), "Update")]
        public static bool PortalManagerV2_Update_Patch(PortalManagerV2 __instance,
            ref List<Portal> ___portalComponents)
        {
            ___portalComponents = ___portalComponents.Where(portal => portal != null).ToList();
            return true;
        }
    }
}