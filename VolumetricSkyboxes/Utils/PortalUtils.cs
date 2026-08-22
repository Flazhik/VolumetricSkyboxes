using HarmonyLib;
using ULTRAKILL.Portal;
using UnityEngine;
using VolumetricSkyboxes.Components;

namespace VolumetricSkyboxes.Utils
{
    public static class PortalUtils
    {
        private delegate void FollowUpdateDelegate(Follow instance);
        private delegate void PortalManagerUpdateDelegate(PortalManagerV2 instance);
        
        private static readonly FollowUpdateDelegate FollowUpdate = AccessTools.MethodDelegate<FollowUpdateDelegate>(
            AccessTools.Method(typeof(Follow), "Update"));
        private static readonly PortalManagerUpdateDelegate PortalManagerUpdate = AccessTools.MethodDelegate<PortalManagerUpdateDelegate>(
            AccessTools.Method(typeof(PortalManagerV2), "Update"));
        
        /*
         * Many ultrakill components weren't designed to move around in world space, so there's no simple fix for this
         * I'll try to make it work at least for MovingPlatform and Follow components
         */
        public static void FixComponentsPositioning(SkyboxParallaxEffect skybox, Matrix4x4 travelMatrix)
        {
            foreach (var platform in skybox.movingPlatforms)
            {
                platform.targetPosition = travelMatrix.MultiplyPoint3x4(platform.targetPosition);
                platform.currentPosition = travelMatrix.MultiplyPoint3x4(platform.currentPosition);
                platform.originalPosition = travelMatrix.MultiplyPoint3x4(platform.originalPosition);
            }
            
            foreach (var follower in skybox.followers)
                FollowUpdate(follower);
        }

        public static void SyncPortals()
        {
            if (VolumetricSkyboxesManager.Instance!.portalsArePresent)
                PortalManagerUpdate(PortalManagerV2.Instance!);
        }
    }
}