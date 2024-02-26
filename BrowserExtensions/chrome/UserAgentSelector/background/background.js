const extension = {
  manifest: null,
  userAgentsPromise: null,
  userAgents: null,

  Init: function () {
    this.manifest = chrome.runtime.getManifest();

    this.userAgentsPromise = this.FetchAgents().then((agents) => {
      console.log('Fetched user-agents', agents);
      const activeAgent = agents.find((ua) => ua.IsActive);
      if (activeAgent) this.ModifyUserAgent(activeAgent.Value);
      else this.ResetUserAgent();
      return agents;
    });

    chrome.runtime.onMessage.addListener((req, sender, sendResponse) => {
      this.OnMessage(req, sender, sendResponse);
      return true;
    });
  },

  OnMessage: async function (req, sender, sendResponse) {
    if (!this.userAgents) {
      this.userAgents = await this.userAgentsPromise.catch((ex) =>
        sendResponse({ ex })
      );
    }

    switch (req.action) {
      case 'FetchAgents':
        sendResponse(this.userAgents);
        break;
      case 'SetActiveAgent':
        this.SetActiveAgent(req.param).catch((ex) => sendResponse({ ex }));
        break;
      case 'ResetToDefaultAgent':
        this.ResetToDefaultAgent().catch((ex) => sendResponse({ ex }));
        break;
      default:
        sendResponse({ ex: 'No such action: ' + req.action });
    }
  },

  FetchAgents: async function () {
    const userAgents = await fetch(
      this.GetBaseUrl() + '?userProfileId=' + this.manifest.userProfileId
    )
      .then((response) => response.json())
      .catch((e) => {
        const msg = "Couldn't fetch user-agents";
        console.error(msg, e);
        throw msg;
      });

    return userAgents;
  },
  SetActiveAgent: async function (activeAgentId) {
    const activeAgent = this.userAgents.find((ua) => ua.Id == activeAgentId);

    this.ModifyUserAgent(activeAgent.Value);
    this.SelectAgent(activeAgent);
    await this.FetchSetActiveAgent(activeAgent.Id);
  },
  ResetToDefaultAgent: async function () {
    this.ResetUserAgent();
    this.UnselectAgents();
    await this.FetchSetActiveAgent(null);
  },

  SelectAgent: function (selectedAgent) {
    this.UnselectAgents();
    selectedAgent.IsActive = true;
  },
  UnselectAgents: function () {
    for (const agent of this.userAgents) agent.IsActive = false;
  },
  FetchSetActiveAgent: async function (activeAgentId) {
    await fetch(this.GetBaseUrl() + '/setactive', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        UserAgentId: activeAgentId,
        UserProfileId: this.manifest.userProfileId,
        BrowserType: this.manifest.browserType,
      }),
    }).catch((e) => {
      const msg = "Couldn't set active user-agent on Chameleon";
      console.error(msg, e);
      throw msg;
    });
  },
  GetBaseUrl: function () {
    return 'http://127.0.0.1:' + this.manifest.applicationPort + '/agents';
  },
  ModifyUserAgent: function (userAgent) {
    const rules = {
      addRules: [
        {
          id: 1,
          priority: 1,
          action: {
            type: 'modifyHeaders',
            requestHeaders: [
              {
                header: 'user-agent',
                operation: 'set',
                value: userAgent,
              },
            ],
          },
          condition: {
            urlFilter: '*',
            resourceTypes: [
              'csp_report',
              'font',
              'image',
              'main_frame',
              'media',
              'object',
              'other',
              'ping',
              'script',
              'stylesheet',
              'sub_frame',
              'webbundle',
              'websocket',
              'webtransport',
              'xmlhttprequest',
            ],
          },
        },
      ],
      removeRuleIds: [1],
    };

    chrome.declarativeNetRequest.updateDynamicRules(rules, () => {
      if (chrome.runtime.lastError) {
        console.error(chrome.runtime.lastError);
      } else {
        chrome.declarativeNetRequest.getDynamicRules((rules) =>
          console.log(rules)
        );
      }
    });
  },
  ResetUserAgent: function () {
    const rules = {
      removeRuleIds: [1],
    };

    chrome.declarativeNetRequest.updateDynamicRules(rules, () => {
      if (chrome.runtime.lastError) {
        console.error(chrome.runtime.lastError);
      } else {
        chrome.declarativeNetRequest.getDynamicRules((rules) =>
          console.log(rules)
        );
      }
    });
  },
};

extension.Init();
