using System.IO;

namespace Chameleon.Core.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static void CopyTo(this DirectoryInfo source, string destination, bool recursive = true)
        {
            if (!source.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {source.FullName}");
            }

            var directories = source.GetDirectories();

            destination.EnsureDirectoryExists();

            foreach (var file in source.GetFiles())
            {
                var targetFilePath = Path.Combine(destination, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            if (recursive)
            {
                foreach (var subDir in directories)
                {
                    var newDestinationDir = Path.Combine(destination, subDir.Name);
                    CopyTo(subDir, newDestinationDir, true);
                }
            }
        }
    }
}
