using System.Collections.ObjectModel;

using Chameleon.App.Shared.Proxies;
using Chameleon.Core.Collections.Views;
using Chameleon.Interfaces.Proxies;
using Chameleon.Interfaces.ProxyCredit;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DynamicData;

namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public class AsyncCollectionViewModel<T> : ObservableObjectBase
		 where T : class {
	private readonly Func<IEnumerable<T>> _getItems;
	public AsyncCollectionViewModel(Func<IEnumerable<T>> getItems, bool isVisible = false)
	{
		_getItems = getItems;
		_isVisible = isVisible;
	}

	private ObservableCollection<T> _items;
	public ObservableCollection<T> Items => _items;

	private T _selectedItem;
	public T SelectedItem {
		get => _selectedItem;
		set {
			if (_selectedItem != value) {
				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
			}
		}
	}

	private bool _isLoading;
	public bool IsLoading {
		get => _isLoading;
		private set {
			if (_isLoading != value) {
				_isLoading = value;
				OnPropertyChanged(nameof(IsLoading));
			}
		}
	}

	public void Add(T item)
	{
		_items.Add(item);
	}

	public void Remove(T item)
	{
		var currentIndex = _items.IndexOf(item);
		if (currentIndex != -1) {
			_items.RemoveAt(currentIndex);
			SelectedItem = _items.ElementAtOrDefault(currentIndex);
		}
	}

	public void Clear()
	{
		if (_items == null) {
			return;
		}

		_items?.Clear();
		SelectedItem = null;
	}

	public Task Load()
	{
		if (_items == null) {
			return AddItemsAsync();
		}
		return Task.CompletedTask;
	}

	public override Task InitAsync(object? param)
	{
		return Load();
	}

	public Task Reload()
	{
		Clear();
		_isBinded = false;
		return AddItemsAsync();
	}

	private async Task AddItemsAsync()
	{
		if (_items == null) {
			_items = [];
		}

		IsLoading = true;
		var items = await Task.Run(_getItems);
		Items.AddRange(items);
		EnsureBinded();
		IsLoading = false;
		//_items.AddRangeAsync(_getItems, DispatcherService).ContinueWith(t =>
		//{
		//    EnsureBinded();
		//    IsLoading = false;
		//});
	}

	private bool _isVisible;
	public bool IsVisible {
		get => _isVisible;
		set {
			if (SetProperty(ref _isVisible, value)) {
				EnsureBinded();
			}
		}
	}

	private bool _isBinded;
	private void EnsureBinded()
	{
		if (_isBinded || !_isVisible) {
			return;
		}
		_isBinded = true;
		OnItemsBinded();
	}

	private void OnItemsBinded()
	{
		RaiseItemsChanged();
		SelectFirstItem();
		Binded?.Invoke(this, new EventArgs());
	}

	private void SelectFirstItem()
	{
		if (_items.Count > 0) {
			SelectedItem = _items[0];
		}
	}

	private void RaiseItemsChanged()
	{
		OnPropertyChanged(nameof(Items));
	}

	public event EventHandler Binded;
}


#region CreditPlan
public class CreditPlan
    : ViewModelObjectBase {
    private readonly IEventAggregator _eventAggregator;
    public CreditPlan(IEventAggregator eventAggregator, decimal amount, string size)
    {
        _eventAggregator = eventAggregator;
        Amount = amount;
        Size = size;
    }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    private string _size;
    public string Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (SetProperty(ref _isChecked, value) && value)
            {
                _eventAggregator
                    .GetEvent<SelectedCreditPlanEvent>()
                    .Publish(new SelectedCreditPlanEventArgs(IsChecked));
            }
        }
    }
}

public class CreditPlans
    : ObservableCollection<CreditPlan>
{
    private readonly IEventAggregator _eventAggregator;
    public CreditPlans(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;

        Add(new CreditPlan(_eventAggregator, 19, "5GB"));
        Add(new CreditPlan(_eventAggregator, 29, "10GB"));
        Add(new CreditPlan(_eventAggregator, 49, "20GB"));
    }
}
#endregion

#region ProxyAccess
public interface IProxyAccessViewModels
    : IList<ProxyAccessViewModel>
    , Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
{
    void AddItems(int count);
}

public partial class ProxyAccessViewModel
    : ViewModelObjectBase {
    public ProxyAccessViewModel()
    {
    }
    private string _url;
    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    [RelayCommand]
    private async Task CopyUrl()
    {
        if (_url == null)
        {
            return;
        }

		 await CopyPasta.Copy(_url);
    }
}

public class ProxyAccessViewModels
    : List<ProxyAccessViewModel>
    , IProxyAccessViewModels
{
    public ProxyAccessViewModels()
    {
    }

    public void AddItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Add(new ProxyAccessViewModel());
        }
    }
}

#endregion

