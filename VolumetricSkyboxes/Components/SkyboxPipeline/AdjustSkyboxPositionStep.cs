using UnityEngine;
using static UnityEngine.Object;

/*
 * Moving a skybox so that its arena anchor matches the real arena surface
 */
namespace VolumetricSkyboxes.Components.SkyboxPipeline
{
    public class AdjustSkyboxPositionStep : ISkyboxPipelineStep
    {
        private static readonly Vector3 ArenaPosition = new(0, 25, 60);
        
        public void Execute(VolumetricSkyboxContainer container, GameObject skybox)
        {
            var anchor = GetAnchor(skybox);
            var anchorDelta = anchor.position - ArenaPosition;
            var playerDelta = MonoSingleton<CameraController>.Instance!.transform.position - ArenaPosition;
            skybox.transform.rotation = anchor.rotation;
            skybox.transform.position += playerDelta * (1 - container.Data.ParallaxCoefficient) - anchorDelta;

            Destroy(anchor.gameObject);
        }
        
        private static Transform GetAnchor(GameObject go) => go.transform.Find("ArenaAnchor");
    }
}