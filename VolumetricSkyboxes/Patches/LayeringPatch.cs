using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using VolumetricSkyboxes.Components;
using VolumetricSkyboxes.Utils;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch]
    public static class LayeringPatch
    {
        static MethodBase[] TargetMethods()
        {
            return typeof(Object)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m =>
                    m.Name == "Instantiate" &&
                    m.IsGenericMethod == false &&
                    m.ContainsGenericParameters == false)
                .Cast<MethodBase>()
                .ToArray();
        }

        public static void Postfix(Object __result) => Handle(__result);

        private static void Handle(Object obj)
        {
            if (!VolumetricSkyboxesPlugin.IsCybergrind || obj == null)
                return;

            switch (obj)
            {
                case GameObject go:
                    Process(go);
                    break;
                case Component c:
                    Process(c.gameObject);
                    break;
            }
        }

        private static void Process(GameObject go)
        {
            if (go.transform.GetComponentInParent<SkyboxParallaxEffect>() != null)
                return;
            
            var lights = go.GetComponentsInChildren<Light>(true);

            foreach (var l in lights)
                l.cullingMask &= ~RenderingUtils.BackgroundLayersMask;
        }
    }
}