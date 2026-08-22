using HarmonyLib;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public static class CoinPatch
    {
        [HarmonyPatch(typeof(Coin), "ReflectRevolver")]
        public static class CoinReflectRevolverPatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
        
        [HarmonyPatch(typeof(Coin), "Punchflection")]
        public static class CoinPunchflectionPatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
        
        [HarmonyPatch(typeof(Coin), "EnemyReflect")]
        public static class CoinEnemyReflectPatch
        {
            static void Prefix() => PortalPatchCallerContext.FromPortalUnawareCaller = true;
            static void Finalizer() => PortalPatchCallerContext.FromPortalUnawareCaller = false;
        }
    }
}