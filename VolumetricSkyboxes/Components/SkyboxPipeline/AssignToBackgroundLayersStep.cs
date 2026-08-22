using UnityEngine;
using static VolumetricSkyboxes.Utils.RenderingUtils;

namespace VolumetricSkyboxes.Components.SkyboxPipeline
{
    public class AssignToBackgroundLayersStep : ISkyboxPipelineStep
    {
        public void Execute(VolumetricSkyboxContainer _, GameObject skybox)
        {
            MoveToBackgroundLayer(skybox);
        }
    }
}