using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VolumetricSkyboxes.Utils;
using static VolumetricSkyboxes.Utils.PathsUtils;

namespace VolumetricSkyboxes.Collections
{
    public class SkyboxesFileTree : IDirectoryTree<VolumetricSkyboxBundleData>
    {
        public string name { get; private set; }
        public IDirectoryTree<VolumetricSkyboxBundleData> parent { get; set; }
        public IEnumerable<IDirectoryTree<VolumetricSkyboxBundleData>> children { get; private set; }
        public IEnumerable<VolumetricSkyboxBundleData> files { get; private set; }
        private DirectoryInfo RealDirectory { get; }

        public SkyboxesFileTree(string path, IDirectoryTree<VolumetricSkyboxBundleData> parent = null)
        {
            RealDirectory = new DirectoryInfo(path);
            this.parent = parent;
            Refresh();
        }

        private SkyboxesFileTree(DirectoryInfo realDirectory, IDirectoryTree<VolumetricSkyboxBundleData> parent = null)
        {
            RealDirectory = realDirectory;
            this.parent = parent;
            Refresh();
        }

        public void Refresh()
        {
            RealDirectory.Create();
            name = RealDirectory.Name;
            children = RealDirectory.GetDirectories().Select(dir => new SkyboxesFileTree(dir, this));

            files = RealDirectory
                .GetFiles()
                .Where(HasValidExtenstion)
                .Select(file => SkyboxFileUtility.UnpackSkybox(file.FullName))
                .Where(data => data != null)
                .ToList();
        }

        public override bool Equals(object obj) =>
            obj != null
            && !(GetType() != obj.GetType())
            && string.Equals(RealDirectory.FullName, (obj as DirectoryInfo)?.FullName, StringComparison.InvariantCultureIgnoreCase);

        public override int GetHashCode() => RealDirectory.GetHashCode();

        public IEnumerable<VolumetricSkyboxBundleData> GetFilesRecursive() =>
            children
                .SelectMany(child => child.GetFilesRecursive())
                .Concat(files);
    }
}