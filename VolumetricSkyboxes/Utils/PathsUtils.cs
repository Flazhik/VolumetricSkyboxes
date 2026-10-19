using System;
using System.IO;
using System.Linq;
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

        private static readonly char[] ForbiddenNameChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|', '\0' };

        public static string GetSafeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = name.Trim();
            if (name is "." or "..")
                return null;

            return name.Any(static c => char.IsControl(c) || Array.IndexOf(ForbiddenNameChars, c) >= 0)
                ? null
                : name;
        }

        public static string InsideRoot(string root, string path)
        {
            var rootFull = Path.GetFullPath(root) + Path.DirectorySeparatorChar;
            var pathFull = Path.GetFullPath(path);
            return pathFull.StartsWith(rootFull)
                ? pathFull
                : null;
        }
    }
}