using System.Text.Json.Serialization;

namespace Chameleon.Infrastructure.ThirdParty.SMSPVA.Models;

public class ApiResponse<T>
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("data")]
    public T Data { get; set; }

    [JsonPropertyName("error")]
    public object Error { get; set; }
}

public class GetNumberData
{
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("phoneNumber")]
    public long PhoneNumber { get; set; }

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; }

    [JsonPropertyName("orderExpireIn")]
    public int OrderExpireIn { get; set; }
}

public class ReceiveSMSData
{
    [JsonPropertyName("sms")]
    public Sms Sms { get; set; }

    [JsonPropertyName("orderId")]
    public string OrderId { get; set; }

    [JsonPropertyName("orderExpireIn")]
    public int OrderExpireIn { get; set; }
}

public class Sms
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
    [JsonPropertyName("fullText")]
    public string FullText { get; set; }
}

public class ErrorData
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}


