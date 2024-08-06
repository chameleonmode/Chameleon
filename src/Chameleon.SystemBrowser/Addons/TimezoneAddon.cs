using Chameleon.ThirdParty.GeoIp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.SystemBrowser.Addons;
public class TimezoneAddon
{
    public const string DirName = "ChameleonTZ";
    public static async Task InitializeExtension(string dir, string json)
    {
        await IOtil.DC(dir);

        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "manifest.json"), Manifestv3);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "worker.js"), SetWorkero(json));

        var dataDir = Path.Combine(dir, "data");
        await IOtil.CreateDirectory(dataDir);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dataDir, "offsets.js"), Offsets);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dataDir, "inject.js"), Inject);

        var optionsDir = Path.Combine(dataDir, "options");
        await IOtil.CreateDirectory(optionsDir);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(optionsDir, "index.html"), Index);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(optionsDir, "index.css"), IndexCSS);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(optionsDir, "index.js"), IndexJS);

        var dataInjectDir = Path.Combine(dataDir, "inject");
        await IOtil.CreateDirectory(dataInjectDir);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dataInjectDir, "main.js"), Main);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dataInjectDir, "isolated.js"), Isolated);

    }
    static string Manifestv3 => """
        {
          "manifest_version": 3,
          "version": "1.0.0",
          "name": "Chameleon Timezone Switcher",
          "description": "This extension alters browser timezone to a random or user-defined value.",
          "permissions": [
            "storage",
            "scripting",
            "webNavigation",
            "contextMenus",
            "notifications"
          ],
          "host_permissions": [
            "<all_urls>",
            "*://*/*"
          ],
          "action":{},
          "content_security_policy": {
            "extension_pages": "script-src 'self' http://localhost:* http://127.0.0.1:*; object-src 'self' http://localhost:* http://127.0.0.1:*"
          },
          "background": {
            "service_worker": "worker.js"
          },
          "options_ui": {
            "page": "data/options/index.html"
          },
          "content_scripts": [{
            "world": "ISOLATED",
            "matches":["*://*/*"],
            "match_about_blank": true,
            "all_frames": true,
            "match_origin_as_fallback": true,
            "run_at": "document_start",
            "js": ["data/inject/isolated.js"]
          }, {
            "world": "MAIN",
            "matches":["*://*/*"],
            "match_about_blank": true,
            "all_frames": true,
            "match_origin_as_fallback": true,
            "run_at": "document_start",
            "js": ["data/inject/main.js"]
          }]
        }
        """;
    //<input type = "button" value="Support Development" id="support">
    const string Index = """
        <!DOCTYPE html>
        <html>
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>Chameleon timezone :: Options page</title>
          <link rel="stylesheet" type="text/css" href="index.css">
        </head>
        <body>
          <form>
            <div class="two">
              <label for="offset">Timezone</label>
              <div class="one">
                <select id="offset"></select>
                <input type="text" id="user" required>
              </div>
              <label for="minutes">Current Offset</label>
              <input type="number" id="minutes" required readonly>
            </div>

            <p class="two">
              <input type="checkbox" id="update">
              <label for="update">Automatically update timezone based on my IP address</label>
              <input type="checkbox" id="random">
              <label for="random">Pick a random time zone</label>
            </p>

            <div style="margin-bottom: 5px;">
              <input type="button" value="Factory Reset" id="reset">
            </div>
            <div>
              <input type="submit" value="Save Options">
              <span id="toast"></span>
            </div>
          </form>

          <script src="../offsets.js"></script>
          <script src="index.js"></script>
        </body>
        </html>
        """;
    const string IndexCSS = """
        body {
          font-size: 14px;
          font-family: system-ui, -apple-system, Segoe UI, Roboto, Ubuntu, Cantarell, Noto Sans, sans-serif;
          background-color: #fff;
          color: #4d5156;
          width: min(100% - 2rem, 70rem);
          margin-inline: auto;
          margin: 10px;
        }
        select,
        button,
        input[type=submit],
        input[type=button] {
          height: 24px;
          color: #444;
          background-image: linear-gradient(rgb(237, 237, 237), rgb(237, 237, 237) 38%, rgb(222, 222, 222));
          box-shadow: rgba(0, 0, 0, 0.08) 0 1px 0, rgba(255, 255, 255, 0.75) 0 1px 2px inset;
          text-shadow: rgb(240, 240, 240) 0 1px 0;
        }
        select,
        button,
        textarea,
        input {
          border: solid 1px rgba(0, 0, 0, 0.25);
        }
        input[type=button]:disabled {
          opacity: 0.5;
        }
        input[type=text] {
          width: 100%;
          box-sizing: border-box;
        }
        textarea,
        input[type=text],
        input[type=number] {
          padding: 5px;
          outline: none;
        }
        textarea:focus,
        input[type=text]:focus,
        input[type=number]:focus {
          background-color: #e5f8ff;
        }
        input[type=number]:read-only {
          background-color: #ededed;
        }
        input[type=text]:invalid {
          background-color: #ffdfdf;
        }
        a,
        a:visited {
          color: #07c;
        }

        .note {
          color: #5d5d5d;
          grid-column: 1/3;
          white-space: normal;
          padding: 10px;
        }
        .one {
          display: grid;
          align-items: center;
          grid-gap: 5px;
        }
        .two {
          display: grid;
          grid-template-columns: min-content 1fr;
          white-space: nowrap;
          grid-gap: 5px;
          align-items: center;
        }
        """;
    static string IndexJS => """
        /* global offsets */
        'use strict';

        const offset = document.getElementById('offset');
        const user = document.getElementById('user');
        const toast = document.getElementById('toast');

        const update = () => chrome.runtime.sendMessage({
          method: 'get-offset',
          value: user.value
        }, offset => document.getElementById('minutes').value = offset);

        offset.addEventListener('change', update);

        document.addEventListener('DOMContentLoaded', () => {
          const f = document.createDocumentFragment();
          Object.keys(offsets).sort((a, b) => offsets[b].offset - offsets[a].offset).forEach(key => {
            const option = document.createElement('option');
            option.value = key;

            const of = offsets[key].offset === 0 ? 'GMT' : (
              (offsets[key].offset > 0 ? '+' : '-') +
              (Math.abs(offsets[key].offset) / 60).toString().split('.')[0].padStart(2, '0') + ':' +
              (Math.abs(offsets[key].offset) % 60).toString().padStart(2, '0')
            );
            option.textContent = `${key} (${of})`;
            f.appendChild(option);
          });
          offset.appendChild(f);
          chrome.storage.local.get({
            timezone: 'Etc/GMT',
            random: false,
            update: false
          }, prefs => {
            offset.value = user.value = prefs.timezone;
            offset.dispatchEvent(new Event('change'));
            document.getElementById('random').checked = prefs.random;
            document.getElementById('update').checked = prefs.update;
          });
        });

        offset.onchange = e => {
          if (e.target.value) {
            user.value = e.target.value;
            user.dispatchEvent(new Event('input'));
          }
        };

        const date = new Date();
        user.oninput = e => {
          try {
            date.toLocaleString('en', {
              timeZone: e.target.value,
              timeZoneName: 'longOffset'
            });
            update();
            offset.value = user.value;
            e.target.setCustomValidity('');
          }
          catch (ee) {
            e.target.setCustomValidity('Not a valid timezone');
          }
        };

        document.addEventListener('submit', e => {
          e.preventDefault();

          chrome.storage.local.set({
            timezone: user.value,
            random: document.getElementById('random').checked,
            update: document.getElementById('update').checked
          }, () => {
            chrome.runtime.sendMessage({
              method: 'update-offset'
            });
            toast.textContent = user.value;
            window.setTimeout(() => toast.textContent = '', 750);
          });
        });

        document.getElementById('support').addEventListener('click', () => chrome.tabs.create({
          url: chrome.runtime.getManifest().homepage_url + '?rd=donate'
        }));

        // reset
        document.getElementById('reset').addEventListener('click', e => {
          if (e.detail === 1) {
            toast.textContent = 'Double-click to reset!';
            window.setTimeout(() => toast.textContent = '', 750);
          }
          else {
            localStorage.clear();
            chrome.storage.session.clear(() => {
              chrome.storage.local.clear(() => {
                chrome.runtime.reload();
                window.close();
              });
            });
          }
        });
        
        """;

    public static string SetWorkero(string tz) => $@"
