Vue.createApp({
  data() {
    return {
      userAgents: [],
      selectedAgentId: null,
      defaultAgentId: 0,
    };
  },
  async created() {
    const defaultUserAgent = {
      Id: this.defaultAgentId,
      Name: 'Default',
      Value: window.navigator.userAgent,
    };

    this.userAgents = await this.SendMessage('FetchAgents');
    console.log('Fetched user-agents', this.userAgents);
    this.userAgents = [defaultUserAgent].concat(this.userAgents);

    const selectedAgent = this.userAgents.find((ua) => ua.IsActive);
    if (selectedAgent) this.selectedAgentId = selectedAgent.Id;
    else this.selectedAgentId = this.defaultAgentId;
  },
  methods: {
    SelectAgent() {
      if (this.selectedAgentId === this.defaultAgentId)
        this.SendMessage('ResetToDefaultAgent');
      else this.SendMessage('SetActiveAgent', this.selectedAgentId);
    },

    SendMessage(action, param = undefined) {
      return new Promise((resolve) => {
        chrome.runtime.sendMessage({ action, param }, (res) => {
          console.log(res);
          if (!res.ex) resolve(res);
          else {
            console.error(res.ex);
            window.alert(res.ex);
          }
        });
      });
    },
  },
}).mount('#app');
