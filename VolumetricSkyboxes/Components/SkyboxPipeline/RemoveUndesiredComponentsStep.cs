using System;
using System.Collections.Generic;
using System.Linq;
using Gravity;
using UnityEngine;
using static UnityEngine.Object;

namespace VolumetricSkyboxes.Components.SkyboxPipeline
{
    public class RemoveUndesiredComponentsStep : ISkyboxPipelineStep
    {
        private static readonly HashSet<Type> UndesiredComponents = new () {
            typeof(Collider),
            typeof(MonoSingleton),
            typeof(HurtZone),
            typeof(Enemy),
            typeof(EnemyIdentifier),
            typeof(AddForce),
            typeof(PortalTravelFlagsSetter),
            typeof(GoreZone),
            typeof(StencilValuesByLayer),
            typeof(AudioReverbZone),
            typeof(GravityVolume),
        };
        
        public void Execute(VolumetricSkyboxContainer _, GameObject skybox)
        {
            foreach (var component in UndesiredComponents.SelectMany(componentType =>
                         skybox.GetComponentsInChildren(componentType, true)))
                Destroy(component);

            foreach (var mesh in skybox.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mesh.isPartOfStaticBatch)
                    Destroy(mesh);
            }
        }
    }
}