/* global offsets */
self.importScripts('/data/offsets.js');

const notify = message => chrome.notifications.create({{
  type: 'basic',
  title: chrome.runtime.getManifest().name,
  message
}});

const uo = () => new Promise(resolve => chrome.storage.local.get({{
  'timezone': 'Etc/GMT'
}}, prefs => {{
  let offset = 0;
  try {{
    offset = uo.engine(prefs.timezone);
    chrome.storage.local.set({{
      offset
    }});
    resolve({{offset, timezone: prefs.timezone}});
  }}
  catch (e) {{
    prefs.timezone = 'Etc/GMT';
    prefs.offset = 0;
    notify(`Cannot detect offset for ""${{prefs.timezone}}"". Using 0 as offset`);
    chrome.storage.local.set(prefs);
    console.error(e);
    resolve(prefs);
  }}
  chrome.action.setTitle({{
    title: chrome.runtime.getManifest().name + ' (' + prefs.timezone + ')'
  }});
}}));
uo.engine = timeZone => {{
  const value = 'GMT' + uo.date.toLocaleString('en', {{
    timeZone,
    timeZoneName: 'longOffset'
  }}).split('GMT')[1];


  if (value === 'GMT') {{
    return 0;
  }}
  const o = /(?<hh>[-+]\d{{2}}):(?<mm>\d{{2}})/.exec(value);
  return Number(o.groups.hh) * 60 + Number(o.groups.mm);
}};
uo.date = new Date();

chrome.runtime.onInstalled.addListener(uo);
chrome.runtime.onStartup.addListener(uo);

chrome.runtime.onMessage.addListener((request, sender, response) => {{
  if (request.method === 'update-offset') {{
    uo();
  }}
  else if (request.method === 'get-offset') {{
    response(uo.engine(request.value));
  }}
  else if (request.method === 'get-prefs') {{
    chrome.storage.local.get({{
      random: false,
      timezone: 'Etc/GMT',
      offset: 0
    }}, prefs => {{
      console.log(prefs);

      if (prefs.random) {{
        const key = 'random.' + sender.tab.id;
        chrome.storage.session.get({{
          [key]: false
        }}, ps => {{
          if (ps[key]) {{
            response(ps[key]);
          }}
          else {{
            response(prefs);
          }}
        }});
      }}
      else {{
        response(prefs);
      }}
    }});
    return true;
  }}
}});

chrome.tabs.onRemoved.addListener(tabId => chrome.storage.session.remove('random.' + tabId));

const onCommitted = ({{url, tabId, frameId}}) => {{
  const send = o => chrome.scripting.executeScript({{
    target: {{
      tabId,
      frameIds: [frameId]
    }},
    injectImmediately: true,
    func: o => {{
      self.prefs = o;
      try {{
        self.update('committed');
      }}
      catch (e) {{}}
    }},
    args: [o]
  }}).catch(() => {{}});

  if (url && url.startsWith('http')) {{
    chrome.storage.local.get({{
      random: false,
      timezone: 'Etc/GMT',
      offset: 0
    }}, prefs => {{
      if (prefs.random) {{
        const key = 'random.' + tabId;

        chrome.storage.session.get({{
          [key]: false
        }}, ps => {{
          if (frameId === 0 || !ps[key]) {{
            const ofs = Object.keys(offsets);
            const n = ofs[Math.floor(Math.random() * ofs.length)];

            try {{
              ps[key] = {{
                offset: uo.engine(n),
                timezone: n
              }};
              chrome.storage.session.set({{
                [key]: ps[key]
              }});
            }}
            catch (e) {{}}
          }}
          send(ps[key] || prefs);
        }});
      }}
      else {{
        send(prefs);
      }}
    }});
  }}
}};
chrome.webNavigation.onCommitted.addListener(onCommitted);

