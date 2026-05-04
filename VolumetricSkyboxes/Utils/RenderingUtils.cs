using UnityEngine;

namespace VolumetricSkyboxes.Utils
{
    public static class RenderingUtils
    {
        public static readonly int SandboxGrabbableLayer = LayerMask.NameToLayer("SandboxGrabbable");
        public static readonly int SpecialLightingLayer = LayerMask.NameToLayer("SpecialLighting");
        public static readonly int SandboxGrabbableMask = 1 << SandboxGrabbableLayer;
        public static readonly int SpecialLightingLayerMask = 1 << SpecialLightingLayer;
        public static readonly int BackgroundLayersMask = SandboxGrabbableMask | SpecialLightingLayerMask;
    }
}