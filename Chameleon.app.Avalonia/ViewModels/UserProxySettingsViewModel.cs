using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Reactive.Subjects;

using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DynamicData;

using static Chameleon.lib.Common.Constants.Enums.Api;

namespace Chameleon.app.Avalonia.ViewModels;

public partial class UserProxySettingsViewModel
			 : ViewModelObjectBase {
	private readonly BehaviorSubject<IPageRequest> pageRequests = new(new PageRequest(0, Consts.PageinationPageItems));

	private readonly ReadOnlyObservableCollection<ObsProxySetting> proxies;
	private readonly ReadOnlyObservableCollection<ObsFolder> folders;
	private readonly BehaviorSubject<Func<ObsProxySetting, bool>> filter;

	[ObservableProperty]
	private ProxCountryDto? country;
	[ObservableProperty]
	private ObsFolder? selectedFolder;
	[ObservableProperty]
	private string? applingProxy;
	[ObservableProperty]
	private int totalCount;
	[ObservableProperty]
	private PaginatorViewModel paginatorViewModel;

	public ObservableCollection<ProxCountryDto> Countries { get; } = [];
	public ReadOnlyObservableCollection<ObsFolder> Folders => folders;
	public ReadOnlyObservableCollection<ObsProxySetting> Proxies => proxies;
	public Func<ObsProxySetting, bool> FilterPredicate => p =>
		SelectedFolder == null || SelectedFolder.Dto?.id == 0 || 
		(SelectedFolder != null && SelectedFolder.Dto?.id != 0 && p.ObsProfile.Dto?.folderId == SelectedFolder.Dto?.id);
	public List<ObsProxySetting> SelectedProfiles => Proxies.Where(p => p.ObsProfile.IsSelected).ToList();
	public bool HasSelectedItems => Proxies.Any(setting => setting.ObsProfile.IsSelected);
	public int SelectedCount => Proxies.Count(setting => setting.ObsProfile.IsSelected);
	public int MaxInFolderItems => SelectedFolder == null || SelectedFolder.Dto!.id == 0 
		? UserProfilesRepo.Instance.ObservableCache.Count 
		: UserProfilesRepo.Instance.ObservableCache.Items.Count(i => i.folderId == SelectedFolder.Dto!.id);
	public UserProxySettingsViewModel() : base("Proxy")
	{
		filter = new BehaviorSubject<Func<ObsProxySetting, bool>>(FilterPredicate);

		_ = UserProfilesRepo
			.Connect()
			.Transform(i=> new ObsProxySetting(new ObsProfile(i, false, onSelectedChanged: (p) => {
				//PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
				//PaginatorViewModel.UpdatePageCount(Math.Max(Consts.PageinationPageItems, Proxies.Count(p => p.ObsProfile.IsSelected)));
				OnPropertyChanged(nameof(HasSelectedItems));
				OnPropertyChanged(nameof(SelectedCount));
			})))
			.Filter(filter)
			.SortAndPage(Compares.ObsProxySettingCompares.AscendingComparer, pageRequests)
			.Bind(out proxies)
			.Subscribe();

		_ = UserProfilesFolderRepo
			.Connect()
			.Transform(i => new ObsFolder(i))
			.SortAndBind(out folders, Compares.ObsFolderCompares.AscendingComparer)
			.Subscribe();
		PaginatorViewModel = new PaginatorViewModel((p) => pageRequests.OnNext(new PageRequest(p.CurrentIndex, p.OnPageItems))) {
			TotalCount = UserProfilesRepo.Instance.ObservableCache.Count,
		};
		TotalCount = PaginatorViewModel.TotalCount;
		SelectedFolder = folders[0];
	}
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		if (!Loaded) {
			var countries = await ProxyAccessRepo.GetCountries();
			Countries.Clear();

			Countries.Add(new() {
				Name = "Random Country"
			});
			Countries.AddRange(countries);

			Country = Countries.First();
		}
	}
	public override async Task OnNavigatedToAsync(object? param)
	{
		await base.OnNavigatedToAsync(param);

		if (param is ObsFolder folderId) {
			_ = await LoadedTCS.Task;
			SelectedFolder = Folders.FirstOrDefault(f=>f.Dto!.id == folderId.Dto!.id) ?? Folders[0];
		}
	}

	partial void OnSelectedFolderChanged(ObsFolder? value)
	{
		UnselectItems();
		filter.OnNext(FilterPredicate);
		TotalCount = PaginatorViewModel.TotalCount = MaxInFolderItems;
		//PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
		//PaginatorViewModel.UpdatePageCount(Math.Max(Consts.PageinationPageItems, Proxies.Count(p => p.ObsProfile.IsSelected)));
		//OnPropertyChanged(nameof(HasSelectedItems));
		//OnPropertyChanged(nameof(SelectedCount));
	}
	
	[RelayCommand]
	public async Task FillProxies()
	{
		var profiles = SelectedProfiles;
		if (profiles.Count == 0) {
			return;
		}
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
	public async Task ApplyProxy()
	{
		if(SelectedProfiles.Count == 0) {
			return;
		}

		if (!ApplingProxy.Is()) {
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
			await ApplyProxy(proxies,	SelectedProfiles);
		}

		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
		Toaster.Success($"Update was successful.");
	}
	private static async Task ApplyProxy(List<ProxDto> proxies, List<ObsProxySetting> models)
	{
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
	private static async Task ApplyProxy(ProxDto? proxySettings, ObsProxySetting model)
	{
		if (proxySettings != null) {
			model.Host = proxySettings.host;
			model.Port = proxySettings.port;
			model.UserName = proxySettings.userName;
			model.Password = proxySettings.password;
		}
		model.SetProfile();
		_ = await UserProfilesRepo.Instance.Put(model.ObsProfile.Dto!);
	}
	private static List<ProxDto> ParseProxiesSettings(string[] proxyList)
	{
		var proxies = new List<ProxDto>();
		foreach (var applingProxy in proxyList) {
			var applingProxies = applingProxy
				.StripPrefix("http://")
				.StripPrefix("https://")
				.Split(':');
			if (applingProxies.Length != 4) {
				Toaster.Error($"Not a valid set {applingProxy}");
				continue;
			}
			var portStr = applingProxies[1];
			var isValidPort = int.TryParse(portStr, out var port);
			if (!isValidPort && portStr.Is()) {
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
	private void UnselectItems()
	{
		foreach (var model in Proxies) {
			model.ObsProfile.IsSelected = false;
		}
		PaginatorViewModel.UpdatePageCount(Consts.PageinationPageItems);
	}
	[RelayCommand]
	private void SelectAll()
	{
		foreach (var model in Proxies) {
			model.ObsProfile.IsSelected = true;
		}
	}
	[RelayCommand]
	private void SelectAllFromFolder()
	{
		PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
		SelectAll();
	}
}