chrome.action.onClicked.addListener(() => {{
  onClicked({{
    menuItemId: 'check-timezone'
  }});
  chrome.storage.local.get({{
    msg: true
  }}, prefs => {{
    if (prefs.msg) {{
      notify('To disable timezone spoofing, please disable this extension and refresh the page!');
      chrome.storage.local.set({{
        msg: false
      }});
    }}
  }});
}});

const server = async (silent = true) => {{
  try {{
    const r = await fetch('http://ip-api.com/json');
    const {{timezone}} = await r.json();

    console.log(timezone);

    if (!timezone) {{
      throw Error('cannot resolve timezone for your IP address. Use options page to set manually');
    }}

    chrome.storage.local.get({{
      timezone: 'Etc/GMT'
    }}, prefs => {{
      if (prefs.timezone !== timezone) {{
        chrome.storage.local.set({{
          timezone
        }}, () => {{
          uo().then(({{timezone, offset}}) => notify('New Timezone: ' + timezone + ' (' + offset + ')'));
        }});
      }}
      else if (silent === false) {{
        notify('Already in Timezone: ' + timezone);
      }}
    }});
  }}
  catch (e) {{
    if (silent === false) {{
      console.warn(e);
      notify(e.message);
    }}
  }}
}};
const initIt = () => {{
    chrome.storage.local.set({tz}, () => {{
        uo().then(({{ timezone, offset }}) => notify('New Timezone: ' + timezone + ' (' + offset + ')'));
    }});
}};
chrome.runtime.onInstalled.addListener(initIt);
chrome.runtime.onStartup.addListener(initIt);

// Ensure the initIt function runs before any other tab or content script
//chrome.tabs.onCreated.addListener((tab) => {{
//    initIt().then(() => {{
//        // Your logic to handle the tab creation after initIt has run
//    }});
//}});


/* update on startup */
{{
  const once = () => chrome.storage.local.get({{
    update: false
  }}, prefs => {{
    if (prefs.update) {{
      server();
    }}
  }});
  chrome.runtime.onInstalled.addListener(once);
  chrome.runtime.onStartup.addListener(once);
}}

/* context menu */
{{
  const once = () => {{
    chrome.contextMenus.create({{
      title: 'Check my Current Timezone',
      id: 'check-timezone',
      contexts: ['action']
    }}, () => chrome.runtime.lastError);
    chrome.contextMenus.create({{
      title: 'Update Timezone from IP',
      id: 'update-timezone',
      contexts: ['action']
    }}, () => chrome.runtime.lastError);
  }};
  chrome.runtime.onInstalled.addListener(once);
  chrome.runtime.onStartup.addListener(once);
}}

