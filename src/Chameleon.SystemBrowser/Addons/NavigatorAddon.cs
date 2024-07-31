using Chameleon.ThirdParty.GeoIp;
using Chameleon.ThirdParty.GeoIp.Models;
using Newtonsoft.Json;

namespace Chameleon.SystemBrowser.Addons;

public static partial class NavigatorAddon
{
    public static async Task InitializeExtension(string dir, Geoiplookup ipLookup)
    {
        await IOtil.DC(dir);

        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "manifest.json"), GetManifestv2);

        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "background.js"), SetBackgroundo());
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "injector.js"), SetInjecto(ipLookup));
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "content.js"), SetContnto());

    }

    public static string GetManifestv2 => """
    {
        "version": "1.0.0",
        "manifest_version": 2,
        "description": "Chameleon browser window and document navigator spoofer and sync",
        "name": "Chameleonair",
        "background": {
            "scripts": [
                 "background.js"
            ]
        },
        "permissions": [
          "<all_urls>",
          "alarms",
          "contextMenus",
          "notifications",
          "storage",
          "tabs",
          "webRequest",
          "webRequestBlocking",
          "http://*/*",
          "https://*/*"
        ],
        "optional_permissions": [
          "privacy"
        ],
        "content_scripts": [
            {
              "matches": ["http://*/*", "https://*/*"],
              "all_frames": true,
              "js": ["injector.js"],
              "run_at": "document_start"
            }
        ],
        "content_security_policy": "script-src 'self' 'unsafe-eval'; object-src 'self'"
    }
    """;

    public static string SetContnto()
    { 
        return $@"
        // content.js
        chrome.runtime.sendMessage({{message: ""contentScriptLoaded""}});
        ";
    }

//CHAMELEON_SPOOF
//timezone: {{ zone: {{ name: 'America/New_York' }} }},
//language: 'en-US',
//userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36',
//cpuClass: 'x86',
//hardwareConcurrency: 8,
//deviceMemory: 16,
//maxTouchPoints: 0,
//vendor: 'Google Inc.',
//appVersion: '5.0 (Windows)'

