using System.Collections.ObjectModel;

using Chameleon.app.Avalonia.Models;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DynamicData;

using static Chameleon.lib.Common.Constants.Enums.Api;

namespace Chameleon.app.Avalonia.ViewModels;

public partial class ProxyCreditViewModel
			: ViewModelObjectBase {
	[ObservableProperty]
	private CreditPlan selectedCreditPlan;
	[ObservableProperty]
	private int countProxies = 5;
	[ObservableProperty]
	private ProxCountryDto? country;
	[ObservableProperty]
	private decimal balance;

	public ObservableCollection<CreditPlan> CreditPlans { get; } = [];
	public ObservableCollection<ProxCountryDto> Countries { get; } = [];
	public ObservableCollection<ObsProxyAccess> Accesses { get; } = [];

	public List<int> CountsProxies { get; } = [5, 10, 100, 500];

	public ProxyCreditViewModel() : base("Proxy Credit")
	{
		CreditPlans.Add(new(19, "5GB", true));
		CreditPlans.Add(new(29, "10GB"));
		CreditPlans.Add(new(49, "20GB"));
		selectedCreditPlan = CreditPlans.First();

		AddObsProxyAccessItems(countProxies);
	}

	public void AddObsProxyAccessItems(int amout)
	{
		for (var i = 0; i < amout; i++) {
			Accesses.Add(new ObsProxyAccess());
		}
	}

	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		if (Loaded)
			return;

		//await InitializeCountriesAsync();
		var countries = await ProxyAccessRepo.GetCountries();
		Countries.Clear();

		Countries.Add(new() {
			Name = "Random Country"
		});
		Countries.AddRange(countries);
		Country = Countries[0];

		await UpdateBalanceAsync();
	}

	private async Task UpdateBalanceAsync()
	{
		var credits = await ProxyCreditRepo.GetCredits();
		Balance = credits.Amount;
	}

	[RelayCommand]//
	public async Task CopyAllUrls()
	{
		var list = Accesses.Select(a => a.Url);
		await CopyPasta.Copy(string.Join("\n", list));
	}
	[RelayCommand]
	private async Task PurchaseCredit()
	{
		var res = await ProxyCreditRepo.CreateOrder(SelectedCreditPlan.Amount);
		if (res?.Url != null) {
			ProUtil.GoToUrlDefault(res.Url);
		}
	}

	[RelayCommand]
	private async Task Refresh()
	{
		await UpdateBalanceAsync();
	}
	[RelayCommand]
	private async Task RefreshProxies()
	{
		await UpdateProxyAccessAsync();
	}

	[RelayCommand]
	private async Task RefreshProxiesCount()
	{
		var currentCount = Accesses.Count;

		if (currentCount > CountProxies) {
			while (Accesses.Count > CountProxies)
				Accesses.RemoveAt(Accesses.Count - 1);
		} else {
			AddObsProxyAccessItems(CountProxies - currentCount);
			await UpdateProxyAccessAsync();
		}
	}

	private async Task UpdateProxyAccessAsync()
	{
		var request = new ProxyAccessRequestDto {
			HostType = ProxyHostType.Hostname,
			IpType = ProxyIpType.Sticky,
			ProtocolType = ProxyProtocolType.Http,
			CountryId = Country?.id,
			Count = Accesses.Count,
		};


		var urls = await ProxyAccessRepo.GetAccess(request);
		for (var i = 0; i < urls.Length; ++i) {
			Accesses[i].Url = urls.ElementAt(i).Url;
		}
	}
}