const onClicked = ({{menuItemId}}) => {{
  if (menuItemId === 'update-timezone') {{
    server(false);
  }}
  else if (menuItemId === 'check-timezone') {{
    chrome.tabs.create({{
      url: 'https://webbrowsertools.com/timezone/'
    }});
  }}
}};
chrome.contextMenus.onClicked.addListener(onClicked);
/* FAQs & Feedback */
{{
  const {{management, runtime: {{onInstalled, setUninstallURL, getManifest}}, storage, tabs}} = chrome;
  if (navigator.webdriver === true) {{
    const page = getManifest().homepage_url;
    const {{name, version}} = getManifest();
    const sv = (Date.now() / 60000).toFixed(0).slice(-3);
    onInstalled.addListener(({{reason, previousVersion}}) => {{
      management.getSelf(({{installType}}) => installType === 'normal' && storage.local.get({{
        'faqs': true,
        'last-update': 0
      }}, prefs => {{
        if (reason === 'install' || (prefs.faqs && reason === 'update')) {{
          const doUpdate = (Date.now() - prefs['last-update']) / 1000 / 60 / 60 / 24 > 45;
          if (doUpdate && previousVersion !== version) {{
            tabs.create({{
              url: page + '?type=' + reason + (previousVersion ? '&p=' + previousVersion : '') + '&version=' + version + '#' + sv,
              active: reason === 'install'
            }});
            storage.local.set({{'last-update': Date.now()}});
          }}
        }}
      }}));
    }});
    setUninstallURL(page + '?rd=feedback&name=' + encodeURIComponent(name) + '&version=' + version);
  }}
}}
    ";

    static string Inject => """
        'use strict';

        const shiftedDate = `{
          const OriginalDate = Date;

          const updates = []; // update this.#ad of each Date object
          // prefs
          const prefs = new Proxy({
            timezone: 'Etc/GMT',
            offset: 0
          }, {
            set(target, prop, value) {
              target[prop] = value;
              if (prop === 'offset') {
                updates.forEach(c => c());
              }
              return true;
            }
          });

          class SpoofDate extends Date {
            #ad; // adjusted date

            #sync() {
              const offset = (prefs.offset + super.getTimezoneOffset());
              this.#ad = new OriginalDate(this.getTime() + offset * 60 * 1000);
            }

            constructor(...args) {
              super(...args);

              updates.push(() => this.#sync());
              this.#sync();
            }
            getTimezoneOffset() {
              return prefs.offset;
            }
            /* to string (only supports en locale) */
            toTimeString() {
              if (isNaN(this)) {
                return super.toTimeString();
              }

              const parts = super.toLocaleString.call(this, 'en', {
                timeZone: prefs.timezone,
                timeZoneName: 'longOffset'
              }).split('GMT');

              if (parts.length !== 2) {
                return super.toTimeString();
              }

              const a = 'GMT' + parts[1].replace(':', '');

              const b = super.toLocaleString.call(this, 'en', {
                timeZone: prefs.timezone,
                timeZoneName: 'long'
              }).split(/(AM |PM )/i).pop();

              return super.toTimeString.apply(this.#ad).split(' GMT')[0] + ' ' + a + ' (' + b + ')';
            }
            /* only supports en locale */
            toDateString() {
              return super.toDateString.apply(this.#ad);
            }
            /* only supports en locale */
            toString() {
              if (isNaN(this)) {
                return super.toString();
              }
              return this.toDateString() + ' ' + this.toTimeString();
            }
            toLocaleDateString(...args) {
              args[1] = args[1] || {};
              args[1].timeZone = args[1].timeZone || prefs.timezone;

              return super.toLocaleDateString(...args);
            }
            toLocaleTimeString(...args) {
              args[1] = args[1] || {};
              args[1].timeZone = args[1].timeZone || prefs.timezone;

              return super.toLocaleTimeString(...args);
            }
            toLocaleString(...args) {
              args[1] = args[1] || {};
              args[1].timeZone = args[1].timeZone || prefs.timezone;

              return super.toLocaleString(...args);
            }
            /* get */
            #get(name, ...args) {
              return super[name].call(this.#ad, ...args);
            }
            getDate(...args) {
              return this.#get('getDate', ...args);
            }
            getDay(...args) {
              return this.#get('getDay', ...args);
            }
            getHours(...args) {
              return this.#get('getHours', ...args);
            }
            getMinutes(...args) {
              return this.#get('getMinutes', ...args);
            }
            getMonth(...args) {
              return this.#get('getMonth', ...args);
            }
            getYear(...args) {
              return this.#get('getYear', ...args);
            }
            getFullYear(...args) {
              return this.#get('getFullYear', ...args);
            }
            /* set */
            #set(type, name, args) {
              if (type === 'ad') {
                const n = this.#ad.getTime();
                const r = this.#get(name, ...args);

                return super.setTime(this.getTime() + r - n);
              }
              else {
                const r = super[name](...args);
                this.#sync();

                return r;
              }
            }
            setHours(...args) {
              return this.#set('ad', 'setHours', args);
            }
            setMinutes(...args) {
              return this.#set('ad', 'setMinutes', args);
            }
            setMonth(...args) {
              return this.#set('ad', 'setMonth', args);
            }
            setDate(...args) {
              return this.#set('ad', 'setDate', args);
            }
            setYear(...args) {
              return this.#set('ad', 'setYear', args);
            }
            setFullYear(...args) {
              return this.#set('ad', 'setFullYear', args);
            }
            setTime(...args) {
              return this.#set('md', 'setTime', args);
            }
            setUTCDate(...args) {
              return this.#set('md', 'setUTCDate', args);
            }
            setUTCFullYear(...args) {
              return this.#set('md', 'setUTCFullYear', args);
            }
            setUTCHours(...args) {
              return this.#set('md', 'setUTCHours', args);
            }
            setUTCMinutes(...args) {
              return this.#set('md', 'setUTCMinutes', args);
            }
            setUTCMonth(...args) {
              return this.#set('md', 'setUTCMonth', args);
            }
          }

          /* prefs */
          {
            const script = document.currentScript;
            const update = () => {
              prefs.timezone = script.dataset.timezone;
              prefs.offset = parseInt(script.dataset.offset);
            };
            update();
            script.addEventListener('change', update);
          }

          /* override */
          self.Date = SpoofDate;
          self.Date = new Proxy(Date, {
            apply(target, self, args) {
              return new SpoofDate(...args);
            }
          });
        }`;

        const intl = `{
          const DateTimeFormat = Intl.DateTimeFormat;
          const script = document.currentScript;

          class SpoofDateTimeFormat extends Intl.DateTimeFormat {
            constructor(...args) {
              if (!args[1]) {
                args[1] = {};
              }
              if (!args[1].timeZone) {
                args[1].timeZone = script.dataset.timezone;
              }

              super(...args);
            }
          }
          Intl.DateTimeFormat = SpoofDateTimeFormat;

          Intl.DateTimeFormat = new Proxy(Intl.DateTimeFormat, {
            apply(target, self, args) {
              return new Intl.DateTimeFormat(...args);
            }
          });
        }`;

        const code = `{
          ${shiftedDate}
          ${intl}
        }`;
          document.addEventListener('DOMContentLoaded', () => {
        // Function to add CSP meta tag to a document
        const addCSPMetaTag = (doc) => {
          const meta = doc.createElement('meta');
          meta.httpEquiv = 'Content-Security-Policy';
          meta.content = "script-src 'self' 'wasm-unsafe-eval' 'inline-speculation-rules' http://localhost:* http://127.0.0.1:*";
          doc.head.appendChild(meta);
        };

        // Add CSP meta tag to the main document
        addCSPMetaTag(document);

        // Select all iframes in the document
        const iframes = document.querySelectorAll('iframe');

        // Iterate through each iframe and add the CSP meta tag
        iframes.forEach(iframe => {
          try {
            const iframeDoc = iframe.contentDocument || iframe.contentWindow.document;
            addCSPMetaTag(iframeDoc);
          } catch (e) {
            console.error('Error accessing iframe document:', e);
          }
        });

          // Your existing code
          let script = self.script = document.createElement('script');
          if (typeof self.prefs === 'undefined') {
            try {
              self.prefs = parent.prefs;
            }
            catch (e) {}
          }
          // ask from bg
          if (typeof self.prefs === 'undefined') {
            self.prefs = self.prefs || {
              offset: 0,
              timezone: 'Etc/GMT'
            };
            chrome.runtime.sendMessage({
              method: 'get-prefs'
            }, prefs => {
              Object.assign(script.dataset, prefs);
              script.dispatchEvent(new Event('change'));
            });
          }
          
          Object.assign(script.dataset, self.prefs);
          script.textContent = code;
          //document.documentElement.append(script);
          //script.remove();
          
          //script = self.script = document.createElement('script');
          script.src = URL.createObjectURL(new Blob([code], { type: 'text/javascript' }));
          (document.head || document.documentElement).appendChild(script);
          try {
            URL.revokeObjectURL(script.src);
          } catch (e) {
              console.log(e);
          }
          script.remove();
        });
        """;
    static string Offsets => """
        self.offsets = {"Pacific/Niue":{"offset":-660,"msg":{"standard":"Niue Time"}},"Pacific/Pago_Pago":{"offset":-660},"Pacific/Honolulu":{"offset":-600},"Pacific/Rarotonga":{"offset":-600},"Pacific/Tahiti":{"offset":-600,"msg":{"standard":"Tahiti Time"}},"Pacific/Marquesas":{"offset":-510,"msg":{"standard":"Marquesas Time"}},"America/Anchorage":{"offset":-540},"Pacific/Gambier":{"offset":-540,"msg":{"standard":"Gambier Time"}},"America/Los_Angeles":{"offset":-480},"America/Tijuana":{"offset":-480},"America/Vancouver":{"offset":-480},"America/Whitehorse":{"offset":-480},"Pacific/Pitcairn":{"offset":-480,"msg":{"standard":"Pitcairn Time"}},"America/Dawson_Creek":{"offset":-420},"America/Denver":{"offset":-420},"America/Edmonton":{"offset":-420},"America/Hermosillo":{"offset":-420},"America/Mazatlan":{"offset":-420},"America/Phoenix":{"offset":-420},"America/Yellowknife":{"offset":-420},"America/Belize":{"offset":-360},"America/Chicago":{"offset":-360},"America/Costa_Rica":{"offset":-360},"America/El_Salvador":{"offset":-360},"America/Guatemala":{"offset":-360},"America/Managua":{"offset":-360},"America/Mexico_City":{"offset":-360},"America/Regina":{"offset":-360},"America/Tegucigalpa":{"offset":-360},"America/Winnipeg":{"offset":-360},"Pacific/Galapagos":{"offset":-360,"msg":{"standard":"Galapagos Time"}},"America/Bogota":{"offset":-300},"America/Cancun":{"offset":-300},"America/Cayman":{"offset":-300},"America/Guayaquil":{"offset":-300},"America/Havana":{"offset":-300},"America/Iqaluit":{"offset":-300},"America/Jamaica":{"offset":-300},"America/Lima":{"offset":-300},"America/Nassau":{"offset":-300},"America/New_York":{"offset":-300},"America/Panama":{"offset":-300},"America/Port-au-Prince":{"offset":-300},"America/Rio_Branco":{"offset":-300},"America/Toronto":{"offset":-300},"Pacific/Easter":{"offset":-300,"msg":{"generic":"Easter Island Time","standard":"Easter Island Standard Time","daylight":"Easter Island Summer Time"}},"America/Caracas":{"offset":-210},"America/Asuncion":{"offset":-180},"America/Barbados":{"offset":-240},"America/Boa_Vista":{"offset":-240},"America/Campo_Grande":{"offset":-180},"America/Cuiaba":{"offset":-180},"America/Curacao":{"offset":-240},"America/Grand_Turk":{"offset":-240},"America/Guyana":{"offset":-240,"msg":{"standard":"Guyana Time"}},"America/Halifax":{"offset":-240},"America/La_Paz":{"offset":-240},"America/Manaus":{"offset":-240},"America/Martinique":{"offset":-240},"America/Port_of_Spain":{"offset":-240},"America/Porto_Velho":{"offset":-240},"America/Puerto_Rico":{"offset":-240},"America/Santo_Domingo":{"offset":-240},"America/Thule":{"offset":-240},"Atlantic/Bermuda":{"offset":-240},"America/St_Johns":{"offset":-150},"America/Araguaina":{"offset":-180},"America/Argentina/Buenos_Aires":{"offset":-180,"msg":{"generic":"Argentina Time","standard":"Argentina Standard Time","daylight":"Argentina Summer Time"}},"America/Bahia":{"offset":-180},"America/Belem":{"offset":-180},"America/Cayenne":{"offset":-180},"America/Fortaleza":{"offset":-180},"America/Godthab":{"offset":-180},"America/Maceio":{"offset":-180},"America/Miquelon":{"offset":-180},"America/Montevideo":{"offset":-180},"America/Paramaribo":{"offset":-180},"America/Recife":{"offset":-180},"America/Santiago":{"offset":-180},"America/Sao_Paulo":{"offset":-120},"Antarctica/Palmer":{"offset":-180},"Antarctica/Rothera":{"offset":-180,"msg":{"standard":"Rothera Time"}},"Atlantic/Stanley":{"offset":-180},"America/Noronha":{"offset":-120,"msg":{"generic":"Fernando de Noronha Time","standard":"Fernando de Noronha Standard Time","daylight":"Fernando de Noronha Summer Time"}},"Atlantic/South_Georgia":{"offset":-120,"msg":{"standard":"South Georgia Time"}},"America/Scoresbysund":{"offset":-60},"Atlantic/Azores":{"offset":-60,"msg":{"generic":"Azores Time","standard":"Azores Standard Time","daylight":"Azores Summer Time"}},"Atlantic/Cape_Verde":{"offset":-60,"msg":{"generic":"Cape Verde Time","standard":"Cape Verde Standard Time","daylight":"Cape Verde Summer Time"}},"Africa/Abidjan":{"offset":0},"Africa/Accra":{"offset":0},"Africa/Bissau":{"offset":0},"Africa/Casablanca":{"offset":0},"Africa/El_Aaiun":{"offset":0},"Africa/Monrovia":{"offset":0},"America/Danmarkshavn":{"offset":0},"Atlantic/Canary":{"offset":0},"Atlantic/Faroe":{"offset":0},"Atlantic/Reykjavik":{"offset":0},"Etc/GMT":{"offset":0,"msg":{"standard":"Greenwich Mean Time"}},"Europe/Dublin":{"offset":0},"Europe/Lisbon":{"offset":0},"Europe/London":{"offset":0},"Africa/Algiers":{"offset":60},"Africa/Ceuta":{"offset":60},"Africa/Lagos":{"offset":60},"Africa/Ndjamena":{"offset":60},"Africa/Tunis":{"offset":60},"Africa/Windhoek":{"offset":120},"Europe/Amsterdam":{"offset":60},"Europe/Andorra":{"offset":60},"Europe/Belgrade":{"offset":60},"Europe/Berlin":{"offset":60},"Europe/Brussels":{"offset":60},"Europe/Budapest":{"offset":60},"Europe/Copenhagen":{"offset":60},"Europe/Gibraltar":{"offset":60},"Europe/Luxembourg":{"offset":60},"Europe/Madrid":{"offset":60},"Europe/Malta":{"offset":60},"Europe/Monaco":{"offset":60},"Europe/Oslo":{"offset":60},"Europe/Paris":{"offset":60},"Europe/Prague":{"offset":60},"Europe/Rome":{"offset":60},"Europe/Stockholm":{"offset":60},"Europe/Tirane":{"offset":60},"Europe/Vienna":{"offset":60},"Europe/Warsaw":{"offset":60},"Europe/Zurich":{"offset":60},"Africa/Cairo":{"offset":120},"Africa/Johannesburg":{"offset":120},"Africa/Maputo":{"offset":120},"Africa/Tripoli":{"offset":120},"Asia/Amman":{"offset":120},"Asia/Beirut":{"offset":120},"Asia/Damascus":{"offset":120},"Asia/Gaza":{"offset":120},"Asia/Jerusalem":{"offset":120},"Asia/Nicosia":{"offset":120},"Europe/Athens":{"offset":120},"Europe/Bucharest":{"offset":120},"Europe/Chisinau":{"offset":120},"Europe/Helsinki":{"offset":120},"Europe/Istanbul":{"offset":120},"Europe/Kaliningrad":{"offset":120},"Europe/Kiev":{"offset":120},"Europe/Riga":{"offset":120},"Europe/Sofia":{"offset":120},"Europe/Tallinn":{"offset":120},"Europe/Vilnius":{"offset":120},"Africa/Khartoum":{"offset":180},"Africa/Nairobi":{"offset":180},"Antarctica/Syowa":{"offset":180,"msg":{"standard":"Syowa Time"}},"Asia/Baghdad":{"offset":180},"Asia/Qatar":{"offset":180},"Asia/Riyadh":{"offset":180},"Europe/Minsk":{"offset":180},"Europe/Moscow":{"offset":180,"msg":{"generic":"Moscow Time","standard":"Moscow Standard Time","daylight":"Moscow Summer Time"}},"Asia/Tehran":{"offset":210},"Asia/Baku":{"offset":240},"Asia/Dubai":{"offset":240},"Asia/Tbilisi":{"offset":240},"Asia/Yerevan":{"offset":240},"Europe/Samara":{"offset":240,"msg":{"generic":"Samara Time","standard":"Samara Standard Time","daylight":"Samara Summer Time"}},"Indian/Mahe":{"offset":240},"Indian/Mauritius":{"offset":240,"msg":{"generic":"Mauritius Time","standard":"Mauritius Standard Time","daylight":"Mauritius Summer Time"}},"Indian/Reunion":{"offset":240,"msg":{"standard":"Réunion Time"}},"Asia/Kabul":{"offset":270},"Antarctica/Mawson":{"offset":300,"msg":{"standard":"Mawson Time"}},"Asia/Aqtau":{"offset":300,"msg":{"generic":"Aqtau Time","standard":"Aqtau Standard Time","daylight":"Aqtau Summer Time"}},"Asia/Aqtobe":{"offset":300,"msg":{"generic":"Aqtobe Time","standard":"Aqtobe Standard Time","daylight":"Aqtobe Summer Time"}},"Asia/Ashgabat":{"offset":300},"Asia/Dushanbe":{"offset":300},"Asia/Karachi":{"offset":300},"Asia/Tashkent":{"offset":300},"Asia/Yekaterinburg":{"offset":300,"msg":{"generic":"Yekaterinburg Time","standard":"Yekaterinburg Standard Time","daylight":"Yekaterinburg Summer Time"}},"Indian/Kerguelen":{"offset":300},"Indian/Maldives":{"offset":300,"msg":{"standard":"Maldives Time"}},"Asia/Calcutta":{"offset":330},"Asia/Colombo":{"offset":330},"Asia/Katmandu":{"offset":345},"Antarctica/Vostok":{"offset":360,"msg":{"standard":"Vostok Time"}},"Asia/Almaty":{"offset":360,"msg":{"generic":"Almaty Time","standard":"Almaty Standard Time","daylight":"Almaty Summer Time"}},"Asia/Bishkek":{"offset":360},"Asia/Dhaka":{"offset":360},"Asia/Omsk":{"offset":360,"msg":{"generic":"Omsk Time","standard":"Omsk Standard Time","daylight":"Omsk Summer Time"}},"Asia/Thimphu":{"offset":360},"Indian/Chagos":{"offset":360},"Asia/Rangoon":{"offset":390},"Indian/Cocos":{"offset":390,"msg":{"standard":"Cocos Islands Time"}},"Antarctica/Davis":{"offset":420,"msg":{"standard":"Davis Time"}},"Asia/Bangkok":{"offset":420},"Asia/Hovd":{"offset":420,"msg":{"generic":"Hovd Time","standard":"Hovd Standard Time","daylight":"Hovd Summer Time"}},"Asia/Jakarta":{"offset":420},"Asia/Krasnoyarsk":{"offset":420,"msg":{"generic":"Krasnoyarsk Time","standard":"Krasnoyarsk Standard Time","daylight":"Krasnoyarsk Summer Time"}},"Asia/Saigon":{"offset":420},"Indian/Christmas":{"offset":420,"msg":{"standard":"Christmas Island Time"}},"Antarctica/Casey":{"offset":480,"msg":{"standard":"Casey Time"}},"Asia/Brunei":{"offset":480,"msg":{"standard":"Brunei Darussalam Time"}},"Asia/Choibalsan":{"offset":480,"msg":{"generic":"Choibalsan Time","standard":"Choibalsan Standard Time","daylight":"Choibalsan Summer Time"}},"Asia/Hong_Kong":{"offset":480,"msg":{"generic":"Hong Kong Time","standard":"Hong Kong Standard Time","daylight":"Hong Kong Summer Time"}},"Asia/Irkutsk":{"offset":480,"msg":{"generic":"Irkutsk Time","standard":"Irkutsk Standard Time","daylight":"Irkutsk Summer Time"}},"Asia/Kuala_Lumpur":{"offset":480},"Asia/Macau":{"offset":480,"msg":{"generic":"Macau Time","standard":"Macau Standard Time","daylight":"Macau Summer Time"}},"Asia/Makassar":{"offset":480},"Asia/Manila":{"offset":480},"Asia/Shanghai":{"offset":480},"Asia/Singapore":{"offset":480,"msg":{"standard":"Singapore Standard Time"}},"Asia/Taipei":{"offset":480,"msg":{"generic":"Taipei Time","standard":"Taipei Standard Time","daylight":"Taipei Daylight Time"}},"Asia/Ulaanbaatar":{"offset":480},"Australia/Perth":{"offset":480},"Asia/Pyongyang":{"offset":510,"msg":{"standard":"Pyongyang Time"}},"Asia/Dili":{"offset":540},"Asia/Jayapura":{"offset":540},"Asia/Seoul":{"offset":540},"Asia/Tokyo":{"offset":540},"Asia/Yakutsk":{"offset":540,"msg":{"generic":"Yakutsk Time","standard":"Yakutsk Standard Time","daylight":"Yakutsk Summer Time"}},"Pacific/Palau":{"offset":540,"msg":{"standard":"Palau Time"}},"Australia/Adelaide":{"offset":630},"Australia/Darwin":{"offset":570},"Antarctica/DumontDUrville":{"offset":600,"msg":{"standard":"Dumont-d’Urville Time"}},"Asia/Magadan":{"offset":600,"msg":{"generic":"Magadan Time","standard":"Magadan Standard Time","daylight":"Magadan Summer Time"}},"Asia/Vladivostok":{"offset":600,"msg":{"generic":"Vladivostok Time","standard":"Vladivostok Standard Time","daylight":"Vladivostok Summer Time"}},"Australia/Brisbane":{"offset":600},"Australia/Hobart":{"offset":660},"Australia/Sydney":{"offset":660},"Pacific/Chuuk":{"offset":600},"Pacific/Guam":{"offset":600,"msg":{"standard":"Guam Standard Time"}},"Pacific/Port_Moresby":{"offset":600},"Pacific/Efate":{"offset":660},"Pacific/Guadalcanal":{"offset":660},"Pacific/Kosrae":{"offset":660,"msg":{"standard":"Kosrae Time"}},"Pacific/Norfolk":{"offset":660,"msg":{"standard":"Norfolk Island Time"}},"Pacific/Noumea":{"offset":660},"Pacific/Pohnpei":{"offset":660},"Asia/Kamchatka":{"offset":720,"msg":{"generic":"Petropavlovsk-Kamchatski Time","standard":"Petropavlovsk-Kamchatski Standard Time","daylight":"Petropavlovsk-Kamchatski Summer Time"}},"Pacific/Auckland":{"offset":780},"Pacific/Fiji":{"offset":780,"msg":{"generic":"Fiji Time","standard":"Fiji Standard Time","daylight":"Fiji Summer Time"}},"Pacific/Funafuti":{"offset":720},"Pacific/Kwajalein":{"offset":720},"Pacific/Majuro":{"offset":720},"Pacific/Nauru":{"offset":720,"msg":{"standard":"Nauru Time"}},"Pacific/Tarawa":{"offset":720},"Pacific/Wake":{"offset":720,"msg":{"standard":"Wake Island Time"}},"Pacific/Wallis":{"offset":720,"msg":{"standard":"Wallis & Futuna Time"}},"Pacific/Apia":{"offset":840,"msg":{"generic":"Apia Time","standard":"Apia Standard Time","daylight":"Apia Daylight Time"}},"Pacific/Enderbury":{"offset":780},"Pacific/Fakaofo":{"offset":780},"Pacific/Tongatapu":{"offset":780},"Pacific/Kiritimati":{"offset":840}}
        """;

    static string Main => """
        {
         const port = document.getElementById('stz-obhgtd');
         port.remove();

         const OriginalDate = Date;

         // prefs
         const prefs = {
           updates: [] // update this.#ad of each Date object
         };
         Object.defineProperties(prefs, {
           'offset': {
             get() {
               return parseInt(port.dataset.offset);
             }
           },
           'timezone': {
             get() {
               return port.dataset.timezone;
             }
           }
         });
         port.addEventListener('change', () => prefs.updates.forEach(c => c()));

         /* Date Spoofing */

         class SpoofDate extends Date {
           #ad; // adjusted date

           #sync() {
             const offset = (prefs.offset + super.getTimezoneOffset());
             this.#ad = new OriginalDate(this.getTime() + offset * 60 * 1000);
           }

           constructor(...args) {
             super(...args);

             prefs.updates.push(() => this.#sync());
             this.#sync();
           }
           getTimezoneOffset() {
             return prefs.offset;
           }
           /* to string (only supports en locale) */
           toTimeString() {
             if (isNaN(this)) {
               return super.toTimeString();
             }

             const parts = super.toLocaleString.call(this, 'en', {
               timeZone: prefs.timezone,
               timeZoneName: 'longOffset'
             }).split('GMT');

             if (parts.length !== 2) {
               return super.toTimeString();
             }

             const a = 'GMT' + parts[1].replace(':', '');

             const b = super.toLocaleString.call(this, 'en', {
               timeZone: prefs.timezone,
               timeZoneName: 'long'
             }).split(/(AM |PM )/i).pop();

             return super.toTimeString.apply(this.#ad).split(' GMT')[0] + ' ' + a + ' (' + b + ')';
           }
           /* only supports en locale */
           toDateString() {
             return super.toDateString.apply(this.#ad);
           }
           /* only supports en locale */
           toString() {
             if (isNaN(this)) {
               return super.toString();
             }
             return this.toDateString() + ' ' + this.toTimeString();
           }
           toLocaleDateString(...args) {
             args[1] = args[1] || {};
             args[1].timeZone = args[1].timeZone || prefs.timezone;

             return super.toLocaleDateString(...args);
           }
           toLocaleTimeString(...args) {
             args[1] = args[1] || {};
             args[1].timeZone = args[1].timeZone || prefs.timezone;

             return super.toLocaleTimeString(...args);
           }
           toLocaleString(...args) {
             args[1] = args[1] || {};
             args[1].timeZone = args[1].timeZone || prefs.timezone;

             return super.toLocaleString(...args);
           }
           /* get */
           #get(name, ...args) {
             return super[name].call(this.#ad, ...args);
           }
           getDate(...args) {
             return this.#get('getDate', ...args);
           }
           getDay(...args) {
             return this.#get('getDay', ...args);
           }
           getHours(...args) {
             return this.#get('getHours', ...args);
           }
           getMinutes(...args) {
             return this.#get('getMinutes', ...args);
           }
           getMonth(...args) {
             return this.#get('getMonth', ...args);
           }
           getYear(...args) {
             return this.#get('getYear', ...args);
           }
           getFullYear(...args) {
             return this.#get('getFullYear', ...args);
           }
           /* set */
           #set(type, name, args) {
             if (type === 'ad') {
               const n = this.#ad.getTime();
               const r = this.#get(name, ...args);

               return super.setTime(this.getTime() + r - n);
             }
             else {
               const r = super[name](...args);
               this.#sync();

               return r;
             }
           }
           setHours(...args) {
             return this.#set('ad', 'setHours', args);
           }
           setMinutes(...args) {
             return this.#set('ad', 'setMinutes', args);
           }
           setMonth(...args) {
             return this.#set('ad', 'setMonth', args);
           }
           setDate(...args) {
             return this.#set('ad', 'setDate', args);
           }
           setYear(...args) {
             return this.#set('ad', 'setYear', args);
           }
           setFullYear(...args) {
             return this.#set('ad', 'setFullYear', args);
           }
           setTime(...args) {
             return this.#set('md', 'setTime', args);
           }
           setUTCDate(...args) {
             return this.#set('md', 'setUTCDate', args);
           }
           setUTCFullYear(...args) {
             return this.#set('md', 'setUTCFullYear', args);
           }
           setUTCHours(...args) {
             return this.#set('md', 'setUTCHours', args);
           }
           setUTCMinutes(...args) {
             return this.#set('md', 'setUTCMinutes', args);
           }
           setUTCMonth(...args) {
             return this.#set('md', 'setUTCMonth', args);
           }
         }

         /* override */
         self.Date = SpoofDate;
         self.Date = new Proxy(Date, {
           apply(target, self, args) {
             return new SpoofDate(...args);
           }
         });

         /* Intl Spoofing */
         class SpoofDateTimeFormat extends Intl.DateTimeFormat {
           constructor(...args) {
             if (!args[1]) {
               args[1] = {};
             }
             if (!args[1].timeZone) {
               args[1].timeZone = port.dataset.timezone;
             }

             super(...args);
           }
         }
         Intl.DateTimeFormat = SpoofDateTimeFormat;

         Intl.DateTimeFormat = new Proxy(Intl.DateTimeFormat, {
           apply(target, self, args) {
             return new Intl.DateTimeFormat(...args);
           }
         });
        }

        /* for iframe[sandbox] */
        window.addEventListener('message', e => {
          if (e.data === 'spoof-sandbox-frame') {
            e.stopPropagation();
            e.preventDefault();
            try {
              e.source.Date = Date;
              e.source.Intl.DateTimeFormat = Intl.DateTimeFormat;
            }
            catch (e) {}
          }
        });
        """;
    static string Isolated => """
        const port2 = self.port = document.getElementById('stz-obhgtd');
        if(port2) {
        parent.postMessage('spoof-sandbox-frame', '*');
        // backup plan
        top.postMessage('spoof-sandbox-frame', '*');
        }else {
        const port = document.createElement('span');
        port.id = 'stz-obhgtd';
        port.dataset.timezone = 'Etc/GMT';
        port.dataset.offset = 0;
        document.documentElement.append(port);
        
        self.update = reason => {
          console.log(self.prefs, reason);
          port.dataset.timezone = self.prefs.timezone;
          port.dataset.offset = self.prefs.offset;
        
          port.dispatchEvent(new Event('change'));
        };
        
        if (typeof self.prefs === 'undefined') {
          try {
            if (self !== parent) {
              self.prefs = parent.prefs;
            }
          }
          catch (e) {}
        }
        
        // ask from bg (just as a backup)
        if (typeof self.prefs === 'undefined') {
          setTimeout(() => {
            if (typeof self.prefs === 'undefined') {
              chrome.runtime.sendMessage({
                method: 'get-prefs'
              }, prefs => {
                self.prefs = prefs;
                self.update('ask from bg');
              });
            }
          }, 500);
        }
        else {
          self.update('top frame or committed');
        }
        
        // updates
        chrome.storage.onChanged.addListener(ps => {
          if (ps.offset) {
            self.prefs.offset = ps.offset.newValue;
          }
          if (ps.timezone) {
            self.prefs.timezone = ps.timezone.newValue;
          }
          if (ps.offset || ps.timezone) {
            self.update('updated');
          }
        });
        
        // Function to update sandbox attribute
        //const updateSandboxAttribute = (iframe) => {
        //  if (iframe.getAttribute('sandbox')) {
        //    iframe.setAttribute('sandbox', 'allow-same-origin allow-scripts');
        //  }
        //};
        //
        //// Event listener for DOMContentLoaded
        //document.addEventListener('DOMContentLoaded', () => {
        //  // Select all iframes in the document
        //  const iframes = document.querySelectorAll('iframe');
        //
        //  // Iterate through each iframe and update the sandbox attribute
        //  iframes.forEach(iframe => {
        //    updateSandboxAttribute(iframe);
        //  });
        //
        //  // Create a MutationObserver instance
        //  const observer = new MutationObserver((mutationsList) => {
        //    for (const mutation of mutationsList) {
        //      if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
        //        mutation.addedNodes.forEach(node => {
        //          if (node.tagName === 'IFRAME') {
        //            updateSandboxAttribute(node);
        //          }
        //        });
        //      }
        //    }
        //  });
        //
        //  // Configuration of the observer
        //  const config = { childList: true };
        //
        //  // Start observing document.body for configured mutations
        //  observer.observe(document.body, config);
        //});
        }
        """;
}
