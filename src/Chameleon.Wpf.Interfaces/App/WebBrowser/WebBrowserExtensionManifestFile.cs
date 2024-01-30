using System.Text.Json;
using System;
using System.IO;
using System.Reflection;

namespace Chameleon.Interfaces.WebBrowser
{
    public class WebBrowserExtensionManifestFile
    {
        public const string ChameleonExtName = "Chameleon";

        public readonly string extensionName;

        public static WebBrowserExtensionManifestFile Instance { get; }
            = new WebBrowserExtensionManifestFile(ChameleonExtName);

        public WebBrowserExtensionManifestFile(string extensionName)
        {
            this.extensionName = extensionName;
        }

        public int GetPort()
        {
            var json = GetJson();
            return (int)json["applicationPort"];
        }

        public void SetPort(int httpPort)
        {
            var json = GetJson();
            json["applicationPort"] = httpPort;
        }

        public int GetUserProfileId()
        {
            var json = GetJson();
            return (int)json["userProfileId"];
        }

        public void SetUserProfileId(int userProfileId)
        {
            var json = GetJson();
            json["userProfileId"] = userProfileId;
        }

        public void SetBrowserType(SystemBrowserType browserType)
        {
            var json = GetJson();
            json["browserType"] = browserType.ToString();
        }

        private dynamic _json;
        private dynamic GetJson()
        {
            if (_json != null)
            {
                return _json;
            }

            var filePath = GetManifestFilePath();
            var template = File.ReadAllText(filePath);
            _json = JsonSerializer.Deserialize<dynamic>(template);
            return _json;
        }

        public void Save()
        {
            SaveTo(GetManifestFilePath());
        }

        public void SaveTo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException();
            }

            if (_json == null)
            {
                return;
            }

            var content = JsonSerializer.Serialize(_json);
            File.WriteAllText(filePath, content);
        }

        public const string ManifestFileName = "manifest.json";
        public string GetManifestFilePath()
        {
            var folder = GetDirectoryPath();
            return Path.Combine(folder, ManifestFileName);
        }

        public string GetDirectoryPath()
        {
            var rootFolderPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location
            );

            return Path.Combine(rootFolderPath, 
                "BrowserExtensions", 
                "chrome",
                extensionName
                );
        }
    }

}
