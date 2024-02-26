using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chameleon.SystemBrowser.Chrome
{
    public class ChameleonBraveExtension : ChameleonChromiumExtension
    {
        public ChameleonBraveExtension(
            IApplicationEnvironment applicationEnvironment
            ) : base(applicationEnvironment, SystemBrowserType.Brave)
        {
        }

        public override string GetLoadExtensionsArgument(string extensionsFolderPath, IUserProfile userProfile)
        {
            //var directories = Directory
            //    .GetDirectories(extensionsFolderPath)
            //    .ToList();

            //// remove template extension folder from list
            //var chameleonExtensionDirectoryPath = directories
            //    .Single(directory => directory.EndsWith(WebBrowserExtensionManifestFile.ChameleonExtName, System.StringComparison.OrdinalIgnoreCase));
            //directories.Remove(chameleonExtensionDirectoryPath);

            //// create a copy of extension folder with proper settings
            //var extensionDirectory = Create(userProfile.Id, WebBrowserExtensionManifestFile.Instance);

            //return $"\"{extensionDirectory.FullName}\"";

            //var directories = Directory
            //    .GetDirectories(extensionsFolderPath)
            //    .Where(directory => !modifiedManifestExtNames.Any(ext_name => directory.EndsWith(ext_name, StringComparison.OrdinalIgnoreCase)))
            //    .ToList();

            //remove template extension folder from list
            //var chameleonExtensionDirectoryPaths = directories
            //    .Where(directory => modifiedManifestExtNames.Any(ext_name => directory.EndsWith(ext_name, StringComparison.OrdinalIgnoreCase)))
            //    .ToList();
            //directories.RemoveRange(0, directories.Count);

            //// create a copy of extension folder with proper settings
            //foreach (var extDirPath in chameleonExtensionDirectoryPaths)
            //{
            //    var extensionDirectory =
            //        Create(userProfile.Id, new WebBrowserExtensionManifestFile(Path.GetFileName(extDirPath)));
            //    directories.Add(extensionDirectory.FullName);
            //}
            return new List<string>()
                .AddQuotesToEachElement()
                .ToCommaSeparatedString();
        }
    }
}
