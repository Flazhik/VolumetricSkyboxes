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
using VolumetricSkyboxes.Utils;
using static VolumetricSkyboxes.Utils.PathsUtils;
using static System.IO.SearchOption;

namespace VolumetricSkyboxes
{
    [BepInProcess("ULTRAKILL.exe")]
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public class VolumetricSkyboxesPlugin : BaseUnityPlugin
    {
        [AddressableAsset("Assets/VolumetricSkyboxes/Bootstrap.prefab", typeof(GameObject))]
        internal static GameObject _bootstrap;

        public static Camera PortalCamera;
        public static bool IsCybergrind;

        private static readonly string CatalogDir;
        private static Harmony _harmony;
        private static bool _init;

        static VolumetricSkyboxesPlugin()
        {
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
            SceneManager.sceneLoaded += OnSceneLoaded;
            _harmony.PatchAll();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene != SceneManager.GetActiveScene())
                return;

            IsCybergrind = SceneHelper.CurrentScene == "Endless";

            switch (SceneHelper.CurrentScene)
            {
                case "Main Menu":
                {
                    Init();
                    break;
                }
                case "Endless":
                {
                    Instantiate(_bootstrap);
                    DefaultReferenceManager.Instance!.radianceEffect.GetComponentInChildren<Light>().cullingMask &=
                        ~RenderingUtils.BackgroundLayersMask;
                    break;
                }
            }
        }

        private static void Init()
        {
            if (_init)
                return;
            
            PrepareFileSystem();
            AssetsManager.Instance!.RegisterPrefabs(Assembly.GetExecutingAssembly());

            var skyboxesManager = VolumetricSkyboxesManager.Instance;
            skyboxesManager.ReloadSkyboxes();
            skyboxesManager.ClearCache();
            _init = true;
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
            
            InitialSkyboxesSetup(initialSkyboxesDir);
        }

        private static void InitialSkyboxesSetup(string initialDir)
        {
            foreach (var dir in Directory.GetDirectories(initialDir, "*", AllDirectories))
                Directory.CreateDirectory(dir.Replace(initialDir, SkyboxesPath));

            foreach (var file in Directory.GetFiles(initialDir, "*.*",AllDirectories))
                File.Copy(file, file.Replace(initialDir, SkyboxesPath), true);
            
            Directory.Delete(initialDir, true);
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