using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles;
using Chameleon.lib;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.lib.Common.Models;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.WebBrowser.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsProfile : Obs<UserProfileDto> {
	private SystemBrowserService SysBrowserService { get; } = SystemBrowserService.Instance;
	public event Action<ObsProfile>? OnSelectedChanged;

	[ObservableProperty]
	private string _isChromeRunning = "False";
	[ObservableProperty]
	private string _isBraveRunning = "False";
	[ObservableProperty]
	private string _isFFRunning = "False";
	[ObservableProperty]
	private bool _isShowGlyph;
	[ObservableProperty]
	private bool _isShowC;
	[ObservableProperty]
	private bool _isShowD;
	[ObservableProperty]
	private bool _isShowF;
	[ObservableProperty]
	private bool _isForeground;

	//
	public bool IsShowCheckboxColumn { get; }
	public bool IsEnabledCheckboxColumn { get; } = true;
	public bool IsSharedProfile { get; }

	//
	public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
	public bool IsFavorite => Dto?.isFavourite ?? false;
	public bool IsDeleteProfileBtnVisible => !IsSharedProfile;
	public Dictionary<SystemBrowserType, ISysBrowserInstance?> SBI => Dto!.SBI;

	public ObsProfile(
			UserProfileDto userProfile,
			bool isShowCheckboxColumn = true,
			bool isShowGlyph = true,
			bool isShowC = true,
			bool isShowD = true,
			bool isShowF = true,
			bool hasActionOptions = true,
			Action<ObsProfile>? onSelectedChanged = default)
		: base(userProfile.title ?? "xxx") {
		SysBrowserService = SystemBrowserService.Instance;

		OnSelectedChanged = onSelectedChanged;
		Dto = userProfile;

		IsShowGlyph = isShowGlyph;
		IsShowD = isShowD;
		IsShowC = isShowC;
		IsShowF = isShowF;
		IsShowCheckboxColumn = isShowCheckboxColumn;
		IsActionOptionsVisible = hasActionOptions;
		IsSharedProfile = userProfile?.creatorUserId != Auther.AuthSession?.UserId;

		if (SysBrowserService != null) {
			async Task setEvents() {
				if (SysBrowserService.OpenTaskCompletionSource != null) {
					_ = await SysBrowserService.OpenTaskCompletionSource.Task;
				}
				foreach (var sbi in SBI) {
					if (sbi.Value != null) {
						_ = SetRunning(sbi.Value.Settings.BrowserType, true);
						sbi.Value.OnEvent += Browser_OnEvent;
					}
				}
			}
			_ = setEvents();
		}

	}

	public override void OnAnyIsSelectedChanged(bool value) {
		OnSelectedChanged?.Invoke(this);
	}
	public void Open() {
		Navigator.NavigateToType(typeof(IdentityView), Dto);
	}

	[RelayCommand]
	private void ShowViewProfile() {
		WShower.ShowTopmost<UserProfileSidePanelUserControl, UserProfileSidePanelViewModel>(new UserProfileSidePanelViewModel(Dto!), vm => {
		}, null, "Copy Pasta", 156);
	}
	[RelayCommand]
	private async Task Favorite() {
		Dto!.isFavourite = !IsFavorite;

		_ = await UserProfilesRepo.SetProfileIsFavorite(Dto.id, Dto.isFavourite);

		OnPropertyChanged(nameof(IsFavorite));
	}
	[RelayCommand]
	private async Task DeleteUserProfile() {
		if (await Mbox.Show("Delete User Profile",
			$"Are you sure you want to delete {Dto!.title}?",
			MBoxButtons.OkCancel,
			"DeleteLines")) {
			_ = await UserProfilesRepo.Instance.Delete(Dto.id);
			if (Navigator.Instance.Frame?.CanGoBack == true && Navigator.Instance.Frame.Content?.GetType() == typeof(IdentityView)){
				Navigator.Instance.Frame?.GoBack();
			}
			MyProfilesViewModel.Instance.SetViewModelsFilter();
		}
	}
	[RelayCommand]
	private void OpenUserProfile() {
		Open();
	}
	[RelayCommand]
	public void OpenUserBrowser() {
		WShower.ShowTopmost(SnapCracklePopViewModel.Instance, SnapCracklePopUserControl.Instance,
				vm => {
					if (!vm.RunningList.Any(p => p.Dto?.id == this.Dto?.id))
						vm.RunningList.Add(new ObsProfile(this.Dto!, false, false, false, false, false));
				},
				vm => {
					vm.RunningList.Clear();
				}, "SCP", 172);
	}
	[RelayCommand]
	private async Task OpenFirefox() {
		await OpenSystemBrowser(SystemBrowserType.Firefox);
	}
	[RelayCommand]
	private async Task OpenChrome() {
		await OpenSystemBrowser(SystemBrowserType.Chrome);
	}
	[RelayCommand]
	private async Task OpenBrave() {
		await OpenSystemBrowser(SystemBrowserType.Brave);
	}
	[RelayCommand]
	public async Task OpenSystemBrowser(SystemBrowserType browserType) {
		if (SysBrowserService == null) {
			return;
		}
		if (SBI.TryGetValue(browserType, out var browser)) {
			IsForeground = false;
			if (browser == null) {
				try {
					browser = await SysBrowserService.Open(new SysBrowserOpenOptions(
					browserType,
					new SysBrowserProfile() {
						Id = Dto!.id,
						Proxy = new SysBrowserProxy() {
							Host = Dto.proxy?.host,
							Port = Dto.proxy?.port ?? 0,
							UserName = Dto.proxy?.userName,
							Password = Dto.proxy?.password
						}
					}), () => {
						var urls = IoC.GetJsonValue<string[]>("DefaultHomePageSettings");
						if (urls is null || urls.Length == 0)
							urls = [Consts.DefaultHomePage];

						var starturl = urls[new Random().Next(urls.Length)];
						starturl = Uri.TryCreate(starturl, UriKind.Absolute, out var uriResult)
							&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
							? starturl
							: "https://" + starturl;
						return starturl;
					}).WaitAsync(TimeSpan.FromSeconds(21));
				} catch {
					browser = null;
				}

				var succeeded = false;
				if (browser != null) {
					try {
						succeeded = await browser.LoadedTCS.Task.WaitAsync(TimeSpan.FromSeconds(SysBrowserService.TimeOut));
					} catch {
						succeeded = false;
					}
				}
				if (!succeeded || browser == null) {
					IsForeground = false;
					_ = SetRunning(browserType, null);
				} else {
					_ = SetRunning(browserType, true);
					browser.OnEvent += Browser_OnEvent;
					SBI[browserType] = browser;
				}
			} else {
				browser.InvokeEvent(SysBrowserEventType.Foreground);
			}
		}
	}

	private void Browser_OnEvent(object sender, SysBrowserEvent args) {
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
		;
	}

	private string SetRunning(SystemBrowserType args, bool? running) => args switch {
		SystemBrowserType.Chrome => running != true && IsChromeRunning == "Error" ? "Error"
		: IsChromeRunning = IsChromeRunning = running is null ? "Error" : running == true ? "True" : "False",

		SystemBrowserType.Firefox => running != true && IsFFRunning == "Error" ? "Error"
		: IsFFRunning = IsFFRunning = running is null ? "Error" : running == true ? "True" : "False",

		SystemBrowserType.Brave => running != true && IsBraveRunning == "Error" ? "Error"
		: IsBraveRunning = IsBraveRunning = running is null ? "Error" : running == true ? "True" : "False",

		_ => "Error"
	};
}