//injectionProperties
//{{ obj: 'window.navigator', prop: 'appCodeName', value: 'Mozilla' }},
//{{ obj: 'window.navigator', prop: 'appName', value: 'Netscape' }},
//{{ obj: 'window.navigator', prop: 'appVersion', value: '5.0 (Windows)' }},
//{{ obj: 'window.navigator', prop: 'userAgent', value: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36' }},
//{{ obj: 'window.navigator', prop: 'productSub', value: '20030107' }},
//{{ obj: 'window.navigator', prop: 'vendor', value: 'Google Inc.' }},
//{{ obj: 'window.navigator', prop: 'hardwareConcurrency', value: 8 }},
//{{ obj: 'window.navigator', prop: 'deviceMemory', value: 16 }},
//{{ obj: 'window.navigator', prop: 'maxTouchPoints', value: 0 }},
//{{ obj: 'window.navigator', prop: 'language', value: 'en-US' }},
//{{ obj: 'window.navigator', prop: 'languages', value: ['en-US', 'en'] }},
//{{ obj: 'window.navigator', prop: 'cpuClass', value: 'x86' }},
//{{ obj: 'window.navigator', prop: 'mimeTypes', value: [
//    {{ type: 'application/pdf', suffixes: 'pdf', description: 'Portable Document Format' }}
//] }},
//{{ obj: 'window.navigator', prop: 'plugins', value: [
//    {{ name: 'Chrome PDF Plugin', filename: 'internal-pdf-viewer', description: 'Portable Document Format' }}
//] }},
//{{ 
//    obj: 'window.navigator', 
//    prop: 'userAgentData', 
//    value: {{
//        brands: [
//            {{ brand: 'Not/A)Brand','version':'8.0.0.0'}},
//            {{ brand: 'Chromium', version: '126' }},
//            {{ brand: 'Google Chrome', version: '126' }}
//        ],
//        mobile: false,
//        platform: '{(AddonsUtil.IMac ? "macOS" : "Windows")}'
//    }} 
//}}
    public static string SetInjecto(Geoiplookup? ipLookup) 
    {
        string randObjName = RemoveNumbersRegex().Replace(Guid.NewGuid().ToString().Replace("-", ""), "");

        string os = $@"""platform"": '{(!AddonsUtil.IMac ? "Win32" : "MacIntel")}'";
        string tz = "";
        if (ipLookup != null)
        {
            var abbrs = GeoIpApi.Instance.GetAbbrs(ipLookup.timezone);
            var untils = GeoIpApi.Instance.GetUntilInstants(ipLookup.timezone);
            var offsets = GeoIpApi.Instance.GetOffsets(ipLookup.timezone);
            tz = $@", 
                ""timezone"": {{ 
                    ""locale"":'{(ipLookup.languages.Contains(',') ? ipLookup.languages.Remove(ipLookup.languages.IndexOf(',')) : ipLookup.languages)}', 
                    ""zone"": {{ 
                        ""name"": '{ipLookup.timezone}' ,
                        ""abbrs"": [ {string.Join(", ", abbrs) } ],                       
                        ""untils"": [ {string.Join(", ", untils) } ],                       
                        ""offsets"": [ {string.Join(", ", offsets)} ]
                    }} 
                }}";
        }

        string cs = $@"CHAMELEON_SPOOF.set(spoofContext, {{ {os}{tz} }});";
        return $@"
        // injector.js
class Injector {{
    constructor() {{
        this.spoof = {{
            custom: '',
            overwrite: [],
            metadata: {{}},
        }};
        this.enabled = true;
        this.randObjName = ""{randObjName}"";
    }}

    injectIntoPage() {{
        const code = this.finalOutput();
        let scriptEl = Object.assign(document.createElement('script'), {{
            textContent: code,
            id: 'chameleon',
        }});
        document.documentElement.appendChild(scriptEl);
        scriptEl.remove();

        scriptEl = document.createElement('script');
        scriptEl.src = URL.createObjectURL(new Blob([code], {{ type: 'text/javascript' }}));
        (document.head || document.documentElement).appendChild(scriptEl);
        try {{
            URL.revokeObjectURL(scriptEl.src);
        }} catch (e) {{ }}
        scriptEl.remove();
    }}

    finalOutput() {{
        return `(function(){{
        const inject = async (spoofContext) => {{
        if (spoofContext.CHAMELEON_SPOOF) return;
        spoofContext.CHAMELEON_SPOOF = ""CHAMELEON_SPOOF"";

        let CHAMELEON_SPOOF = new WeakMap();
        {cs}

        let ORIGINAL_INTL = spoofContext.Intl.DateTimeFormat;
        let ORIGINAL_INTL_PROTO = spoofContext.Intl.DateTimeFormat.prototype;
        let _supportedLocalesOfDTF = spoofContext.Intl.DateTimeFormat.supportedLocalesOf;
        let _supportedLocalesOfRTF = spoofContext.Intl.RelativeTimeFormat.supportedLocalesOf;
        let _supportedLocalesOfLF = spoofContext.Intl.ListFormat.supportedLocalesOf;
        let _supportedLocalesOfNF = spoofContext.Intl.NumberFormat.supportedLocalesOf;
        let _supportedLocalesOfPR = spoofContext.Intl.PluralRules.supportedLocalesOf;
        let _supportedLocalesOfC = spoofContext.Intl.Collator.supportedLocalesOf;
        let _open = spoofContext.open;

        let _enumerateDevices;
        if (spoofContext.navigator.mediaDevices && spoofContext === spoofContext.parent) {{
          _enumerateDevices = spoofContext.navigator.mediaDevices.enumerateDevices.bind(spoofContext.navigator.mediaDevices);
        }}

        let uad;
        if (navigator.userAgentData && typeof navigator.userAgentData.getHighEntropyValues === 'function') {{
            try {{
                uad = await navigator.userAgentData.getHighEntropyValues([
                    'platform', 'platformVersion', 'architecture', 'model', 'uaFullVersion', 'bitness', 'wow64', 'fullVersionList'
                ]);
            }} catch (error) {{
                
            }}
        }}
    
        let modifiedAPIs = [];

        let injectionProperties = 
        [
            {{ 
                obj: 'window.navigator', 
                prop: 'platform', 
                value: '{(!AddonsUtil.IMac ? "Win32" : "MacIntel")}' 
            }},
            {{ 
                obj: 'window.navigator', 
                prop: 'vendorSub', 
                value: '' 
            }},
            {{ 
                obj: 'window.navigator', 
                prop: 'oscpu', 
                value: 'undefined'
            }},
            {{ 
                obj: 'window.navigator', 
                prop: 'userAgentData', 
                value: {{
                    brands: window.navigator.userAgentData.brands,
                    mobile: window.navigator.userAgentData.mobile,
                    platformVersion: uad && uad.platformVersion,
                    architecture: uad && uad.architecture,
                    bitness: uad && uad.bitness,
                    wow64: uad && uad.wow64,
                    model: uad && uad.model,
                    uaFullVersion: uad && uad.uaFullVersion,
                    fullVersionList: uad && uad.fullVersionList,
                    platform: '{(!AddonsUtil.IMac ? "Windows" : "macOS")}'
                }} 
            }}
        ];
    
    
        injectionProperties.forEach(injProp => {{
          if (injProp.obj === 'window') {{
            Object.defineProperty(spoofContext, injProp.prop, {{
              get: (() => injProp.value).bind(null)
            }});
          }} else if (injProp.obj === 'window.navigator' && injProp.value === null) {{
            delete spoofContext.navigator.__proto__[injProp.prop];
          }} else if (injProp.obj === 'window.navigator' && injProp.prop == 'mimeTypes') {{
            let mimes = (() => {{
              const mimeArray = {{}}
              injProp.value.forEach((m, i) => {{
                function FakeMimeType () {{ return m }}
                const mime = new FakeMimeType()
                Object.setPrototypeOf(mime, MimeType.prototype);
                Object.defineProperty(mimeArray, i, {{
                  configurable: false,
                  enumerable: true,
                  value: mime
                }});
                Object.defineProperty(mimeArray, m.type, {{
                  configurable: false,
                  enumerable: false,
                  value: mime
                }});
              }})
              Object.setPrototypeOf(mimeArray, MimeTypeArray.prototype);
              Object.defineProperty(mimeArray, 'length', {{
                configurable: false,
                enumerable: true,
                value: injProp.value.length
              }});
              Object.defineProperty(mimeArray, 'item', {{
                configurable: false,
                enumerable: true,
                value: function item() {{
                  return this[arguments[0]] || null;
                }}
              }});
              Object.defineProperty(mimeArray, 'namedItem', {{
                configurable: false,
                enumerable: true,
                value: function namedItem() {{
                  return this[arguments[0]] || null;
                }}
              }});
              return mimeArray
            }})();
            Object.defineProperty(spoofContext.navigator, 'mimeTypes', {{
              configurable: true,
              value: mimes
            }});
          }} else if (injProp.obj === 'window.navigator' && injProp.prop == 'plugins') {{
            let plugins = (() => {{
              const pluginArray = {{}};
              injProp.value.forEach((p, i) => {{
                function FakePlugin () {{ return p }}
                const plugin = new FakePlugin();
                Object.setPrototypeOf(plugin, Plugin.prototype);
                Object.defineProperty(plugin, 'length', {{
                  configurable: false,
                  enumerable: true,
                  value: p.__mimeTypes.length
                }});
                Object.defineProperty(plugin, 'version', {{
                  configurable: false,
                  enumerable: false,
                  value: undefined
                }});
                Object.defineProperty(plugin, 'item', {{
                  configurable: false,
                  enumerable: true,
                  value: function item() {{
                    return this[arguments[0]] || null;
                  }}
                }});
                Object.defineProperty(plugin, 'namedItem', {{
                  configurable: false,
                  enumerable: true,
                  value: function namedItem() {{
                    return this[arguments[0]] || null;
                  }}
                }});

                // iterate mime types
                for (let j = 0; j < p.__mimeTypes.length; j++) {{
                  Object.defineProperty(plugin, j, {{
                    configurable: false,
                    enumerable: true,
                    value: navigator.mimeTypes[p.__mimeTypes[j]]
                  }});

                  Object.defineProperty(plugin, p.__mimeTypes[j], {{
                    configurable: false,
                    enumerable: false,
                    value: navigator.mimeTypes[p.__mimeTypes[j]]
                  }});
                }}

                delete p.__mimeTypes;

                Object.defineProperty(pluginArray, i, {{
                  configurable: false,
                  enumerable: true,
                  value: p
                }});

                Object.defineProperty(pluginArray, p.name, {{
                  configurable: false,
                  enumerable: false,
                  value: p
                }});
              }})
              Object.defineProperty(pluginArray, 'length', {{
                configurable: false,
                enumerable: true,
                value: injProp.value.length
              }});
              Object.defineProperty(pluginArray, 'item', {{
                configurable: false,
                enumerable: true,
                value: function item() {{
                  return this[arguments[0]] || null;
                }}
              }});
              Object.defineProperty(pluginArray, 'namedItem', {{
                configurable: false,
                enumerable: true,
                value: function namedItem() {{
                  return this[arguments[0]] || null;
                }}
              }});
              Object.defineProperty(pluginArray, 'refresh', {{
                configurable: false,
                enumerable: true,
                value: function refresh() {{
                  return;
                }}
              }});

              pluginArray[Symbol.iterator] = function() {{
                const numPlugins = Object.keys(this).length - 4;
                let index = 0;

                return {{
                  next: () => {{
                    if (index < numPlugins) {{
                      const value = this[index];
                      index++;
                      return {{
                        value,
                        done: false
                      }};
                    }}
                    return {{
                      value: undefined,
                      done: true
                    }};
                  }}
                }};
              }};

              return pluginArray;
            }})();
            Object.defineProperty(spoofContext.navigator, 'plugins', {{
              configurable: true,
              value: plugins
            }});

            let pluginsArray = Array.from(navigator.plugins);

            // iterate mimetypes to add enabledPlugin property
            for (let i = 0; i < navigator.mimeTypes.length; i++) {{
              let p = pluginsArray.find(p => p[navigator.mimeTypes[i].type] != undefined);
              Object.defineProperty(navigator.mimeTypes[i], 'enabledPlugin', {{
                configurable: false,
                enumerable: true,
                value: p
              }});
            }}
          }} else {{
            let tmpObj = injProp.obj.split('.').reduce((p,c)=>p&&p[c]||null, spoofContext);

            if (tmpObj[injProp.prop] != injProp.value) {{
              Object.defineProperty(tmpObj, injProp.prop, {{
                configurable: true,
                value: injProp.value
              }});
            }}
          }}
        }});

        (() => {{
            if (new Date()[spoofContext.CHAMELEON_SPOOF]) {{
              spoofContext.Date = Date;
              return;
            }}

            let ORIGINAL_DATE = spoofContext.Date;
            
            const {{
              getDate, getDay, getFullYear, getHours, getMinutes, getMonth, getTime, getTimezoneOffset,
              setDate, setFullYear, setHours, setMinutes, setMilliseconds, setMonth, setSeconds,
              setTime, toDateString, toLocaleString, toLocaleDateString, toLocaleTimeString, toTimeString
            }} = ORIGINAL_DATE.prototype;

            const TZ_LONG_A = new ORIGINAL_DATE(2020, 0, 1).toLocaleDateString(undefined, {{ timeZoneName: 'long' }}).split(', ')[1];
            const TZ_LONG_B = new ORIGINAL_DATE(2020, 6, 1).toLocaleDateString(undefined, {{ timeZoneName: 'long' }}).split(', ')[1];
            const TZ_SHORT_A = new ORIGINAL_DATE(2020, 0, 1).toLocaleDateString(undefined, {{ timeZoneName: 'short' }}).split(', ')[1];
            const TZ_SHORT_B = new ORIGINAL_DATE(2020, 6, 1).toLocaleDateString(undefined, {{ timeZoneName: 'short' }}).split(', ')[1];
            const TZ_INTL = ORIGINAL_INTL('en-us', {{ timeZone: CHAMELEON_SPOOF.get(spoofContext).timezone.zone.name, timeZoneName: 'long'}});
            const TZ_LOCALE_STRING = ORIGINAL_INTL('en-us', {{
              timeZone: CHAMELEON_SPOOF.get(spoofContext).timezone.zone.name,
              year: 'numeric',
              month: 'numeric',
              day: 'numeric',
              hour: 'numeric',
              minute: 'numeric',
              second: 'numeric'
            }});
            const TZ_DIFF = 3 * 60 * 60 * 1000;

            const modifyDate = (d) => {{
                let timestamp = getTime.call(d);
console.log('timestamp', timestamp);
                let spoofData = CHAMELEON_SPOOF.get(spoofContext).timezone;
console.log('spoofData', spoofData);
                let offsetIndex = spoofData.zone.untils.findIndex(o => o === null || (timestamp < o) );
console.log('offsetIndex', offsetIndex);
                let offsetNum = spoofData.zone.offsets[offsetIndex];
console.log('offsetNum', offsetNum);

                //let offsetStr = \`\${{offsetNum < 0 ? '+' : '-' }}\${{String(Math.abs(offsetNum / 60)).padStart(2, '0')}}\${{String(offsetNum % 60).padStart(2, '0')}}\`;
                //const sign = offsetNum < 0 ? '+' : '-';
                //const hours = String(Math.abs(Math.floor(offsetNum / 60))).padStart(2, '0');
                //const minutes = String(Math.abs(offsetNum % 60)).padStart(2, '0');
                let offsetStr = \`\${{offsetNum}}\`;
console.log('offsetStr', offsetStr);
                let tzName = TZ_INTL.format(d).split(', ')[1];

                let tmp = new ORIGINAL_DATE(TZ_LOCALE_STRING.format(d));

                d[spoofContext.CHAMELEON_SPOOF] = {{
                  date: tmp,
                  zoneInfo_offsetNum: offsetNum,
                  zoneInfo_offsetStr: offsetStr,
                  zoneInfo_tzAbbr: spoofData.zone.abbrs[offsetIndex],
                  zoneInfo_tzName: tzName
                }};
            }}

            const replaceName = (d, name) => {{
              d = d.replace(TZ_LONG_A, name);
              d = d.replace(TZ_LONG_B, name);
              d = d.replace(TZ_SHORT_A, name);
              d = d.replace(TZ_SHORT_B, name);

              return d;
            }}

            spoofContext.Date = function() {{
              'use strict';

              let tmp = new ORIGINAL_DATE(...arguments);
              let timestamp = getTime.call(tmp);

              if (isNaN(timestamp)) {{
                return tmp;
              }}
              
              modifyDate(tmp);
              
              return (this instanceof Date) ? tmp : tmp.toString();
            }};

            Object.defineProperty(spoofContext.Date, 'length', {{
              configurable: false,
              value: 7
            }})
            
            spoofContext.Date.prototype = ORIGINAL_DATE.prototype;
            spoofContext.Date.UTC = ORIGINAL_DATE.UTC;
            spoofContext.Date.now = ORIGINAL_DATE.now;
            spoofContext.Date.parse = ORIGINAL_DATE.parse;

            spoofContext.Date.prototype.getDate = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              return getDate.call(this[spoofContext.CHAMELEON_SPOOF].date);
            }}
            spoofContext.Date.prototype.getDay = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              return getDay.call(this[spoofContext.CHAMELEON_SPOOF].date);
            }}
            spoofContext.Date.prototype.getFullYear = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              return getFullYear.call(this[spoofContext.CHAMELEON_SPOOF].date);
            }}
            spoofContext.Date.prototype.getHours = function(){{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              return getHours.call(this[spoofContext.CHAMELEON_SPOOF].date);
            }}
            spoofContext.Date.prototype.getMinutes = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              return getMinutes.call(this[spoofContext.CHAMELEON_SPOOF].date);
            }}
            spoofContext.Date.prototype.getMonth = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              return getMonth.call(this[spoofContext.CHAMELEON_SPOOF].date);
            }}
            spoofContext.Date.prototype.getTimezoneOffset = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              return this[spoofContext.CHAMELEON_SPOOF].zoneInfo_offsetNum;
            }}
            spoofContext.Date.prototype.setDate = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              let nd = setDate.apply(this, arguments);
              if (isNaN(nd)) {{
                return ""Invalid Date"";
              }}

              modifyDate(this);
              
              return nd;
            }}
            spoofContext.Date.prototype.setFullYear = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              let nd = setFullYear.apply(this, arguments);
              if (isNaN(nd)) {{
                return ""Invalid Date"";
              }}

              modifyDate(this);

              return nd;
            }}
            spoofContext.Date.prototype.setHours = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              let nd = setHours.apply(this, arguments);
              if (isNaN(nd)) {{
                return ""Invalid Date"";
              }}

              modifyDate(this);

              return nd;
            }}
            spoofContext.Date.prototype.setMilliseconds = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              let nd = setMilliseconds.apply(this, arguments);
              if (isNaN(nd)) {{
                return ""Invalid Date"";
              }}

              modifyDate(this);

              return nd;
            }}
            spoofContext.Date.prototype.setMonth = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              let nd = setMonth.apply(this, arguments);
              if (isNaN(nd)) {{
                return ""Invalid Date"";
              }}

              modifyDate(this);

              return nd;
            }}
            spoofContext.Date.prototype.setSeconds = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              let nd = setSeconds.apply(this, arguments);
              if (isNaN(nd)) {{
                return ""Invalid Date"";
              }}

              modifyDate(this);

              return nd;
            }}
            spoofContext.Date.prototype.setTime = function() {{
              if (isNaN(getTime.call(this))) {{
                return NaN;
              }}

              let nd = setTime.apply(this, arguments);
              if (isNaN(nd)) {{
                return ""Invalid Date"";
              }}

              modifyDate(this);

              return nd;
            }}
            spoofContext.Date.prototype.toDateString = function() {{    
              if (isNaN(getTime.call(this))) {{
                return ""Invalid Date"";
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              return toDateString.apply(this[spoofContext.CHAMELEON_SPOOF].date);
            }}
            spoofContext.Date.prototype.toString = function() {{    
              if (isNaN(getTime.call(this))) {{
                return ""Invalid Date"";
              }}

              return this.toDateString() + ' ' + this.toTimeString();
            }}
            spoofContext.Date.prototype.toTimeString = function() {{    
              if (isNaN(getTime.call(this))) {{
                return ""Invalid Date"";
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              let parts = toTimeString.apply(this[spoofContext.CHAMELEON_SPOOF].date).split(' ', 1);

              // fix string formatting for negative timestamp
              let tzName;

              if (getTime.call(this) >= 0) {{
                tzName = \`(\${{this[spoofContext.CHAMELEON_SPOOF].zoneInfo_tzName}})\`;
              }} else {{
                tzName = ""("" + TZ_LONG_A + "")"";
              }}

              parts = parts.concat(['GMT' + this[spoofContext.CHAMELEON_SPOOF].zoneInfo_offsetStr, tzName]);

              return parts.join(' ');
            }}
            spoofContext.Date.prototype.toJSON = function() {{
              if (isNaN(getTime.call(this))) {{
                return null;
              }}
              return this.toISOString();
            }}
            spoofContext.Date.prototype.toLocaleString = function() {{
              if (isNaN(getTime.call(this))) {{
                return ""Invalid Date"";
              }}
              
              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              let tmp = toLocaleString.apply(this[spoofContext.CHAMELEON_SPOOF].date, arguments);

              return replaceName(tmp, this[spoofContext.CHAMELEON_SPOOF].zoneInfo_tzName);
            }}
            spoofContext.Date.prototype.toLocaleDateString = function() {{
              if (isNaN(getTime.call(this))) {{
                return ""Invalid Date"";
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              let tmp = toLocaleDateString.apply(this[spoofContext.CHAMELEON_SPOOF].date, arguments);
              
              return replaceName(tmp, this[spoofContext.CHAMELEON_SPOOF].zoneInfo_tzName);
            }}
            spoofContext.Date.prototype.toLocaleTimeString = function() {{
              if (isNaN(getTime.call(this))) {{
                return ""Invalid Date"";
              }}

              if (!this[spoofContext.CHAMELEON_SPOOF]) modifyDate(this);

              let tmp = toLocaleTimeString.apply(this[spoofContext.CHAMELEON_SPOOF].date, arguments);
              
              return replaceName(tmp, this[spoofContext.CHAMELEON_SPOOF].zoneInfo_tzName);
            }}

            modifiedAPIs = modifiedAPIs.concat([
              [spoofContext.Date, ""Date""],
              [spoofContext.Date.prototype.getDate, ""getDate""],
              [spoofContext.Date.prototype.getDay,  ""getDay""],
              [spoofContext.Date.prototype.getFullYear, ""getFullYear""],
              [spoofContext.Date.prototype.getHours, ""getHours""],
              [spoofContext.Date.prototype.getMinutes, ""getMinutes""],
              [spoofContext.Date.prototype.getMonth, ""getMonth""],
              [spoofContext.Date.prototype.getTimezoneOffset, ""getTimezoneOffset""],
              [spoofContext.Date.prototype.setDate, ""setDate""],
              [spoofContext.Date.prototype.setFullYear, ""setFullYear""],
              [spoofContext.Date.prototype.setHours, ""setHours""],
              [spoofContext.Date.prototype.setMilliseconds, ""setMilliseconds""],
              [spoofContext.Date.prototype.setMonth, ""setMonth""],
              [spoofContext.Date.prototype.setSeconds, ""setSeconds""],
              [spoofContext.Date.prototype.setTime, ""setTime""],
              [spoofContext.Date.prototype.toDateString, ""toDateString""],
              [spoofContext.Date.prototype.toString, ""toString""],
              [spoofContext.Date.prototype.toTimeString, ""toTimeString""],
              [spoofContext.Date.prototype.toJSON, ""toJSON""],
              [spoofContext.Date.prototype.toLocaleString, ""toLocaleString""],
              [spoofContext.Date.prototype.toLocaleDateString, ""toLocaleDateString""],
              [spoofContext.Date.prototype.toLocaleTimeString, ""toLocaleTimeString""],
            ]);
        }})();

         
        spoofContext.Intl.DateTimeFormat = function(...args) {{
            let locale = spoofContext.navigator.language || ""en-US"";
            
            if (CHAMELEON_SPOOF.has(spoofContext)) {{
              if (CHAMELEON_SPOOF.get(spoofContext).timezone) {{
                let spoofData = Object.assign({{}}, CHAMELEON_SPOOF.get(spoofContext).timezone);

                if (args.length == 2) {{
                  if (!args[1].timeZone) {{
                    args[1].timeZone = spoofData.zone.name;
                  }}
                }} else if (args.length == 1) {{
                  args.push({{
                    timeZone: spoofData.zone.name
                  }});
                }} else {{
                  args = [
                    locale,
                    {{ timeZone: spoofData.zone.name }}
                  ];
                }}
              }} else if (CHAMELEON_SPOOF.get(spoofContext).language) {{
                if (args.length == 0 || !args[0]) {{
                  args[0] = locale;
                }}
              }}
            }}

            return new (Function.prototype.bind.apply(ORIGINAL_INTL, [null].concat(args)));
          }}

          modifiedAPIs.push([
            spoofContext.Intl.DateTimeFormat, ""DateTimeFormat""
          ]);

          spoofContext.Intl.DateTimeFormat.prototype = ORIGINAL_INTL_PROTO;
          spoofContext.Intl.DateTimeFormat.supportedLocalesOf = _supportedLocalesOfDTF;
          spoofContext.Intl.RelativeTimeFormat.supportedLocalesOf = _supportedLocalesOfRTF;
          spoofContext.Intl.NumberFormat.supportedLocalesOf = _supportedLocalesOfNF;
          spoofContext.Intl.PluralRules.supportedLocalesOf = _supportedLocalesOfPR;
          spoofContext.Intl.ListFormat.supportedLocalesOf = _supportedLocalesOfLF;
          spoofContext.Intl.Collator.supportedLocalesOf = _supportedLocalesOfC;

          spoofContext.open = function(){{
            let w;
            if (arguments.length) {{
              w = _open.call(this, ...arguments);
            }} else {{
              w = _open.call(this);
            }}

            if (w) {{
              Object.defineProperty(w, 'Date', {{
                value: spoofContext.Date
              }});
  
              Object.defineProperty(w.Intl, 'DateTimeFormat', {{
                value: spoofContext.Intl.DateTimeFormat
              }});
  
              Object.defineProperty(w, 'screen', {{
                value: spoofContext.screen
              }});
  
              Object.defineProperty(w, 'navigator', {{
                value: spoofContext.navigator
              }});
  
              Object.defineProperty(w.Element.prototype, 'getBoundingClientRect', {{
                value: spoofContext.Element.prototype.getBoundingClientRect
              }});
  
              Object.defineProperty(w.Element.prototype, 'getClientRects', {{
                value: spoofContext.Element.prototype.getClientRects
              }});
  
              Object.defineProperty(w.Range.prototype, 'getBoundingClientRect', {{
                value: spoofContext.Range.prototype.getClientRects
              }});
  
              Object.defineProperty(w.Range.prototype, 'getClientRects', {{
                value: spoofContext.Range.prototype.getClientRects
              }});
            }}

            return w;
          }}
          
          modifiedAPIs.push([
            spoofContext.open, ""open""
          ]);
        

        (
          (spoofContext, inject, fn) => {{
            [""appendChild"", ""insertBefore"", ""replaceChild""].forEach(method => {{
              const _original = spoofContext.Node.prototype[method];

              spoofContext.Node.prototype[method] = function() {{
                let e = _original.apply(this, arguments);

                if (e && e.tagName === ""IFRAME"") {{
                  try {{
                    inject(e.contentWindow);                    
                  }} catch (err) {{}};
                }} else {{
                  for (let i = 0; i < spoofContext.length; i++) {{
                    try {{
                      inject(spoofContext[i]);                    
                    }} catch (err) {{}};
                  }}
                }}

                if (e && e.nodeName === 'LINK' && fn.CHAMELEON_SPOOF_f) CHAMELEON_SPOOF_f();

                return e;
              }}
            }});

            [""append"", ""insertAdjacentElement"", ""insertAdjacentHTML"", 
              ""insertAdjacentText"", ""prepend"", ""replaceWith""].forEach(method => {{
              const _original = spoofContext.Element.prototype[method];

              spoofContext.Element.prototype[method] = function() {{
                let e = _original.apply(this, Array.from(arguments));

                if (e && e.tagName === ""IFRAME"") {{
                  try {{
                    inject(e.contentWindow);                    
                  }} catch (err) {{}};
                }} else {{
                  for (let i = 0; i < spoofContext.length; i++) {{
                    try {{
                      inject(spoofContext[i]);                    
                    }} catch (err) {{}};
                  }}
                }} 

                return e;
              }}
            }});

            ['innerHTML', 'outerHTML'].forEach(p => {{
              let obj = Object.getOwnPropertyDescriptor(spoofContext.Element.prototype, p);

              Object.defineProperty(spoofContext.Element.prototype, p, {{
                set(html) {{
                  obj.set.call(this, html);

                  for (let i = 0; i < spoofContext.length; i++) {{
                    try {{
                      inject(spoofContext[i]);                    
                    }} catch (err) {{}};
                  }}

                  if (fn.modifyNodeFont) {{
                    modifyNodeFont(this.parentNode);
                  }}
                }}
              }});
            }});
          }}
        )(spoofContext, inject, {{
            modifyNodeFont: typeof modifyNodeFont !== 'undefined' ? modifyNodeFont : null,
            CHAMELEON_SPOOF_f: typeof CHAMELEON_SPOOF_f !== 'undefined' ? CHAMELEON_SPOOF_f : null
        }});

        modifiedAPIs.push([
          [ spoofContext.Element.innerHTML, ""innerHTML"" ],
          [ spoofContext.Element.outerHTML, ""outerHTML"" ],
          [ spoofContext.Node.appendChild, ""appendChild"" ],
          [ spoofContext.Node.insertBefore, ""insertBefore"" ],
          [ spoofContext.Node.replaceChild, ""replaceChild"" ],
          [ spoofContext.Element.append, ""append"" ],
          [ spoofContext.Element.insertAdjacentElement, ""insertAdjacentElement"" ],
          [ spoofContext.Element.insertAdjacentHTML, ""insertAdjacentHTML"" ],
          [ spoofContext.Element.insertAdjacentText, ""insertAdjacentText"" ],
          [ spoofContext.Element.prepend, ""prepend"" ],
          [ spoofContext.Element.replaceWith, ""replaceWith"" ]
        ]);

        for (let m of modifiedAPIs) {{
          Object.defineProperty(m[0], 'toString', {{
            configurable: false,
            value: function toString() {{
              return \`function \$\{{m[1]\}}() {{\n    [native code]\n}}\`;
            }}
          }})
  
          Object.defineProperty(m[0], 'name', {{
            configurable: false,
            value: m[1]
          }})
        }}
    }};

        inject(window);
    }})()`
.replace(/CHAMELEON_SPOOF/g, this.randObjName)
.replace(/ORIGINAL_INTL/g, String.fromCharCode(65 + Math.floor(Math.random() * 26)) + Math.random().toString(36).substring(Math.floor(Math.random() * 5) + 5))
.replace(/ORIGINAL_DATE/g, String.fromCharCode(65 + Math.floor(Math.random() * 26)) + Math.random().toString(36).substring(Math.floor(Math.random() * 5) + 5));
    }}
}}

 //console.log('navigator spoofed');
 const chameleonInjector = new Injector();
 chameleonInjector.injectIntoPage();
 //console.log('navigator spoofed2');
 ";
    }

public static string SetBackgroundo() 
{ 
    return $@"
    // background.js
    chrome.runtime.onInstalled.addListener(() => {{
        console.log('Background script running');
    }});

    chrome.webRequest.onBeforeSendHeaders.addListener(
      function(details) {{
        let headerFound = false;
        for (var i = 0; i < details.requestHeaders.length; ++i) {{
          if (details.requestHeaders[i].name === 'Sec-CH-UA-Platform') {{
            details.requestHeaders[i].value = '{(!AddonsUtil.IMac ? "Windows" : "macOS")}'; // change this to the desired value
            headerFound = true;
            break;
          }}
        }}
        if (!headerFound) {{
          details.requestHeaders.push({{name: 'Sec-CH-UA-Platform', value: '{(!AddonsUtil.IMac ? "Windows" : "macOS")}'}}); // add header if not found
        }}
        return {{requestHeaders: details.requestHeaders}};
      }},
      {{urls: [""<all_urls>""]}},
      [""blocking"", ""requestHeaders""]
    );
    ";
}

    static string updateInjectionsDatas(string d)
    {
        return $"(() => {{{ d }}})();";
    }


    public static string GetNavigator(string navigator) => @$"
        Object.defineProperty(window,""navigator"", {{
        get: function () {{ return {navigator}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorAppName(string appName) => @$"
        Object.defineProperty(navigator,""appName"", {{
        get: function () {{ return ""{appName}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorAppCodeName(string appCodeName) => @$"
        Object.defineProperty(navigator,""appCodeName"", {{
        get: function () {{ return ""{appCodeName}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorAppMinorVersion(string appMinorVersion) => @$"
        Object.defineProperty(navigator,""appMinorVersion"", {{
        get: function () {{ return ""{appMinorVersion}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorBuildID(string buildID) => @$"
        Object.defineProperty(navigator,""buildID"", {{
        get: function () {{ return ""{buildID}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorUserAgent(string userAgent) => @$"
        Object.defineProperty(navigator,""userAgent"", {{
        get: function () {{ return ""{userAgent}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorVendor(string vendor) => @$"
        Object.defineProperty(navigator,""vendor"", {{
        get: function () {{ return ""{vendor}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorVendorSub(string vendorSub) => @$"
        Object.defineProperty(navigator,""vendorSub"", {{
        get: function () {{ return ""{vendorSub}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorAppVersion(string appVersion) => @$"
        Object.defineProperty(navigator,""appVersion"", {{
        get: function () {{ return ""{appVersion}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorProduct(string product) => @$"
        Object.defineProperty(navigator,""product"", {{
        get: function () {{ return ""{product}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorProductSub(string productSub) => @$"
        Object.defineProperty(navigator,""productSub"", {{
        get: function () {{ return ""{productSub}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorLanguage(string language) => @$"
        Object.defineProperty(navigator,""language"", {{
        get: function () {{ return ""{language}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorLanguages(string[] languages) => @$"
        Object.defineProperty(navigator,""languages"", {{
        get: function () {{ return {JsonConvert.SerializeObject(languages)}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorOnLine(bool onLine) => @$"
        Object.defineProperty(navigator,""onLine"", {{
        get: function () {{ return {onLine.ToString().ToLower()}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorCookieEnabled(bool cookieEnabled) => @$"
        Object.defineProperty(navigator,""cookieEnabled"", {{
        get: function () {{ return {cookieEnabled.ToString().ToLower()}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorDoNotTrack(string doNotTrack) => @$"
        Object.defineProperty(navigator,""doNotTrack"", {{
        get: function () {{ return ""{doNotTrack}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorGeolocation(bool geolocation) => @$"
        Object.defineProperty(navigator,""geolocation"", {{
        get: function () {{ return {geolocation.ToString().ToLower()}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorMediaDevices(bool mediaDevices) => @$"
        Object.defineProperty(navigator,""mediaDevices"", {{
        get: function () {{ return {mediaDevices.ToString().ToLower()}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorPlugins(string[] plugins) => @$"
        Object.defineProperty(navigator,""plugins"", {{
        get: function () {{ return {JsonConvert.SerializeObject(plugins)}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorMimeTypes(string[] mimeTypes) => @$"
        Object.defineProperty(navigator,""mimeTypes"", {{
        get: function () {{ return {JsonConvert.SerializeObject(mimeTypes)}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorMaxTouchPoints(int maxTouchPoints) => @$"
        Object.defineProperty(navigator,""maxTouchPoints"", {{
        get: function () {{ return {maxTouchPoints}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorHardwareConcurrency(int hardwareConcurrency) => @$"
        Object.defineProperty(navigator,""hardware
        Concurrency"", {{
        get: function () {{ return {hardwareConcurrency}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorDeviceMemory(int deviceMemory) => @$"
        Object.defineProperty(navigator,""deviceMemory"", {{
        get: function () {{ return {deviceMemory}; }},
        set: function (a) {{}}
    }});    
";

    public static string GetNavigatorConnection(string connection) => @$"
        Object.defineProperty(navigator,""connection"", {{
        get: function () {{ return ""{connection}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorKeyboard(string keyboard) => @$"
        Object.defineProperty(navigator,""keyboard"", {{
        get: function () {{ return ""{keyboard}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorGamepads(string[] gamepads) => @$"
        Object.defineProperty(navigator,""gamepads"", {{
        get: function () {{ return {JsonConvert.SerializeObject(gamepads)}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorVibrate(bool vibrate) => @$"
        Object.defineProperty(navigator,""vibrate"", {{
        get: function () {{ return {vibrate.ToString().ToLower()}; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorStorage(string storage) => @$"
        Object.defineProperty(navigator,""storage"", {{
        get: function () {{ return ""{storage}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorServiceWorker(string serviceWorker) => @$"
        Object.defineProperty(navigator,""serviceWorker"", {{
        get: function () {{ return ""{serviceWorker}""; }},
        set: function (a) {{}}
    }});    
";

    public static string GetNavigatorWebkitTemporaryStorage(string webkitTemporaryStorage) => @$"
        Object.defineProperty(navigator,""webkitTemporaryStorage"", {{
        get: function () {{ return ""{webkitTemporaryStorage}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitPersistentStorage(string webkitPersistentStorage) => @$"
        Object.defineProperty(navigator,""webkitPersistentStorage"", {{
        get: function () {{ return ""{webkitPersistentStorage}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitGetUserMedia(string webkitGetUserMedia) => @$"
        Object.defineProperty(navigator,""webkitGetUserMedia"", {{
        get: function () {{ return ""{webkitGetUserMedia}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitPointer(string webkitPointer) => @$"
        Object.defineProperty(navigator,""webkitPointer"", {{
        get: function () {{ return ""{webkitPointer}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitRequestFileSystem(string webkitRequestFileSystem) => @$"
        Object.defineProperty(navigator,""webkitRequestFileSystem"", {{
        get: function () {{ return ""{webkitRequestFileSystem}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitResolveLocalFileSystemURL(string webkitResolveLocalFileSystemURL) => @$"
        Object.defineProperty(navigator,""webkitResolveLocalFileSystemURL"", {{
        get: function () {{ return ""{webkitResolveLocalFileSystemURL}""; }},
        set: function (a) {{}}
    }});
";
    
        public static string GetNavigatorWebkitStorageInfo(string webkitStorageInfo) => @$"
            Object.defineProperty(navigator,""webkitStorageInfo"", {{
            get: function () {{ return ""{webkitStorageInfo}""; }},
            set: function (a) {{}}
        }});    
    ";

    public static string GetNavigatorWebkitIDBFactory(string webkitIDBFactory) => @$"
        Object.defineProperty(navigator,""webkitIDBFactory"", {{
        get: function () {{ return ""{webkitIDBFactory}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBDatabase(string webkitIDBDatabase) => @$"
        Object.defineProperty(navigator,""webkitIDBDatabase"", {{
        get: function () {{ return ""{webkitIDBDatabase}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBTransaction(string webkitIDBTransaction) => @$"
        Object.defineProperty(navigator,""webkitIDBTransaction"", {{
        get: function () {{ return ""{webkitIDBTransaction}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBKeyRange(string webkitIDBKeyRange) => @$"
        Object.defineProperty(navigator,""webkitIDBKeyRange"", {{
        get: function () {{ return ""{webkitIDBKeyRange}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBIndex(string webkitIDBIndex) => @$"
        Object.defineProperty(navigator,""webkitIDBIndex"", {{
        get: function () {{ return ""{webkitIDBIndex}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBCursor(string webkitIDBCursor) => @$"
        Object.defineProperty(navigator,""webkitIDBCursor"", {{
        get: function () {{ return ""{webkitIDBCursor}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBObjectStore(string webkitIDBObjectStore) => @$"
        Object.defineProperty(navigator,""webkitIDBObjectStore"", {{
        get: function () {{ return ""{webkitIDBObjectStore}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBRequest(string webkitIDBRequest) => @$"
        Object.defineProperty(navigator,""webkitIDBRequest"", {{
        get: function () {{ return ""{webkitIDBRequest}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBOpenDBRequest(string webkitIDBOpenDBRequest) => @$"
        Object.defineProperty(navigator,""webkitIDBOpenDBRequest"", {{
        get: function () {{ return ""{webkitIDBOpenDBRequest}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBVersionChangeEvent(string webkitIDBVersionChangeEvent) => @$"
        Object.defineProperty(navigator,""webkitIDBVersionChangeEvent"", {{
        get: function () {{ return ""{webkitIDBVersionChangeEvent}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDB(string webkitIDB) => @$"
        Object.defineProperty(navigator,""webkitIDB"", {{
        get: function () {{ return ""{webkitIDB}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBDatabaseException(string webkitIDBDatabaseException) => @$"
        Object.defineProperty(navigator,""webkitIDBDatabaseException"", {{
        get: function () {{ return ""{webkitIDBDatabaseException}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBDatabaseError(string webkitIDBDatabaseError) => @$"
        Object.defineProperty(navigator,""webkitIDBDatabaseError"", {{
        get: function () {{ return ""{webkitIDBDatabaseError}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBDatabaseExceptionCode(string webkitIDBDatabaseExceptionCode) => @$"
        Object.defineProperty(navigator,""webkitIDBDatabaseExceptionCode"", {{
        get: function () {{ return ""{webkitIDBDatabaseExceptionCode}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBDatabaseExceptionName(string webkitIDBDatabaseExceptionName) => @$"
        Object.defineProperty(navigator,""webkitIDBDatabaseExceptionName"", {{
        get: function () {{ return ""{webkitIDBDatabaseExceptionName}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBDatabaseExceptionMessage(string webkitIDBDatabaseExceptionMessage) => @$"
        Object.defineProperty(navigator,""webkitIDBDatabaseExceptionMessage"", {{
        get: function () {{ return ""{webkitIDBDatabaseExceptionMessage}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBDatabaseExceptionData(string webkitIDBDatabaseExceptionData) => @$"
        Object.defineProperty(navigator,""webkitIDBDatabaseExceptionData"", {{
        get: function () {{ return ""{webkitIDBDatabaseExceptionData}""; }},
        set: function (a) {{}}
    }});
";

    public static string GetNavigatorWebkitIDBDatabaseExceptionCodeName(string webkitIDBDatabaseExceptionCodeName) => @$"
        Object.defineProperty(navigator,""webkitIDBDatabaseExceptionCodeName"", {{
        get: function () {{ return ""{webkitIDBDatabaseExceptionCodeName}""; }},
        set: function (a) {{}}
    }});
";
    [GeneratedRegex("[0-9]")]
    private static partial Regex RemoveNumbersRegex();
}
//using System.IO;
//using System.Threading.Tasks;
//
//namespace Chameleon.SystemBrowser.Addons
//{
//    public static class NavigatorAddon
//    {
//        public static async Task InitializeExtension(string dir)
//        {
//            await IOtil.DC(dir);
//
//            //await IOtil.WriteTextToFileAsync(Path.Combine(dir, "manifest.json"), GetManifestv3);
//            await IOtil.WriteTextToFileAsync(Path.Combine(dir, "manifest.json"), GetManifestv2);
//            await IOtil.WriteTextToFileAsync(Path.Combine(dir, "background.js"), SetBackgroundo());
//            await IOtil.WriteTextToFileAsync(Path.Combine(dir, "injector.js"), SetInjecto());
//            await IOtil.WriteTextToFileAsync(Path.Combine(dir, "injectedScript.js"), SetInjectScripto());
//            await IOtil.WriteTextToFileAsync(Path.Combine(dir, "content.js"), SetContnto());
//        }
//
//        public static string GetManifestv2 => @"
//        {
//            ""manifest_version"": 2,
//            ""name"": ""Chameleonavigator"",
//            ""version"": ""1"",
//            ""description"": ""Chameleon ChaChaCha"",
//            ""background"": {
//                ""scripts"": [""background.js""]
//            },
//            ""permissions"": [
//                ""webRequest"",
//                ""webRequestBlocking"",
//                ""<all_urls>"",
//                ""alarms"",
//                ""contextMenus"",
//                ""notifications"",
//                ""storage"",
//                ""tabs"",
//                ""activeTab""
//            ],
//            ""content_scripts"": [
//                {
//                    ""matches"": [""<all_urls>""],
//                    ""js"": [""injector.js""],
//                    ""all_frames"": true
//                }
//            ],
//            ""web_accessible_resources"": [
//                ""injectedScript.js""
//            ],
//            ""content_security_policy"": ""script-src 'self' 'wasm-unsafe-eval'; object-src 'self'""
//        }";
//
//        public static string GetManifestv3 => @"
//        {
//            ""version"": ""1"",
//            ""manifest_version"": 3,
//            ""description"": ""Chameleon ChaChaCha"",
//            ""name"": ""Chameleonavigator"",
//            ""background"": {
//                ""service_worker"": ""background.js""
//            },
//            ""permissions"": [
//                ""webRequest"",
//                ""webRequestBlocking"",
//                ""declarativeNetRequestWithHostAccess"",
//                ""<all_urls>"",
//                ""alarms"",
//                ""contextMenus"",
//                ""notifications"",
//                ""storage"",
//                ""tabs"",
//                ""scripting"",
//                ""activeTab"",
//                ""host_permissions""
//            ],
//            ""host_permissions"": [
//                ""<all_urls>""
//            ],
//            ""content_scripts"": [
//                {
//                    ""matches"": [""<all_urls>""],
//                    ""js"": [""injector.js""],
//                    ""all_frames"": true
//                }
//            ],
//            ""web_accessible_resources"": [
//                {
//                    ""resources"": [""injectedScript.js""],
//                    ""matches"": [""<all_urls>""]
//                }
//            ],
//            ""content_security_policy"": {
//                ""extension_pages"": ""script-src 'self' 'wasm-unsafe-eval'; object-src 'self'""
//            }
//        }";
//
//        public static string SetContnto()
//        {
//            return @"
//            // content.js
//            chrome.runtime.sendMessage({message: ""contentScriptLoaded""});
//            ";
//        }
//
//        public static string SetInjectScripto()
//        {
//            return $@"
//    (function() {{
//        const inject = (spoofContext) => {{
//            if (spoofContext.CHAMELEON_SPOOF) return;
//
//            spoofContext.CHAMELEON_SPOOF = ""CHAMELEON_SPOOF"";
//            let CHAMELEON_SPOOF = new WeakMap();
//            CHAMELEON_SPOOF.set(spoofContext, {{
//                timezone: {{ zone: {{ name: 'America/New_York' }} }},
//                language: 'en-US',
//                platform: 'Win32',
//                userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36',
//                cpuClass: 'x86',
//                hardwareConcurrency: 8,
//                deviceMemory: 16,
//                maxTouchPoints: 0,
//                vendor: 'Google Inc.',
//                appVersion: '5.0 (Windows)'
//            }});
//
//            const injectionProperties = [
//                {{ obj: 'window.navigator', prop: 'appCodeName', value: 'Mozilla' }},
//                {{ obj: 'window.navigator', prop: 'appName', value: 'Netscape' }},
//                {{ obj: 'window.navigator', prop: 'appVersion', value: '5.0 (Windows)' }},
//                {{ obj: 'window.navigator', prop: 'platform', value: 'Win32' }},
//                {{ obj: 'window.navigator', prop: 'userAgent', value: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36' }},
//                {{ obj: 'window.navigator', prop: 'productSub', value: '20030107' }},
//                {{ obj: 'window.navigator', prop: 'vendor', value: 'Google Inc.' }},
//                {{ obj: 'window.navigator', prop: 'vendorSub', value: '' }},
//                {{ obj: 'window.navigator', prop: 'hardwareConcurrency', value: 8 }},
//                {{ obj: 'window.navigator', prop: 'deviceMemory', value: 16 }},
//                {{ obj: 'window.navigator', prop: 'maxTouchPoints', value: 0 }},
//                {{ obj: 'window.navigator', prop: 'language', value: 'en-US' }},
//                {{ obj: 'window.navigator', prop: 'languages', value: ['en-US', 'en'] }},
//                {{ obj: 'window.navigator', prop: 'cpuClass', value: 'x86' }},
//                {{ obj: 'window.navigator', prop: 'oscpu', value: 'Windows NT 10.0' }},
//                {{ obj: 'window.navigator', prop: 'mimeTypes', value: [
//                    {{ type: 'application/pdf', suffixes: 'pdf', description: 'Portable Document Format' }}
//                ] }},
//                {{ obj: 'window.navigator', prop: 'plugins', value: [
//                    {{ name: 'Chrome PDF Plugin', filename: 'internal-pdf-viewer', description: 'Portable Document Format' }}
//                ] }},
//                {{ obj: 'window.navigator', prop: 'userAgentData', value: {{
//                    brands: [
//                        {{ brand: 'Chromium', version: '92' }},
//                        {{ brand: 'Google Chrome', version: '92' }}
//                    ],
//                    mobile: false,
//                    platform: 'Windows'
//                }} }}
//            ];
//
//            injectionProperties.forEach(injProp => {{
//                const setProperty = (obj, prop, value) => {{
//                    Object.defineProperty(obj, prop, {{
//                        get: () => value,
//                        configurable: true,
//                        enumerable: true
//                    }});
//                }};
//
//                if (injProp.obj === 'window') {{
//                    setProperty(spoofContext, injProp.prop, injProp.value);
//                }} else if (injProp.obj === 'window.navigator' && injProp.value === null) {{
//                    delete spoofContext.navigator.__proto__[injProp.prop];
//                }} else if (injProp.obj === 'window.navigator' && injProp.prop === 'mimeTypes') {{
//                    let mimes = () => {{
//                        const mimeArray = {{}};
//                        injProp.value.forEach((m, i) => {{
//                            function FakeMimeType() {{ return m }}
//                            const mime = new FakeMimeType();
//                            Object.setPrototypeOf(mime, MimeType.prototype);
//                            setProperty(mimeArray, i, mime);
//                            setProperty(mimeArray, m.type, mime);
//                        }});
//                        Object.setPrototypeOf(mimeArray, MimeTypeArray.prototype);
//                        //setProperty(mimeArray, 'length', injProp.value.length);
//                        setProperty(mimeArray, 'item', function item() {{
//                            return this[arguments[0]] || null;
//                        }});
//                        setProperty(mimeArray, 'namedItem', function namedItem() {{
//                            return this[arguments[0]] || null;
//                        }});
//                        return mimeArray;
//                    }};
//                    setProperty(spoofContext.navigator, 'mimeTypes', mimes());
//                }} else if (injProp.obj === 'window.navigator' && injProp.prop === 'plugins') {{
//                    let plugins = () => {{
//                        const pluginArray = {{}};
//                        injProp.value.forEach((p, i) => {{
//                            function FakePlugin() {{ return p }}
//                            const plugin = new FakePlugin();
//                            Object.setPrototypeOf(plugin, Plugin.prototype);
//                            //setProperty(plugin, 'length', p.__mimeTypes.length);
//                            setProperty(plugin, 'version', undefined);
//                            setProperty(plugin, 'item', function item() {{
//                                return this[arguments[0]] || null;
//                            }});
//                            setProperty(plugin, 'namedItem', function namedItem() {{
//                                return this[arguments[0]] || null;
//                            }});
//
//                            //for (let j = 0; j < p.__mimeTypes.length; j++) {{
//                            //    setProperty(plugin, j, navigator.mimeTypes[p.__mimeTypes[j]]);
//                            //    setProperty(plugin, p.__mimeTypes[j], navigator.mimeTypes[p.__mimeTypes[j]]);
//                            //}}
//
//                            //delete p.__mimeTypes;
//
//                            setProperty(pluginArray, i, p);
//                            setProperty(pluginArray, p.name, p);
//                        }});
//                        setProperty(pluginArray, 'length', injProp.value.length);
//                        setProperty(pluginArray, 'item', function item() {{
//                            return this[arguments[0]] || null;
//                        }});
//                        setProperty(pluginArray, 'namedItem', function namedItem() {{
//                            return this[arguments[0]] || null;
//                        }});
//                        setProperty(pluginArray, 'refresh', function refresh() {{
//                            return;
//                        }});
//
//                        pluginArray[Symbol.iterator] = function() {{
//                            const numPlugins = Object.keys(this).length - 4;
//                            let index = 0;
//                            return {{
//                                next: () => {{
//                                    if (index < numPlugins) {{
//                                        const value = this[index];
//                                        index++;
//                                        return {{ value, done: false }};
//                                    }}
//                                    return {{ value: undefined, done: true }};
//                                }}
//                            }};
//                        }};
//                        return pluginArray;
//                    }};
//                    setProperty(spoofContext.navigator, 'plugins', plugins());
//
//                    let pluginsArray = Array.from(navigator.plugins);
//
//                    //for (let i = 0; i < navigator.mimeTypes.length; i++) {{
//                    //    let p = pluginsArray.find(p => p[navigator.mimeTypes[i].type] !== undefined);
//                    //   setProperty(navigator.mimeTypes[i], 'enabledPlugin', p);
//                    //}}
//                }} else {{
//                    let tmpObj = injProp.obj.split('.').reduce((p, c) => p && p[c] || null, spoofContext);
//                    if (tmpObj[injProp.prop] != injProp.value) {{
//                        setProperty(tmpObj, injProp.prop, injProp.value);
//                    }}
//                }}
//            }});
//        }};
//
//        inject(window);
//    }})();
//    ";
//        }
//
//        public static string SetInjecto()
//        {
//            return @"
//            // injector.js
//            (function() {
//                if (!window.chameleonInjected) {
//                    window.chameleonInjected = true;
//
//                    class Injector {
//                        constructor() {
//                            this.spoof = {
//                                custom: '',
//                                overwrite: [],
//                                metadata: {},
//                            };
//                            this.enabled = true;
//                            this.randObjName = 'randObjName';
//                        }
//
//                        injectScript(windowInstance) {
//                            const scriptEl = windowInstance.document.createElement('script');
//                            scriptEl.src = chrome.runtime.getURL('injectedScript.js'); // Path to your script in the extension directory
//                            windowInstance.document.head.appendChild(scriptEl);
//                            scriptEl.onload = function() {
//                                scriptEl.remove();
//                            };
//                            scriptEl.onerror = function() {
//                                console.error('Failed to load the injectedScript.js');
//                            };
//                        }
//
//                        injectIntoIframe(iframe) {
//                            const attemptInjection = () => {
//                                try {
//                                    if (iframe.contentDocument && iframe.contentWindow) {
//                                        this.injectScript(iframe.contentWindow);
//                                    }
//                                } catch (e) {
//                                    console.error('Failed to inject script into iframe:', e);
//                                }
//                            };
//
//                            // Wait for the iframe to load before attempting injection
//                            iframe.addEventListener('load', () => {
//                                attemptInjection();
//                            });
//
//                            // Attempt immediate injection if the iframe is already loaded
//                            if (iframe.contentDocument && iframe.contentWindow) {
//                                attemptInjection();
//                            }
//                        }
//
//                        injectIntoPage() {
//                            this.injectScript(window); // Inject into main window
//
//                            // Inject into all existing iframes
//                            const iframes = document.querySelectorAll('iframe');
//                            for (const iframe of iframes) {
//                                this.injectIntoIframe(iframe);
//                            }
//
//                            // Observe the document for new iframes
//                            const observer = new MutationObserver(mutations => {
//                                for (const mutation of mutations) {
//                                    for (const node of mutation.addedNodes) {
//                                        if (node.tagName === 'IFRAME') {
//                                            this.injectIntoIframe(node);
//                                        }
//                                    }
//                                }
//                            });
//
//                            observer.observe(document.body, { childList: true, subtree: true });
//                        }
//                    }
//
//                    console.log('navigator spoofed');
//                    window.chameleon = new Injector();
//                    window.chameleon.injectIntoPage();
//                    console.log('navigator spoofed2');
//                }
//                else
//                {
//                    console.log('navigator spoofed3');
//                    window.chameleon.injectIntoPage();
//                    console.log('navigator spoofed4');
//                }
//            })();
//            ";
//        }
//
//public static string SetBackgroundo()
//{
//    return @"
//    // background.js
//   chrome.runtime.onInstalled.addListener(() => {
//    console.log('Background script running');
//});
//
//chrome.webRequest.onBeforeSendHeaders.addListener(
//    (details) => {
//        const headers = details.requestHeaders;
//        const headerMap = new Map(headers.map(header => [header.name.toLowerCase(), header]));
//
//        // Modify User-Agent and Client Hints headers
//        const userAgent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36';
//        const clientHints = {
//            'sec-ch-ua': '""Not/A)Brand"";v=""8"", ""Chromium"";v=""126"", ""Google Chrome"";v=""126""',
//            'sec-ch-ua-platform': '""Win64""',
//            'sec-ch-ua-mobile': '?0',
//            'sec-ch-ua-full-version': '""126.0.6478.183""',
//            'sec-ch-ua-full-version-list': '""Not/A)Brand"";v=""8.0.0.0"", ""Chromium"";v=""126.0.6478.183"", ""Google Chrome"";v=""126.0.6478.183""',
//            'sec-ch-ua-platform-version': '""14.5.0""',
//            'sec-ch-ua-arch': '""x86""',
//            'sec-ch-ua-bitness': '""64""',
//            'sec-ch-ua-wow64': '?0',
//            'sec-ch-ua-model': '""""',
//            'sec-ch-device-memory': '8',
//            'sec-ch-dpr': '2',
//            'sec-ch-viewport-width': '967',
//            'sec-ch-viewport-height': '1155'
//        };
//
//        if (headerMap.has('user-agent')) {
//            headerMap.get('user-agent').value = userAgent;
//        } else {
//            headers.push({ name: 'User-Agent', value: userAgent });
//        }
//
//        for (const [key, value] of Object.entries(clientHints)) {
//            if (headerMap.has(key)) {
//                headerMap.get(key).value = value;
//            } else {
//                headers.push({ name: key, value: value });
//            }
//        }
//
//        return { requestHeaders: headers };
//    },
//    { urls: ['<all_urls>'] },
//    ['blocking', 'requestHeaders', 'extraHeaders']
//);
//
//chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
//    if (changeInfo.status === 'complete' && /^http/.test(tab.url)) {
//        // Inject into the main frame and all sub-frames
//        chrome.scripting.executeScript({
//            target: { tabId: tabId, allFrames: true },
//            files: ['injector.js']
//        }).then(() => {
//            console.log('Injected injector.js into tab:', tabId);
//        }).catch(err => {
//            console.error('Failed to inject script:', err);
//        });
//    }
//});
//
//chrome.webNavigation.onCommitted.addListener(details => {
//    if (chrome.scripting && details.frameId !== 0) {
//        // Inject the script into the iframe
//        chrome.scripting.executeScript({
//            target: { tabId: details.tabId, frameIds: [details.frameId] },
//            files: ['injector.js']
//        }).then(() => {
//            console.log('Injected injector.js into iframe:', details.frameId);
//        }).catch(err => {
//            console.error('Failed to inject script into iframe:', err);
//        });
//    }
//}, { url: [{ urlMatches: '.*' }] });
//    ";
//}
//    }
//}
//
////SetBackgroundo
//        // Function to send a message to the content script
//        function sendMessageToContentScript(id, message) {{
//            chrome.tabs.sendMessage(id, message, (response) => {{
//                if (response) {{
//                     console.log(response.farewell);
//                }}
//            }});
//        }}
//        // Add a listener for the browser action
//        chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {{
//            if (message.message === ""contentScriptLoaded"") {{
//                 console.log(""response.farewell"");
//            }}
//        }});
//       
//        chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {{ 
//            console.log(changeInfo.status); 
//            //chrome.scripting.executeScript({{
//            //  target : {{tabId : tabId, allFrames: true}},
//            //  files: ['injector.js']
//            //}}).then(() => console.log(""injected a function""));
//            
//            chrome.tabs.sendMessage(tabId, {{""message"": tabId}});
//            if (changeInfo.status === 'loading') {{ 
//                // Define your inline JavaScript code as a string 
//                const inlineScript = ` 
//                    console.log('navigator', navigator); 
//                `;
//
//                // Execute the inline script in the context of the tab 
//                //chrome.scripting.executeScript({{ 
//                //   target: {{tabId: tabId, allFrames: true}}, 
//                //   function: () => {{ inlineScript }} 
//                //}}); 
//            }} 
//
//            // Check for a specific condition before doing something 
//            if (changeInfo.status === 'complete' && /^http/.test(tab.url)) {{ 
//                // Do something when a tab is updated and meets the condition 
//               console.log(`Tab {{tab.url}}`); 
//                // For example, injecting a content script: 
//            
//                //chrome.tabs.executeScript(tabId, {{
//                //    file: 'injector.js',
//                //    allFrames: true
//                //}}, () => {{
//                //    console.log(""injected a function"");
//                //}});
//            }} 
//        }});