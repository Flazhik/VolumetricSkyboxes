using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace VolumetricSkyboxes.Assets
{
    [ConfigureSingleton(SingletonFlags.PersistAutoInstance)]
    public class AssetsManager : MonoSingleton<AssetsManager>
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        public void RegisterPrefabs(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                CheckType(type);
        }
        
        private static void CheckType(IReflect type)
        {
            type.GetFields(Flags)
                .ToList()
                .ForEach(ProcessField);
        }

        private static void ProcessField(FieldInfo field)
        {
            if (field.FieldType.IsArray
                || !typeof(Object).IsAssignableFrom(field.FieldType)
                || !field.IsStatic)
                return;
            var addressableAsset = field.GetCustomAttribute<AddressableAsset>();
            
            if (addressableAsset != null)
                Addressables.LoadAssetAsync<GameObject>(addressableAsset.Path).Completed += value =>
                    field.SetValue(null, Convert.ChangeType(value.Result, addressableAsset.AssetType));
        }
    }
}