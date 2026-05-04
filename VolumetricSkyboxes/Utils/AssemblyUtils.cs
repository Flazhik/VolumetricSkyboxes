using System.IO;
using System.Reflection;
using static System.IO.Path;
namespace VolumetricSkyboxes.Utils
{
    public static class AssemblyUtils
    {
        public static void LoadAssembly(string scriptName) =>
            Assembly.Load(File.ReadAllBytes(Combine(PathsUtils.AssemblyPath, scriptName)));
    }
}