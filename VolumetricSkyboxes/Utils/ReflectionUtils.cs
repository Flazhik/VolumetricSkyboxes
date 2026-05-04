using System.Reflection;

namespace VolumetricSkyboxes.Utils
{
    public static class ReflectionUtils
    {
        private const BindingFlags PrivateFields = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static T GetPrivate<T>(this object instance, string field)
        {
            var privateField = instance.GetType().GetField(field, PrivateFields);
            return (T)(privateField != null ? privateField.GetValue(instance) : null);
        }
        
        public static T GetPrivateProperty<T>(this object instance, string field)
        {
            var privateField = instance.GetType().GetProperty(field, PrivateFields);
            return (T)(privateField != null ? privateField.GetValue(instance) : null);
        }
        
        public static void SetPrivate<TV>(this object instance, string field, TV value)
        {
            var privateField = instance.GetType().GetField(field, PrivateFields | BindingFlags.SetField);
            if (privateField != null)
                privateField.SetValue(instance, value);
        }
    }
}