using Chameleon.Interfaces.UserProfiles;
using Microsoft.Playwright;

namespace Chameleon.SystemBrowser.Addons;

public static class ProxyAddonUtil
{
    public const string AutoProxyFolderName = "ChameleonAutoExt";
    public const string FirefoxAutoProxyAddonName = "autoproxy.chameleon.zip";

    public const string UrlSchemeEnd = "://";
    public const string HTTPSScheme = "https://";
    public const string DomainLevelDelimiter = ".";

    public static string ProxyExtDir(string browserProfileFolderPath) =>
        Path.Combine(browserProfileFolderPath, AutoProxyFolderName);

    public static bool ServerPortDelimiter(string starturl) => starturl.Contains(DomainLevelDelimiter);

    public static string GetManifest() => """
    {
        "manifest_version": 2,
        "name": "Chameleon Auto Proxy",
        "description": "A Chameleon addon to set proxy username and password.",
        "version": "1.0.0",
        "permissions": [
            "proxy",
            "storage",
            "webRequest",
            "webRequestBlocking",
            "webRequestAuthProvider",
            "<all_urls>"
        ],
        "background": {
            "scripts": ["background.js"]
        },
        "browser_specific_settings": {
            "gecko": {
                "id": "autoproxy@chameleonmode.com",
                "strict_min_version": "42.0"
            }
        }
    }
    """;
    public static string GetManifestv3() => """
    {
      "manifest_version": 3,
      "name": "Chameleon Auto Proxy",
      "version": "1.0.0",
      "permissions": [
        "webRequest",
        "webRequestBlocking",
        "webRequestAuthProvider",
        "<all_urls>"
      ],
      "host_permissions": [
        "<all_urls>"
      ],
      "background": {
        "service_worker": "background.js"
      }
    }
    """;

    public static string GetBgJsv3(IProxySettings proxy) => """
    chrome.webRequest.onAuthRequired.addListener((details) => 
    {
        return { 
            authCredentials: {
    """
                + "username:" + $"\"{proxy.UserName}\","
                + "password: " + $"\"{proxy.Password}\"" +
   """
            }
        };
    }, 
    { urls: ['<all_urls>'] }, ['blocking']);
    
    chrome.tabs.reload();
    """;

    public static string GetBgJs(string loadUrl, IProxySettings proxy) => """
        browser.webRequest.onAuthRequired.addListener((details) => {
            return {
                authCredentials: {
        """
                    + $"username: \"{proxy.UserName}\","
                    + $"password: \"{proxy.Password}\"" +
        """
                    }
                };
            },
            { urls: ['<all_urls>'] },
            ['blocking']
        );
        const proxyConfig = {
                proxyType: "manual",
        """
              + $"http: \"{proxy.Server}\"," +
        """       
                httpProxyAll : true,
                autoLogin: false
            };
        browser.proxy.settings.set(
            { value: proxyConfig, scope: 'regular' }
        """
        + loadUrl;

}
