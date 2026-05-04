using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VolumetricSkyboxes.Utils;

namespace VolumetricSkyboxes.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VolumetricSkyboxesList
    {
        public static string CurrentPath => Path.Combine(PathsUtils.PreferencesPath, "VolumetricSkyboxes.json");

        [JsonProperty]
        private HashSet<string> _guids = new();

        public HashSet<string> Guids => _guids;
        public int Count => _guids.Count;
        public event Action OnChanged;

        public void Add(string guid)
        {
            _guids.Add(guid);
            OnChanged?.Invoke();
        }
        
        public void Remove(string guid)
        {
            _guids.Remove(guid);
            OnChanged?.Invoke();
        }

        public bool Has(string guid) => _guids.Contains(guid);
    }
}