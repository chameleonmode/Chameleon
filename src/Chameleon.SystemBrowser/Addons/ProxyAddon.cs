using System;

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
        "tabs",
        "webRequest",
        "webRequestAuthProvider"
      ],
      "host_permissions": [
        "<all_urls>"
      ],
      "background": {
        "service_worker": "background.js"
      }
    }
    """;

    public static string GetBgJsv3(string loadUrl, IProxySettings proxy) =>
    $@"
    function delay(ms) {{
        return new Promise(resolve => setTimeout(resolve, ms));
    }}

    chrome.webRequest.onAuthRequired.addListener((details) => {{
        return {{ 
            authCredentials: {{
               username: ""{proxy.UserName}"",
               password: ""{proxy.Password}""
            }}
        }};
    }}, {{urls: ['<all_urls>'] }}, ['blocking']);
   (async function(){{
    await chrome.tabs.update({{ url:""{loadUrl}"" }}); 
     let tabs = await chrome.tabs.query({{}});
     for(let i = 0; i < tabs.length; i++) {{
        await chrome.tabs.reload(tabs[i].id);
     }}
    }})();
    ";
    //+ $@"
    //    function getTabInfo(callback) {{
    //        chrome.tabs.query({{ }}, callback);
    //    }}

    //   function processTabInfo(tabs) {{
    //        if (tabs.length > 1) {{
    //             chrome.tabs.remove(tabs[tabs.length - 1].id);
    //         }}
    //         // Update the current tab with a new URL
    //         //chrome.tabs.update(tabs[tabs.length - 1].id, {{ url: ""{url}"" }});
    //         chrome.tabs.update({{ url:""{url}"" }});
    //    }}

    //    // Call the function to get tab information
    //    getTabInfo(processTabInfo);
    //";

    //let queryOptions = {{ active: true, lastFocusedWindow: true }};
    //chrome.tabs.query(queryOptions, ([tab]) => {{
    //  if (chrome.runtime.lastError)
    //  console.error(chrome.runtime.lastError);
    //  // `tab` will either be a `tabs.Tab` instance or `undefined`.
    //  chrome.tabs.update(tab.id, {{active: true, url: ""{url}"" }});
    //}});

    public static string GetBgJs(string loadUrl, IProxySettings proxy) => 
    $@"
        browser.webRequest.onAuthRequired.addListener((details) => {{
            return {{
                authCredentials: {{ 
                    username: ""{proxy.UserName}"", 
                    password: ""{proxy.Password}"" 
                }}
            }};
        }},
        {{ urls: ['<all_urls>'] }}, ['blocking']);

        const proxyConfig = {{
                proxyType: ""manual"",
                http: ""{proxy.Server}"",
                httpProxyAll : true,
                autoLogin: false
        }};
        browser.proxy.settings.set({{ value: proxyConfig, scope: 'regular' }}, 
            async () => {{ 
                let tabs = await browser.tabs.query({{}});
                if (tabs.length > 1) {{
                    await browser.tabs.remove(tabs[tabs.length - 1].id);
                }}
                browser.tabs.update({{ url:""{loadUrl}"" }}); 
        }});
    ";

}
