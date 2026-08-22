using HarmonyLib;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public static class ZapperPatch
    {
        [HarmonyPatch(typeof(Zapper), "FixedUpdate")]
        public static class ZapperFixedUpdatePatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }        
        
        [HarmonyPatch(typeof(Zapper), "TryMigrateToAdjacentPortal")]
        public static class ZapperTryMigrateToAdjacentPortalPatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
    }
}