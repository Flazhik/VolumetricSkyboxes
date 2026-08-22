using System;
using System.Collections.Generic;
using System.Linq;
using ULTRAKILL.Portal;
using UnityEngine;
using VolumetricSkyboxes.Components.SkyboxPipeline;
using VolumetricSkyboxes.Utils;
using PortalUtils = VolumetricSkyboxes.Utils.PortalUtils;

namespace VolumetricSkyboxes.Components
{
    public class VolumetricSkyboxesManager: MonoSingleton<VolumetricSkyboxesManager>
    {
        private static readonly List<ISkyboxPipelineStep> SkyboxesPipeline = new()
        {
            new AdjustSkyboxPositionStep(),
            new RemoveUndesiredComponentsStep(),
            new PortalsSanitizeStep(),
            new ApplyComponentsStep(),
            new SetAudioMixerStep(),
            new AssignToBackgroundLayersStep(),
            new SetLightsCullingMaskStep()
        };
        
        public ICollection<VolumetricSkyboxContainer> Skyboxes => _skyboxes.Values;
        public Action<ICollection<VolumetricSkyboxContainer>> OnSkyboxesListChanged;
        public bool portalsArePresent;

        private readonly Dictionary<string, VolumetricSkyboxContainer> _skyboxes = new();
        private readonly Dictionary<string, GameObject> _skyboxObjects = new();
        private readonly Dictionary<string, SkyboxParallaxEffect> _parallaxComponents = new();
        private VolumetricSkyboxesTextureManager _textureManager;

        private void Awake()
        {
            _textureManager = new VolumetricSkyboxesTextureManager();
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
            if (skyboxTemplate == null || skyboxTemplate.transform.Find("ArenaAnchor") == null)
            {
                Debug.LogError("Invalid skybox: GameObject is null or doesn't have an ArenaAnchor object in its root");
                return;
            }
            
            var skybox = Instantiate(skyboxTemplate);
            _skyboxObjects.Add(guid, skybox);
            _skyboxes.Add(guid, container);
            
            foreach (var action in SkyboxesPipeline)
                action.Execute(container, skybox);
            
            skybox.SetActive(true);
            
            _parallaxComponents.Add(guid, skybox.GetComponent<SkyboxParallaxEffect>());
            
            SetClipPlanes();
            _textureManager.ChangePanorama();
            _textureManager.ChangeGridTextures();
            OnSkyboxesListChanged?.Invoke(_skyboxes.Values);
            UpdatePortals();
        }
        
        public void RemoveSkybox(string guid)
        {
            if (!_skyboxObjects.TryGetValue(guid, out var skybox))
                return;

            _skyboxes.Remove(guid);
            OnSkyboxesListChanged?.Invoke(_skyboxes.Values);
            Destroy(skybox);
            _parallaxComponents.Remove(guid);
            _skyboxObjects.Remove(guid);
            _textureManager.ChangePanorama();
            _textureManager.ChangeGridTextures();
            UpdatePortals();
        }
        
        public void ReloadSkyboxes(bool forced = false) => SkyboxFileUtility.ReloadSkyboxes(forced);

        public void ClearCache() => SkyboxFileUtility.ClearCache();
        
        public void ChangePanorama() => _textureManager.ChangePanorama();
        
        public void ChangeGridTextures() => _textureManager.ChangeGridTextures();

        public void UpdateParallax()
        {
            foreach (var parallax in _parallaxComponents.Values)
                parallax.AdjustPosition();
            
            PortalUtils.SyncPortals();
        }

        public void SaveTexturesEditor(Material mat) => _textureManager.SaveSkyboxMaterial(mat);
        
        public void SaveOriginalGridTextures(Material[] mats) => _textureManager.SaveOriginalGridTextures(mats);

        public void ChangeOriginalGridTextures(Texture2D tex, bool baseTex, bool top, bool topRow) =>
            _textureManager.ChangeOriginalGridTextures(tex, baseTex, top, topRow);

        private void UpdatePortals()
        {
            portalsArePresent = _skyboxes.Any(skybox =>
                skybox.Value.SkyboxGameObject.transform.GetComponentInChildren<Portal>(true) != null);
        }

        private void SetClipPlanes()
        {
            var bgCamera = FindObjectOfType<BackgroundCamera>()?.camera;
            if (bgCamera == null)
                return;

            bgCamera.nearClipPlane = _skyboxes.Values.Min(entry => entry.Data.NearClipPlane);
            bgCamera.farClipPlane = _skyboxes.Values.Max(entry => entry.Data.FarClipPlane);
        }
    }
}