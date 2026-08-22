using UnityEngine;
using static VolumetricSkyboxes.Utils.RenderingUtils;

namespace VolumetricSkyboxes.Components.SkyboxPipeline
{
    public class SetLightsCullingMaskStep : ISkyboxPipelineStep
    {
        public void Execute(VolumetricSkyboxContainer _, GameObject skybox)
        {
            foreach (var light in skybox.GetComponentsInChildren<Light>(true))
                light.cullingMask = (light.cullingMask & SpecialLightingLayerMask) != 0
                                    && (light.cullingMask & SandboxGrabbableMask) == 0
                    ? SpecialLightingLayerMask
                    : SandboxGrabbableMask;
        }
    }
}