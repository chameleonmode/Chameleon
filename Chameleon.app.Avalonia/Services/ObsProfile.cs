using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Chameleon.app.Avalonia.Controls;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.WebBrowser.Services;
using Chameleon.lib.Util;
using static Chameleon.lib.Common.Constants.Enums;
using Chameleon.lib.WebBrowser;
using DynamicData.Binding;
using Chameleon.lib.Helpers;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class UserProfileSidePanelViewModel : ViewModelObjectBase {
	private readonly ReadOnlyObservableCollection<UPAddressDto> addresses;
	private readonly ReadOnlyObservableCollection<UPLoginDto> logins;
	private readonly ReadOnlyObservableCollection<UPPersonDto> persons;

	public ObservableCollection<CountryzDto> Countries { get; } = new ObservableCollection<CountryzDto>(CountryzRepo.Instance.Countryz);
	public ReadOnlyObservableCollection<UPAddressDto> ProfileAddresses => addresses;
	public ReadOnlyObservableCollection<UPLoginDto> ProfileLogins => logins;
	public ReadOnlyObservableCollection<UPPersonDto> ProfilePersons => persons;

	[ObservableProperty]
	private UPLoginDto? selectedLogin;
	[ObservableProperty]
	private UPPersonDto? selectedPerson;
	[ObservableProperty]
	private UPAddressDto? selectedAddress;
	[ObservableProperty]
	private ObsProfile? userProfile;

	public string? CountryName => Countries?.FirstOrDefault(x => SelectedAddress?.CountryId == x.id)?.Name;
	public bool HasPersons => ProfilePersons.Count > 0;
	public bool HasAddresses => ProfileAddresses?.Count > 0;
	public bool HasLogins => ProfileLogins?.Count > 0;

	public UserProfileSidePanelViewModel(UserProfileDto up) {
		_ = UPAdditionalDataRepo.Instance.Personz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out persons)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasPersons));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Loginz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out logins)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasLogins));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Addrez
			.Connect(i => i.ProfileId == up.id)
			.Bind(out addresses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasAddresses));
			});
		userProfile = new ObsProfile(up);
	}

	public override Task InitAsync(object? param) {
		return UPAdditionalDataRepo.Instance.Load();
	}

	partial void OnSelectedAddressChanged(UPAddressDto? value) => OnPropertyChanged(nameof(CountryName));
}

public class SnapCracklePopViewModel : ViewModelObjectBase {
	public ObservableCollection<ObsProfile> RunningList { get; set; } = [];

	public static SnapCracklePopViewModel Instance { get; } = new SnapCracklePopViewModel();
}

public partial class ObsProfile : ObservableDtoViewModelBase<UserProfileDto> {
	[ObservableProperty] string isChromeRunning = "False";
	[ObservableProperty] string isBraveRunning = "False";
	[ObservableProperty] string isFFRunning = "False";
	[ObservableProperty] bool isShowGlyph;
	[ObservableProperty] bool isForeground;
	[ObservableProperty] bool isFavorite;

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

	public new event Action<ObsProfile>? OnSelectedChanged;

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
			if (await MessageBox.Show(
				title: "Delete User Profile",
				content: $"Are you sure you want to delete {userProfile.title}?",
				btns: MBoxButtons.OkCancel,
				fontIconInfo: "DeleteLines"
			)) {
				_ = await UserProfilesRepo.Instance.Delete(userProfile.id);
				if (Navigator.Instance.CanGoBack && Navigator.Instance.IsCurrentView("IdentityView")) Navigator.Instance.GoBack();
				
				onDeleted?.Invoke(this);
			}
		};

		CommandMap["OpenTopmostController"] = OpenTopmostController;
		CommandMap["ShowViewProfile"] = () => DialogBox.ShowTopmost<UserProfileSidePanelUserControl, UserProfileSidePanelViewModel>(
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

	public void View() {
		if (!IsActionOptionsVisible) return;

		Navigator.Instance.NavigateTo("IdentityView", Dto);
	}

	public void OpenTopmostController() {
		DialogBox.ShowTopmost(
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
