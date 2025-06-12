using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using Chameleon.lib.WebBrowser.Services;
using Chameleon.lib.Util;

using Chameleon.lib.WebBrowser;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Projects.Profiles.Dialogs;
using Chameleon.client.Services;
using Chameleon.lib.WebBrowser.Browsers;
using Chameleon.lib.Api.Dto;

namespace Chameleon.client.Features.Projects.Profiles;

public partial class ObsProfile : ObservableDtoViewModelBase<UserProfileDto> {
	[ObservableProperty] string isChromeRunning = "False";
	[ObservableProperty] string isBraveRunning = "False";
	[ObservableProperty] string isFFRunning = "False";
	[ObservableProperty] bool isShowGlyph = true;
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

	string SetRunning(SystemBrowserType args, bool running) => args switch {
		SystemBrowserType.Chrome => IsChromeRunning = running.ToString(),
		SystemBrowserType.Firefox => IsFFRunning = running.ToString(),
		SystemBrowserType.Brave => IsBraveRunning = running.ToString(),
		_ => throw new NotImplementedException()
	};
	public ObsProfile(UserProfileDto profile, Action<ObsProfile>? selectedChanged = default, Action<ObsProfile>? onDeleted = default)
	: base(profile, onSelectedChanged: selectedChanged != null ? (vm) => selectedChanged((ObsProfile)vm) : null) {
		AsyncCommandMap["OpenFirefox"] = async () => await OpenSystemBrowser(SystemBrowserType.Firefox);
		AsyncCommandMap["OpenChrome"] = async () => await OpenSystemBrowser(SystemBrowserType.Chrome);
		AsyncCommandMap["OpenBrave"] = async () => await OpenSystemBrowser(SystemBrowserType.Brave);
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
			// TODO: ? IsForeground = args.EventType == SysBrowserEventType.Foreground;
			var runnin = args.EventType switch {
				SysBrowserEventType.Foreground or
				SysBrowserEventType.Background or
				SysBrowserEventType.Opened => SetRunning(args.OpenOptions.BrowserType, true),
				SysBrowserEventType.Closed => SetRunning(args.OpenOptions.BrowserType, false),
				_ => "Error"
			};
			if (runnin is "Error" or "False") SBI[args.OpenOptions.BrowserType] = null;
		});
		// browsers.ForEach(b => _ = SetRunning(b, true));
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
				vm.RunningList.AddIfNotExists(new ObsProfile(Dto) { IsShowGlyph = false, IsShowCheckboxColumn = false }, p => p.Dto?.id == Dto.id);
			},
			onClosed: vm => {
				vm.RunningList.Clear();
			},
			title: "SCP",
			width: 172
		);
	}

	public async Task<IBrowserInstance?> OpenSystemBrowser(SystemBrowserType browserType, bool foreground = true) {
		if (SBI[browserType] is IBrowserInstance browser) {
			if (foreground && OperatingSystem.IsMacOS()) browser.Brocessor(false).Start();
			else if (foreground) browser.InvokeEvent(SysBrowserEventType.Foreground);
		} else SBI[browserType] = await SystemBrowserService.Instance.Open(new(browserType, SystemBrowserProfile));

		return SBI[browserType];
	}
}
