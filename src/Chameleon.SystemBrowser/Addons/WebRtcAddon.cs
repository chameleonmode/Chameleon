using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chameleon.SystemBrowser.Addons;
public class WebRtcAddon
{
    public const string DirName = "WebRtcAddon";
    public static async Task InitializeExtension(string dir)
    {
        await IOtil.DC(dir);

        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "manifest.json"), Manifestv3);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "context.js"), Context);
        await IOtil.WriteTextToFileAsync(
            Path.Combine(dir, "worker.js"), Worker);

        var dataDir = Path.Combine(dir, "data");

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
          "name": "Chameleon WebRTC Defender",
          "description": "Hides your private and public IP addresses by configuring how WebRTC's network traffic is routed.",
          "version": "1.2.3",
          "manifest_version": 3,
          "permissions": [
            "storage",
            "privacy",
            "contextMenus"
          ],
          "host_permissions": [
            "*://*/*"
          ],
          "background": {
            "service_worker": "worker.js"
          },
          "action": {},
          "options_ui": {
            "page": "/data/options/index.html"
          },
          "content_scripts": [{
            "matches": ["*://*/*"],
            "js": ["/data/inject/main.js"],
            "run_at": "document_start",
            "all_frames": true,
            "match_about_blank": true,
            "world": "MAIN"
          }, {
            "matches": ["*://*/*"],
            "js": ["/data/inject/isolated.js"],
            "run_at": "document_start",
            "all_frames": true,
            "match_about_blank": true,
            "world": "ISOLATED"
          }],
          "commands": {
            "_execute_action": {}
          }
        }
        """;
    static string Context => """
                
        {
          const isFirefox = /Firefox/.test(navigator.userAgent) || typeof InstallTrigger !== 'undefined';

          const update = () => chrome.storage.local.get({
            dAPI: true,
            eMode: isFirefox ? 'proxy_only' : 'disable_non_proxied_udp',
            dMode: 'default_public_interface_only'
          }, prefs => {
            chrome.contextMenus.update('dAPI', {
              checked: prefs.dAPI
            });
            chrome.contextMenus.update(prefs.eMode, {
              checked: true
            });
            chrome.contextMenus.update(prefs.dMode, {
              checked: true
            });
          });

          const onStartup = async () => {
            if (onStartup.done) {
              return;
            }
            onStartup.done = true;

            await chrome.contextMenus.create({
              id: 'test',
              contexts: ['action'],
              title: 'Check WebTRC Leakage'
            }, () => chrome.runtime.lastError);
            await chrome.contextMenus.create({
              id: 'dAPI',
              contexts: ['action'],
              title: 'Disable WebRTC Media Device Enumeration API',
              type: 'checkbox'
            }, () => chrome.runtime.lastError);
            await chrome.contextMenus.create({
              id: 'when-enabled',
              contexts: ['action'],
              title: 'When Enabled'
            }, () => chrome.runtime.lastError);
            await chrome.contextMenus.create({
              id: 'disable_non_proxied_udp',
              contexts: ['action'],
              title: 'Disable non-proxied UDP (force proxy)',
              parentId: 'when-enabled',
              type: 'radio'
            }, () => chrome.runtime.lastError);
            await chrome.contextMenus.create({
              id: 'proxy_only',
              contexts: ['action'],
              title: 'Only connections using TURN on a TCP connection through a proxy',
              parentId: 'when-enabled',
              type: 'radio',
              enabled: isFirefox
            }, () => chrome.runtime.lastError);
            await chrome.contextMenus.create({
              id: 'when-disabled',
              contexts: ['action'],
              title: 'When Disabled'
            }, () => chrome.runtime.lastError);
            await chrome.contextMenus.create({
              id: 'default_public_interface_only',
              contexts: ['action'],
              title: 'Use the default public interface only',
              parentId: 'when-disabled',
              type: 'radio'
            }, () => chrome.runtime.lastError);
            await chrome.contextMenus.create({
              id: 'default_public_and_private_interfaces',
              contexts: ['action'],
              title: 'Use the default public interface and private interface',
              parentId: 'when-disabled',
              type: 'radio'
            }, () => chrome.runtime.lastError);
            //
            update();
          };

          chrome.runtime.onInstalled.addListener(onStartup);
          chrome.runtime.onStartup.addListener(onStartup);

          chrome.contextMenus.onClicked.addListener((info, tab) => {
            if (info.menuItemId === 'test') {
              chrome.tabs.create({
                url: 'https://webbrowsertools.com/ip-address/',
                index: tab.index + 1
              });
            }
            else if (info.menuItemId === 'dAPI') {
              chrome.storage.local.set({
                dAPI: info.checked
              });
            }
            else if (info.menuItemId === 'disable_non_proxied_udp' || info.menuItemId === 'proxy_only') {
              chrome.storage.local.set({
                eMode: info.menuItemId
              });
            }
            else if (
              info.menuItemId === 'default_public_interface_only' ||
              info.menuItemId === 'default_public_and_private_interfaces'
            ) {
              chrome.storage.local.set({
                dMode: info.menuItemId
              });
            }
          });

          chrome.storage.onChanged.addListener(ps => {
            if (ps.dAPI || ps.eMode || ps.dMode) {
              update();
            }
          });
        }
        """;
    static string Worker => """
                self.importScripts('context.js');

        const isFirefox = /Firefox/.test(navigator.userAgent) || typeof InstallTrigger !== 'undefined';

        function action() {
          chrome.storage.local.get({
            enabled: true,
            eMode: isFirefox ? 'proxy_only' : 'disable_non_proxied_udp',
            dMode: 'default_public_interface_only'
          }, prefs => {
            // webRTCIPHandlingPolicy
            const value = prefs.enabled ? prefs.eMode : prefs.dMode;
            chrome.privacy.network.webRTCIPHandlingPolicy.clear({}, () => {
              chrome.privacy.network.webRTCIPHandlingPolicy.set({
                value
              }, () => {
                chrome.privacy.network.webRTCIPHandlingPolicy.get({}, s => {
                  let path = '/data/icons/';
                  let title = 'WebRTC Protection in On';

                  if (s.value !== value) {
                    path += 'red/';
                    title = 'WebRTC access cannot be changed. It is controlled by another extension';
                  }
                  else if (prefs.enabled === false) {
                    path += 'disabled/';
                    title = 'WebRTC Protection in Off';
                  }
                  // icon
                  chrome.action.setIcon({
                    path: {
                      16: path + '16.png',
                      32: path + '32.png',
                      48: path + '48.png'
                    }
                  });
                  // tooltip
                  chrome.action.setTitle({
                    title
                  });
                });
              });
            });
          });
        }

        action();

        chrome.storage.onChanged.addListener(() => {
          action();
        });

        chrome.action.onClicked.addListener(() => chrome.storage.local.get({
          enabled: true
        }, prefs => chrome.storage.local.set({
          enabled: !prefs.enabled
        })));

        /* FAQs & Feedback */
        {
          const {management, runtime: {onInstalled, setUninstallURL, getManifest}, storage, tabs} = chrome;
          if (navigator.webdriver !== true) {
            const page = getManifest().homepage_url;
            const {name, version} = getManifest();
            onInstalled.addListener(({reason, previousVersion}) => {
              management.getSelf(({installType}) => installType === 'normal' && storage.local.get({
                'faqs': true,
                'last-update': 0
              }, prefs => {
                if (reason === 'install' || (prefs.faqs && reason === 'update')) {
                  const doUpdate = (Date.now() - prefs['last-update']) / 1000 / 60 / 60 / 24 > 45;
                  if (doUpdate && previousVersion !== version) {
                    tabs.query({active: true, lastFocusedWindow: true}, tbs => tabs.create({
                      url: page + '?version=' + version + (previousVersion ? '&p=' + previousVersion : '') + '&type=' + reason,
                      active: reason === 'install',
                      ...(tbs && tbs.length && {index: tbs[0].index + 1})
                    }));
                    storage.local.set({'last-update': Date.now()});
                  }
                }
              }));
            });
            setUninstallURL(page + '?rd=feedback&name=' + encodeURIComponent(name) + '&version=' + version);
          }
        }
        
        """;
    
    //<input type="button" value="Support Development" id="support">
    static string Index => """
        <!DOCTYPE html>
        <html>
        <head>
          <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>Options Page :: Chameleon WebRTC Protect</title>
          <link rel="stylesheet" type="text/css" href="index.css">
        </head>
        <body>
          <div class="second">
            <input type="radio" name="select" id="enabled">
            <label for="enabled">Enabled (recommended):</label>
            <span></span>
            <select id="when-enabled">
              <option value="disable_non_proxied_udp">Disable non-proxied UDP (force proxy)</option>
              <option value="proxy_only">Only connections using TURN on a TCP connection through a proxy</option>
            </select>
            <input type="radio" name="select" id="disabled">
            <label for="disabled">Disabled:</label>
            <span></span>
            <select id="when-disabled">
              <option value="default_public_interface_only">Use the default public interface only</option>
              <option value="default_public_and_private_interfaces">Use the default public interface and private interface</option>
            </select>
          </div>
          <p class="second">
            <input type="checkbox" id="device-enum-api">
            <label for="device-enum-api">Disable WebRTC Media Device Enumeration API</label>
          </p>

          <p class="fgtt">
            Can this extension protect incognito (private) mode? <span id="incognito">No</span>
            <div class="note">To enable this option in Chrome, check the "Allow in incognito" box for this extension.</div>
          </p>
          <div id="buttons">
            <input type="button" value="Factory Reset" id="reset">

            <input type="button" value="Save Options" id="save">
            <span id="toast"></span>
          </div>
          <script src="index.js"></script>
        </body>
        </html>
        """;
    static string IndexCSS => """
        body {
          font-size: 14px;
          font-family: Arial, "Helvetica Neue", Helvetica, sans-serif;
          background-color: #fff;
          color: #4d5156;
          margin: 10px;
        }
        select,
        button,
        input[type=submit],
        input[type=button] {
          height: 28px;
          color: #444;
          background-image: linear-gradient(rgb(237, 237, 237), rgb(237, 237, 237) 38%, rgb(222, 222, 222));
          box-shadow: rgba(0, 0, 0, 0.08) 0 1px 0, rgba(255, 255, 255, 0.75) 0 1px 2px inset;
          text-shadow: rgb(240, 240, 240) 0 1px 0;
        }
        select,
        button,
        textarea,
        input[type=submit],
        input[type=button] {
          border: solid 1px rgba(0, 0, 0, 0.25);
        }
        select {
          width: 100%;
        }
        input[type=button]:disabled {
          opacity: 0.5;
        }
        input[type=radio],
        input[type=checkbox] {
          margin: 0;
        }
        textarea {
          width: 100%;
          box-sizing: border-box;
          display: block;
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
        a,
        a:visited {
          color: #07c;
        }

        #incognito {
          color: #fff;
          background-color: #e41655;
          padding: 2px 10px;
          border-radius: 2px;
        }
        #incognito[data-enabled=true] {
          background-color: #1b8057;
        }

        .second {
          display: grid;
          grid-template-columns: min-content 1fr;
          grid-gap: 10px;
          align-items: center;
        }
        .fgtt {
          display: grid;
          grid-template-columns: 1fr min-content;
          grid-gap: 10px;
          align-items: center;
        }
        .note {
          background-color: rgba(0, 0, 0, 0.05);
          padding: 5px 10px;
          margin-top: 5px;
        }

        label[for="device-enum-api"] {
          grid-column: 2/4;
        }

        #buttons {
          display: grid;
          grid-template-columns: min-content 1fr;
          grid-gap: 5px;
          align-items: center;
        }
        #support {
          justify-self: start;
        }
        
        """;
    static string IndexJS => """
        'use strict';

        const toast = document.getElementById('toast');
        const isFF = /Firefox/.test(navigator.userAgent);

        const notify = msg => {
          toast.textContent = msg;
          clearTimeout(notify.id);
          notify.id = setTimeout(() => toast.textContent = '', 750);
        };

        if (isFF === false) {
          document.querySelector('[value="proxy_only"]').disabled = true;
        }

        chrome.extension.isAllowedIncognitoAccess(result => {
          document.getElementById('incognito').textContent = result ? 'Yes' : 'No';
          document.getElementById('incognito').dataset.enabled = result;
        });

        chrome.storage.local.get({
          enabled: true,
          eMode: isFF ? 'proxy_only' : 'disable_non_proxied_udp',
          dMode: 'default_public_interface_only',
          dAPI: true
        }, prefs => {
          document.getElementById(prefs.enabled ? 'enabled' : 'disabled').checked = true;
          document.getElementById('when-enabled').value = prefs.eMode;
          document.getElementById('when-disabled').value = prefs.dMode;
          document.getElementById('device-enum-api').checked = prefs.dAPI;
        });

        document.getElementById('save').onclick = () => chrome.storage.local.set({
          enabled: document.getElementById('enabled').checked,
          eMode: document.getElementById('when-enabled').value,
          dMode: document.getElementById('when-disabled').value,
          dAPI: document.getElementById('device-enum-api').checked
        }, () => {
          notify('Settings saved!');
        });

        // reset
        document.getElementById('reset').addEventListener('click', e => {
          if (e.detail === 1) {
            notify('Double-click to reset!');
          }
          else {
            localStorage.clear();
            chrome.storage.local.clear(() => {
              chrome.runtime.reload();
              window.close();
            });
          }
        });
        // support
        document.getElementById('support').addEventListener('click', () => chrome.tabs.create({
          url: chrome.runtime.getManifest().homepage_url + '?rd=donate'
        }));
        """;

    static string Isolated => """
        let port;
        try {
          port = document.getElementById('webrtc-protect');
          port.remove();
        }
        catch (e) {
          port = document.createElement('span');
          port.id = 'webrtc-protect';
          document.documentElement.append(port);
        }

        chrome.storage.local.get({
          dAPI: true
        }, prefs => {
          port.dataset.dAPI = prefs.dAPI;
        });
        chrome.storage.onChanged.addListener(ps => {
          if (ps.dAPI) {
            port.dataset.dAPI = ps.dAPI.newValue;
          }
        });
        """;
    static string Main => """
        {
          let port;
          try {
            port = document.getElementById('webrtc-protect');
            port.remove();
          }
          catch (e) {
            port = document.createElement('span');
            port.id = 'webrtc-protect';
            document.documentElement.append(port);
          }

          if (navigator.mediaDevices?.enumerateDevices) {
            navigator.mediaDevices.enumerateDevices = new Proxy(navigator.mediaDevices.enumerateDevices, {
              apply(target, self, args) {
                if (port.dataset.dAPI === 'true') {
                  return Promise.resolve([]);
                }
                return Reflect.apply(target, self, args);
              }
            });
          }
        }
        """;
}
