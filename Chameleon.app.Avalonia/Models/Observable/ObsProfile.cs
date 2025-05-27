using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.WebBrowser.Services;
using Chameleon.lib.Util;
using static Chameleon.lib.Common.Constants.Enums;
using Chameleon.lib.WebBrowser;
using DynamicData.Binding;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsProfile : ObservableDtoViewModelBase<UserProfileDto> {

	private INavigatorService NavigationService => Navigator.Instance;

	[ObservableProperty]
	private string isChromeRunning = "False";
	[ObservableProperty]
	private string isBraveRunning = "False";
	[ObservableProperty]
	private string isFFRunning = "False";
	[ObservableProperty]
	private bool isShowGlyph;
	[ObservableProperty]
	private bool isForeground;
	[ObservableProperty]
	private bool isFavorite;

	//
	public bool IsShowCheckboxColumn { get; }
	public bool IsSharedProfile { get; }

	//
	public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
	public bool IsDeleteProfileBtnVisible => !IsSharedProfile;
	public Dictionary<SystemBrowserType, IBrowserInstance?> SBI { get; } = new() {
		[SystemBrowserType.Chrome] = null,
		[SystemBrowserType.Firefox] = null,
		[SystemBrowserType.Brave] = null
	};

	public BrowserProfile SystemBrowserProfile => new() {
		Id = Dto!.id,
		Proxy = new BrowserProxy() {
			Host = Dto.proxy?.host,
			Port = Dto.proxy?.port ?? 0,
			UserName = Dto.proxy?.userName,
			Password = Dto.proxy?.password
		}
	};

	public ReadOnlyObservableCollection<UPLoginDto> ProfileLogins {get;}

	public event Action<ObsProfile>? OnSelectedChanged;

	public ObsProfile(
			UserProfileDto userProfile,
			bool isShowCheckboxColumn = true,
			bool isShowGlyph = true,
			bool hasActionOptions = true,
			Action<ObsProfile>? onSelectedChanged = default,
			Action<ObsProfile>? onDeleted = default
	) : base(
			userProfile,
			userProfile.title,
			onSelectedChanged == null ? null : x => onSelectedChanged((ObsProfile)x)
	) {
		_ = UPAdditionalDataRepo.Instance.Loginz
					.Connect(i => i.ProfileId == userProfile.id)
					.Bind(out var logins)
					.Subscribe();
		ProfileLogins = logins;
		IsShowGlyph = isShowGlyph;
		IsShowCheckboxColumn = isShowCheckboxColumn;
		IsActionOptionsVisible = hasActionOptions;
		IsSharedProfile = userProfile.creatorUserId != Auther.AuthSession?.UserId;
		IsFavorite = userProfile.isFavourite;

		AsyncCommandMap["OpenFirefox"] = () => OpenSystemBrowser(SystemBrowserType.Firefox);
		AsyncCommandMap["OpenChrome"] = () => OpenSystemBrowser(SystemBrowserType.Chrome);
		AsyncCommandMap["OpenBrave"] = () => OpenSystemBrowser(SystemBrowserType.Brave);
		AsyncCommandMap["Favorite"] = async () => {
			IsFavorite = !IsFavorite;
			_ = await UserProfilesRepo.SetProfileIsFavorite(userProfile, IsFavorite);
		};
		AsyncCommandMap["DeleteUserProfile"] = async () => {
			if (await Mbox.Show(
				title: "Delete User Profile",
				content: $"Are you sure you want to delete {userProfile.title}?",
				btns: MBoxButtons.OkCancel,
				fontIconInfo: "DeleteLines"
			)) {
				_ = await UserProfilesRepo.Instance.Delete(userProfile.id);
				if (NavigationService.CanGoBack == true && NavigationService.IsCurrentView("IdentityView")) {
					NavigationService.GoBack();
				}
				onDeleted?.Invoke(this);
			}
		};

		CommandMap["OpenTopmostController"] = OpenTopmostController;
		CommandMap["ShowViewProfile"] = () => WShower.ShowTopmost<UserProfileSidePanelUserControl, UserProfileSidePanelViewModel>(
			vm: new UserProfileSidePanelViewModel(userProfile),
			title: "Copy Pasta", 
			width: 156
		);

		var browsers = SystemBrowserService.Instance.HasInstanceOf(Dto.id, (sender, args) => {
			IsForeground = args.EventType == SysBrowserEventType.Foreground;
			if (!IsForeground && args.EventType != SysBrowserEventType.Background) {
				var runnin = args.EventType switch {
					SysBrowserEventType.Opened => SetRunning(args.OpenOptions.BrowserType, true),
					SysBrowserEventType.Closed => SetRunning(args.OpenOptions.BrowserType, false),
					SysBrowserEventType.Error => SetRunning(args.OpenOptions.BrowserType, null),
					_ => SetRunning(args.OpenOptions.BrowserType, null)
				};

				if (runnin is "Error" or "False") {
					SBI[args.OpenOptions.BrowserType] = null;
				}
			}
		});
		browsers.ForEach(b => _ = SetRunning(b, true));

		_ = this.WhenValueChanged(x => x.IsSelected)
			.Subscribe(x => OnSelectedChanged?.Invoke(this));
	}

	public void Open() {
		NavigationService.NavigateTo("IdentityView", Dto);
	}

	public void OpenTopmostController() {
		WShower.ShowTopmost(
			vm: SnapCracklePopViewModel.Instance,
			v: SnapCracklePopUserControl.Instance,
			initialize: vm => {
				vm.RunningList.AddIfNotExists(new ObsProfile(Dto, false, false), p => p.Dto?.id == Dto.id);
			},
			onClosed: vm => {
				vm.RunningList.Clear();
			},
			title: "SCP",
			width: 172
		);
	}

	public async Task<IBrowserInstance?> OpenSystemBrowser(SystemBrowserType browserType) {
		// TODO:
		// if(SystemBrowserService.Instance.OpenTaskCompletionSource != null)
		// 	_ = await SystemBrowserService.Instance.OpenTaskCompletionSource.Task;
		if (SBI.TryGetValue(browserType, out var browser)) {
			if (browser == null) {
					browser = await SystemBrowserService.Instance.Open(new(browserType, SystemBrowserProfile));
					//_ = SetRunning(browserType, browser == null ? null : true);
					SBI[browserType] = browser;
			} else {
				browser.InvokeEvent(SysBrowserEventType.Foreground);
			}
		}

		return SBI[browserType];
	}

	private string SetRunning(SystemBrowserType args, bool? running) => args switch {
		SystemBrowserType.Chrome => 
		running != true && IsChromeRunning == "Error" 
		? "Error"
		: IsChromeRunning = IsChromeRunning = running is null 
		? "Error" 
		: running == true 
		? "True" 
		: "False",

		SystemBrowserType.Firefox => running != true && IsFFRunning == "Error" ? "Error"
		: IsFFRunning = IsFFRunning = running is null ? "Error" : running == true ? "True" : "False",

		SystemBrowserType.Brave => running != true && IsBraveRunning == "Error" ? "Error"
		: IsBraveRunning = IsBraveRunning = running is null ? "Error" : running == true ? "True" : "False",

		_ => "Error"
	};
}
