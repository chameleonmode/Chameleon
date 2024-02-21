using Newtonsoft.Json;

namespace Chameleon.App.PacketStream
{
    public class UpdateBalanceInputDto : UserNameInputDto
    {
        [JsonProperty("amount_usd_cents")]
        public long BalanceInCents { get; set; }
    }
}
