using UnityEngine;

namespace VolumetricSkyboxes.Utils
{
    public static class RenderingUtils
    {
        public static readonly int SandboxGrabbableLayer = LayerMask.NameToLayer("SandboxGrabbable");
        public static readonly int SpecialLightingLayer = LayerMask.NameToLayer("SpecialLighting");
        public static readonly int PortalLayer = LayerMask.NameToLayer("Portal");
        public static readonly int SandboxGrabbableMask = 1 << SandboxGrabbableLayer;
        public static readonly int SpecialLightingLayerMask = 1 << SpecialLightingLayer;
        public static readonly int PortalLayerMask = 1 << SpecialLightingLayer;
        public static readonly int BackgroundLayersMask = SandboxGrabbableMask | SpecialLightingLayerMask | PortalLayerMask;

        public static void MoveToBackgroundLayer(GameObject gameObject)
        {
            if (gameObject.layer != SpecialLightingLayer && gameObject.layer != PortalLayer)
                gameObject.layer = SandboxGrabbableLayer;
            
            foreach (Transform child in gameObject.transform)
            {
                if (child.gameObject.layer != SpecialLightingLayer && child.gameObject.layer != PortalLayer)
                    child.gameObject.layer = SandboxGrabbableLayer;

                var hasChildren = child.GetComponentInChildren<Transform>();
                if (hasChildren != null)
                    MoveToBackgroundLayer(child.gameObject);
            }
        }
    }
}