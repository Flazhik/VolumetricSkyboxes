using UnityEngine;

namespace VolumetricSkyboxes.Components.SkyboxPipeline
{
    public interface ISkyboxPipelineStep
    {
        void Execute(VolumetricSkyboxContainer container, GameObject skybox);
    }
}