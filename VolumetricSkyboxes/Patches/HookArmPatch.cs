using HarmonyLib;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public static class HookArmPatch
    {
        [HarmonyPatch(typeof(HookArm), "TryMigrateToAdjacentPortal")]
        public static class HookArmTryMigrateToAdjacentPortalPatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
        
        [HarmonyPatch(typeof(HookArm), "FixedUpdate")]
        public static class HookArmFixedUpdatePatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
    }
}