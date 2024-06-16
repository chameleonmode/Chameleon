using Chameleon.Interfaces.UserProfiles;
using Microsoft.Playwright;

namespace Chameleon.SystemBrowser.Addons;

public static class ProxyAddonUtil
{
    public const string FirefoxAutoProxyFolderName = "ChameleonAutoExt";
    public const string FirefoxAutoProxyAddonName = "autoproxy.chameleon.zip";

    public const string UrlSchemeEnd = "://";
    public const string HTTPSScheme = "https://";
    public const string DomainLevelDelimiter = ".";

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
    //$@"
    //browser.webRequest.onAuthRequired.addListener((details) => {{
    //    return {{
    //        authCredentials: {{ 
    //            username: ""{proxy.UserName}"",
    //            password: ""{proxy.Password}""
    //        }}
    //    }};
    //}},
    //{{ urls: ['<all_urls>'] }},
    //['blocking']
    //);

    //const proxyConfig = {{
    //    proxyType: 'manual',
    //    http: '{proxy.Server}',
    //    httpProxyAll: true,
    //    autoLogin: false
    //}};

    //browser.proxy.settings.set({{ value: proxyConfig, scope: 'regular' }}" + loadUrl;


    //bool needLoadUrl = startUrl.Contains(DomainLevelDelimiter);

    //string thisStartUrl = startUrl.Contains(UrlSchemeEnd)
    //    ? startUrl
    //    : $"{HTTPSScheme}{startUrl}";

    //string loadUrl =
    //    needLoadUrl ? $", () => {{ browser.tabs.update({{ url:\"{startUrl}\" }}); }});"
    //: ");";

    //// Define the proxy details (example values)
    //return

    //return """
    //            browser.webRequest.onAuthRequired.addListener((details) => {
    //                return {
    //                    authCredentials: { 
    //       """
    //                + $"username: \"{proxy.UserName}\","
    //                + $"password: \"{proxy.Password}\"" +
    //        """
    //                    }
    //                };
    //            },
    //            { urls: ['<all_urls>'] },
    //            ['blocking']
    //        );
    //        const proxyConfig = {
    //                proxyType: "manual",
    //        """
    //      + $"http: \"{proxy.Server}\"," +
    //"""       
    //                httpProxyAll : true,
    //                autoLogin: false
    //            };
    //        browser.proxy.settings.set(
    //            { value: proxyConfig, scope: 'regular' }
    //        """
    //        + loadUrl;

}
