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
using Chameleon.Interfaces.ThirdParty;

namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public partial class PVApiModel
    : SubPageViewModelBase,
    IPVApiModel
{
    private readonly IPVAInstance _pnapinstance;

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
    private RService selectedApp;
    public IList<RService> Apps { get; set; }

    [ObservableProperty]
    private RCountry selectedCountry;
    public IList<RCountry> Countries { get; set; }


    public PVApiModel(IPVAInstance pnapinstance)
    {
        _pnapinstance = pnapinstance;
        title = pnapinstance.Name;
        Apps = new AvaloniaList<RService>(pnapinstance.Services);
        SelectedApp = Apps[0];

        Countries = new AvaloniaList<RCountry>(pnapinstance.Countries);
        SelectedCountry = Countries[0];

        _ = DoInit();

        AsyncCommandMap["GetNumber"] = GetNumber;
        AsyncCommandMap["GetCode"] = GetCode;
        AsyncCommandMap["Save"] = Save;

        CommandMap["Popout"] = Popout;
    }
    async Task DoInit()
    {
        await _pnapinstance.Init();
        ApiKey = _pnapinstance.ApiKey;
    }
    public async Task Save()
    {
        _pnapinstance.ApiKey = ApiKey;
        await _pnapinstance.Save();
    }

    public async Task GetNumber()
    {
        if (SelectedCountry is null || SelectedApp is null)
            return;

        await MakeRequest(async ()=>
        {
            var response = await _pnapinstance.GetNumberAsync(SelectedCountry, SelectedApp);
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
            var response = await _pnapinstance.GetCodeAsync(SelectedCountry, SelectedApp, GetNumberData);
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

    public void Popout()
    {
        WindowDialogService.ShowTopmost<IPVApiView, IPVApiModel>(new PVApiModel(_pnapinstance), async vm =>
        {
            vm.IsVisibleSave = false;
            await vm.LoadedTCS.Task;
        }, null, Title, 526);
    }
}

public partial class PhoneVerificationViewModel()
       : SubPageViewModelBase("Phone Verification")
       , IPhoneVerificationViewModel
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
 

    public AvaloniaList<IPVApiModel> PVApis { get; set; } = 
    [
        new PVApiModel(CodesVerifyAPI.Instance), 
        new PVApiModel(SMSPVAService.Instance),
    ];
    public IPVApiModel CodesVerify => PVApis[0];
    public IPVApiModel SMSPVA => PVApis[1];

    public override async Task InitAsync(object? param)
    {                       
        await base.InitAsync(param);

        OnPropertyChanged(nameof(PVApis));
    }
}
