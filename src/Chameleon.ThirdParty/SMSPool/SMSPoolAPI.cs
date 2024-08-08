using Chameleon.Common.Helpers;
using Chameleon.Infrastructure.Settings;
using Chameleon.Interfaces.ThirdParty;
using Chameleon.ThirdParty.SMSPool.Models;
using System.Text.Json;

namespace Chameleon.ThirdParty.SMSPool;
public class SMSPoolAPI : IPVAInstance
{
    public readonly JsonSerializerOptions JSOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string Name => "SMS Pool API";
    public string ApiKey { get; set; } = "";
    public List<RCountry> Countries { get; } = [];
    public List<RService> Services { get; } = [];

    public async Task Init()
    {
        var appSetting = await ApplicationSettingsService.Instance.GetAsync();
        ApiKey = appSetting.Settings.SMSPoolApiKey;

        var getCountriesUrl = $"https://api.smspool.net/country/retrieve_all";
        string getCountriesResponse = await HttpClientHelper.GetAsync(getCountriesUrl);
        var countries = JsonSerializer.Deserialize<Country[]>(getCountriesResponse, JSOptions);
        Countries.Clear();
        Countries.AddRange(countries);

        var getServicesUrl = $"https://api.smspool.net/service/retrieve_all";
        string getServicesResponse = await HttpClientHelper.GetAsync(getServicesUrl);
        var services = JsonSerializer.Deserialize<Service[]>(getCountriesResponse, JSOptions);
        Services.Clear();
        Services.AddRange(services);
    }

    public async Task Save()
    {
        var appSetting = await ApplicationSettingsService.Instance.GetAsync();
        appSetting.Settings.SMSPoolApiKey = ApiKey;
        await ApplicationSettingsService.Instance.Save();
    }

    public Task<Tuple<string, string>> GetNumberAsync(RCountry country, RService app)
        => OrderSMSAsync((Country)country, (Service)app);
    async Task<Tuple<string, string>> OrderSMSAsync<T1, T2>(T1 country, T2 service)
    where T1 : Country
    where T2 : Service
    {
        var apiUrl = $"https://api.smspool.net/purchase/sms";
        string responseBody = await HttpClientHelper.PostAsync(apiUrl, ApiKey, new MultipartFormDataContent() { });
        var jsonResponse = JsonSerializer.Deserialize<SuccessfullOrder>(responseBody, JSOptions);
        return new Tuple<string, string>(responseBody, jsonResponse?.number.ToString());
    }

    public async Task<Tuple<string, string>> GetCodeAsync(RCountry country, RService app, string numberData)
    {
        using HttpClient client = new();

        HttpResponseMessage response = await client.GetAsync($"http://codesverify.com/user/api/get_sms.php?customer={ApiKey}&number={numberData}&country={country.Name}&app={app.Name}");
        string responseBody = await response.Content.ReadAsStringAsync();
        return new Tuple<string, string>(responseBody, responseBody);
    }

    // Make class singleton
    private SMSPoolAPI()
    {
    }
    private static SMSPoolAPI instance;
    public static SMSPoolAPI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SMSPoolAPI();
            }
            return instance;
        }
    }
}
