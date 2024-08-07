using Avalonia.Platform;
using Chameleon.Interfaces.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chameleon.SystemBrowser.Addons;
public class FontDefenderAddon
{

    public static async Task InitializeExtension(string dir)
    {
        //await AddonsUtil.LoadFromInternal(AddonsUtil.FontDefenderAddon, dir);
        //await IOtil.DC(dir);

        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(dir, "manifest.json"), Manifestv3);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(dir, "background.js"), Background);

        //var libDir = Path.Combine(dir, "lib");
        //await IOtil.CreateDirectory(libDir);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(libDir, "chrome.js"), Chrome);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(libDir, "common.js"), Common);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(libDir, "config.js"), CConfig);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(libDir, "runtime.js"), Runtime);

        //var dataDir = Path.Combine(dir, "data");

        //var optionsDir = Path.Combine(dataDir, "popup");
        //await IOtil.CreateDirectory(optionsDir);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(optionsDir, "popup.html"), PopupHTLM);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(optionsDir, "popup.css"), PopupCSS);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(optionsDir, "popup.js"), PopupJS);

        //var exploreoptionsDir = Path.Combine(optionsDir, "explore");
        //await IOtil.CreateDirectory(exploreoptionsDir);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(exploreoptionsDir, "explore.css"), ExploreCSS);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(exploreoptionsDir, "explore.json"), ExploreJSON);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(exploreoptionsDir, "explore.js"), ExploreJS);


        //var contentScriptDir = Path.Combine(dataDir, "content_script");
        //await IOtil.CreateDirectory(contentScriptDir);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(contentScriptDir, "inject.js"), Inject);

        //var contentScriptPageDir = Path.Combine(contentScriptDir, "page_context");
        //await IOtil.CreateDirectory(contentScriptPageDir);
        //await IOtil.WriteTextToFileAsync(
        //    Path.Combine(contentScriptPageDir, "inject.js"), InjectContent);
    }

    static string Manifestv3 => """
        {
          "version": "1.1.5",
          "manifest_version": 3,
          "offline_enabled": true,
          "name": "Chameleon Font Defender",
          "permissions": ["storage", "contextMenus", "notifications"],
          "description": "Defending against Font fingerprinting by reporting a obfuscated value.",
          "commands": {
            "_execute_action": {}
          },
          "background": {
            "service_worker": "background.js"
          },
          "action": {
            "default_popup": "data/popup/popup.html",
            "default_title": "Chameleon Font Defender",
            "default_icon": {
              "16": "data/icons/16.png",
              "32": "data/icons/32.png",
              "64": "data/icons/64.png",
              "128": "data/icons/128.png",
              "256": "data/icons/256.png",
              "512": "data/icons/512.png"
            }
          },
          "content_scripts": [
            {
              "world": "MAIN",
              "all_frames": true,
              "matches": ["*://*/*"],
              "match_about_blank": true,
              "run_at": "document_start",
              "match_origin_as_fallback": true,
              "js": ["data/content_script/page_context/inject.js"]
            },
            {
              "world": "ISOLATED",
              "all_frames": true,
              "matches": ["*://*/*"],
              "match_about_blank": true,
              "run_at": "document_start",
              "match_origin_as_fallback": true,
              "js": ["data/content_script/inject.js"]
            }
          ]
        }
        """;
    
    static string Background => """
        importScripts("lib/config.js");
        importScripts("lib/chrome.js");
        importScripts("lib/runtime.js");
        importScripts("lib/common.js");
        """;

    static string Chrome => """
        var app = {};

        app.error = function () {
          return chrome.runtime.lastError;
        };

        app.name = function () {
          return chrome.runtime.getManifest().name;
        };

        app.notifications = {
          "create": function (e, callback) {
            if (chrome.notifications) {
              chrome.notifications.create(app.notifications.id, {
                "type": e.type ? e.type : "basic",
                "message": e.message ? e.message : '',
                "title": e.title ? e.title : "Notifications",
              }, function (e) {
                if (callback) callback(e);
              });
            }
          }
        };

        app.popup = {
          "port": null,
          "message": {},
          "receive": function (id, callback) {
            if (id) {
              app.popup.message[id] = callback;
            }
          },
          "send": function (id, data) {
            if (id) {
              chrome.runtime.sendMessage({"data": data, "method": id, "path": "background-to-popup"}, app.error);
            }
          },
          "post": function (id, data) {
            if (id) {
              if (app.popup.port) {
                app.popup.port.postMessage({"data": data, "method": id, "path": "background-to-popup"});
              }
            }
          }
        };

        app.contextmenu = {
          "create": function (options, callback) {
            if (chrome.contextMenus) {
              chrome.contextMenus.create(options, function (e) {
                if (callback) callback(e);
              });
            }
          },
          "update": function (id, options, callback) {
            if (chrome.contextMenus) {
              chrome.contextMenus.update(id, options, function (e) {
                if (callback) callback(e);
              });
            }
          },
          "on": {
            "clicked": function (callback) {
              if (chrome.contextMenus) {
                chrome.contextMenus.onClicked.addListener(function (info, tab) {
                  app.storage.load(function () {
                    callback(info, tab);
                  });
                });
              }
            }
          }
        };

        app.tab = {
          "query": {
            "index": function (callback) {
              chrome.tabs.query({"active": true, "currentWindow": true}, function (tabs) {
                var tmp = chrome.runtime.lastError;
                if (tabs && tabs.length) {
                  callback(tabs[0].index);
                } else callback(undefined);
              });
            }
          },
          "open": function (url, index, active, callback) {
            var properties = {
              "url": url, 
              "active": active !== undefined ? active : true
            };
            /*  */
            if (index !== undefined) {
              if (typeof index === "number") {
                properties.index = index + 1;
              }
            }
            /*  */
            chrome.tabs.create(properties, function (tab) {
              if (callback) callback(tab);
            }); 
          }
        };

        app.storage = {
          "local": {},
          "read": function (id) {
            return app.storage.local[id];
          },
          "update": function (callback) {
            if (app.session) app.session.load();
            /*  */
            chrome.storage.local.get(null, function (e) {
              app.storage.local = e;
              if (callback) {
                callback("update");
              }
            });
          },
          "write": function (id, data, callback) {
            let tmp = {};
            tmp[id] = data;
            app.storage.local[id] = data;
            //
            chrome.storage.local.set(tmp, function (e) {
              if (callback) {
                callback(e);
              }
            });
          },
          "load": function (callback) {
            const keys = Object.keys(app.storage.local);
            if (keys && keys.length) {
              if (callback) {
                callback("cache");
              }
            } else {
              app.storage.update(function () {
                if (callback) callback("disk");
              });
            }
          } 
        };

        app.on = {
          "management": function (callback) {
            chrome.management.getSelf(callback);
          },
          "uninstalled": function (url) {
            chrome.runtime.setUninstallURL(url, function () {});
          },
          "installed": function (callback) {
            chrome.runtime.onInstalled.addListener(function (e) {
              app.storage.load(function () {
                callback(e);
              });
            });
          },
          "startup": function (callback) {
            chrome.runtime.onStartup.addListener(function (e) {
              app.storage.load(function () {
                callback(e);
              });
            });
          },
          "connect": function (callback) {
            chrome.runtime.onConnect.addListener(function (e) {
              app.storage.load(function () {
                if (callback) callback(e);
              });
            });
          },
          "storage": function (callback) {
            chrome.storage.onChanged.addListener(function (changes, namespace) {
              app.storage.update(function () {
                if (callback) {
                  callback(changes, namespace);
                }
              });
            });
          },
          "message": function (callback) {
            chrome.runtime.onMessage.addListener(function (request, sender, sendResponse) {
              app.storage.load(function () {
                callback(request, sender, sendResponse);
              });
              /*  */
              return true;
            });
          }
        };

        app.page = {
          "port": null,
          "message": {},
          "sender": {
            "port": {}
          },
          "receive": function (id, callback) {
            if (id) {
              app.page.message[id] = callback;
            }
          },
          "post": function (id, data, tabId) {
            if (id) {
              if (tabId) {
                if (app.page.sender.port[tabId]) {
                  app.page.sender.port[tabId].postMessage({"data": data, "method": id, "path": "background-to-page"});
                }
              } else if (app.page.port) {
                app.page.port.postMessage({"data": data, "method": id, "path": "background-to-page"});
              }
            }
          },
          "send": function (id, data, tabId, frameId) {
            if (id) {
              chrome.tabs.query({}, function (tabs) {
                var tmp = chrome.runtime.lastError;
                if (tabs && tabs.length) {
                  var message = {
                    "method": id, 
                    "data": data ? data : {}, 
                    "path": "background-to-page"
                  };
                  /*  */
                  tabs.forEach(function (tab) {
                    if (tab) {
                      message.data.tabId = tab.id;
                      message.data.top = tab.url ? tab.url : '';
                      message.data.title = tab.title ? tab.title : '';
                      /*  */
                      if (tabId !== null && tabId !== undefined) {
                        if (tabId === tab.id) {
                          if (frameId !== null && frameId !== undefined) {
                            chrome.tabs.sendMessage(tab.id, message, {"frameId": frameId}, app.error);
                          } else {
                            chrome.tabs.sendMessage(tab.id, message, app.error);
                          }
                        }
                      } else {
                        chrome.tabs.sendMessage(tab.id, message, app.error);
                      }
                    }
                  });
                }
              });
            }
          }
        };
        """;
    static string Common => """
                var core = {
          "start": function () {
            core.load();
          },
          "install": function () {
            core.load();
          },
          "action": {
            "contextmenu": function (e) {
              if (e.menuItemId === "test.page") {
                app.tab.open(config.test.page);
              } else {
                config.notification.show = !config.notification.show;
              }
            },
            "storage": function (changes, namespace) {
              if ("notification" in changes) {
                app.contextmenu.update("notification.checkbox", {
                  "checked": config.notification.show,
                }, app.error);
              }
            }
          },
          "load": function () {
            app.contextmenu.create({
              "type": "normal",
              "id": "test.page",
              "contexts": ["browser_action"],
              "title": "What is my Fingerprint?"
            }, app.error);
            /*  */
            app.contextmenu.create({
              "type": "checkbox",
              "id": "notification.checkbox",
              "contexts": ["browser_action"],
              "checked": config.notification.show,
              "title": "Show Desktop Notifications"
            }, app.error);
          }
        };

        app.popup.receive("support", function () {app.tab.open(app.homepage())});
        app.popup.receive("fingerprint", function () {app.tab.open(config.test.page)});
        app.popup.receive("donation", function () {app.tab.open(app.homepage() + "?reason=support")});

        app.popup.receive("load", function () {
          app.popup.send("storage", {
            "notifications": config.notification.show
          });
        });

        app.popup.receive("notifications", function () {
          config.notification.show = !config.notification.show;
          app.popup.send("storage", {
            "notifications": config.notification.show
          });
        });

        app.page.receive("fingerprint", function (e) {
          const message = "\nA fingerprinting attempt is detected!\nYour browser is reporting a fake value.";
          //
          if (config.notification.show) {
            if (config.notification.timeout) clearTimeout(config.notification.timeout);
            config.notification.timeout = setTimeout(function () {
              app.notifications.create({
                "type": "basic",
                "title": app.name(),
                "message": e.host + message
              });
            }, 1000);
          }
        });

        app.on.startup(core.start);
        app.on.installed(core.install);
        app.on.storage(core.action.storage);
        app.contextmenu.on.clicked(core.action.contextmenu);
        """;
    static string CConfig => """
        var config = {};

        config.test = {"page": "https://webbrowsertools.com/font-fingerprint/"};

        config.welcome = {
          set lastupdate (val) {app.storage.write("lastupdate", val)},
          get lastupdate () {return app.storage.read("lastupdate") !== undefined ? app.storage.read("lastupdate") : 0}
        };

        config.notification = {
          "timeout": null,
          set show (val) {app.storage.write("notification", val)},
          get show () {return app.storage.read("notification") !== undefined ? app.storage.read("notification") : false}
        };
        
        """;
    static string Runtime => """
        app.version = function () {return chrome.runtime.getManifest().version};
        app.homepage = function () {return chrome.runtime.getManifest().homepage_url};

        if (!navigator.webdriver) {
          app.on.uninstalled(app.homepage() + "?v=" + app.version() + "&type=uninstall");
          app.on.installed(function (e) {
            app.on.management(function (result) {
              if (result.installType === "normal") {
                app.tab.query.index(function (index) {
                  var previous = e.previousVersion !== undefined && e.previousVersion !== app.version();
                  var doupdate = previous && parseInt((Date.now() - config.welcome.lastupdate) / (24 * 3600 * 1000)) > 45;
                  if (e.reason === "install" || (e.reason === "update" && doupdate)) {
                    var parameter = (e.previousVersion ? "&p=" + e.previousVersion : '') + "&type=" + e.reason;
                    var url = app.homepage() + "?v=" + app.version() + parameter;
                    app.tab.open(url, index, e.reason === "install");
                    config.welcome.lastupdate = Date.now();
                  }
                });
              }
            });
          });
        }

        app.on.message(function (request, sender) {
          if (request) {
            if (request.path === "popup-to-background") {
              for (var id in app.popup.message) {
                if (app.popup.message[id]) {
                  if ((typeof app.popup.message[id]) === "function") {
                    if (id === request.method) {
                      app.popup.message[id](request.data);
                    }
                  }
                }
              }
            }
            /*  */
            if (request.path === "page-to-background") {
              for (var id in app.page.message) {
                if (app.page.message[id]) {
                  if ((typeof app.page.message[id]) === "function") {
                    if (id === request.method) {
                      var a = request.data || {};
                      if (sender) {
                        a.frameId = sender.frameId;
                        /*  */
                        if (sender.tab) {
                          if (a.tabId === undefined) a.tabId = sender.tab.id;
                          if (a.title === undefined) a.title = sender.tab.title ? sender.tab.title : '';
                          if (a.top === undefined) a.top = sender.tab.url ? sender.tab.url : (sender.url ? sender.url : '');
                        }
                      }
                      /*  */
                      app.page.message[id](a);
                    }
                  }
                }
              }
            }
          }
        });

        app.on.connect(function (port) {
          if (port) {
            if (port.name) {
              if (port.name in app) {
                app[port.name].port = port;
              }
            }
            /*  */
            port.onDisconnect.addListener(function (e) {
              app.storage.load(function () {
                if (e) {
                  if (e.name) {
                    if (e.name in app) {
                      app[e.name].port = null;
                    }
                  }
                }
              });
            });
            /*  */
            port.onMessage.addListener(function (e) {
              app.storage.load(function () {
                if (e) {
                  if (e.path) {
                    if (e.port) {
                      if (e.port in app) {
                        if (e.path === (e.port + "-to-background")) {
                          for (var id in app[e.port].message) {
                            if (app[e.port].message[id]) {
                              if ((typeof app[e.port].message[id]) === "function") {
                                if (id === e.method) {
                                  app[e.port].message[id](e.data);
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              });
            });
          }
        });
        
        """;

                      //    <tr>
                      //  <td class="icon support">
                      //    <svg height = "11" width="11" viewBox="0 0 1792 1792">
                      //      <path d = "M1088 1256v240q0 16-12 28t-28 12h-240q-16 0-28-12t-12-28v-240q0-16 12-28t28-12h240q16 0 28 12t12 28zm316-600q0 54-15.5 101t-35 76.5-55 59.5-57.5 43.5-61 35.5q-41 23-68.5 65t-27.5 67q0 17-12 32.5t-28 15.5h-240q-15 0-25.5-18.5t-10.5-37.5v-45q0-83 65-156.5t143-108.5q59-27 84-56t25-76q0-42-46.5-74t-107.5-32q-65 0-108 29-35 25-107 115-13 16-31 16-12 0-25-8l-164-125q-13-10-15.5-25t5.5-28q160-266 464-266 80 0 161 31t146 83 106 127.5 41 158.5z" />
                      //    </ svg >
                      //  </ td >
                      //  < td class="button" id="support">Open support page</td>
                      //</tr>
                      //<tr>
                      //  <td class="icon donation">
                      //    <svg height = "11" width="11" viewBox="-0.709 -11.555 141.732 141.732">
                      //      <path d = "M140.314,37.654C140.314,16.858,123.402,0,102.537,0c-13.744,0-25.77,7.317-32.379,18.255C63.549,7.317,51.521,0,37.777,0 C16.912,0,0,16.858,0,37.654c0,10.821,4.588,20.57,11.922,27.438h-0.01l54.084,51.584c0.992,1.188,2.48,1.945,4.148,1.945 c1.545,0,2.936-0.653,3.92-1.696l54.346-51.833h-0.016C135.729,58.225,140.314,48.476,140.314,37.654" />
                      //    </ svg >
                      //  </ td >
                      //  < td class="button" id="donation">Make a donation</td>
                      //</tr>
    static string PopupHTLM => """
        <!DOCTYPE html>
        <html>
          <head>
            <meta charset="utf-8">
            <link type="text/css" rel="stylesheet" href="popup.css">
          </head>
          <body>
            <div class="content">
                <table class="buttons">
                  <tr>
                    <td class="icon fingerprint">
                      <svg width="13" height="13" viewBox="0 0 512 512">
                        <path d="M 256.2172,246.23419 C 243.32969,246.23419 232.87371,256.68034 232.87371,269.57757 233.98259,339.85106 224.9564,407.59571 205.93146,475.34023 203.27626,484.79439 208.02278,504.99613 228.41906,504.99613 238.61235,504.99613 247.97887,498.2655 250.87734,487.95545 264.03723,441.35605 281.06825,365.97627 279.55085,269.57757 279.56056,256.69006 269.11442,246.23419 256.2172,246.23419 Z M 255.38074,166.74012 C 195.79665,166.67205 154.21606,212.52233 155.04282,265.06461 155.77237,311.69301 151.39549,358.35071 142.03868,403.71483 139.42222,416.33976 147.55349,428.70197 160.16868,431.299 172.86165,433.89587 185.13639,425.79377 187.75285,413.16884 197.80992,364.44928 202.50783,314.36779 201.72969,264.33505 201.33089,239.09506 221.10482,213.28103 254.69006,213.42696 285.14353,213.88408 310.27663,238.07377 310.73387,267.37947 311.48273,314.11494 308.00062,361.0741 300.41409,407.00239 298.30348,419.70507 306.92102,431.72694 319.63342,433.82785 339.05716,437.0668 345.71003,419.1409 346.45902,414.59865 354.51253,365.93739 358.18903,316.15751 357.42073,266.64992 356.56468,212.37653 310.79219,167.55714 255.38074,166.74012 Z M 147.71885,147.5012 C 137.70068,139.38936 123.0429,140.88721 114.88247,150.88594 89.963314,181.52425 76.570011,220.21606 77.192506,259.82196 77.766368,296.374 74.79009,333.03282 68.331725,368.80671 66.055751,381.49967 74.469106,393.63819 87.162102,395.9336 106.72189,399.33795 113.49155,381.52883 114.28914,377.10336 121.28248,338.35322 124.50186,298.64998 123.87936,259.09255 123.42225,230.02999 132.82766,202.79602 151.11334,180.33759 159.23489,170.33883 157.71761,155.64217 147.71885,147.5012 Z M 254.19407,86.876373 C 239.20567,86.477592 224.16853,88.276986 209.66643,91.710404 197.12911,94.686695 189.36748,107.25322 192.32428,119.8101 195.30051,132.34742 207.88656,140.07018 220.42386,137.15226 231.17159,134.61369 242.27926,133.48549 253.49381,133.56325 326.8602,134.66228 387.45585,193.38074 388.57448,264.46155 389.14822,301.30519 387.32949,338.48943 383.14714,374.98288 381.68811,387.79264 390.87962,399.37683 403.67951,400.83579 419.94218,402.73243 428.49168,389.28076 429.53242,380.30329 A 903.64367,903.64367 0 0 0 435.26133,263.73199 C 433.74392,167.6835 352.51857,88.335338 254.19407,86.876373 Z M 499.36824,205.00383 C 496.58639,192.43736 484.2825,184.38385 471.54093,187.23372 458.95489,190.01543 450.99871,202.46525 453.78042,215.06103 458.36155,235.88527 458.55609,251.44756 458.35184,274.97573 458.24478,287.88281 468.61316,298.40672 481.5008,298.51377 H 481.69519 C 494.50494,298.51377 504.93163,288.19399 505.03869,275.3648 505.2138,253.79158 505.42778,232.46162 499.36824,205.00383 Z M 460.3458,116.7657 C 412.978,49.273995 335.46816,8.2576457 252.99768,7.0223887 185.11696,6.2053689 122.33293,31.231476 75.636274,77.947513 30.534829,123.10739 6.2284787,183.09026 7.240027,246.90529 L 7.1233101,267.78798 C 6.7439781,280.67549 16.878923,291.43294 29.766441,291.80259 29.990152,291.82202 30.233304,291.82202 30.466746,291.82202 43.033287,291.82202 53.411384,281.8037 53.780991,269.15933 L 53.936622,246.16615 C 53.129326,195.10241 72.572456,147.09267 108.67701,110.96874 146.37654,73.230195 197.51819,52.931143 252.31688,53.718984 319.867,54.720801 383.36111,88.306161 422.15985,143.58143 429.56158,154.14433 444.10258,156.66347 454.66548,149.28119 465.20897,141.8696 467.75725,127.30917 460.3458,116.7657 Z"/>
                      </svg>
                    </td>
                    <td class="button" id="fingerprint">What is my Fingerprint</td>
                  </tr>
                </table>
            </div>
        <script type="text/javascript" src="popup.js"></script>
          </body>
        </html>
        """;
    static string PopupCSS => """
        html, body {
          height: 0;
        }

        body {
          border: 0;
          margin: 0;
          padding: 0;
          width: 500px;
        }

        .content {
          border: 0;
          margin: 0;
          padding: 0;
          width: 100%;
        }

        .content table {
          width: 100%;
          margin: auto;
          border-spacing: 0;
        }

        .content > table {
          border-spacing: 0 10px;
          border-bottom: solid 1px rgba(0,0,0,0.1);
        }

        .content table tr td {
          color: #555;
          font-size: 12px;
          font-family: arial,sans-serif;
        }

        .content .logo {
          height: 110px;
          background-size: 64px;
        }

        .content .name {
          color: #777;
          user-select: none;
          text-align: center;
        }

        .content .buttons {
          height: 150px;
          border-left: solid 1px rgba(0,0,0,0.1);
        }

        .content .buttons .icon {
          width: 32px;
          font-size: 17px;
          cursor: pointer;
          line-height: 37px;
          user-select: none;
          text-align: center;
          font-family: monospace;
        }

        .content .buttons .icon svg {
          fill: #777;
          margin: -5px 0 0 0;
          vertical-align: middle;
        }

        .content .buttons .button {
          padding: 0;
          cursor: pointer;
          text-indent: 5px;
          user-select: none;
        }

        .content .buttons tr {
          transition: 300ms ease all;
        }

        .content .buttons tr:hover {
          background-color: rgba(0,0,0,0.1);
        }

        @-moz-document url-prefix() {
          html, body {
            height: auto;
          }
        }
        """;
    static string PopupJS => """
          var background = {
          "port": null,
          "message": {},
          "receive": function (id, callback) {
            if (id) {
              background.message[id] = callback;
            }
          },
          "send": function (id, data) {
            if (id) {
              chrome.runtime.sendMessage({
                "method": id,
                "data": data,
                "path": "popup-to-background"
              }, function () {
                return chrome.runtime.lastError;
              });
            }
          },
          "connect": function (port) {
            chrome.runtime.onMessage.addListener(background.listener); 
            /*  */
            if (port) {
              background.port = port;
              background.port.onMessage.addListener(background.listener);
              background.port.onDisconnect.addListener(function () {
                background.port = null;
              });
            }
          },
          "post": function (id, data) {
            if (id) {
              if (background.port) {
                background.port.postMessage({
                  "method": id,
                  "data": data,
                  "path": "popup-to-background",
                  "port": background.port.name
                });
              }
            }
          },
          "listener": function (e) {
            if (e) {
              for (let id in background.message) {
                if (background.message[id]) {
                  if ((typeof background.message[id]) === "function") {
                    if (e.path === "background-to-popup") {
                      if (e.method === id) {
                        background.message[id](e.data);
                      }
                    }
                  }
                }
              }
            }
          }
        };

        var config = {
          "render": function (e) {
            let name = document.querySelector(".name");
            let notifications = document.querySelector(".notifications");
            /*  */
            name.textContent = chrome.runtime.getManifest().name;
            notifications.textContent = e.notifications ? '☑' : '☐';
          },
          "load": function () {
            let ids = ["fingerprint"];
            //
            for (let i = 0; i < ids.length; i++) {
              let icon = document.querySelector("." + ids[i]);
              let button = document.querySelector("#" + ids[i]);
              /*  */
              button.addEventListener("click", function (e) {background.send(e.target.id)});
              icon.addEventListener("click", function (e) {background.send(e.target.className.replace("icon ", ''))});
            }
            /*  */
            if (navigator.userAgent.indexOf("Edg") !== -1) {
              document.getElementById("explore").style.display = "none";
            }
            /*  */
            background.send("load");
            window.removeEventListener("load", config.load, false);
          }
        };

        background.receive("storage", config.render);
        window.addEventListener("load", config.load, false);
        background.connect(chrome.runtime.connect({"name": "popup"}));
        """;
    static string ExploreCSS => """
        #explore {
          padding: 0;
          color: #565252;
          min-height: 18px;
          position: relative;
          box-sizing: border-box;
          background: transparent;
        }

        #explore span {
          user-select: none;
          vertical-align: middle;
          -moz-user-select: none;
          -webkit-user-select: none;
        }

        #explore[data-loaded=true] {
          border: 0;
          margin: 0;
          padding: 5px;
          font-size: 12px;
          background: transparent;
          font-family: arial, sans-serif;;
        }

        #explore .container {
          border: 0;
          width: 100%;
          margin: auto;
          font-size: 12px;
          margin-top: 10px;
          border-spacing: 0;
          table-layout: fixed;
        }

        #explore .container tr {
          outline: none;
          background-color: transparent;
        }

        #explore .container tr td {
          border: 0;
          margin: 0;
          padding: 0;
          box-shadow: none;
        }

        #explore .explore {
          top: 0;
          right: 0;
          margin: 0;
          cursor: pointer;
          font-size: 12px;
          z-index: 1000000;
          line-height: 15px;
          position: absolute;
          padding: 1px 5px 0 0;
          color: rgba(0,0,0,0.3);
        }

        #explore .close {
          top: 3px;
          right: 3px;
          cursor: pointer;
          font-size: 11px;
          padding: 1px 8px;
          position: absolute;
          background-color: transparent;
        }

        #explore a {
          border: 0;
          margin: 0;
          padding: 5px;
          display: flex;
          color: #565252;
          padding-left: 10px;
          text-align: center;
          align-items: center;
          text-decoration: none;
          justify-content: center;
        }

        #explore .icon {
          margin: 0;
          padding: 0;
          width: 24px;
          color: #FFF;
          height: 24px;
          font-size: 11px;
          min-width: 24px;
          line-height: 24px;
          text-align: center;
          font-weight: normal;
          display: inline-block;
          font-family: arial, sans-serif;
        }

        #explore .spacer {
          border-left: solid 1px rgba(0,0,0,0.2) !important;
        }

        #explore .name {
          padding: 0;
          overflow: hidden;
          margin: 0 0 0 5px;
          font-weight: normal;
          white-space: nowrap;
          display: inline-block;
          text-overflow: ellipsis;
          font-family: arial, sans-serif;
        }

        #explore a, #explore .close, #explore .explore {
          transition: 300ms ease all;
          -moz-transition: 300ms ease all;
          -webkit-transition: 300ms ease all;
        }

        #explore .close:hover {
          color: #FFF;
          background-color: #C75050;
        }

        #explore a:hover, #explore .explore:hover {
          background-color: rgba(0,0,0,0.03);
        }
        
        """;
    static string ExploreJSON => """
         [
          {"id": "block-site", "title": "Block Site"},
          {"id": "smart-https", "title": "Smart HTTPS"},
          {"id": "rule-blocker", "title": "Rule AdBlocker"},
          {"id": "webapi-blocker", "title": "WebAPI Blocker"},
          {"id": "noscript-lite", "title": "No-Script Suite Lite"},
          {"id": "hide-tabs", "title": "Hide Tabs (Panic Button)"},
          {"id": "block-image-video", "title": "Block Image|Video"},
          {"id": "block-miners", "title": "NoMiner - Block Coin Miners"},
          {"id": "webgl-defender", "title": "WebGL Fingerprint Defender"},
          {"id": "html-content-blocker", "title": "HTML Content Blocker"},
          {"id": "javascript-switch", "title": "JavaScript Switch ON|OFF"},
          {"id": "canvas-defender", "title": "Canvas Fingerprint Defender"},
          {"id": "notrack", "title": "NoTrack - Block Redirection Tracking"},
          {"id": "change-timezone", "title": "Change Timezone (Time Shift)"},
          {"id": "file-encryptor", "title": "File Guard (Encryptor | Decryptor)"},
          {"id": "modify-header-value", "title": "Modify Header Value (HTTP Headers)"},
          {"id": "change-geolocation", "title": "Change Geolocation (location Guard)"},
          {"id": "audiocontext-defender", "title": "AudioContext Fingerprint Defender"},
          {"id": "content-security-policy", "title": "Allow CSP: Content-Security-Policy"},
          {"id": "access-control-allow-origin", "title": "Allow CORS: Access-Control-Allow-Origin"}
        ]
        """;
    static string ExploreJS => """
         {
          const INC = 50;
          const SORT = localStorage.getItem('explore-sort') ? Number(localStorage.getItem('explore-sort')) : 1;
          const COUNT = localStorage.getItem('explore-count') ? Number(localStorage.getItem('explore-count')) : (INC - 5);

          var randcolor = function () {
            var color = [
              "#D92121", "#E77200", "#5E8C31", "#00755E", "#C7A00F",
              "#0066FF", "#3F26BF", "#733380", "#BB3385", "#E30B5C",
              "#CA3435", "#87421F", "#299617", "#E936A7", "#DB91EF",
              "#214FC6", "#B56917", "#BB3385", "#652DC1", "#02A4D3"
            ];
            /*  */
            return color[Math.floor(Math.random() * color.length)];
          };

          const cload = () => fetch("explore/explore.json").then(r => r.json()).then(build);

          const shuffle = function (a) {
            for (let i = a.length - 1; i > 0; i--) {
              const j = Math.floor(Math.random() * (i + 1));
              [a[i], a[j]] = [a[j], a[i]];
            }
            /*  */
            return a;
          };

          const explore = () => {
            const root = document.getElementById('explore');
            const span = document.createElement('span');
            span.textContent = '◱';
            span.title = 'Explore more';
            span.classList.add('explore');
            root.appendChild(span);
            span.onclick = () => {
              root.textContent = '';
              localStorage.setItem('explore-count', INC);
              cload();
            };
          };

          const build = json => {
            if (json.length === 0) return;
            /*  */
            if (SORT % 4 === 0) {
              json = shuffle(json);
              localStorage.setItem('explore-sort', 1);
              localStorage.setItem('explore-json', JSON.stringify(json));
            } else {
              localStorage.setItem('explore-sort', SORT + 1);
              json = localStorage.getItem('explore-json') ? JSON.parse(localStorage.getItem('explore-json')) : json;
            }
            /*  */
            const root = document.getElementById('explore');
            root.textContent = 'Explore more';
            root.dataset.loaded = true;
            /*  */
            const table = document.createElement('table');
            const span = document.createElement('span');
            const tr = document.createElement('tr');
            /*  */
            table.setAttribute("class", "container");
            span.classList.add('close');
            span.textContent = '✕';
            /*  */
            span.onclick = () => {
              root.textContent = '';
              root.dataset.loaded = false;
              localStorage.setItem("explore-count", 0);
              explore();
            };
            /*  */
            root.appendChild(span);
            table.appendChild(tr);
            root.appendChild(table);
            /*  */
            json.slice(0, 4).forEach(({id, title}, index) => {
              if (id && title) {
                const a = document.createElement('a');
                const td = document.createElement('td');
                const homepage = chrome.runtime.getManifest().homepage_url;
                const short = homepage.split('/').pop().split('.').shift();
                const url = homepage.split('/').slice(0, -1).join('/') + '/';
                a.href = url + id + ".html?context=explore&from=" + short;
                a.setAttribute("title", title);
                a.target = '_blank';
                /*  */
                const icon = document.createElement('span');
                icon.textContent = title.replace(' -', '').split(' ').map(e => e[0]).slice(0, 2).join('').toUpperCase();
                icon.style.backgroundColor = randcolor();
                icon.setAttribute("class", "icon");
                a.appendChild(icon);
                /*  */
                const name = document.createElement('span');
                name.setAttribute("class", "name");
                name.textContent = title;
                a.appendChild(name);
                /*  */
                if (index) td.setAttribute("class", "spacer");
                td.appendChild(a);
                tr.appendChild(td);
              }
            });
          };

          if (COUNT >= INC) {
            if (COUNT < INC + 4) cload(); else explore();
            /*  */
            if (COUNT > INC + 5) localStorage.setItem('explore-count', INC);
            else localStorage.setItem('explore-count', COUNT + 1);
          } else {
            explore();
            localStorage.setItem('explore-count', COUNT + 1);
          }
        }
        
        """;

    static string Inject => """
          var background = (function () {
          let tmp = {};
          /*  */
          chrome.runtime.onMessage.addListener(function (request) {
            for (let id in tmp) {
              if (tmp[id] && (typeof tmp[id] === "function")) {
                if (request.path === "background-to-page") {
                  if (request.method === id) {
                    tmp[id](request.data);
                  }
                }
              }
            }
          });
          /*  */
          return {
            "receive": function (id, callback) {
              tmp[id] = callback;
            },
            "send": function (id, data) {
              chrome.runtime.sendMessage({
                "method": id, 
                "data": data,
                "path": "page-to-background"
              }, function () {
                return chrome.runtime.lastError;
              });
            }
          }
        })();

        const ikey = "font-defender-sandboxed-frame";

        if (document.documentElement.getAttribute(ikey) === null) {
          parent.postMessage(ikey, '*');
          window.top.postMessage(ikey, '*');
        } else {
          document.documentElement.removeAttribute(ikey);
        }

        window.addEventListener("message", function (e) {
          if (e.data && e.data === "font-defender-alert") {
            e.preventDefault();
            e.stopPropagation();
            /*  */
            background.send("fingerprint", {
              "host": document.location.host
            });
          }
        }, false);
        """;
    static string InjectContent => """
         {
          const rand = {
            "noise": function () {
              const SIGN = Math.random() < Math.random() ? -1 : 1;
              return Math.floor(Math.random() + SIGN * Math.random());
            },
            "sign": function () {
              const tmp = [-1, -1, -1, -1, -1, -1, +1, -1, -1, -1];
              const index = Math.floor(Math.random() * tmp.length);
              return tmp[index];
            }
          };
          //
          Object.defineProperty(HTMLElement.prototype, "offsetHeight", {
            "get": new Proxy(Object.getOwnPropertyDescriptor(HTMLElement.prototype, "offsetHeight").get, {
              apply(target, self, args) {
                try {
                  const height = Math.floor(self.getBoundingClientRect().height);
                  const valid = height && rand.sign() === 1;
                  const result = valid ? height + rand.noise() : height;
                  //
                  if (valid && result !== height) {
                    window.top.postMessage("font-defender-alert", '*');
                  }
                  //
                  return result;
                } catch (e) {
                  //return Reflect.apply(target, self, args);
                }
              }
            })
          });
          //
          Object.defineProperty(HTMLElement.prototype, "offsetWidth", {
            "get": new Proxy(Object.getOwnPropertyDescriptor(HTMLElement.prototype, "offsetWidth").get, {
              apply(target, self, args) {
                const width = Math.floor(self.getBoundingClientRect().width);
                const valid = width && rand.sign() === 1;
                const result = valid ? width + rand.noise() : width;
                //
                if (valid && result !== width) {
                  window.top.postMessage("font-defender-alert", '*');
                }
                //
                return result;
              }
            })
          });
        }

        {
          const mkey = "font-defender-sandboxed-frame";
          document.documentElement.setAttribute(mkey, '');
          //
          window.addEventListener("message", function (e) {
            if (e.data && e.data === mkey) {
              e.preventDefault();
              e.stopPropagation();
              //
              if (e.source) {
                if (e.source.HTMLElement) {
                  Object.defineProperty(e.source.HTMLElement.prototype, "offsetWidth", {
                    "get": Object.getOwnPropertyDescriptor(HTMLElement.prototype, "offsetWidth").get
                  });
                  //
                  Object.defineProperty(e.source.HTMLElement.prototype, "offsetHeight", {
                    "get": Object.getOwnPropertyDescriptor(HTMLElement.prototype, "offsetHeight").get
                  });
                }
              }
            }
          }, false);
        }
        """;
}
