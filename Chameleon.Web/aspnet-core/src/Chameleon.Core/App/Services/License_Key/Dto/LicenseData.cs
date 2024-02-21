using Newtonsoft.Json;

namespace Chameleon.App.Services.License_Key.Dto
{
    public class LicenseData
    {
        [JsonProperty("product_id")]
        public LicenseType ProductId { get; set; }
    }
}
