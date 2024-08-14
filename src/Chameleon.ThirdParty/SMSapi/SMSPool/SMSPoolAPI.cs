using Chameleon.Common.Helpers;
using Chameleon.Infrastructure.Settings;
using Chameleon.Interfaces.ThirdParty;
using Chameleon.ThirdParty.SMSapi.SMSPool.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Chameleon.ThirdParty.SMSapi.SMSPool;
public class SMSPoolAPI : PVAInstanceBase
{
    AuthenticationHeaderValue Authorization =>
         new AuthenticationHeaderValue("Token", ApiKey);

    public override async Task Init()
    {
        var appSetting = await ApplicationSettingsService.Instance.GetAsync();
        ApiKey = appSetting.Settings.SMSPoolApiKey;

        var getCountriesUrl = $"https://api.smspool.net/country/retrieve_all";
        Countries = [];
        Countries.AddRange(JsonSerializer.Deserialize<Country[]>(
            await HttpClientHelper.GetAsync(getCountriesUrl), JSOptions));

        var getServicesUrl = $"https://api.smspool.net/service/retrieve_all";
        Services = [];
        Services.AddRange(JsonSerializer.Deserialize<Service[]>(
            await HttpClientHelper.GetAsync(getServicesUrl), JSOptions));
    }

    public override async Task Save()
    {
        var appSetting = await ApplicationSettingsService.Instance.GetAsync();
        appSetting.Settings.SMSPoolApiKey = ApiKey;
        await ApplicationSettingsService.Instance.Save();
    }

    public override Task<Tuple<string, string>> GetNumberAsync(RCountry country, RService app)
        => OrderSMSAsync((Country)country, (Service)app);
    async Task<Tuple<string, string>> OrderSMSAsync<T1, T2>(T1 country, T2 service)
    where T1 : Country
    where T2 : Service
    {
        var apiUrl = $"https://api.smspool.net/purchase/sms";
        using var response = await HttpClientHelper.PostAsync(apiUrl, Authorization, null, new MultipartFormDataContent
        {
            { new StringContent(country.ID.ToString()), "country" },
            { new StringContent(service.ID.ToString()), "service" },
            { new StringContent(ApiKey), "key" }
        });
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = JsonSerializer.Deserialize<SuccessfullOrder>(responseContent, JSOptions);
            string formattedJson = JsonSerializer.Serialize(jsonResponse, JSOptions);
            return new Tuple<string, string>(formattedJson, jsonResponse?.number.ToString());
        }
        else
        {
            var jsonResponse = JsonSerializer.Deserialize<UnSuccessfullOrder>(responseContent, JSOptions);
            string formattedJson = JsonSerializer.Serialize(jsonResponse, JSOptions);
            return new Tuple<string, string>(formattedJson, jsonResponse?.message);
        }

    }
    public override async Task<Tuple<string, string>> CancelOrderAsync(string orderid)
    {
        var phoneNumberData =
            JsonSerializer.Deserialize<SuccessfullOrder>(orderid, JSOptions);

        var apiUrl = "https://api.smspool.net/sms/cancel";
        using var response = await HttpClientHelper.PostAsync(apiUrl, Authorization, null, new MultipartFormDataContent
        {
            { new StringContent(phoneNumberData?.order_id), "orderid" },
            { new StringContent(ApiKey), "key" }
        });
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<OrderBase>(responseContent, JSOptions);
        string formattedJson = JsonSerializer.Serialize(jsonResponse, JSOptions);
        return new Tuple<string, string>(formattedJson, (jsonResponse?.success > 0).ToString());
    }

    public override async Task<Tuple<string, string>> GetCodeAsync(RCountry country, RService app, string numberData)
    {
        var phoneNumberData =
           JsonSerializer.Deserialize<SuccessfullOrder>(numberData, JSOptions);

        var apiUrl = "https://api.smspool.net/sms/check";
        using var response = await HttpClientHelper.PostAsync(apiUrl, Authorization, null, new MultipartFormDataContent
        {
            { new StringContent(phoneNumberData.order_id), "orderid" },
            { new StringContent(ApiKey), "key" }
        });
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<SMSOrder>(responseContent, JSOptions);
        string formattedJson = JsonSerializer.Serialize(jsonResponse, JSOptions);
        return new Tuple<string, string>(formattedJson, jsonResponse?.sms);
    }

    private SMSPoolAPI()
        : base("SMS Pool")
    {
    }
    public static SMSPoolAPI Instance { get; } = new SMSPoolAPI();
}
