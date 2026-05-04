using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.SceneManagement;
using VolumetricSkyboxes.Assets;
using VolumetricSkyboxes.Components;
using static VolumetricSkyboxes.Utils.PathsUtils;
using static VolumetricSkyboxes.Utils.AssemblyUtils;
using static System.IO.SearchOption;

namespace VolumetricSkyboxes
{
    [BepInProcess("ULTRAKILL.exe")]
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public class VolumetricSkyboxesPlugin : BaseUnityPlugin
    {
        [AddressableAsset("Assets/VolumetricSkyboxes/Bootstrap.prefab", typeof(GameObject))]
        private static GameObject _bootstrap;

        private static readonly string CatalogDir;
        private static Harmony _harmony;
        private static bool _init;

        static VolumetricSkyboxesPlugin()
        {
            LoadAssembly("VolumetricSkyboxesScripts.dll");
            CatalogDir = Path.Combine(AssemblyPath!, "Assets");
        }

        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            SetAddressableProperties();
            Addressables.InitializeAsync().WaitForCompletion();
            Addressables.LoadContentCatalogAsync(Path.Combine(CatalogDir, "catalog.json"), true).WaitForCompletion();
            
            _harmony = new Harmony(PluginInfo.Guid);
            Startup();
        }

        private void Startup()
        {
            _harmony.PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene != SceneManager.GetActiveScene())
                return;

            switch (SceneHelper.CurrentScene)
            {
                case "Main Menu":
                {
                    if (_init)
                        return;
                    
                    PrepareFileSystem();
                    AssetsManager.Instance!.RegisterPrefabs(Assembly.GetExecutingAssembly());

                    var skyboxesManager = VolumetricSkyboxesManager.Instance;
                    skyboxesManager.ReloadSkyboxes();
                    skyboxesManager.ClearCache();
                    _init = true;
                    
                    break;
                }
                case "Endless":
                {
                    Instantiate(_bootstrap);
                    break;
                }
            }
        }

        private static void PrepareFileSystem()
        {
            foreach (var dir in new [] { SkyboxesPath, DataPath, UnpackedSkyboxesPath })
            {
                if  (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            
            var initialSkyboxesDir = Path.Combine(AssemblyPath, "Skyboxes");
            if  (!Directory.Exists(initialSkyboxesDir))
                return;
            
            foreach (var dir in Directory.GetDirectories(initialSkyboxesDir, "*", AllDirectories))
                Directory.CreateDirectory(dir.Replace(initialSkyboxesDir, SkyboxesPath));

            foreach (var file in Directory.GetFiles(initialSkyboxesDir, "*.*",AllDirectories))
                File.Copy(file, file.Replace(initialSkyboxesDir, SkyboxesPath), true);
            
            Directory.Delete(initialSkyboxesDir, true);
        }

        private void SetAddressableProperties()
        {
            AddressablesRuntimeProperties.SetPropertyValue(
                "UnpackedVolumetricSkyboxesPath",
                UnpackedSkyboxesPath
            );
            AddressablesRuntimeProperties.SetPropertyValue(
                "VolumetricSkyboxesRuntimePath",
                AssemblyPath
            );
            AddressablesRuntimeProperties.SetPropertyValue(
                "VolumetricSkyboxesAssetsPath",
                CatalogDir
            );
        }
    }
}