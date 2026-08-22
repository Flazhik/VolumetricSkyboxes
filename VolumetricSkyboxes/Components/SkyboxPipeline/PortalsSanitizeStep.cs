using ULTRAKILL.Portal;
using UnityEngine;
using static ULTRAKILL.Portal.PortalTravellerFlags;

/*
 * Making sure the player, enemies and projectiles are unable to traverse a portal
 */
namespace VolumetricSkyboxes.Components.SkyboxPipeline
{
    public class PortalsSanitizeStep : ISkyboxPipelineStep
    {
        public void Execute(VolumetricSkyboxContainer _, GameObject skybox)
        {
            foreach (var portal in skybox.GetComponentsInChildren<Portal>(true))
            {
                portal.entryTravelFlags = Other;
                portal.exitTravelFlags = Other;
                portal.passThroughNonTraversals = true;
            }
        }
    }
}