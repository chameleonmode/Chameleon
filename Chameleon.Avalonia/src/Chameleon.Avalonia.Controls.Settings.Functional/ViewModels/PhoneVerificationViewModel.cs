using AutoMapper.Internal;
using Avalonia.Collections;
using Chameleon.Core.Util;
using Chameleon.Infrastructure.Settings;
using Chameleon.Infrastructure.ThirdParty.Codesverify;
using Chameleon.Infrastructure.ThirdParty.Codesverify.Models;
using Chameleon.Infrastructure.ThirdParty.SMSPVA;
using Chameleon.Infrastructure.ThirdParty.SMSPVA.Models;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Services;
using System.Text.Json;

namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public partial class PhoneVerificationViewModel(IUserSettingsService userSettingsService)
       : SubPageViewModelBase("Phone Verification")
       , IPhoneVerificationViewModel
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    
    private IUserSetting _userSetting;
    private IApplicationSettings _appSetting;

    [ObservableProperty]
    private bool _isChangeApiKey;

    #region smspva
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
    private string? _smspvApiKey;
    public AvaloniaList<Country> Countries { get; } = new(SMSPVAService.Instance.Countries);
    public AvaloniaList<Service> Services { get; } = new(SMSPVAService.Instance.Services);
    #endregion

    [ObservableProperty]
    private string? _codesverifyApiKey;
    [ObservableProperty]
    private string _lastNumberResponseCodesverify;
    [ObservableProperty]
    private string _lastCodeResponseCodesverify;
    [ObservableProperty]
    private string _lastFormatedResponseCodesverify;

    [ObservableProperty]
    private AppData? _selectedCodesverifyApp;
    public AvaloniaList<AppData> CodesverifyApps { get; } = new(CodesVerifyAPI.Instance.Apps);


    partial void OnSmspvApiKeyChanged(string? value)
    {
        IsChangeApiKey = _userSetting.SmsPvaApiKey != value;
        SMSPVAService.Instance.ApiKey = _userSetting.SmsPvaApiKey = value ?? "";
    }

    partial void OnCodesverifyApiKeyChanged(string? value)
    {
        IsChangeApiKey = CodesVerifyAPI.Instance.ApiKey != value;
        CodesVerifyAPI.Instance.ApiKey = _appSetting.Settings.CodesverifyApiKey = value ?? "";
    }

    public override async Task InitAsync(object? param)
    {                       
        await base.InitAsync(param);

        if (!Loaded)
        {

            _userSetting = await Task.Run(userSettingsService.Get);
            SmspvApiKey = _userSetting.SmsPvaApiKey;
            SelectedCountry = Countries[0];
            SelectedService = Services[0];

            _appSetting = await ApplicationSettingsService.Instance.GetAsync();
            CodesverifyApiKey = _appSetting.Settings.CodesverifyApiKey;
            SelectedCodesverifyApp = CodesverifyApps[0];

            AsyncCommandMap["GetNumberSMSPVA"] = GetNumber;
            AsyncCommandMap["GetCodeSMSPVA"] = GetCode;
            AsyncCommandMap["GetNumberCodesverify"] = GetNumberCodesverify;
            AsyncCommandMap["GetCodeCodesverify"] = GetCodeCodesverify;
            AsyncCommandMap["SaveCV"] = ApplicationSettingsService.Instance.Save;
            AsyncCommandMap["SaveSMSPVA"] = SaveSMSPVA;
        }
    }

    public async Task SaveSMSPVA()
    {
        await Task.Run(() => userSettingsService.Save(_userSetting));
        IsChangeApiKey = false;
    }


    public async Task GetNumberCodesverify()
    {
        if (IsAwaiting || SelectedCodesverifyApp is null)
            return;

        await MakeRequest(async () =>
        {
            LastFormatedResponseCodesverify = LastNumberResponseCodesverify = await CodesVerifyAPI.Instance.GetActivationNumberAsync(SelectedCodesverifyApp);
        }, e => LastFormatedResponseCodesverify = e);
       
    }

    public async Task GetCodeCodesverify()
    {
        if (IsAwaiting || LastNumberResponseCodesverify is null)
            return;

        await MakeRequest(async () =>
        {
            LastFormatedResponseCodesverify = LastCodeResponseCodesverify = await CodesVerifyAPI.Instance.GetCodeAsync(LastNumberResponseCodesverify, SelectedCodesverifyApp);
        }, e => LastFormatedResponseCodesverify = e);
    }



    public async Task GetNumber()
    {
        if (IsAwaiting || SelectedCountry is null || SelectedService is null)
            return;

        await MakeRequest(async () =>
        {
            lastGetNumberData = await SMSPVAService.Instance.GetActivationNumberAsync<GetNumberData>(SelectedCountry, SelectedService);
            GetNumberData = lastGetNumberData?.Data?.PhoneNumber.ToString() ?? "";
            LastFormatedResponse = JsonSerializer.Serialize(lastGetNumberData, jsonSerializerOptions);
        }, e => LastFormatedResponse = e);
    }

    public async Task GetCode()
    {
        if (IsAwaiting || lastGetNumberData?.Data?.OrderId is null)
            return;

        await MakeRequest(async () =>
        {
            lastReceiveSMSData = await SMSPVAService.Instance.ReceiveSMS<ReceiveSMSData>(lastGetNumberData.Data.OrderId);
            ReceiveSMSData = lastReceiveSMSData?.Data?.Sms?.Code ?? "";
            LastFormatedResponse = JsonSerializer.Serialize(lastReceiveSMSData, jsonSerializerOptions);
        }, e => LastFormatedResponse = e);
    }

    async Task MakeRequest(Func<Task> func, Action<string> onErr)
    {
        IsAwaiting = true;
        await ExUtil.AsyncTryCatch(func, e => onErr(JsonSerializer.Serialize(e, jsonSerializerOptions)));
        IsAwaiting = false; 
    }
}
