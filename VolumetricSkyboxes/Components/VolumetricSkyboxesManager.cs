using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ULTRAKILL.Portal;
using UnityEngine;
using VolumetricSkyboxes.Utils;
using static VolumetricSkyboxes.Utils.PathsUtils;
namespace VolumetricSkyboxes.Components
{
    public class VolumetricSkyboxesManager: MonoSingleton<VolumetricSkyboxesManager>
    {
        private const float NearClipPlaneThreshold = 0.1f;
        private const float FarClipPlaneThreshold = 20000f;
        private static readonly Vector3 ArenaPosition = new(0, 25, 60);

        private static readonly HashSet<Type> UndesiredComponents = new () {
            typeof(Collider),
            typeof(MonoSingleton),
            typeof(HurtZone),
            typeof(Enemy),
            typeof(EnemyIdentifier),
            typeof(Portal),
            typeof(AddForce),
        };

        public ICollection<VolumetricSkyboxContainer> Skyboxes => _skyboxes.Values;
        public Action<ICollection<VolumetricSkyboxContainer>> OnSkyboxesListChanged;

        private readonly Dictionary<string, VolumetricSkyboxContainer> _skyboxes = new();
        private readonly Dictionary<string, GameObject> _skyboxObjects = new();
        
        private Material _originalSkyboxMaterial;
        private readonly Texture[] _originalGridTextures = new Texture[3];

        public void SaveTexturesEditor(Material mat)
        {
            _originalSkyboxMaterial = mat;
        }
        
        public void SaveOriginalGridTextures(Material[] gridMaterials)
        {
            for (var i = 0; i < _originalGridTextures.Length; i++)
                _originalGridTextures[i] = gridMaterials[i].mainTexture;
        }

        public void ReloadSkyboxes(bool forced = false)
        {
            foreach (var skyboxFile in Directory.GetFiles(SkyboxesPath, $"*{SkyboxExtension}", SearchOption.AllDirectories))
                SkyboxFileUtility.UnpackSkybox(skyboxFile, forced);
        }

        public void ClearCache()
        {
            var bundlesGuids = Directory.GetFiles(SkyboxesPath, $"*{SkyboxExtension}", SearchOption.AllDirectories)
                .Select(skyboxFile => SkyboxFileUtility.UnpackSkybox(skyboxFile))
                .Where(data => !string.IsNullOrEmpty(data.guid))
                .Select(data => data.guid)
                .ToHashSet();

            foreach (var directory in new DirectoryInfo(UnpackedSkyboxesPath).GetDirectories())
            {
                if (bundlesGuids.Contains(directory.Name))
                    continue;
                try
                {
                    directory.Delete(true);
                }
                catch (Exception)
                {
                    // nvm
                }
            }
        }

        public void AddSkyboxes(ICollection<string> guids)
        {
            foreach (var guid in guids)
                AddSkybox(guid);
        }
        
        public void AddSkybox(string guid)
        {
            if (_skyboxObjects.ContainsKey(guid))
                return;
            
            var container = VolumetricSkyboxesLoader.Instance!.Load(guid);
            if (container == null)
                return;

            var skyboxTemplate = container.SkyboxGameObject;
            if (skyboxTemplate == null || GetAnchor(skyboxTemplate) == null)
            {
                Debug.LogError("Invalid skybox: GameObject is null or doesn't have an ArenaAnchor object in its root");
                return;
            }
            
			RemoveUndesiredComponents(skyboxTemplate);
            var skybox = Instantiate(skyboxTemplate);
            _skyboxObjects.Add(guid, skybox);
            _skyboxes.Add(guid, container);
            
            ChangePanorama();
            ChangeGridTextures();
            
            OnSkyboxesListChanged?.Invoke(_skyboxes.Values);

            var parallaxEffect = skybox.AddComponent<SkyboxParallaxEffect>();
            parallaxEffect.parallaxCoefficient = container.Data.ParallaxCoefficient;

            AdjustSkyboxPosition(skybox, container.Data.ParallaxCoefficient);
            MoveToBackgroundLayer(skybox);
            SetLightsCullingMask(skybox);
            RemoveAnchor(skybox);
            SetClipPlanes();
        }
        
