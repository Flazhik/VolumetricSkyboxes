using System.ComponentModel;
using Newtonsoft.Json;

namespace VolumetricSkyboxes
{
    public class VolumetricSkyboxBundleData
    {
        public string skyboxName { get; set; }
        public string author { get; set; }
        public string guid { get; set; }
        public string buildHash { get; set; }
        [DefaultValue(-1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int version { get; set; }
        public string dataPath { get; set; }
        public string prefabPath { get; set; }
    }
}
