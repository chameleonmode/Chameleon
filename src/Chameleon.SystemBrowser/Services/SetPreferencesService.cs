using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Chameleon.SystemBrowser.Services
{
    public class SetPreferencesService : ISetPreferencesService
    {
        private void SetWebRTC(JObject values)
        {
            const string disableNonProxiedUDP = "disable_non_proxied_udp";
            const string preferenceName = "webrtc";
            
            var webrtc = JToken.FromObject(new { ip_handling_policy = disableNonProxiedUDP });

            if (values.ContainsKey(preferenceName))
            {
                values[preferenceName] = webrtc;
            }
            else
            {
                values.Add(preferenceName, webrtc);
            }            
        }

        private void SetTracking(JObject values, bool tracing)
        {
            const string preferenceName = "enable_do_not_track";
            bool doNotTrack = !tracing;

            if (values.ContainsKey(preferenceName))
            {
                values[preferenceName] = doNotTrack;
            }
            else
            {
                values.Add(preferenceName, doNotTrack);
            }            
        }

        private string GetPathToPreferencesFile(string browserProfileFolderPath)
        {
            string PathToPreferencesDirectory = Path.Combine(browserProfileFolderPath, "Default");

            if (!Directory.Exists(PathToPreferencesDirectory))
            {
                Directory.CreateDirectory(PathToPreferencesDirectory);
            }

            return Path.Combine(PathToPreferencesDirectory, "Preferences");
        }
        private JObject GetChromiumPreferencesFromFile(string pathToPreferencesFile)
        {
            string json = "{}";

            if (File.Exists(pathToPreferencesFile))
            {
                json = File.ReadAllText(pathToPreferencesFile);
            }

            return JObject.Parse(json);
        }

        private void WriteChromiumPreferencesToFile(string pathToPreferencesFile, JObject values)
        {
            File.WriteAllText(pathToPreferencesFile, values.ToString());
        }

        private void SetChromiumPreferences(IWebBrowserSettings webBrowserSettings, string browserProfileFolderPath, SystemBrowserType browserType)
        {
            var pathToFile = GetPathToPreferencesFile(browserProfileFolderPath);
            var values = GetChromiumPreferencesFromFile(pathToFile);
            
            SetTracking(values, webBrowserSettings.Tracking);
            
            if (browserType == SystemBrowserType.Brave)
            {
                SetWebRTC(values);
            }

            WriteChromiumPreferencesToFile(pathToFile, values);
        }

        public void SetPreferences(IWebBrowserSettings webBrowserSettings, string browserProfileFolderPath, SystemBrowserType browserType)
        {
            if (browserType == SystemBrowserType.Chrome || browserType == SystemBrowserType.Brave)
            {
                SetChromiumPreferences(webBrowserSettings, browserProfileFolderPath, browserType);
            }
        }
    }
}
