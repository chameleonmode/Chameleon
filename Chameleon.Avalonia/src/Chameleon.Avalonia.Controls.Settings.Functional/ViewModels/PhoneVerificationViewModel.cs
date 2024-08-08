using AutoMapper.Internal;
using Avalonia.Collections;
using Chameleon.Core.Util;
using Chameleon.Infrastructure.Settings;
using Chameleon.ThirdParty.Codesverify;
using Chameleon.ThirdParty.Codesverify.Models;
using Chameleon.ThirdParty.SMSPVA;
using Chameleon.ThirdParty.SMSPVA.Models;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Services;
using System.Text.Json;
using Chameleon.Interfaces.App.Settings;
using System.Security.Cryptography;
using System;
using Microsoft.Playwright;
using System.Runtime.CompilerServices;

namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public partial class PVApiModel<TApps, TCountries>
    : SubPageViewModelBase,
    IPVApiModel
{
    [ObservableProperty]
    private string apiKey;
    [ObservableProperty]
    private string lastFormatedResponse;
    [ObservableProperty]
    private string getNumberData;
    [ObservableProperty]
    private string receiveSMSData;

    [ObservableProperty]
    private bool isVisible = true;
    [ObservableProperty]
    private bool isVisibleSave = true;
    [ObservableProperty]
    private bool _isAwaiting;

    [ObservableProperty]
    private TApps selectedApp;
    public IList<TApps> Apps { get; set; }

    [ObservableProperty]
    private TCountries selectedCountry;
    public IList<TCountries> Countries { get; set; }

    Func<TApps, TCountries, Task<Tuple<string,string>>> GetNumberRequest { get; set; }
    Func<TApps, TCountries, string, Task<Tuple<string, string>>> GetCodeRequest { get; set; }
    Func<string, Task> OnSave { get; set; }
   Action OnPopout { get; set; }
    public PVApiModel(string title, 
        List<TApps> apps,
        List<TCountries> countries,
        Func<Task<string>> init,
        Func<TApps, TCountries, Task<Tuple<string,string>>> getNumber,
        Func<TApps, TCountries, string, Task<Tuple<string, string>>> getCode,
        Func<string, Task> save,
        Action popout)
    {
        Apps = new AvaloniaList<TApps>(apps);
        SelectedApp = Apps[0];
        Countries = new AvaloniaList<TCountries>(countries);
        SelectedCountry = Countries[0];
        _ = DoInit(init);
        

        GetNumberRequest = getNumber;
        GetCodeRequest = getCode;
        OnSave = save;

        AsyncCommandMap["GetNumber"] = GetNumber;
        AsyncCommandMap["GetCode"] = GetCode;
        AsyncCommandMap["Save"] = Save;

        CommandMap["Popout"] = popout;

        this.title = title;
    }
    async Task DoInit(Func<Task<string>> init)
    {
        ApiKey = await init();
    }
    public async Task Save()
    {
        await OnSave(ApiKey);
    }

    public async Task GetNumber()
    {
        if (SelectedCountry is null || SelectedApp is null)
            return;

        await MakeRequest(async ()=>
        {
            var response = await GetNumberRequest(SelectedApp, SelectedCountry);
            LastFormatedResponse = response.Item1;
            GetNumberData = response.Item2;
        }, e => LastFormatedResponse = e);
    }

    public async Task GetCode()
    {
        if (!GetNumberData.HasAny() || !LastFormatedResponse.HasAny() || SelectedCountry is null || SelectedApp is null)
            return;

        await MakeRequest(async () =>
        {
            var response = await GetCodeRequest(SelectedApp, SelectedCountry, GetNumberData);
            LastFormatedResponse = response.Item1;
            ReceiveSMSData = response.Item2;
        }, e => LastFormatedResponse = e);
    }

    async Task MakeRequest(Func<Task> func, Action<string> onErr)
    {
        IsAwaiting = true;
        await ExUtil.AsyncTryCatch(func, e => onErr(e.Message));
        IsAwaiting = false;
    }
}

