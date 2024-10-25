using System.Collections.ObjectModel;
using System.Reactive.Subjects;

using Chameleon.app.Avalonia.Com.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.App.Shared.Proxies;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DynamicData;

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
	private bool showCustomizeProxies;
	[ObservableProperty] 
	private bool isSelectedAll;

	private PaginatorViewModel? _paginatorViewModel;
	public PaginatorViewModel? PaginatorViewModel {
		get => _paginatorViewModel;
		set {
			if (SetProperty(ref _paginatorViewModel, value)) {
				_paginatorViewModel!.ChangePageIndex += (s, a) => { pageRequests.OnNext(new PageRequest(_paginatorViewModel.PageIndex, Consts.PageinationPageItems)); };
			}
		}
	}

	public ObservableCollection<ProxCountryDto> Countries { get; } = [];
	public ReadOnlyObservableCollection<ObsFolder> Folders => folders;
	public ReadOnlyObservableCollection<ObsProxySetting> Proxies => proxies;
	public Func<ObsProxySetting, bool> FilterPredicate => p =>
		SelectedFolder == null || SelectedFolder.Dto?.id == 0 || 
		(SelectedFolder != null && SelectedFolder.Dto?.id != 0 && p.ObsProfile.Dto?.folderId == SelectedFolder.Dto?.id);
	public List<ObsProxySetting> SelectedProfiles => Proxies.Where(p => p.ObsProfile.IsSelected).ToList();
	public bool HasSelectedItems => Proxies.Any(setting => setting.ObsProfile.IsSelected);
	public int SelectedCount => Proxies.Count(setting => setting.ObsProfile.IsSelected);
	public UserProxySettingsViewModel() : base("Proxy")
	{
		filter = new BehaviorSubject<Func<ObsProxySetting, bool>>(FilterPredicate);

		_ = UserProfilesRepo
			.Connect()
			.Transform(i=> new ObsProxySetting(new ObsProfile(i, false, onSelectedChanged: () => {
				OnPropertyChanged(nameof(HasSelectedItems));
				OnPropertyChanged(nameof(SelectedCount));
			})))
			.Filter(filter)
			.SortAndBind(out proxies, Compares.ObsProxySettingCompares.AscendingComparer)
			.Subscribe(i => {
				if (Proxies != null) {
					PaginatorViewModel ??= new PaginatorViewModel(Proxies.Count);
					PaginatorViewModel.TotalCount = Proxies.Count;
					TotalCount = PaginatorViewModel.TotalCount;
				}
			});

		_ = UserProfilesFolderRepo
			.Connect()
			.Transform(i => new ObsFolder(i))
			.SortAndBind(out folders, Compares.ObsFolderCompares.AscendingComparer)
			.Subscribe();
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
	
	partial void OnIsSelectedAllChanged(bool value)
	{
		foreach (var item in Proxies) {
			item.ObsProfile.IsSelected = value;
		}
	}
	partial void OnSelectedFolderChanged(ObsFolder? value)
	{
		IsSelectedAll = false;
		filter.OnNext(FilterPredicate);
	}
	
	[RelayCommand]
	public async Task FillProxies()
	{
		var profiles = SelectedProfiles;
		if (profiles.Count == 0) {
			return;
		}

		var request = new ProxyAccessRequestDto {
			HostType = ProxyHostType.Hostname,
			IpType = ProxyIpType.Sticky,
			ProtocolType = ProxyProtocolType.Http,
			CountryId = Country?.id,
			Count = profiles.Count,
		};

		var urls = await ProxyAccessRepo.GetAccess(request);

		if (urls.Length == 0) {
			_ = await Mbox.ShowErrorAsync("No Proxy Credit", "You have no proxy to set. Purchase them on Proxy Credit tab");
			return;
		}

		var proxies = ParseProxiesSettings(urls.Select(p=> p.Url).ToArray()!);

		await ApplyProxy(proxies, profiles);

		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
	}
	[RelayCommand]
	public async Task ApplyProxy()
	{
		var models = SelectedProfiles;
		if(models.Count == 0) {
			return;
		}

		if (!ApplingProxy.Is()) {
			if (models != null) {
				foreach (var model in models) {
					if (model.ObsProfile.Dto!.proxy.host != model.Host ||
						model.Port != model.ObsProfile.Dto!.proxy.port ||
						model.ObsProfile.Dto!.proxy.userName != model.UserName ||
						model.ObsProfile.Dto!.proxy.password != model.Password) {
						await ApplyProxy(null, model);
					}
				}
			}
		} else {
			await ApplyProxy(ParseProxiesSettings(
				ApplingProxy!.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries)), 
				models);
		}

		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
	}
	private async Task ApplyProxy(List<ProxDto> proxies, List<ObsProxySetting> models)
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
		//model.ObsProfile.IsSelected = true;
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
				Toaster.ShowErr($"Not a valid set {applingProxy}");
				continue;
			}
			var portStr = applingProxies[1];
			var isValidPort = int.TryParse(portStr, out var port);
			if (!isValidPort && portStr.Is()) {
				Toaster.ShowErr($"Port cann't be text {applingProxy}");
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
		IsSelectedAll = false;
		foreach (var model in Proxies) {
			model.ObsProfile.IsSelected = false;
		}
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
		IsSelectedAll = true;
	}
	[RelayCommand]
	private void HideCustomizeProxies()
	{
		ShowCustomizeProxies = false;
	}
}
