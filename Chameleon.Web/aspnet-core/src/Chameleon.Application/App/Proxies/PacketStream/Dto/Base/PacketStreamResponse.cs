using Newtonsoft.Json;

namespace Chameleon.App.PacketStream
{
    public class PacketStreamResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        public int StatusCode
        {
            get
            {
                int.TryParse(Status, out var code);
                return code;
            }
        }

        public bool IsSuccess => StatusCode == 200;

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }

    public class PacketStreamResponse<TBody> : PacketStreamResponse
    {
        [JsonProperty("data")]
        public TBody Data { get; set; }
    }
}
