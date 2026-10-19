using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using static VolumetricSkyboxes.Utils.PathsUtils;
using static Newtonsoft.Json.JsonConvert;
using static System.IO.Path;

namespace VolumetricSkyboxes.Utils
{
    public static class SkyboxFileUtility
    {
        public static void ReloadSkyboxes(bool forced = false)
        {
            foreach (var skyboxFile in Directory.GetFiles(SkyboxesPath, $"*{SkyboxExtension}", SearchOption.AllDirectories))
                UnpackSkybox(skyboxFile, forced);
        }
        
        public static VolumetricSkyboxBundleData UnpackSkybox(string path, bool forced = false)
        {
            var skyboxFile = new FileInfo(path);
            if (!File.Exists(path) || !HasValidExtenstion(skyboxFile)) return null;

            var bundleData = GetBundleData(path);
            if (bundleData?.guid == null) return null;

            var folderName = GetSafeName(bundleData.guid);
            if (folderName == null)
            {
                Debug.LogError($"Refused skybox with unsafe guid [{bundleData.guid}] from [{path}]");
                return null;
            }

            var dataPath = Combine(UnpackedSkyboxesPath, folderName);
            var dataFolderExists = Directory.Exists(dataPath);

            var previousDataExists = dataFolderExists
                                     && File.Exists(Combine(dataPath, "data.json"))
                                     && File.Exists(Combine(dataPath, "catalog.json"));
            var unzip = true;
            if (previousDataExists)
            {
                var previousData = DeserializeObject<VolumetricSkyboxBundleData>(File.ReadAllText(Combine(dataPath, "data.json")));
                if (previousData.guid != null && previousData.buildHash == bundleData.buildHash && !forced)
                    unzip = false;
            }

            if (!unzip)
                return bundleData;

            try
            {
                if (dataFolderExists && InsideRoot(UnpackedSkyboxesPath, dataPath) != null)
                    Directory.Delete(dataPath, true);
                Directory.CreateDirectory(dataPath);

                using var zip = new ZipArchive(File.Open(path, FileMode.Open, FileAccess.Read));
                ExtractArchive(zip, dataPath);

                return bundleData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error unpacking skybox guid=[{bundleData.guid}]");
                Debug.LogException(e);
                return null;
            }
        }

        private static void ExtractArchive(ZipArchive zip, string destination)
        {
            foreach (var entry in zip.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                var target = destination;
                var safe = true;

                foreach (var name in entry.FullName
                             .Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(GetSafeName))
                {
                    if (name == null)
                    {
                        safe = false;
                        break;
                    }

                    target = Combine(target, name);
                }

                if (!safe || InsideRoot(destination, target) == null)
                {
                    Debug.LogWarning($"Ignored unsafe archive entry [{entry.FullName}]");
                    continue;
                }

                Directory.CreateDirectory(GetDirectoryName(target)!);

                using var source = entry.Open();
                using var file = File.Create(target);
                source.CopyTo(file);
            }
        }

        public static void ClearCache()
        {
            var bundlesGuids = Directory.GetFiles(SkyboxesPath, $"*{SkyboxExtension}", SearchOption.AllDirectories)
                .Select(static skyboxFile => UnpackSkybox(skyboxFile))
                .Select(static data => GetSafeName(data?.guid))
                .Where(static guid => guid != null)
                .ToHashSet();

            foreach (var directory in new DirectoryInfo(UnpackedSkyboxesPath).GetDirectories()
                         .Where(directory => !bundlesGuids.Contains(directory.Name))
                         .Where(static directory => InsideRoot(UnpackedSkyboxesPath, directory.FullName) != null))
            {
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

        private static VolumetricSkyboxBundleData GetBundleData(string path)
        {
            using var fs = File.Open(path, FileMode.Open, FileAccess.Read);
            using var zip = new ZipArchive(fs);
            var entry = zip.GetEntry("data.json");
            if (entry == null)
                return null;

            using var dataReader = new StreamReader(entry.Open());
            return DeserializeObject<VolumetricSkyboxBundleData>(dataReader.ReadToEnd());
        }
    }
}