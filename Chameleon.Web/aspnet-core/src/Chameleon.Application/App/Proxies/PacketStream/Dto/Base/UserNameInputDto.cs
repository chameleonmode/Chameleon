using Newtonsoft.Json;

namespace Chameleon.App.PacketStream
{
    public class UserNameInputDto
    {
        [JsonProperty("username")]
        public string UserName { get; set; }
    }
}