        public void RemoveSkybox(string guid)
        {
            if (!_skyboxObjects.TryGetValue(guid, out var skybox))
                return;

            _skyboxes.Remove(guid);
            OnSkyboxesListChanged?.Invoke(_skyboxes.Values);
            Destroy(skybox);
            _skyboxObjects.Remove(guid);
            ChangePanorama();
            ChangeGridTextures();
        }

        public void ChangePanorama()
        {
            if (PrefsManager.Instance.GetBoolLocal("cyberGrind.volumetricSkyboxes.arenaLighting.useSkyboxesPanorama", true)
                && _skyboxes.Values.Select(entry => entry.Data).FirstOrDefault(data => data.skyboxMaterial) is { } panorama)
                OutdoorLightMaster.Instance.SetPrivate("skyboxMaterial", panorama.skyboxMaterial);
            else
                OutdoorLightMaster.Instance.SetPrivate("skyboxMaterial", _originalSkyboxMaterial);
            
            OutdoorLightMaster.Instance.UpdateSkyboxMaterial();
        }
        
        public void ChangeGridTextures()
        {
            var customTextures = FindObjectOfType<CustomTextures>();
            var gridMaterials = customTextures.GetPrivate<Material[]>("gridMaterials");
            var useSkyboxGridTexture =
                PrefsManager.Instance.GetBoolLocal("cyberGrind.volumetricSkyboxes.useSkyboxesGridTextures", true);
            
            if (useSkyboxGridTexture
                && _skyboxes.Values
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
                    gridMaterials[i].mainTexture = _originalGridTextures[i];
            }
            
            customTextures.SetPrivate("gridMaterials", gridMaterials);
            return;

            void SetTexture(int index, Texture texture) =>
                gridMaterials[index].mainTexture = texture != null ? texture : _originalGridTextures[index];
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

        private void SetClipPlanes()
        {
            var bgCamera = FindObjectOfType<BackgroundCamera>()?.camera;
            if (bgCamera == null)
                return;

            bgCamera.nearClipPlane = Math.Max(_skyboxes.Values.Min(entry => entry.Data.NearClipPlane), NearClipPlaneThreshold);
            bgCamera.farClipPlane = Math.Min(_skyboxes.Values.Max(entry => entry.Data.FarClipPlane), FarClipPlaneThreshold);
        }

        private static void AdjustSkyboxPosition(GameObject skybox, float parallaxCoefficient)
        {
            var anchor = GetAnchor(skybox);
            var anchorDelta = anchor.position - ArenaPosition;
            var playerDelta = MonoSingleton<CameraController>.Instance!.transform.position - ArenaPosition;
            skybox.transform.rotation = anchor.rotation;
            skybox.transform.position += playerDelta * (1 - parallaxCoefficient) - anchorDelta;
        }
        
        private static void MoveToBackgroundLayer(GameObject go)
        {
            if (go.layer != RenderingUtils.SpecialLightingLayer)
                go.layer = RenderingUtils.SandboxGrabbableLayer;
            foreach (Transform child in go.transform)
            {
                if (child.gameObject.layer != RenderingUtils.SpecialLightingLayer)
                    child.gameObject.layer = RenderingUtils.SandboxGrabbableLayer;

                var hasChildren = child.GetComponentInChildren<Transform>();
                if (hasChildren != null)
                    MoveToBackgroundLayer(child.gameObject);
            }
        }

        private static void SetLightsCullingMask(GameObject skybox)
        {
            foreach (var light in skybox.GetComponentsInChildren<Light>(true))
                light.cullingMask = (light.cullingMask & RenderingUtils.SpecialLightingLayerMask) != 0
                                    && (light.cullingMask & RenderingUtils.SandboxGrabbableMask) == 0
                    ? RenderingUtils.SpecialLightingLayerMask
                    : RenderingUtils.SandboxGrabbableMask;
        }

        private static void RemoveUndesiredComponents(GameObject skybox)
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
        
        private static Transform GetAnchor(GameObject go) => go.transform.Find("ArenaAnchor");
        
        private static void RemoveAnchor(GameObject go) => Destroy(go.transform.Find("ArenaAnchor").gameObject);
    }
}