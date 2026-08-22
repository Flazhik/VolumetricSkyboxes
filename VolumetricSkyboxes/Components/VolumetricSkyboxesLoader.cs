using System.Collections.Generic;
using static SingletonFlags;

namespace VolumetricSkyboxes.Components
{
    [ConfigureSingleton(DestroyDuplicates | PersistAutoInstance | HideAutoInstance)]
    public class VolumetricSkyboxesLoader : MonoSingleton<VolumetricSkyboxesLoader>
    {
        private readonly Dictionary<Playlist.SongIdentifier, VolumetricSkyboxContainer> _cache = new();
        
        public VolumetricSkyboxContainer Load(string guid)
        {
            if (_cache.TryGetValue(guid, out var result))
                return result;

            var container = VolumetricSkyboxContainer.FromGuid(guid);

            if (container == null)
                return null;
            
            container.SkyboxGameObject.SetActive(false);

            _cache.Add(guid, container);
            return _cache[guid];
        }
    }
}