using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace VolumetricSkyboxes.Utils
{
    public static class PathsUtils
    {
        public const string SkyboxExtension = ".cgvsb";
        public static bool HasValidExtenstion(FileInfo path) => SkyboxExtension.Equals(path.Extension.ToLower());
        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string ApplicationPath = 
                Path.Combine(Directory.GetParent(Application.dataPath)?.FullName);
        public static readonly string SkyboxesPath = Path.Combine(ApplicationPath, "Cybergrind", "VolumetricSkyboxes");
        public static readonly string PreferencesPath = Path.Combine(ApplicationPath, "Preferences");
        public static readonly string AssemblyPath =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string DataPath = Path.Combine(AppData, "VolumetricSkyboxes");
        public static readonly string UnpackedSkyboxesPath = Path.Combine(DataPath, "Unpacked");
    }
}