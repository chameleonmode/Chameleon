using Avalonia.Collections;

using Chameleon.Core.Util;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Records;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.ThirdParty.SMSapi.Codesverify;
using Chameleon.lib.ThirdParty.SMSapi.Interfaces;
using Chameleon.lib.ThirdParty.SMSapi.SMSPool;
using Chameleon.lib.ThirdParty.SMSapi.SMSPVA;

namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public partial class PVApiModel
		: ViewModelObjectBase,
		IPVApiModel {
	private readonly IPVAInstance _pnapinstance;

	[ObservableProperty]
	private string? apiKey;
	[ObservableProperty]
	private string? lastFormatedResponse;
	private string? lastJsonResponse;
	[ObservableProperty]
	private string? getNumberData;
	[ObservableProperty]
	private string? receiveSMSData;

	[ObservableProperty]
	private bool isVisible = true;
	[ObservableProperty]
	private bool isVisibleSave = true;
	[ObservableProperty]
	private bool _isAwaiting;
	[ObservableProperty]
	private bool canCancel;

	[ObservableProperty]
	private RCountry? selectedCountry;
	public IList<RCountry>? Countries { get; set; }

	[ObservableProperty]
	private RService? selectedApp;
	public IList<RService>? Apps { get; set; }

	public bool HasCancel { get; set; } = true;

	public PVApiModel(IPVAInstance pnapinstance)
	{
		_pnapinstance = pnapinstance;
		Title = pnapinstance.Name;

		_ = DoInit();

		AsyncCommandMap["GetNumber"] = GetNumber;
		AsyncCommandMap["GetCode"] = GetCode;
		AsyncCommandMap["Save"] = Save;
		AsyncCommandMap["CancelOrder"] = CancelOrder;

		CommandMap["Popout"] = Popout;
	}

	private async Task DoInit()
	{
		await _pnapinstance.Init();
		ApiKey = _pnapinstance.ApiKey;
		Apps = new AvaloniaList<RService>(_pnapinstance.Services);
		OnPropertyChanged(nameof(Apps));
		SelectedApp = Apps[0];

		Countries = new AvaloniaList<RCountry>(_pnapinstance.Countries);
		OnPropertyChanged(nameof(Countries));
		SelectedCountry = Countries[0];
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

		await MakeRequest(async () => {
			var response = await _pnapinstance.GetNumberAsync(SelectedCountry, SelectedApp);
			LastFormatedResponse = lastJsonResponse = response.Item1;
			GetNumberData = response.Item2;
			CanCancel = GetNumberData.HasAny() && HasCancel;
		}, e => LastFormatedResponse = e);
	}

	public async Task GetCode()
	{
		if (lastJsonResponse?.Is() == false)
			return;

		await MakeRequest(async () => {
			var response = await _pnapinstance.GetCodeAsync(SelectedCountry!, SelectedApp!, lastJsonResponse!);
			LastFormatedResponse = response.Item1;
			ReceiveSMSData = response.Item2;
		}, e => LastFormatedResponse = e);
	}

	private async Task CancelOrder()
	{
		if (lastJsonResponse?.Is() == false)
			return;

		await MakeRequest(async () => {
			var response = await _pnapinstance.CancelOrderAsync(lastJsonResponse!);
			LastFormatedResponse = response.Item1;
			if (response.Item2 == "True") {
				GetNumberData = string.Empty;
				CanCancel = false;
			}
		}, e => LastFormatedResponse = e);
	}

	private async Task MakeRequest(Func<Task> func, Action<string> onErr)
	{
		IsAwaiting = true;
		await ExUtil.AsyncTryCatch(func, e => onErr(e.Message));
		IsAwaiting = false;
	}

	public void Popout()
	{
		var windowDialogService = ContainerServiceHelper.Resolve<IWindowDialogService>();

		windowDialogService?.ShowTopmost<IPVApiView, IPVApiModel>(new PVApiModel(_pnapinstance) { HasCancel = HasCancel }, async vm => {
			vm.IsVisibleSave = false;
			_ = await vm.LoadedTCS.Task;
		}, null, Title!, 560);
	}
}

public partial class PhoneVerificationViewModel()
			 : ViewModelObjectBase("Phone Verification")
			 , IPhoneVerificationViewModel {
	public AvaloniaList<IPVApiModel> PVApis { get; set; } =
	[
			new PVApiModel(CodesVerifyAPI.Instance){ HasCancel = false},
			new PVApiModel(SMSPoolAPI.Instance),
			new PVApiModel(SMSPVAPI.Instance),
	];
	public IPVApiModel CodesVerify => PVApis[0];
	public IPVApiModel SMSPVA => PVApis[2];
	public IPVApiModel SMSPool => PVApis[1];

	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		OnPropertyChanged(nameof(PVApis));
	}
}
