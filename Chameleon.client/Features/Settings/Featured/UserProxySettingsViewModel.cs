using System.Collections.ObjectModel;
using System.Reactive.Subjects;

using Chameleon.lib.Api.Repos;
using Chameleon.lib.Util;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DynamicData;
using DynamicData.Binding;

using Chameleon.client.UI.Components.ViewModels;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Dto;

namespace Chameleon.client.Features.Settings.Featured;

public partial class ObsProxySetting : ViewModelObjectBase {
	[ObservableProperty] string? host;
	[ObservableProperty] int port = 80;
	[ObservableProperty] string? userName;
	[ObservableProperty] string? password;
	[ObservableProperty] ObsProfile obsProfile;

	public ObsProxySetting(ObsProfile profile) {
		obsProfile = profile;
		host = obsProfile.Dto!.proxy!.host;
		port = obsProfile.Dto.proxy.port;
		userName = obsProfile.Dto.proxy.userName;
		password = obsProfile.Dto.proxy.password;
	}

	partial void OnPortChanged(int value) {
		if (value < 0 || value >= 65535) {
			Port = 0;
		}
	}

	public void SetProfile() {
		ObsProfile.Dto!.proxy!.host = Host;
		ObsProfile.Dto.proxy.port = Port;
		ObsProfile.Dto.proxy.userName = UserName;
		ObsProfile.Dto.proxy.password = Password;
	}
}

public partial class ObsProxyAccess : ViewModelObjectBase {
	[ObservableProperty] string? url;
}

public partial class UserProxySettingsViewModel : ViewModelObjectBase {
	public static SortExpressionComparer<ObsProxySetting> AscendingComparer => SortExpressionComparer<ObsProxySetting>.Descending(p => p.ObsProfile.IsSelected).ThenByAscending(p => p.ObsProfile.Title!);
	public static SortExpressionComparer<ObsProxySetting> DescendingComparer => SortExpressionComparer<ObsProxySetting>.Descending(p => p.ObsProfile.Title!);

	private readonly BehaviorSubject<IPageRequest> pageRequests = new(new PageRequest(0, 9));
	private readonly BehaviorSubject<Func<ObsProxySetting, bool>> filter;

	[ObservableProperty] ProxCountryDto? country;
	[ObservableProperty] ObsFolder? selectedFolder;
	[ObservableProperty] string? applingProxy;
	[ObservableProperty] PaginatorViewModel paginatorViewModel;

