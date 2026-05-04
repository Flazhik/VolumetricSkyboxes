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

            _cache.Add(guid, VolumetricSkyboxContainer.FromGuid(guid));
            return _cache[guid];
        }
    }
}