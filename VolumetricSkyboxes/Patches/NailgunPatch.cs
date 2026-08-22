using HarmonyLib;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public static class NailgunPatch
    {
        [HarmonyPatch(typeof(Nailgun), "Shoot")]
        public static class NailgunShootPatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
        
        [HarmonyPatch(typeof(Nailgun), "BurstFire")]
        public static class NailgunBurstFirePatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
        
        [HarmonyPatch(typeof(Nailgun), "SuperSaw")]
        public static class NailgunSuperSawPatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
    }
}