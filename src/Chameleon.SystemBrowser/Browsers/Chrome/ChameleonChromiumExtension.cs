using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace Chameleon.SystemBrowser.Chrome
{
    public class ChameleonChromiumExtension
    {
        public static readonly string[] modifiedManifestExtNames = { 
            WebBrowserExtensionManifestFile.ChameleonExtName,
            "UserAgentSelector" 
        };

        private readonly SystemBrowserType _browserType;
        private readonly IApplicationEnvironment _applicationEnvironment;

        public ChameleonChromiumExtension(
            IApplicationEnvironment applicationEnvironment,
            SystemBrowserType browserType
            )
        {
            _browserType = browserType;
            _applicationEnvironment = applicationEnvironment;
        }

        public int GetHttpPort()
        {
            return WebBrowserExtensionManifestFile.Instance.GetPort();
        }

        public Uri GetUrlToOpen(Uri urlToOpen)
        {
            var httpPort = GetHttpPort();
            var encodedUrl = HttpUtility.UrlEncode(urlToOpen.AbsoluteUri);
            urlToOpen = new Uri($"http://127.0.0.1:{httpPort}/?open={encodedUrl}");
            return urlToOpen;
        }

        public virtual DirectoryInfo Create(int userProfileId, WebBrowserExtensionManifestFile manifestFile)
        {
            var directoryPath = manifestFile.GetDirectoryPath();
            var directoryInfo = new DirectoryInfo(directoryPath);

            var destinationFolder = Path.Combine(
                _applicationEnvironment.ApplicationDataFolderPath,
                _browserType.ToString(), "extension", userProfileId.ToString(), manifestFile.extensionName
                );

            directoryInfo.CopyTo(destinationFolder);

            var manifestFilePath = Path.Combine(
                destinationFolder,
                WebBrowserExtensionManifestFile.ManifestFileName
                );

            manifestFile.SetBrowserType(_browserType);
            manifestFile.SetUserProfileId(userProfileId);
            manifestFile.SaveTo(manifestFilePath);

            return new DirectoryInfo(destinationFolder);
        }

        public virtual string GetLoadExtensionsArgument(string extensionsFolderPath, IUserProfile userProfile)
        {
            var directories = Directory
                .GetDirectories(extensionsFolderPath)
                //.Where(directory => !directory.EndsWith("UserAgentSelector", StringComparison.OrdinalIgnoreCase))
                .ToList();

            //remove template extension folder from list
            var chameleonExtensionDirectoryPaths = directories
                .Where(directory => modifiedManifestExtNames.Any(ext_name => directory.EndsWith(ext_name, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            directories.RemoveAll(d => chameleonExtensionDirectoryPaths.Contains(d));

            // create a copy of extension folder with proper settings
           //foreach (var extDirPath in chameleonExtensionDirectoryPaths)
           //{
           //    var extensionDirectory =
           //        Create(userProfile.Id, new WebBrowserExtensionManifestFile(Path.GetFileName(extDirPath)));
           //    directories.Add(extensionDirectory.FullName);
           //}

            return directories
                .AddQuotesToEachElement()
                .ToCommaSeparatedString();
        }
    }
}
