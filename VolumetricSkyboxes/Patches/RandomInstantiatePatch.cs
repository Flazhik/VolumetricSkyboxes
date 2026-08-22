using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Randomness;
using UnityEngine;
using VolumetricSkyboxes.Components;
using VolumetricSkyboxes.Utils;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(RandomInstantiate))]
    public class RandomInstantiatePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RandomInstantiate), "PerformTheAction")]
        public static void RandomInstantiate_PerformTheAction_Postfix(RandomInstantiate __instance)
        {
            if (!VolumetricSkyboxesPlugin.IsCybergrind)
                return;

            var createdObjects = __instance.GetPrivate<List<GameObject>>("createdObjects");
            foreach (var go in createdObjects)
                RenderingUtils.MoveToBackgroundLayer(go);
            
            foreach (var audioSource in createdObjects.SelectMany(go => go.GetComponentsInChildren<AudioSource>(true)))
                audioSource.outputAudioMixerGroup = VolumetricSkyboxesBootstrap.Instance!.skyboxesMixerGroup;
        }
    }
}