public partial class PhoneVerificationViewModel(IUserSettingsService userSettingsService, IToastNotificationService ts)
       : SubPageViewModelBase("Phone Verification")
       , IPhoneVerificationViewModel
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    
    private IUserSetting _userSetting;
    private IApplicationSettings _appSetting;

    public AvaloniaList<IPVApiModel> PVApis { get; set; } = [];
    public IPVApiModel CodesVerify => PVApis[0];
    public IPVApiModel SMSPVA => PVApis[1];

    public override async Task InitAsync(object? param)
    {                       
        await base.InitAsync(param);

        if (!Loaded)
        {
            PVApis.Add(new PVApiModel<AppData, ThirdParty.Codesverify.Models.Country>(
                    "Codesverify",
                    CodesVerifyAPI.Instance.Apps,
                    CodesVerifyAPI.Instance.Countries,
                    async () =>
                    {
                        var appSetting = await ApplicationSettingsService.Instance.GetAsync();
                        CodesVerifyAPI.Instance.ApiKey = appSetting.Settings.CodesverifyApiKey;
                        return appSetting.Settings.CodesverifyApiKey;
                    },
                    async (app, country) =>
                    {
                        var response = await CodesVerifyAPI.Instance.GetActivationNumberAsync(app);
                        return new Tuple<string, string>(response, response);
                    },
                    async (app, country, lastnumber) =>
                    {
                        var response = await CodesVerifyAPI.Instance.GetCodeAsync(lastnumber, app);
                        return new Tuple<string, string>(response, response);
                    },
                    async (key) =>
                    {
                        CodesVerifyAPI.Instance.ApiKey = _appSetting.Settings.CodesverifyApiKey = key ?? "";
                        await ApplicationSettingsService.Instance.Save();
                        ts.ShowSuccess("Saved");
                    },
                    () =>
                    {
                        ContainerServiceHelper.Resolve<IWindowDialogService>().ShowTopmost<IPhoneVerificationView, IPhoneVerificationViewModel>(async vm =>
                        {
                            await vm.LoadedTCS.Task;
                            vm.CodesVerify.IsVisibleSave = vm.SMSPVA.IsVisible = false;
                        }, null, "Codeverify API", 720);
                    }));
            PVApis.Add(new PVApiModel<Service, ThirdParty.SMSPVA.Models.Country>(
                    "SMS PVA",
                    SMSPVAService.Instance.Services,
                    SMSPVAService.Instance.Countries, async () =>
                    {
                        var userSetting = await Task.Run(userSettingsService.Get);
                        SMSPVAService.Instance.ApiKey = userSetting.SmsPvaApiKey;
                        return userSetting.SmsPvaApiKey;
                    },
                    async (app, country) =>
                    {
                        var response =
                           await SMSPVAService.Instance.GetActivationNumberAsync(country, app);

                        var jsonResponse =
                           JsonSerializer.Deserialize<ApiResponse<GetNumberData>>(response, SMSPVAService.Instance.JSOptions);

                        return new Tuple<string, string>(response, jsonResponse?.Data?.PhoneNumber);
                    },
                    async (app, country, lastresponse) =>
                    {
                        var jsonResponse = JsonSerializer.Deserialize<ApiResponse<GetNumberData>>(lastresponse, SMSPVAService.Instance.JSOptions);
                        var response = await SMSPVAService.Instance.ReceiveSMS(jsonResponse.Data.OrderId);
                        var lastReceiveSMSData = JsonSerializer.Deserialize<ApiResponse<ReceiveSMSData>>(response, SMSPVAService.Instance.JSOptions);
                        return new Tuple<string, string>(response, lastReceiveSMSData?.Data?.Sms?.Code);
                    },
                    async (key) =>
                    {
                        SMSPVAService.Instance.ApiKey = _userSetting.SmsPvaApiKey = key ?? "";
                        await Task.Run(() => userSettingsService.Save(_userSetting));
                    },
                    () =>
                    {
                        ContainerServiceHelper.Resolve<IWindowDialogService>().ShowTopmost<IPhoneVerificationView, IPhoneVerificationViewModel>(async vm =>
                        {
                            await vm.LoadedTCS.Task;
                            vm.SMSPVA.IsVisibleSave = vm.CodesVerify.IsVisible = false;
                        }, null, "SMSPVA API", 720);
                    }));
        }

        OnPropertyChanged(nameof(PVApis));
    }



    //private async Task SaveCV()
    //{
    //    CodesVerifyAPI.Instance.ApiKey = _appSetting.Settings.CodesverifyApiKey = CodesverifyApiKey ?? "";
    //    await ApplicationSettingsService.Instance.Save();
    //    ts.ShowSuccess("Saved");
    //}

    //public async Task SaveSMSPVA()
    //{
    //    SMSPVAService.Instance.ApiKey = _userSetting.SmsPvaApiKey = SmspvApiKey ?? "";
    //    await Task.Run(() => userSettingsService.Save(_userSetting));
    //    IsChangeApiKey = false;
    //}

    //private Task PoputSMSPVA()
    //{

    //    return Task.CompletedTask;
    //}

    //private Task PoputCodeverify()
    //{
    //    ContainerServiceHelper.Resolve<IWindowDialogService>().ShowTopmost<IPhoneVerificationView, IPhoneVerificationViewModel>(vm =>
    //    {
    //        vm.IsCodesverifyVisible = true;
    //        vm.IsSMSPVAVisible = false;
    //    }, null, "Codeverify", 720);
    //    return Task.CompletedTask;
    //}
}
