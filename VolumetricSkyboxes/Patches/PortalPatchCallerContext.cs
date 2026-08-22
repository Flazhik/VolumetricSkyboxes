using System;

namespace VolumetricSkyboxes.Patches
{
    public static class PortalPatchCallerContext
    {
        [ThreadStatic]
        public static bool FromPortalUnawareCaller;
    }
}