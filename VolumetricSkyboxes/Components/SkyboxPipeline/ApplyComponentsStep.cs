using UnityEngine;

namespace VolumetricSkyboxes.Components.SkyboxPipeline
{
    public class ApplyComponentsStep : ISkyboxPipelineStep
    {
        public void Execute(VolumetricSkyboxContainer container, GameObject skybox)
        {
            var parallaxEffect = skybox.AddComponent<SkyboxParallaxEffect>();
            parallaxEffect.parallaxCoefficient = container.Data.ParallaxCoefficient;
        }
    }
}