using System.Linq;
using UnityEngine;
using VolumetricSkyboxes.Components;
using VolumetricSkyboxes.Utils;

namespace VolumetricSkyboxes
{
    public class VolumetricSkyboxesTextureManager
        {
            private Material _originalSkyboxMaterial;
            private readonly Texture[] _originalGridTextures = new Texture[3];
            
            public void SaveSkyboxMaterial(Material mat)
            {
                _originalSkyboxMaterial = mat;
            }
        
            public void SaveOriginalGridTextures(Material[] gridMaterials)
            {
                for (var i = 0; i < _originalGridTextures.Length; i++)
                    _originalGridTextures[i] = gridMaterials[i].mainTexture;
            }
            
            public void ChangeOriginalGridTextures(Texture2D texture, bool baseTex, bool top, bool topRow)
            {
                if (baseTex)
                    _originalGridTextures[0] = texture;
            
                if (top)
                    _originalGridTextures[1] = texture;
            
                if (topRow)
                    _originalGridTextures[2] = texture;
            
                ChangeGridTextures();
            }
            
            public void ChangePanorama()
            {
                var olm = OutdoorLightMaster.Instance!;
                if (PrefsManager.Instance.GetBoolLocal(
                        "cyberGrind.volumetricSkyboxes.arenaLighting.useSkyboxesPanorama", true)
                    && VolumetricSkyboxesManager.Instance.Skyboxes
                        .Select(entry => entry.Data)
                        .FirstOrDefault(data => data.skyboxMaterial) is { } panorama)
                {
                    olm.SetPrivate("skyboxMaterial", panorama.skyboxMaterial);
                    olm.SetPrivate("skyboxRotation", 0f);
                    olm.skyboxAnimation = SkyboxAnimation.None;
                    RenderSettings.skybox.SetFloat("_Rotation", 0f);
                }
                else
                {
                    olm.SetPrivate("skyboxMaterial", _originalSkyboxMaterial);
                    olm.skyboxAnimation = SkyboxAnimation.Rotate;
                }

                OutdoorLightMaster.Instance.UpdateSkyboxMaterial();
            }
        
            public void ChangeGridTextures()
            {
                var gridMaterials = EndlessGrid.Instance.cubes[0][0].MeshRenderer.sharedMaterials;
                var useSkyboxGridTexture =
                    PrefsManager.Instance.GetBoolLocal("cyberGrind.volumetricSkyboxes.useSkyboxesGridTextures", true);
                
                if (useSkyboxGridTexture
                    && VolumetricSkyboxesManager.Instance.Skyboxes
                        .Select(entry => entry.Data)
                        .FirstOrDefault(data => 
                            data.baseGridTexture != null
                            || data.topRowGridTexture != null
                            || data.topGridTexture != null
                    ) is {} textureSource)
                {
                    SetTexture(0, textureSource.baseGridTexture);
                    SetTexture(1, textureSource.topGridTexture);
                    SetTexture(2, textureSource.topRowGridTexture);
                }
                
                else
                {
                    for (var i = 0; i < 3; i++)
                        SetTexture(i, null);
                }
                
                return;

                void SetTexture(int index, Texture texture) =>
                    gridMaterials[index].mainTexture = texture != null ? texture : _originalGridTextures[index];
            }
        }
}