	public ObservableCollection<ProxCountryDto> Countries { get; } = [];
	public ReadOnlyObservableCollection<ObsProxySetting> Proxies { get; }
	public ReadOnlyObservableCollection<ObsFolder> Folders => FoldersViewModel.Instance.Folders;
	public Func<ObsProxySetting, bool> FilterPredicate => p =>
		SelectedFolder == null || SelectedFolder.Dto?.id == 0 ||
		(SelectedFolder != null && SelectedFolder.Dto?.id != 0 && p.ObsProfile.Dto?.folderId == SelectedFolder.Dto?.id);
	public List<ObsProxySetting> SelectedProfiles => Proxies.Where(p => p.ObsProfile.IsSelected).ToList();
	public bool HasSelectedItems => Proxies.Any(setting => setting.ObsProfile.IsSelected);
	public int SelectedCount => Proxies.Count(setting => setting.ObsProfile.IsSelected);
	public int MaxInFolderItems => SelectedFolder == null || SelectedFolder.Dto!.id == 0
		? UserProfilesRepo.Instance.ObservableCache.Count
		: UserProfilesRepo.Instance.ObservableCache.Items.Count(i => i.folderId == SelectedFolder.Dto!.id);
	public UserProxySettingsViewModel() : base("Proxy") {
		filter = new BehaviorSubject<Func<ObsProxySetting, bool>>(FilterPredicate);
		_ = UserProfilesRepo.Connect().Transform(i => new ObsProxySetting(new(i, selectedChanged: (p) => {
			OnPropertyChanged(nameof(HasSelectedItems));
			OnPropertyChanged(nameof(SelectedCount));
		}) { IsShowCheckboxColumn = false }))
		.Filter(filter)
		.SortAndPage(AscendingComparer, pageRequests)
		.Bind(out var proxies)
		.Subscribe();
		Proxies = proxies;
		PaginatorViewModel = new PaginatorViewModel((p) => pageRequests.OnNext(new PageRequest(p.PageIndex + 1, p.OnPageItems))) {
			TotalCount = UserProfilesRepo.Instance.ObservableCache.Count,
		};
		PaginatorViewModel.UpdatePageCount(9);

		AsyncCommandMap["ApplyProxy"] = ApplyProxy;
		AsyncCommandMap["FillProxies"] = FillProxies;
		CommandMap["UnselectItems"] = UnselectItems;
		CommandMap["SelectAll"] = SelectAll;
		CommandMap["SelectAllFromFolder"] = SelectAllFromFolder;
	}
	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);

		if (Countries.Count == 0) {
			var countries = await ProxyAccessRepo.GetCountries();
			Countries.Add(new() {
				Name = "Random Country"
			});
			Countries.AddRange(countries);
			Country = Countries.First();
		}
	}
	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);
		SelectedFolder = FoldersViewModel.Instance.SelectedFolder;
		SelectedFolder ??= Folders[0];
	}

	partial void OnSelectedFolderChanged(ObsFolder? value) {
		filter.OnNext(FilterPredicate);
		PaginatorViewModel.TotalCount = MaxInFolderItems;
		PaginatorViewModel.UpdatePageCount(MaxInFolderItems > 0 ? 9 : 1);
	}

	[RelayCommand]
	public async Task FillProxies() {
		var profiles = SelectedProfiles;
		if (profiles.Count == 0) return;
		
		try {
			var urls = await ProxyAccessRepo.GetAccess(new ProxyAccessRequestDto {
				HostType = ProxyHostType.Hostname,
				IpType = ProxyIpType.Sticky,
				ProtocolType = ProxyProtocolType.Http,
				CountryId = Country?.id,
				Count = profiles.Count,
			});
			if (urls.Length == 0) {
				throw new Exception("No Proxy Credit");
			}

			var proxies = ParseProxiesSettings(urls.Select(p => p.Url).ToArray()!);
			await ApplyProxy(proxies, profiles);

			OnPropertyChanged(nameof(HasSelectedItems));
			OnPropertyChanged(nameof(SelectedCount));

			Toaster.Success($"Update was successful.");
		} catch {
			Toaster.Error("No Proxy Credit");
		}
	}
	[RelayCommand]
	public async Task ApplyProxy() {
		if (SelectedProfiles.Count == 0) {
			return;
		}

		if (ApplingProxy.Is()) {
			if (SelectedProfiles != null) {
				foreach (var model in SelectedProfiles) {
					if (
						model.ObsProfile.Dto!.proxy.host != model.Host ||
						model.ObsProfile.Dto!.proxy.port != model.Port ||
						model.ObsProfile.Dto!.proxy.userName != model.UserName ||
						model.ObsProfile.Dto!.proxy.password != model.Password) {
						await ApplyProxy(null, model);
					}
				}
			}
		} else {
			var proxies = ParseProxiesSettings(ApplingProxy!.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
			await ApplyProxy(proxies, SelectedProfiles);
		}

		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
		Toaster.Success($"Update operation completed.");
	}
	private static async Task ApplyProxy(List<ProxDto> proxies, List<ObsProxySetting> models) {
		if (proxies.Count == 1) {
			for (var i = 0; i < models.Count; ++i) {
				await ApplyProxy(proxies[0], models[i]);
			}
		} else {
			var minCount = Math.Min(proxies.Count, models.Count);

			List<Task> tasks = [];
			for (var i = 0; i < minCount; ++i) {
				tasks.Add(ApplyProxy(proxies[i], models[i]));
			}
			await Task.WhenAll(tasks);
		}
	}
	private static async Task ApplyProxy(ProxDto? proxySettings, ObsProxySetting model) {
		try {
			if (proxySettings != null) {
				model.Host = proxySettings.host;
				model.Port = proxySettings.port;
				model.UserName = proxySettings.userName;
				model.Password = proxySettings.password;
			}
			model.SetProfile();
			_ = await UserProfilesRepo.Instance.Put(model.ObsProfile.Dto!);
		} catch (Exception ex) {
			Toaster.Error($"Failed to apply proxy for profile '{model.ObsProfile.Title}': {ex.Message}");
		}
	}
	private static List<ProxDto> ParseProxiesSettings(string[] proxyList) {
		var proxies = new List<ProxDto>();
		foreach (var applingProxy in proxyList) {
			var applingProxies = applingProxy
				.Strip("http://")
				.Strip("https://")
				.Split(':');
			if (applingProxies.Length != 4) {
				Toaster.Error($"Not a valid set {applingProxy}");
				continue;
			}
			var portStr = applingProxies[1];
			var isValidPort = int.TryParse(portStr, out var port);
			if (!isValidPort && portStr.IsNot()) {
				Toaster.Error($"Port cann't be text {applingProxy}");
				continue;
			}

			proxies.Add(new() {
				host = applingProxies[0],
				port = port,
				userName = applingProxies[2],
				password = applingProxies[3]
			});
		}
		return proxies;
	}

	[RelayCommand]
	private void UnselectItems() {
		foreach (var model in Proxies) {
			model.ObsProfile.IsSelected = false;
		}
		PaginatorViewModel.UpdatePageCount(9);
	}
	[RelayCommand]
	private void SelectAll() {
		foreach (var model in Proxies) {
			model.ObsProfile.IsSelected = true;
		}
	}
	[RelayCommand]
	private void SelectAllFromFolder() {
		PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
		SelectAll();
	}
}