public partial class ProxyCreditViewModel
      : ViewModelObjectBase
{
    private readonly IProxyService _proxyService;
    private readonly IProxyCreditService _proxyCreditService;
    private readonly IProxyAccessViewModels _proxyAccessViewModels;

    [ObservableProperty]
    public CreditPlans _creditPlans;

    [ObservableProperty]
    private CreditPlan _selectedCreditPlan;

    [ObservableProperty]
    private bool _hasSelectedCreditPlan;

    [ObservableProperty]
    private int _countProxies;

    [ObservableProperty]
    private bool _isGettingAccess;

    [ObservableProperty]
    private bool _isGettingBallance;

    public List<int> CountsProxies { get; } = [5, 10, 100, 500];
    public string Balance => $"${_balanceAmount}";

    public ProxyCreditViewModel(
        IProxyService proxyService,
        IProxyCreditService proxyCreditService,
        IProxyAccessViewModels proxyAccessViewModels
        )
    {
        Title = "Proxy Credit";
        CountProxies = 5;

        _proxyService = proxyService;
        _proxyCreditService = proxyCreditService;
        _proxyAccessViewModels = proxyAccessViewModels;
        _proxyAccessViewModels.AddItems(CountProxies);
    }

    public override async Task InitAsync(object? param)
    {       
        await base.InitAsync(param);

        if (Loaded)
            return;

        await UpdateBalanceAsync();
        await InitializeCountriesAsync();
        CreditPlans = new CreditPlans(EventAggregator);

        EventAggregator
            .GetEvent<SelectedCreditPlanEvent>()
            .Subscribe(args => OnSelectedCreditPlan(args));

        OnPropertyChanged(string.Empty);
    }

    private Task InitializeCountriesAsync()
    {
        Countries = new AsyncCollectionViewModel<IProxyCountry>(GetCountries, true);
        Countries.Clear();
        return Countries.Load();
    }     
    private IList<IProxyCountry> GetCountries()
    {
        var countries = _proxyService.GetCountries();
				Country = countries[0];
        return countries;
    }

    [RelayCommand]
    public async Task CopyAllUrls()
    {
        var list = Access.Select(a => a.Url);
        await CopyPasta.Copy(string.Join("\n", list));
    }

    [RelayCommand]
    private async Task PurchaseCredit()
    {                      
        if (!HasSelectedCreditPlan)
        {
            return;
        }
        IsLoadingIndicatorVisible = true;
        await MakePaymentAsync();
        IsLoadingIndicatorVisible = false;
    }

    [ObservableProperty]
    private bool _isLoadingIndicatorVisible;

    private void OnSelectedCreditPlan(SelectedCreditPlanEventArgs args)
    {
        SelectedCreditPlan = CreditPlans.First(a => a.IsChecked);
        HasSelectedCreditPlan = true;
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
        var currentCount = _proxyAccessViewModels.Count;
        _access = null;

        if (currentCount > CountProxies)
        {
            while (_proxyAccessViewModels.Count > CountProxies)
                _proxyAccessViewModels.RemoveAt(_proxyAccessViewModels.Count - 1);
        }
        else
        {
            _proxyAccessViewModels.AddItems(CountProxies - currentCount);
            await UpdateProxyAccessAsync();
        }


        OnPropertyChanged(nameof(Access));
    }


    private async Task MakePaymentAsync()
    {
        IProxyCreditOrder proxyCreditOrder;
        IsLoadingIndicatorVisible = true;
        try
        {
            var request = new CreateBuyCreditOrderDto
            {
                Amount = SelectedCreditPlan.Amount
            };

            proxyCreditOrder = await Task.Run(
                () => _proxyCreditService.CreateOrder(request));
        }
        finally
        {
            IsLoadingIndicatorVisible = false;
        }

        Core.Util.ProUtil.GoToUrlDefault(proxyCreditOrder.Url);
    }

    private async Task UpdateBalanceAsync()
    {
        IsGettingBallance = true;

        BalanceAmount = await Task.Run(
        () => _proxyCreditService.GetCredits().Amount);

        IsGettingBallance = false;
    }

    private async Task UpdateProxyAccessAsync()
    {
        IsGettingAccess = true;

        var request = new ProxyAccessRequestDto
        {
            HostType = ProxyHostType.Hostname,
            IpType = ProxyIpType.Sticky,
            ProtocolType = ProxyProtocolType.Http,
            Count = _proxyAccessViewModels.Count,
        };

        var urls = await Task.Run(() => {
            return _proxyService
            .GetAccess(request)
            .Select(access => access.Url)
            .ToList();
        });

        for (var i = 0; i < urls.Count; ++i)
        {
            _proxyAccessViewModels[i].Url = urls[i];
        }

        IsGettingAccess = false;
    }

    private ObservableCollectionView<ProxyAccessViewModel> _access;
    public ObservableCollectionView<ProxyAccessViewModel> Access
    {
        get
        {
            if (_access == null && _proxyAccessViewModels != null)
            {
                _access = new ObservableCollectionView<ProxyAccessViewModel>(_proxyAccessViewModels);
            }

            return _access;
        }
    }

    public AsyncCollectionViewModel<IProxyCountry> Countries { get; private set; }

    public IProxyCountry Country
    {
        get => _proxyService.CurrentCountry;
        set
        {
            if (_proxyService.CurrentCountry != value)
            {
                _proxyService.CurrentCountry = value;
                OnPropertyChanged(nameof(Country));
            }
        }
    }

    private decimal _balanceAmount;
    public decimal BalanceAmount
    {
        get => _balanceAmount;
        set
        {
            if (SetProperty(ref _balanceAmount, value))
            {
                OnPropertyChanged(nameof(Balance));
            }
        }
    }
}
