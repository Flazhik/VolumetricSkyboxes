using System;
using HarmonyLib;
using UnityEngine;
using static HarmonyLib.ArgumentType;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public static class PortalPhysicsPatch
    {
        private static readonly PortalTraversalV2[] EmptyTraversals = Array.Empty<PortalTraversalV2>();
        private static readonly ICastable RaycastCastable = new Raycast();
        
        [HarmonyPrefix]
        [HarmonyPatch(
            typeof(PortalPhysicsV2), 
            "Raycast",
            new[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(int), typeof(PhysicsCastResult), typeof(PortalTraversalV2[]), typeof(Vector3), typeof(QueryTriggerInteraction) },
            new[] { Normal, Normal, Normal, Normal, Out, Out, Out, Normal })]
        public static bool PortalPhysicsV2_Raycast_Prefix(
            ref bool __result,
            Vector3 origin,
            Vector3 direction,
            float maxDistance,
            int layerMask,
            ref PhysicsCastResult hitInfo,
            ref PortalTraversalV2[] portalTraversals,
            ref Vector3 endPoint,
            QueryTriggerInteraction queryTriggerInteraction)
        {
            if (!VolumetricSkyboxesPlugin.IsCybergrind || !PortalPatchCallerContext.FromPortalUnawareCaller)
                return true;

            var state = new PortalCastStateV2
            {
                layerMask = layerMask,
                queryTriggerInteraction = queryTriggerInteraction,
                origin = origin,
                direction = direction.normalized,
                maxDistance = maxDistance
            };
            endPoint = state.origin + state.direction * state.maxDistance;
            var num = RaycastCastable.Cast(state, out hitInfo) ? 1 : 0;
            portalTraversals = EmptyTraversals;
            __result = num != 0;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PortalPhysicsV2), "PortalCastAll")]
        public static bool PortalPhysicsV2_PortalCastAll_Prefix(
            ICastable castable,
            PortalCastStateV2 state,
            ref PortalTraversalV2[] portalTraversals,
            ref PhysicsCastResult[] __result)
        {
            if (!VolumetricSkyboxesPlugin.IsCybergrind || !PortalPatchCallerContext.FromPortalUnawareCaller)
                return true;

            var physicsCastResultArray = castable.CastAll(state);
            portalTraversals = EmptyTraversals;
            __result = physicsCastResultArray;
            return false;
        }
    }
}