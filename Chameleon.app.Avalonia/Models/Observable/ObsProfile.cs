using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.Authorization;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Models;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.WebBrowser.Interfaces;
using Chameleon.lib.WebBrowser.Models;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsProfile : Vim<UserProfileDto> {
	private readonly IAuthSession _authSession = ContainerServiceHelper.Resolve<IAuthSession>()!;
	private readonly ISysBrowserService? SysBrowserServiceBase = IoC.GetService<ISysBrowserService>();

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
	[ObservableProperty]
	private bool _isSelected;

	public bool IsShowCheckboxColumn { get; }
	public bool IsEnabledCheckboxColumn { get; }
	public bool IsSharedProfile { get; }

	public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
	public bool IsFavorite => Dto?.isFavourite ?? false;
	public bool IsDeleteProfileBtnVisible => !IsSharedProfile;
	//public bool IsOutreachBtnEnabled => !IsSharedProfile || _applicationUser.HasPemission(PermissionNames.Pages_Outreach);

	public Dictionary<SystemBrowserType, ISysBrowserInstance?> SBI { get; set; } = new Dictionary<SystemBrowserType, ISysBrowserInstance?>(){
			{ SystemBrowserType.Chrome, null },
			{ SystemBrowserType.Firefox, null },
			{ SystemBrowserType.Brave, null }
		};

	public ObsProfile(
			UserProfileDto userProfile,
			bool isShowCheckboxColumn = true,
			bool isShowGlyph = true,
			bool isShowC = true,
			bool isShowD = true,
			bool isShowF = true)
		: base(userProfile.title ?? "xxx")
	{
		Dto = userProfile;

		//IsChromeRunning = _userProfile.IsChromeRunning;
		//IsBraveRunning = _userProfile.IsBraveRunning;
		//IsFFRunning = _userProfile.IsFFRunning;
		IsShowGlyph = isShowGlyph;
		IsShowD = isShowD;
		IsShowC = isShowC;
		IsShowF = isShowF;
		IsShowCheckboxColumn = isShowCheckboxColumn;
		IsEnabledCheckboxColumn = userProfile?.creatorUserId != _authSession.UserId;
		IsSharedProfile = userProfile?.creatorUserId != _authSession.UserId;

		if (SysBrowserServiceBase != null) {
			async Task setEvents()
			{
				if (SysBrowserServiceBase.OpenTaskCompletionSource != null) {
					_ = await SysBrowserServiceBase.OpenTaskCompletionSource.Task;
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


		_ = EventAggregator.GetEvent<SavedUserProfileEvent>().Subscribe(a => {
			if (a.UserProfile.Id == Dto!.id) {
				Title = Dto.title;
				OnPropertyChanged(nameof(Title));
				OnPropertyChanged(nameof(Code));
			}
		});
	}

	[RelayCommand]
	private void ShowViewProfile()
	{
		WShower.ShowTopmost<UserProfileSidePanelUserControl, UserProfileSidePanelViewModel>(new UserProfileSidePanelViewModel(Dto!), vm => {
			//vm.UserProfile = Dto;
		}, null, "Copy Pasta", 156);
	}

	[RelayCommand]
	private async Task Favorite()
	{
		Dto!.isFavourite = !IsFavorite;

		_ = await UserProfilesRepo.SetProfileIsFavorite(Dto.id, Dto.isFavourite);

		OnPropertyChanged(nameof(IsFavorite));
	}
	[RelayCommand]
	private async Task DeleteUserProfile()
	{
		if (await Mbox.Show("Delete User Profile",
			$"Are you sure you want to delete {Dto!.title}?",
			MBoxButtons.OkCancel,
			"DeleteLines")) {
			_ = await UserProfilesRepo.Instance.Delete(Dto.id);
		}
	}
	public void Open()
	{
		Navigator.NavigateToType(typeof(IUserProfileIdentityView), Dto);
		//OpenUserProfile();
	}

	[RelayCommand]
	private void OpenUserProfile()
	{
		Open();
	}
	[RelayCommand]
	public void OpenUserBrowser()
	{
		WShower.ShowTopmost(SnapCracklePopViewModel.Instance, SnapCracklePopUserControl.Instance,
				vm => {
					if (!vm.RunningList.Contains(this))
						vm.RunningList.Add(this);
				},
				vm => {
					vm.RunningList.Clear();
				}, "SCP", 172);
	}
	[RelayCommand]
	private async Task OpenFirefox()
	{
		await OpenSystemBrowser(SystemBrowserType.Firefox);
	}
	[RelayCommand]
	private async Task OpenChrome()
	{
		await OpenSystemBrowser(SystemBrowserType.Chrome);
	}
	[RelayCommand]
	private async Task OpenBrave()
	{
		await OpenSystemBrowser(SystemBrowserType.Brave);
	}
	[RelayCommand]
	public async Task OpenSystemBrowser(SystemBrowserType browserType)
	{
		if (SysBrowserServiceBase == null) {
			return;
		}
		if (SBI.TryGetValue(browserType, out var browser)) {
			IsForeground = false;
			if (browser == null) {
				browser = await SysBrowserServiceBase.Open(new SysBrowserOpenOptions(
				browserType,
				new UserProfileModel() {
					Id = Dto!.id,
					Proxy = new ProxySettingsModel() {
						Host = Dto.proxy?.host,
						Port = Dto.proxy?.port ?? 0,
						UserName = Dto.proxy?.userName,
						Password = Dto.proxy?.password
					}
				})
			);

				if (browser != null && await browser.LoadedTCS.Task) {
					_ = SetRunning(browserType, true);
					browser.OnEvent += Browser_OnEvent;
					SBI[browserType] = browser;
				} else {
					IsForeground = false;
					_ = SetRunning(browserType, null);
				}
			} else {
				browser.InvokeEvent(Enums.SysBrowserEventType.Foreground);
			}
		}
	}

	private void Browser_OnEvent(object sender, SysBrowserEvent args)
	{
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
		};
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
