using System;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static VolumetricSkyboxes.Utils.PathsUtils;
using static Newtonsoft.Json.JsonConvert;

namespace VolumetricSkyboxes
{
    public class VolumetricSkyboxContainer : IDisposable
    {
        public readonly GameObject SkyboxGameObject;
        public readonly VolumetricSkyboxData Data;

        private readonly AsyncOperationHandle<VolumetricSkyboxData> _dataHandle;
        private readonly AsyncOperationHandle<GameObject> _gameObjectHandle;

        public static VolumetricSkyboxContainer FromGuid(string guid)
        {
            var folderName = GetSafeName(guid);
            if (folderName == null)
            {
                Debug.LogError($"Refused skybox with unsafe guid [{guid}]");
                return null;
            }

            var skyboxPath = Path.Combine(UnpackedSkyboxesPath, folderName);
            var dataPath = Path.Combine(skyboxPath, "data.json");
            var catalogPath = Path.Combine(skyboxPath, "catalog.json");
            
            if (!Directory.Exists(skyboxPath) || !File.Exists(dataPath) || !File.Exists(catalogPath))
            {
                Debug.LogWarning($"Can't locate unpacked skybox [guid={guid}]");
                return null;
            }

            var data = DeserializeObject<VolumetricSkyboxBundleData>(File.ReadAllText(dataPath));
            return new VolumetricSkyboxContainer(data, catalogPath);
        }
        
        private VolumetricSkyboxContainer(VolumetricSkyboxBundleData data, string catalogPath)
        {
            Addressables.LoadContentCatalogAsync(catalogPath, false).WaitForCompletion();
            _dataHandle = Addressables.LoadAssetAsync<VolumetricSkyboxData>(data.dataPath);
            _gameObjectHandle = Addressables.LoadAssetAsync<GameObject>(data.prefabPath);

            Data = _dataHandle.WaitForCompletion();
            SkyboxGameObject = _gameObjectHandle.WaitForCompletion();
        }
        
        public void Dispose()
        {
           Addressables.Release(_dataHandle);
           Addressables.Release(_gameObjectHandle);
        }
    }
}