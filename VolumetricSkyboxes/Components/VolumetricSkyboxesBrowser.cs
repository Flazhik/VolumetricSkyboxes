using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VolumetricSkyboxes.Collections;
using VolumetricSkyboxes.Models;
using static Newtonsoft.Json.JsonConvert;
using static VolumetricSkyboxes.Utils.PathsUtils;

namespace VolumetricSkyboxes.Components
{
    public class VolumetricSkyboxesBrowser : DirectoryTreeBrowser<VolumetricSkyboxBundleData>
    {
        private readonly Dictionary<string, Sprite> _spriteCache = new();

        private VolumetricSkyboxesList _skyboxes = new();
        
        protected override int maxPageLength => 6;

        protected override IDirectoryTree<VolumetricSkyboxBundleData> baseDirectory => new SkyboxesFileTree(SkyboxesPath);

        private VolumetricSkyboxesManager _skyboxesManager;

        private void Awake()
        {
            currentDirectory = baseDirectory;
            _skyboxesManager = VolumetricSkyboxesManager.Instance;
            
            LoadSkyboxesList();
        }

        private void Start()
        {
            _skyboxes.OnChanged += SaveSkyboxes;
            _skyboxesManager.AddSkyboxes(_skyboxes.Guids);
        }

        private void OnDestroy()
        {
            _skyboxes.OnChanged += SaveSkyboxes;
        }

        private void LoadSkyboxesList()
        {
            VolumetricSkyboxesList deserializedSkyboxes;
            using (var streamReader = new StreamReader(File.Open(VolumetricSkyboxesList.CurrentPath, FileMode.OpenOrCreate)))
                deserializedSkyboxes = DeserializeObject<VolumetricSkyboxesList>(streamReader.ReadToEnd());

            if (deserializedSkyboxes != null)
                _skyboxes = deserializedSkyboxes;
            
            Rebuild();
        }
        
        public void SaveSkyboxesList() => File.WriteAllText(VolumetricSkyboxesList.CurrentPath, SerializeObject(_skyboxes));

        public void ReloadSkyboxes()
        {
            _skyboxesManager.ReloadSkyboxes();
            _skyboxesManager.ClearCache();
            Rebuild();
        }

        protected override Action BuildLeaf(VolumetricSkyboxBundleData skybox, int indexInPage)
        {
            var go = Instantiate(itemButtonTemplate, itemParent, false);
            var skyboxEntry = go.GetComponent<VolumetricSkyboxEntry>();
            if (_skyboxes.Has(skybox.guid))
            {
                skyboxEntry.button.gameObject.GetComponent<Image>().color = Color.green;
                skyboxEntry.iconInset.color = Color.green;
            }
            skyboxEntry.button.onClick.AddListener(() =>
            {
                if (!_skyboxes.Has(skybox.guid))
                {
                    _skyboxes.Add(skybox.guid);
                    _skyboxesManager.AddSkybox(skybox.guid);
                }
                else
                {
                    _skyboxes.Remove(skybox.guid);
                    _skyboxesManager.RemoveSkybox(skybox.guid);
                }
                
                Rebuild(false);
            });
            skyboxEntry.title.text = skybox.skyboxName;
            skyboxEntry.author.text = $"by {skybox.author}";
            if (LoadIcon(skybox.guid) is { } icon)
                skyboxEntry.icon.sprite = icon;
            go.SetActive(true);
            return () => Destroy(go);
        }
        
        protected override Action BuildDirectory(IDirectoryTree<VolumetricSkyboxBundleData> folder, int indexInPage)
        {
            var btn = Instantiate(folderButtonTemplate, itemParent, false);
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetComponent<Button>().onClick.AddListener(() => StepDown(folder));
            btn.GetComponentInChildren<TMP_Text>().text = folder.name;
            btn.SetActive(true);

            return () => Destroy(btn);
        }
        
        private Sprite LoadIcon(string guid)
        {
            if (_spriteCache.TryGetValue(guid, out var icon))
                return icon;

            try
            {
                var data = File.ReadAllBytes(Path.Combine(UnpackedSkyboxesPath, guid, "icon.png"));
                var tex = new Texture2D(0, 0, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point
                };
                tex.LoadImage(data);
                var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f),
                    100f);
                sprite.texture.filterMode = FilterMode.Point;

                _spriteCache[guid] = sprite;
                return sprite;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        private void SaveSkyboxes() => File.WriteAllText(VolumetricSkyboxesList.CurrentPath, SerializeObject(_skyboxes));
    }
}