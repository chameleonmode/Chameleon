using Chameleon.Authorization;
using Chameleon.Av.Fluent.Dialogs.ViewModels;
using Chameleon.Avalonia.Controls.UserProfileView.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Models;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.WebBrowser.Interfaces;
using Chameleon.lib.WebBrowser.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileViewModel : SubPageViewModelBase, IUserProfileActionsViewModel {
	private readonly IUserProfileService _userProfileService;
	private readonly IApplicationUser _applicationUser;
	private readonly ISysBrowserService? SysBrowserServiceBase = IoC.GetService<ISysBrowserService>();

	[ObservableProperty]
	private UserProfile _userProfile;
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

	public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
	public bool IsFavorite => UserProfile?.IsFavourite ?? false;
	public bool IsSharedProfile => _userProfileService.IsSharedProfile(UserProfile);
	public bool IsShowCheckboxColumn { get; }
	public bool IsEnabledCheckboxColumn { get; }
	public bool IsDeleteProfileBtnVisible => !IsSharedProfile && _applicationUser.HasPemission(PermissionNames.Pages_DeleteProfiles);
	public bool IsOutreachBtnEnabled => !IsSharedProfile || UserProfile.HasPermission(PermissionNames.Pages_Outreach);
	public bool IsRssBtnEnabled => !IsSharedProfile || UserProfile.HasPermission(PermissionNames.Pages_RSS);

	public Dictionary<SystemBrowserType, ISysBrowserInstance?> SBI  => UserProfile.SBI;

	IUserProfile IUserProfileViewModelBase.UserProfile => UserProfile;

	public UserProfileViewModel(
			IUserProfileService userProfileService,
			UserProfile userProfile,
			IApplicationUser applicationUser,
			bool isShowCheckboxColumn = true,
			bool isShowGlyph = true,
			bool isShowC = true,
			bool isShowD = true,
			bool isShowF = true
			)
	{
		_userProfileService = userProfileService;
		_applicationUser = applicationUser;
		_userProfile = userProfile;

		Title = userProfile.Title ?? "xxx";

		IsChromeRunning = _userProfile.IsChromeRunning;
		IsBraveRunning = _userProfile.IsBraveRunning;
		IsFFRunning = _userProfile.IsFFRunning;
		IsShowGlyph = isShowGlyph;
		IsShowD = isShowD;
		IsShowC = isShowC;
		IsShowF = isShowF;
		IsShowCheckboxColumn = isShowCheckboxColumn && _applicationUser.HasPemission(PermissionNames.Pages_DeleteProfiles);
		IsEnabledCheckboxColumn = !_userProfileService.IsSharedProfile(_userProfile);

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
			if (a.UserProfile.Id == UserProfile.Id) {
				Title = _userProfile.Title;
				OnPropertyChanged(nameof(Title));
				OnPropertyChanged(nameof(Code));
			}
		});
	}

	[RelayCommand]
	private void ShowViewProfile()
	{
		WShower.ShowTopmost<UserProfileSidePanelView, UserProfileSidePanelViewModel>(vm => {
			vm.UserProfile = UserProfile;
		}, null, "Copy Pasta", 156);
	}

	[RelayCommand]
	private void Favorite()
	{
		if (!IsFavorite) {
			UserProfile.IsFavourite = true;
			EventAggregator
					.GetEvent<FavoriteUserProfileEvent>()
					.Publish(new UserProfileEventArgs(UserProfile));

		} else {
			UserProfile.IsFavourite = false;
			EventAggregator
					.GetEvent<UnfavoriteUserProfileEvent>()
					.Publish(new UserProfileEventArgs(UserProfile));

		}

		EventAggregator
				.GetEvent<UpdateFavoriteFolderEvent>()
				.Publish();

		OnPropertyChanged(nameof(IsFavorite));
	}
	[RelayCommand]
	private async Task DeleteUserProfile()
	{
		if (await Mbox.Show("Delete User Profile",
			$"Are you sure you want to delete {UserProfile.Title}?",
			MBoxButtons.OkCancel,
			"DeleteLines"))
			EventAggregator
			 .GetEvent<DeleteUserProfileEvent>()
			 .Publish(new UserProfileEventArgs(UserProfile));
	}
	public void Open()
	{
		NavigationService?.NavigateToType(typeof(IUserProfileIdentityView), UserProfile);
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
		WShower.ShowTopmost(TopMostSidePanelViewModel.Instance, TopMostSidePanelView.Instance,
				vm => {
					if (!vm.RunningList.Contains(this))
						vm.RunningList.Add(this);

					vm.Update();
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
					Id = UserProfile.Id,
					Proxy = new ProxySettingsModel() {
						Host = UserProfile.Proxy.Host,
						Port = UserProfile.Proxy.Port,
						UserName = UserProfile.Proxy.UserName,
						Password = UserProfile.Proxy.Password
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
		: IsChromeRunning = UserProfile.IsChromeRunning = running is null ? "Error" : running == true ? "True" : "False",

		SystemBrowserType.Firefox => running != true && IsFFRunning == "Error" ? "Error"
		: IsFFRunning = UserProfile.IsFFRunning = running is null ? "Error" : running == true ? "True" : "False",

		SystemBrowserType.Brave => running != true && IsBraveRunning == "Error" ? "Error"
		: IsBraveRunning = UserProfile.IsBraveRunning = running is null ? "Error" : running == true ? "True" : "False",

		_ => "Error"
	};

	private bool _isSelected;
	public bool IsSelected {
		get => _isSelected;
		set {
			if (SetProperty(ref _isSelected, value)) {
				EventAggregator
						.GetEvent<SelectedChangeUserProfileEvent>()
						.Publish(new SelectedUserProfileEventArgs(UserProfile, _isSelected));
			}
		}
	}
}