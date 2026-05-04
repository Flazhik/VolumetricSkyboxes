using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using VolumetricSkyboxes.Components;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(CustomTextures))]
    public class CustomTexturesPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomTextures), "Start")]
        public static void CustomTextures_Start_Postfix(CustomTextures __instance, Material ___skyMaterial, Material[] ___gridMaterials)
        {
            VolumetricSkyboxesManager.Instance.SaveTexturesEditor(___skyMaterial);
            VolumetricSkyboxesManager.Instance.SaveOriginalGridTextures(___gridMaterials);
        }        
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomTextures), "SetTexture")]
        public static void CustomTextures_SetTexture_Postfix(
            CustomTextures __instance,
            Dictionary<string, Texture2D> ___imageCache,
            string key,
            bool ___editBase,
            bool ___editTop,
            bool ___editTopRow)
        {
            var editMode = (int)Traverse.Create(__instance)
                .Field("currentEditMode")
                .GetValue();
            
            if (editMode != 1)
                return;
            
            VolumetricSkyboxesManager.Instance.ChangeOriginalGridTextures(___imageCache[key], ___editBase, ___editTop, ___editTopRow);
        }
    }
}