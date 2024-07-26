using Avalonia.Collections;
using Chameleon.Core.Util;
using Chameleon.Infrastructure.ThirdParty.SMSPVA;
using Chameleon.Infrastructure.ThirdParty.SMSPVA.Models;
using System.Text.Json;

namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public partial class PhoneVerificationViewModel(IUserSettingsService userSettingsService)
       : SubPageViewModelBase("Phone Verification")
       , IPhoneVerificationViewModel
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    private IUserSetting _userSetting;
    private ApiResponse<GetNumberData> lastGetNumberData;
    private ApiResponse<ReceiveSMSData> lastReceiveSMSData;
    
    [ObservableProperty]
    private Country? _selectedCountry;
    [ObservableProperty]
    private Service? _selectedService;

    [ObservableProperty]
    private bool _isAwaiting;
    [ObservableProperty]
    private string _getNumberData;
    [ObservableProperty]
    private string _receiveSMSData;
    [ObservableProperty]
    private string _lastFormatedResponse;
//        = """
//{
//"statusCode": 200,
//"data": {
//"orderId": 123456,
//"phoneNumber": 9876544321,
//"countryCode": "RU",
//"orderExpireIn": 600
//}
//}
//""";


    [ObservableProperty]
    private string? _apiKey;
    [ObservableProperty]
    private bool _isChangeApiKey;

    public AvaloniaList<Country> Countries { get; } = new(SMSPVAService.Instance.Countries);
    public AvaloniaList<Service> Services { get; } = new(SMSPVAService.Instance.Services);

    public override async Task InitAsync(object? param)
    {                       
        await base.InitAsync(param);

        if (!Loaded)
        {

            _userSetting = userSettingsService.Get();
            ApiKey = _userSetting.SmsPvaApiKey;
            SelectedCountry = Countries[0];
            SelectedService = Services[0];
        }
    }
    partial void OnApiKeyChanged(string? value)
    {
        IsChangeApiKey = _userSetting.SmsPvaApiKey != value;
        _userSetting.SmsPvaApiKey = value;
        SMSPVAService.Instance.SetApiKey(ApiKey ?? "");
    }

    [RelayCommand]
    public void Save()
    {
        userSettingsService.Save(_userSetting);
        IsChangeApiKey = false;
    }
    [RelayCommand]
    public async Task GetNumber()
    {
        if (IsAwaiting || SelectedCountry is null || SelectedService is null)
            return;


        IsAwaiting = true;
        try
        {
            lastGetNumberData = await SMSPVAService.Instance.GetActivationNumberAsync<GetNumberData>(SelectedCountry, SelectedService);
            GetNumberData = lastGetNumberData?.Data?.PhoneNumber.ToString() ?? "";
            LastFormatedResponse = JsonSerializer.Serialize(lastGetNumberData, jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            LastFormatedResponse = JsonSerializer.Serialize(ex, jsonSerializerOptions);
        }
        IsAwaiting = false;
    }
    [RelayCommand]
    public async Task GetCode()
    {
        if (IsAwaiting || lastGetNumberData?.Data?.OrderId is null)
            return;

        IsAwaiting = true;
        try
        {
            lastReceiveSMSData = await SMSPVAService.Instance.ReceiveSMS<ReceiveSMSData>(lastGetNumberData.Data.OrderId);
            ReceiveSMSData = lastReceiveSMSData?.Data?.Sms?.Code ?? "";
            LastFormatedResponse = JsonSerializer.Serialize(lastReceiveSMSData, jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            LastFormatedResponse = JsonSerializer.Serialize(ex, jsonSerializerOptions);
        }
        IsAwaiting = false;
    }
}
