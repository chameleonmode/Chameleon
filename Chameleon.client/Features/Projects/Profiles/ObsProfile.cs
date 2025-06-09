using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.client.MvvM;
using Chameleon.lib.WebBrowser.Services;
using Chameleon.lib.Util;
using static Chameleon.lib.Common.Constants.Enums;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Projects.Profiles.Dialogs;
using Chameleon.client.Services;

namespace Chameleon.client.Features.Projects.Profiles;

public partial class ObsProfile : ObservableDtoViewModelBase<UserProfileDto> {
	[ObservableProperty] string isChromeRunning = "False";
	[ObservableProperty] string isBraveRunning = "False";
	[ObservableProperty] string isFFRunning = "False";
	[ObservableProperty] bool isShowGlyph = true;
	[ObservableProperty] bool isForeground;
	[ObservableProperty] bool isShowCheckboxColumn = true;

	public Dictionary<SystemBrowserType, IBrowserInstance?> SBI { get; } = new() {
		[SystemBrowserType.Chrome] = null,
		[SystemBrowserType.Firefox] = null,
		[SystemBrowserType.Brave] = null
	};

	public bool IsSharedProfile => Dto.creatorUserId != Auther.AuthSession?.UserId;
	public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
	public bool IsFavorite => Dto.isFavourite;

	public BrowserProfile SystemBrowserProfile => new() {
		Id = Dto.id,
		Proxy = new BrowserProxy() {
			Host = Dto.proxy?.host,
			Port = Dto.proxy?.port ?? 0,
			UserName = Dto.proxy?.userName,
			Password = Dto.proxy?.password
		}
	};
	public ReadOnlyObservableCollection<UPLoginDto> ProfileLogins {
		get {
			_ = UPAdditionalDataRepo.Instance.Loginz
			.Connect(i => i.ProfileId == Dto.id)
			.Bind(out var logins)
			.Subscribe();
			return logins;
		}
	}

	public ObsProfile(UserProfileDto profile,
		Action<ObservableDtoViewModelBase<UserProfileDto>>? onSelectedChanged = default,
		Action<ObsProfile>? onDeleted = default)
	: base(profile, onSelectedChanged) {

		AsyncCommandMap["OpenFirefox"] = () => OpenSystemBrowser(SystemBrowserType.Firefox);
		AsyncCommandMap["OpenChrome"] = () => OpenSystemBrowser(SystemBrowserType.Chrome);
		AsyncCommandMap["OpenBrave"] = () => OpenSystemBrowser(SystemBrowserType.Brave);

		AsyncCommandMap["Favorite"] = async () => {
			_ = await UserProfilesRepo.SetProfileIsFavorite(profile);
			OnPropertyChanged(nameof(IsFavorite));
		};
		AsyncCommandMap["DeleteUserProfile"] = async () => {
			if (await MessageBox.Show(
				title: "Delete User Profile",
				content: $"Are you sure you want to delete {profile.title}?",
				btns: MBoxButtons.OkCancel,
				fontIconInfo: "DeleteLines"
			)) {
				_ = await UserProfilesRepo.Instance.Delete(profile.id);
				if (Navigator.Instance.IsCurrentView("IdentityView")) Navigator.GoBack();
				
				onDeleted?.Invoke(this);
			}
		};

		CommandMap["OpenTopmostController"] = OpenTopmostController;
		CommandMap["ShowViewProfile"] = () => DialogBox.ShowTopmost<UserProfileSidePanelUserControl, UserProfileSidePanelViewModel>(
			vm: new UserProfileSidePanelViewModel(profile),
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
	}

	public void View() {
		if (!IsActionOptionsVisible) return;

		Navigator.Instance.NavigateTo("IdentityView", Dto);
	}

	public void OpenTopmostController() {
		DialogBox.ShowTopmost(
			vm: SnapCracklePopViewModel.Instance,
			v: SnapCracklePopUserControl.Instance,
			initialize: vm => {
				vm.RunningList.AddIfNotExists(new ObsProfile(Dto){IsShowGlyph = false, IsShowCheckboxColumn = false }, p => p.Dto?.id == Dto.id);
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
