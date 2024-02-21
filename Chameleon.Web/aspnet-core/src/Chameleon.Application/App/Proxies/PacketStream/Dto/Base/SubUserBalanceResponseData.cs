using Newtonsoft.Json;
using System;

namespace Chameleon.App.PacketStream
{
    public class SubUserBalanceResponseData
    {
        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("proxy_authkey")]
        public string ProxyAuthKey { get; set; }

        [JsonProperty("date_created")]
        public DateTime CreatedDate { get; set; }
    }
}
