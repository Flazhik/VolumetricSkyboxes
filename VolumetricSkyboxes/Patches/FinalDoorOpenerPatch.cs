using System.Collections;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using static UnityEngine.Object;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(FinalDoorOpener))]
    public class FinalDoorOpenerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FinalDoorOpener), "OnEnable")]
        public static void FinalDoorOpener_OnEnable_Prefix(FinalDoorOpener __instance)
        {
            if (SceneHelper.CurrentScene != "Endless")
                return;

            __instance.StartCoroutine(Execute());
        }

        private static IEnumerator Execute()
        {
            yield return new WaitForSeconds(1f);
            var events = FindObjectsOfType<OnDoorsOpen>(true);

            foreach (var e in events.OrderBy(e => e.priority))
                e.Execute();
        }
    }
}