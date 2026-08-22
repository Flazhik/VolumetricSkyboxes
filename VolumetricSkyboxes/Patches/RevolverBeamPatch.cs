using HarmonyLib;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public static class RevolverBeamPatch
    {
        [HarmonyPatch(typeof(RevolverBeam), "Shoot")]
        public static class RevolverBeamShootPatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
    }
}