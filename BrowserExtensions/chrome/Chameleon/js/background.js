debugger;

var __noop = function(){};

var application = {
	enabled: false,
	disable: function () {
		application.log.info('application:disable');
		this.enabled = false;
	},
	enable: function () {
		application.log.info('application:enable');
		this.enabled = true;
	},

	log: {
		_call: function (fn, args) {
			args = Array.prototype.slice.call(args);
			fn.apply(console, args);
		},
		info: function () {
			this._call(console.info, arguments);
		},
		warn: function () {
			this._call(console.warn, arguments);
		},
		error: function () {
			debugger;
			this._call(console.error, arguments);
		},
	},

	error: {
		throw: function (message) {
			var error = new Error(message);
			application.log.error(error);
			this.show(message);
			throw error;
		},
		show: function (message) {
			message = 'Error: ' + message;
			application.tabs.getActive(function (tab) {
				application.toast.show(tab.id, message, 10000);
			});
        }
	},

	toast: {
		show: function (tabId, message, timeout, handler) {
			handler = handler || __noop;
			timeout = timeout || 3000;
			var code = this._createJsCode(message, timeout);
			application.tabs.executeScript(tabId, code, handler);
		},

		_createJsCode: function (message, timeout) {
			return `(function() {
	debugger;
	var timeout = ${timeout};
	var message = '${message}';
	var containerId = 'snackbar';
	var containerTextId = 'snackbar-text';
	var containerTitleId = 'snackbar-title';
	var containerTimerId = 'snackbar-timer';

	var snackbar = window.__snackbar;
	if (!snackbar){
		snackbar = {};
		window.__snackbar = snackbar;
	}

	function clearTimers(){
		var snackbar = window.__snackbar;
		if (snackbar.intervalId){
			clearInterval(snackbar.intervalId);
			snackbar.intervalId = null;
		}

		if (snackbar.timeoutId){
			clearTimeout(snackbar.timeoutId);
			snackbar.timeoutId = null;
		}
	}

	clearTimers();

	// just place a div at top right
	var titleElement, textElement, timerElement;
	var containerElement = document.getElementById(containerId);
	if (containerElement) {
		textElement = document.getElementById(containerTextId);
		titleElement = document.getElementById(containerTitleId);
		timerElement = document.getElementById(containerTimerId);
	} else {
		containerElement = document.createElement('div');
		containerElement.setAttribute('id', containerId);
		document.body.appendChild(containerElement);

		titleElement = document.createElement('div');
		titleElement.setAttribute('id', containerTitleId);
		titleElement.textContent = 'Chameleon';
		containerElement.appendChild(titleElement);

		textElement = document.createElement('div');
		textElement.setAttribute('id', containerTextId);
		containerElement.appendChild(textElement);

		timerElement = document.createElement('div');
		timerElement.setAttribute('id', containerTimerId);
		containerElement.appendChild(timerElement);

		const style = document.createElement('style');
		style.textContent = \`${this._css}\`;
		document.head.append(style);
	}

	textElement.textContent = message;

	var countdown = Math.round(timeout/1000);
	timerElement.textContent = 'Will be closed within ' + countdown + ' seconds';

	containerElement.className = 'show';

	snackbar.intervalId = setInterval(function(){
		if (--countdown > 0){
			timerElement.textContent = 'Will be closed within ' + countdown + ' seconds';
		} else {
			timerElement.textContent = 'Will be closed automatically';
		}
	}, 1000);

	snackbar.timeoutId = setTimeout(function(){
		clearTimers();
		timerElement.textContent = 'Popup is closing automatically';
		containerElement.className = 'fadeout';
	}, timeout);
})();
`
		},

		_css: `
/* The snackbar - position it at the bottom and in the middle of the screen */
#snackbar {
  visibility: hidden; /* Hidden by default. Visible on click */
  min-width: 250px; /* Set a default minimum width */
  margin-left: -125px; /* Divide value of min-width by 2 */
  background-color: #333; /* Black background color */
  color: #fff; /* White text color */
  text-align: center; /* Centered text */
  border-radius: 2px; /* Rounded borders */
  padding: 8px 12px; /* Padding */
  position: fixed; /* Sit on top of the screen */
  z-index: 2; /* Add a z-index if needed */
  left: 50%; /* Center the snackbar */
  bottom: 30px; /* 30px from the bottom */
}

#snackbar-title{
  text-align: left;
  color: #00bfff;
  margin-bottom: 4px;
  font-size: 14px;
  font-weight: bold;
}

#snackbar-timer{
  text-align: right;
  font-size: 12px;
  margin-top: 4px;
}

/* Show the snackbar when clicking on a button (class added with JavaScript) */
#snackbar.show {
  visibility: visible !important; /* Show the snackbar */
  /* Add animation: Take 0.5 seconds to fade in and out the snackbar.
  However, delay the fade out process for 2.5 seconds */
  -webkit-animation: fadein 0.5s;
  animation: fadein 0.5s;
}

#snackbar.fadeout {
  visibility: visible !important; /* Visible the snackbar */
  -webkit-animation: fadeout 0s 2s;
  animation: fadeout 0s 2s;
}

/* Animations to fade the snackbar in and out */
@-webkit-keyframes fadein {
  from {bottom: 0; opacity: 0;}
  to {bottom: 30px; opacity: 1;}
}

@keyframes fadein {
  from {bottom: 0; opacity: 0;}
  to {bottom: 30px; opacity: 1;}
}

@-webkit-keyframes fadeout {
  from {bottom: 30px; opacity: 1;}
  to {bottom: 0; opacity: 0;}
}

@keyframes fadeout {
  from {bottom: 30px; opacity: 1;}
  to {bottom: 0; opacity: 0;}
}
`,
	},
	
	manifest: {
		_userProfileId: 0,
		_applicationPort: 0,
		_browserType: '',

		initialize: function () {
			if (!!this._userProfileId) {
				return;
			}

			var minifest = chrome.runtime.getManifest();
			this._userProfileId = minifest.userProfileId;
			if (!this._userProfileId) {
				application.error.throw(
					'Extension manifest missing userProfileId setting'
				);
				return;
			}
			application.log.info('manifest:initialize:userProfileId: ' + this._userProfileId);

			this._applicationPort = minifest.applicationPort;
			if (!this._applicationPort) {
				application.error.throw(
					'Extension manifest missing applicationPort setting'
				);
				return;
			}
			application.log.info('manifest:initialize:applicationPort: ' + this._applicationPort);

			this._browserType = minifest.browserType;
			if (!this._browserType) {
				application.error.throw(
					'Extension manifest missing browserType setting'
				);
				return;
			}

			application.log.info('manifest:initialize:browserType: ' + this._browserType);

			application.log.info('manifest:initialize');
		},

		getUserProfileId: function () {
			this.initialize();
			return this._userProfileId;
		},

		getApplicationPort: function () {
			this.initialize();
			return this._applicationPort;
		},

		getBrowserType: function () {
			this.initialize();
			return this._browserType;
		},
		
		getApplicationUrl: function(){
			this.initialize();
			return 'http://127.0.0.1:' + this._applicationPort;
		},

		isApplicationUrl: function (url) {
			var applicationUrl = this.getApplicationUrl();
			if (url.indexOf(applicationUrl) == 0) {
				return true;
			}
			return false;
        },
		
		isChrome: function() {
			return application.manifest._browserType.trim().toLowerCase() == 'chrome'
		}
	},

	cookies: {
		all: [],
		getAll: function (handler) {
			var self = this;
			chrome.cookies.getAll({}, function (cookies) {
				self.all = cookies;
				application.log.info('cookies:getAll');
				if (handler) {
					handler(cookies);
				}
			});
		},

		clearAll: function() {
			return new Promise(clearAllResolve => {
				application.log.info('cookies:clearAll');
				
				chrome.cookies.getAll({}, rm_cookies => {
					const cookie_rm_promises = [];
					
					for (const cookie of rm_cookies) {
						const url = "http" + (cookie.secure ? "s" : "") + ":" +
									cookie.domain +
									cookie.path;
						const rm_promise = new Promise(removeResolve => { 
							chrome.cookies.remove({ url, name: cookie.name }, removeResolve);
						});
						cookie_rm_promises.push(rm_promise);
					}
				
					Promise.all(cookie_rm_promises)
						.then(clearAllResolve);
				});
		 });
		},

		setMultiple: function(cookies) {
			return new Promise(setMultipleResolve => {
				application.log.info('cookies:setMultiple');
				
				const cookie_set_promises = [];
				
				for(const cookie of cookies){
					const set_promise = new Promise(setResolve => { 
						chrome.cookies.set(cookie, setResolve);
					});
					cookie_set_promises.push(set_promise);
				}
				
				Promise.all(cookie_set_promises)
					.then(setMultipleResolve);
			});
		},

		_onChanged: function (handler) {
			application.log.info('cookies:_onChanged');
			this.getAll(handler);
		},

		_onChangedTimerId: 0,
		_stopChangedTimerId: function () {
			if (this._onChangedTimerId !== 0) {
				clearTimeout(this._onChangedTimerId);
				this._onChangedTimerId = 0;
			}
		},

		_startOnChangedTimer: function (handler) {
			var self = this;
			self._stopChangedTimerId();
			self._onChangedTimerId = setTimeout(function () {
				self._stopChangedTimerId();
				self._onChanged(handler);
			}, 1000);
		},

		watch: function (handler) {
			var self = this;
			chrome.cookies.onChanged.addListener(function () {
				application.log.info('cookies:onChanged');
				self._startOnChangedTimer(handler);
			});
		}
	},

	server: {
		_baseUrl: undefined,
		getBaseUrl: function () {
			if (!!this._baseUrl) {
				return this._baseUrl;
			}

			var manifest = application.manifest;
			var applicationPort = manifest.getApplicationPort();
			this._baseUrl = "http://127.0.0.1:" + applicationPort + "/";
			return this._baseUrl;
		},

		send: function (relativeUrl, data, callback) {
			callback = callback || __noop;
			var request = new XMLHttpRequest();
			var serverUrl = this.getBaseUrl() + relativeUrl;
			request.onreadystatechange = function () {
				if (request.readyState == 4) {
					callback(request);
				}
				application.log.info('server:readyState', request.readyState);
			};

			request.open("POST", serverUrl, true);
			request.setRequestHeader("Content-type", "application/json");

			data.browserType = application.manifest.getBrowserType();
			data.userProfileId = application.manifest.getUserProfileId();
			var json = JSON.stringify(data, null, 4);
			application.log.info('server:send');
			request.send(json);
		}
	},

	http: {
		watch(handler) {
			var webRequest = chrome.webRequest;

			webRequest.onCompleted.addListener(
				handler, this._filter, this._options
			);

			webRequest.onBeforeRedirect.addListener(
				handler, this._filter, this._options
			);

			webRequest.onErrorOccurred.addListener(
				handler, this._filter, ["extraHeaders"]
			);
		},

		getHeaderByName: function (details, headerName) {
			var header = details.responseHeaders.filter(function (item) {
				return item.name == headerName;
			})[0];
			if (header && header.value) {
				return header.value;
			}
			return undefined;
		},

		getLocationHeader: function (details) {
			return this.getHeaderByName(details, 'location');
		},

		_initialized: false,
		_intialize: function () {
			if (this._initialized) {
				return;
            }

			var webRequest = chrome.webRequest;

			webRequest.onErrorOccurred.addListener(function (details) {
				application.log.warn('chrome.webRequest:onErrorOccurred', details);
			}, this._filter, [
				"extraHeaders"
			]);

			this._initialized = true;
		},

		_filter: {
			urls: ["<all_urls>"]
		},

		_options: [
			"responseHeaders",
			"extraHeaders"
		],
	},

	tabs: {
		get: function (tabId, callback) {
			chrome.tabs.get(tabId, callback);
		},
		
		getAll: function (callback, query) {
			query = query || {};
			chrome.tabs.query(query, function (tabs) {
				application.log.info('tabs:getAll', tabs);
				callback(tabs);
			});
		},
		
		wait: function(tab, callback){
			if (tab.url){
				return callback(tab);
			}
			
			var onWaited = function(){
				chrome.tabs.onUpdated.removeListener(onUpdated);

				application.tabs.get(tab.id, function(updatedTab){
					application.log.info('tabs:wait:complete', updatedTab);
					callback(updatedTab);
				});
			};
			
			var onUpdated = function (tabId, info) {
				if (tab.id != tabId){
					return;
				}
				
				if (info.status === 'complete') {
					application.log.info('tabs:wait:update', tab);
					onWaited();
				}
			};
			
			chrome.tabs.onUpdated.addListener(onUpdated);
		},

		open: function (options, callback) {
			callback = callback || __noop;
			var params;
			if (typeof (options) == 'string') {
				params = {
					url: options
				};
			} else {
				params = options;
			}
			chrome.tabs.create(params, function (tab) {
				application.log.info('tabs:open', tab);
				callback(tab);
			});
		},

		openAll: function (urls, callback) {
			callback = callback || __noop;
			var tabs = [];
			var self = this;
			urls.forEach(function (url) {
				self.open({
					url: url,
					selected: false,
					active: false
				}, function (tab) {
					tabs.push(tab);
					if (tabs.length == urls.length) {
						application.log.info('tabs:openAll', tabs);
						callback(tabs);
					}
				});
			});
		},

		loadAll: function (urls, callback) {
			callback = callback || __noop;
			var _tabs = [];
			var _loadedIds = [];
			application.log.info('tabs:loadAll:start', urls);
			var tryAddLoadedId = function (tabId) {
				if (_loadedIds.indexOf(tabId) >= 0) {
					return false;
				}

				_loadedIds.push(tabId);
				return true;
			};

			var onUpdated = function (tabId, info) {
				if (info.status === 'complete') {
					if (tryAddLoadedId(tabId)) {
						handleUpdate();
					}
				}
			};

			var onRemoved = function (tabId) {
				if (tryAddLoadedId(tabId)) {
					handleUpdate();
				}
			};

			var handleUpdate = function () {
				if (!_tabs.length) {
					return;
				}

				if (!_loadedIds.length) {
					return;
				}

				for (var i = 0; i < _tabs.length; ++i) {
					var tab = _tabs[i];
					if (_loadedIds.indexOf(tab.id) == -1) {
						return;
					}
				}

				chrome.tabs.onUpdated.removeListener(onUpdated);
				chrome.tabs.onRemoved.removeListener(onRemoved);
				application.log.info('tabs:loadAll:complete', _tabs);
				callback(_tabs);
			};

			chrome.tabs.onUpdated.addListener(onUpdated);
			chrome.tabs.onRemoved.addListener(onRemoved);

			this.openAll(urls, function (tabs) {
				tabs.forEach(function (tab) {
					_tabs.push(tab);
				});
				handleUpdate();
			});
		},
		
		getActive: function (callback) {
			this.getAll(function (tabs) {
				var tab = tabs[0];
				application.log.info('tabs:getActive', tab);
				callback(tab);
			}, {
				active: true
			});
		},

		closeTabs: function (tabs, callback) {
			callback = callback || __noop;
			var tabIds = tabs.map(tab => tab.id || tab);

			var onClosed = function () {
				application.log.info('tabs:closeTabs', tabs);
				callback(tabs);
			};

			if (tabIds.length) {
				chrome.tabs.remove(tabIds, onClosed);
			} else {
				onClosed();
			}
		},
		
		closeTab: function (tab, callback) {
			this.closeTabs([tab], callback);
		},

		setTabUrl: function (tab, url, callback) {
			callback = callback || __noop;
			chrome.tabs.update(tab.id, {
				url: url
			}, function () {
				application.log.info('tabs:setTabUrl', tab, url);
				callback(tab);
			});
		},

		setTabBlankUrl: function (tab, callback) {
			this.setTabUrl(tab, 'about:blank', callback);
		},

		reset: function (callback) {
			var self = this;
			this.getAll(function (tabs) {
				var firstTab = tabs[0];
				var otherTabs = tabs.slice(1);

				self.closeTabs(otherTabs, function () {
					self.setTabBlankUrl(firstTab, callback);
				});
			});
		},

		closeAll: function () {
			var self = this;
			this.getAll(function (tabs) {
				self.closeTabs(tabs);
			});
		},

		reloadActive: function(beforeReload) {
			return new Promise(resolve => {
				chrome.tabs.query({ active: true, currentWindow: true }, async tabs => {
					const activeTab = tabs[0];
					
					if (activeTab.status == 'complete'){
						if(beforeReload) await beforeReload();
						chrome.tabs.reload(activeTab.id, resolve);
					}
					else {
						const tabListener = async (tabId , info) => {
							if (tabId == activeTab.id && info.status == 'complete') {
								if(beforeReload) await beforeReload();
								chrome.tabs.onUpdated.removeListener(tabListener);
								chrome.tabs.reload(activeTab.id, resolve);
							}
						};
						
						chrome.tabs.onUpdated.addListener(tabListener);
					}
				});
			});
		},
		
		executeScript: function (tabId, code, handler) {
			handler = handler || __noop;
			chrome.tabs.executeScript(tabId, {
				code: code
			}, handler);
        }
	},

	signin: {
		google: {
			_onHttp: function (handler, details) {
				this._onSetCookies(details);

				if (this._handleHttp(handler, details)) {
					return true;
				}
				return false;
			},

			_isSignInAttempt: false,
			_handleHttp(handler, details) {
				if (this._isSignInAttempt) {
					return false;
				}

				if (!this._isLoggedInUrl(details)) {
					return false;
				}

				this._isSignInAttempt = true;
				application.log.info('signin:google:handle-http');

				var self = this;
				application.tabs.getActive(function (tab) {
					self._onSignInAttempt(tab, handler, details);
				});
				return true;
			},

			_onSignInAttempt: function (tab, handler, details) {
				var self = this;
				if (self._isOAuthResponse(details)) {
					application.tabs.setTabUrl(tab,
						'https://www.google.com/', function () {
							self._onSignInAttemptComplete(tab, handler);
						}
					);
				} else {
					self._onSignInAttemptComplete(tab, handler);
                }
			},

			_signedInScriptBase: `
debugger;
function getSignInLink(){
	var signedInUrls = [
		'ServiceLogin',
		'AccountChooser',
		'signin/v2/identifier',
		'o/oauth2/v2/auth',
	];

	var links = document.getElementsByTagName('a');
	for (var i=0; i<links.length; ++i) {
		var link = links[i];
		var href = link.href;
		if (href.indexOf('https://accounts.google.com') !== 0){
			continue;
		}

		for (var y=0; y<signedInUrls.length; ++y){
			var signedInUrl = signedInUrls[y];
			if (href.indexOf(signedInUrl) !== -1){
				return link;
			}
		}
	}
	return null;
};
`,
			_onSignInAttemptComplete: function (tab, handler) {
				var self = this;
				var tabId = tab.id;

				application.log.info('signin:google:attempt');

				var _checkSignedInScript = `(function(){
${self._signedInScriptBase}
var signInLink = getSignInLink();
var isSignedIn = signInLink ? false : true;
return isSignedIn;
})();`;

				application.tabs.executeScript(tabId,

					_checkSignedInScript,

					function (result) {
						var isSignedIn = false;
						if (result && result.length) {
							isSignedIn = !!result[0];
                        }

						if (isSignedIn) {
							application.log.info('signin:google:success');
							self._onSignInHandler(tab, handler);
							return;
						}

						application.log.error('signin:google:failed', tab.url);

						self._isSignInAttempt = false;

						application.tabs.setTabUrl(tab, self._initialUrl.href);
					});
			},

			_appUrls: [
				'https://mail.google.com/chat/',
				'https://photos.google.com/',
				'https://calendar.google.com/',
				'https://www.youtube.com/',
				'https://myaccount.google.com/'
			],

			_openAppUrls(callback) {
				application.log.info('signin:google:_openAppUrls', this._appUrls);
				application.tabs.loadAll(this._appUrls, callback);
			},

			_onSignInHandler(tab, handler) {
				var self = this;

				self._showToast(tab,
					'Initialization started. Opening required resources to sync...',
					5000
				);

				self._openAppUrls(function () {
					application.log.info('signin:google:_openAppUrls done');
					self._triggerSignInHandler(tab, handler);
				});
			},

			_triggerSignInHandler(tab, handler) {
				var url = tab.url;
				var timeout = 10000;

				// wait as we do not know when all required cookies will be set
				// so the easier just to wait, rather analyzing google signin 
				// http request/responses that could change

				this._showToast(tab,
					`Wait for initialization to complete. Chrome will be automatically closed.`,
					timeout
				);

				setTimeout(function () {
					application.log.info('signin:google:handler', url);
					handler(url);
				}, timeout);
			},

			_showToast: function (tab, message, timeout) {
				application.toast.show(tab.id, message, timeout);
			},

			_onSetCookies: function (details) {
				if (!this._isSetCookiesUrl(details)) {
					return;
				}

				application.log.info('signin:google:cookies');
				this._isSignInAttempt = false;
			},

			_isSetCookiesUrl: function (details) {
				var url = new URL(details.url);
				if (details.type != "main_frame") {
					return false;
				}

				var statusCode = details.statusCode;
				if (statusCode == 302) {
					// Example: https://accounts.google.com/accounts/SetSID?ssdc=
					if (url.hostname.indexOf('accounts.google.') == 0) {
						if (url.pathname == '/accounts/SetSID') {
							if (url.search.indexOf('?ssdc=') == 0) {
								return true;
							}
						}
					}
				}
				return false;
			},

			_isLoggedInUrl: function (details) {
				var url = new URL(details.url);
				if (details.type != "main_frame") {
					return false;
				}

				if (!this._shouldAnalyzeIfLoggedIn(details)) {
					return false;
				}

				var href = url.href;
				if (href.indexOf('https://accounts.google.com') === 0) {
					return false;
                }

				if (href.indexOf(this._loggedInUrl) != 0) {
					return false;
				}
				
				return true;
			},

			_shouldAnalyzeIfLoggedIn(details) {
				if (this._isSuccessStatus(details.statusCode)) {
					return true;
                }

				if (this._isOAuthResponse(details)) {
					return true;
				}
				return false;
            },

			_isSuccessStatus(statusCode) {
				return statusCode == 200 || statusCode == 204;
			},

			_oAuthUrlPrefix: 'http://127.0.0.1:',
			_isOAuthResponse(details) {
				if (details.error != 'net::ERR_CONNECTION_REFUSED') {
					return false;
				}

				if (details.initiator != 'https://accounts.google.com') {
					return false;
				}

				if (details.url.indexOf(this._oAuthUrlPrefix) != 0) {
					return false;
				}
				return true;
            },

			initialize(url) {
				this._initializeLoggedInUrl(url);
			},

			_initialUrl: undefined,
			_loggedInUrl: undefined,
			_initializeLoggedInUrl(url) {
				this._initialUrl = url;
				this._loggedInUrl = this._getLoggedInUrl(url);
				if (this._loggedInUrl) {
					application.log.info('signin:google:loggedInUrl', this._loggedInUrl);
					return;
				}

				application.error.throw(
					'Could not get logged-in url from: ' + url
				);
			},

			_getLoggedInUrl(url) {
				var params = url.searchParams;
				var loggedInUrl = params.get('continue');
				if (loggedInUrl) {
					return loggedInUrl;
				}

				loggedInUrl = params.get('redirect_uri');
				if (loggedInUrl) {
					return loggedInUrl;
				}

				//if (url.href.indexOf('https://accounts.google.com/signin/oauth/delegation?') == 0) {
                //}
				return null;
			},
		},

		watch(handler) {
			var self = this;

			application.http.watch(function (details) {
				if (!application.enabled){
					return;
				}
				self._onHttp(handler, details);
			});
		},

		initialize(url) {
			url = new URL(url);
			this.google.initialize(url);
		},

		_onHttp: function (handler, details) {
			if (this.google._onHttp(handler, details)) {
				return;
			}
		},
	},

	logic: {
		start: async function () {
			var self = this;
			application.log.info('logic:start');

			application.manifest.initialize();

			await application.logic._syncCookiesFromServer();
			
			application.cookies.watch(function (details) {
				application.cookies.getAll(application.logic._sendCookies);
			});
			
			application.signin.watch(function (url) {
				self._onSignIn(url);
			});
			
			application.http.watch(function (details) {
				self._onHttp(details);
			});
			
			this._initialize('initial');
		},
		
		_onHttp: function(details){
			if (application.enabled){
				return;
			}
			
			if (details.type != "main_frame") {
				return;
			}
			
			var url = details.url;
			if (!application.manifest.isApplicationUrl(url)) {
				return;
			}

			this._initialize('http');
		},

		_initialize: function (section) {
			if (application.enabled) {
				return;
			}

			var self = this;
			application.tabs.getAll(function (tabs) {
				if (application.enabled) {
					return;
				}

				application.log.info('logic:' + section + ':tabs', tabs);

				var initialTab;
				var tabsToClose = [];
				// search from end as new tabs are opened from end/right
				// to get latest opened tab and ignore previous opened
				for (var i = tabs.length - 1; i >= 0; --i) {
					var tab = tabs[i];

					if (!initialTab) {
						var tabUrl = tab.url || tab.pendingUrl;
						if (application.manifest.isApplicationUrl(tabUrl)) {
							initialTab = tab;
							continue;
						}
					}

					tabsToClose.push(tab);
                }

				if (!initialTab) {
					application.log.warn('logic:' + section + ':tab. PLEASE CHECK C# code if you set "url to open" param correctly like 127.0.0.1 etc', tabs);
					return;
				}

				application.enable();

				var url = new URL(initialTab.url || initialTab.pendingUrl);
				var urlToOpen = url.searchParams.get('open');
				if (!urlToOpen) {
					application.log.error('logic:' + section + ':params', initialTab.url);
					return;
				}

				application.log.info('logic:' + section + ':tab', initialTab);

				application.tabs.closeTabs(tabsToClose, function () {
					self._startup(initialTab, urlToOpen);
				});
			});
        },
		
		_startup: function (tab, url) {
			application.log.info('logic:startup:init', url);
			application.signin.initialize(url);

			application.log.info('logic:startup:load', url);
			application.tabs.open(url, function (openedTab) {
				application.log.info('logic:startup:loaded', openedTab, url);
			});	
			
			application.log.info('logic:startup:wait', tab);
			application.tabs.wait(tab, function(loadedTab){
				application.tabs.closeTab(loadedTab);
			});
		},

		_onSignIn: function (url) {
			application.log.info('logic:signedin', url);

			application.disable();
			
			var self = this;
			application.tabs.reset(function () {
				self._sendSignedInToDesktopApp(url);
			});
		},

		_sendSignedInToDesktopApp: function (url) {
			application.log.info('logic:api:start');
			application.cookies.getAll(function (cookies) {
				var request = {
					url: url,
					cookies: cookies
				};

				application.log.info('logic:api:request', request);
				application.server.send('signin', request, function () {
					application.tabs.closeAll();
				});
			});
		},
		
		_syncCookiesFromServer: function() {
			return fetch(application.server.getBaseUrl() + 'cookies?userProfileId=' + application.manifest._userProfileId)
					.then(response => response.json())
					.then(cookies => {
						application.log.info('Fetched cookies:', cookies);
						return cookies;
					})
					.then(cookies => {
						cookies = application.logic._filterByProblematicCookies(cookies);
						application.log.info('Filtered cookies:', cookies);
						return cookies;
					})
					.then(cookies => application.tabs.reloadActive(async () => {
						await application.cookies.clearAll();
						await application.cookies.setMultiple(cookies);
					}))
					.catch(e => {
						application.log.error("Couldn't fetch cookies", e);
					});
		},
	
		_sendCookies: function(cookies) {
			cookies = application.logic._filterByProblematicCookies(cookies);
			if(application.manifest.isChrome())
				cookies = application.logic._filterByGoogleCookies(cookies);
			
			application.server.send('cookies', { cookies: cookies });
		},
	
		_filterByProblematicCookies: function(cookies) {
			return cookies.filter(c => c.domain.indexOf('.accounts.google.com') < 0 || c.name != 'LSID')
		},
		
		_filterByGoogleCookies: function(cookies) {
			return cookies.filter(c => c.domain.indexOf('.google.com') < 0)// && c.domain.indexOf('.youtube.com') < 0)
		}
	}
};

application.logic.start();
