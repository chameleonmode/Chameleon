using Chameleon.App.Shared.Proxies;
using Chameleon.Avalonia.Common.Services;
using Chameleon.Avalonia.Controls.Settings.ViewModels.CreditPlan;
using Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;
using Chameleon.CT.Common.Collections;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Proxies;
using Chameleon.Interfaces.ProxyCredit;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.Input;
using ExCSS;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class ProxyCreditViewModel
      : SubPageViewModelBase
      , IProxyCreditViewModel
{
    private readonly IProxyService _proxyService;
    private readonly IProxyCreditService _proxyCreditService;
    private readonly IProxyAccessViewModels _proxyAccessViewModels;
    private readonly IToastNotificationService _toastNotificationService;
    public ProxyCreditViewModel(
        IProxyService proxyService,
        IProxyCreditService proxyCreditService,
        IProxyAccessViewModels proxyAccessViewModels,
        IToastNotificationService toastNotificationService
        )
    {
        Title = "Proxy Credit";
        CountProxies = 5;

        _proxyService = proxyService;
        _proxyCreditService = proxyCreditService;
        _proxyAccessViewModels = proxyAccessViewModels;
        _proxyAccessViewModels.AddItems(CountProxies);
        _toastNotificationService = toastNotificationService;
    }

    public override async Task InitAsync(object? param)
    {       
        await base.InitAsync(param);

        if (Loaded)
            return;

        await InitializeBalanceAsync();
        await InitializeCountriesAsync();
        InitializeCreditPlans();    

        EventAggregator
            .GetEvent<SelectedCreditPlanEvent>()
            .Subscribe(args => OnSelectedCreditPlan(args));

        OnPropertyChanged(string.Empty);
    }
   [RelayCommand]
    public async Task CopyAllUrls()
    {
        var list = Access.Select(a => a.Url);
        await ClipboardService.Instance.SetTextAsync(string.Join("\n", list));
    }

    private void InitializeCreditPlans()
    {
        CreditPlans = new CreditPlans(EventAggregator);
    }

    private async Task InitializeBalanceAsync()
    {
        BalanceAmount = await Task.Run(GetBalance);
    }

    private decimal GetBalance()
    {
        return _proxyCreditService
            .GetCredits()
            .Amount;
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
        DispatcherService.InvokeOnUiThread(() => Country = countries.FirstOrDefault());
        return countries;
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

    public string Balance => $"${_balanceAmount}";

    [RelayCommand]
    private void PurchaseCredit()
    {
        IsLoadingIndicatorVisible = true;
        DispatcherService.InvokeOnUiThreadAsync(BuyCredits, null,
            () => IsLoadingIndicatorVisible = false);
    }

    private bool _isLoadingIndicatorVisible;
    public bool IsLoadingIndicatorVisible
    {
        get => _isLoadingIndicatorVisible;
        set => SetProperty(ref _isLoadingIndicatorVisible, value);
    }

    private void OnSelectedCreditPlan(SelectedCreditPlanEventArgs args)
    {
        SelectedCreditPlan = CreditPlans.First(a => a.IsChecked);
        HasSelectedCreditPlan = true;

        OnPropertyChanged(nameof(SelectedCreditPlan));
        OnPropertyChanged(nameof(HasSelectedCreditPlan));
    }

    public CreditPlans _creditPlans;
    public CreditPlans CreditPlans
    {
        get => _creditPlans;
        set => SetProperty(ref _creditPlans, value);
    }

    private CreditPlan.CreditPlan _selectedCreditPlan;
    public CreditPlan.CreditPlan SelectedCreditPlan
    {
        get => _selectedCreditPlan;
        set => SetProperty(ref _selectedCreditPlan, value);
    }

    private bool _hasSelectedCreditPlan;
    public bool HasSelectedCreditPlan
    {
        get => _hasSelectedCreditPlan;
        set => SetProperty(ref _hasSelectedCreditPlan, value);
    }

    private void BuyCredits()
    {
        if (!HasSelectedCreditPlan)
        {
            return;
        }
        PurchaseCredits();
    }

    private void PurchaseCredits()
    {
        MakePaymentAsync();
    }

    [RelayCommand]
    private void Refresh()
    {
        UpdateBalance();
        UpdateProxyAccessAsync();
    }

    private void MakePaymentAsync()
    {
        IProxyCreditOrder proxyCreditOrder;
        IsLoadingIndicatorVisible = true;
        try
        {
            var request = new CreateBuyCreditOrderDto
            {
                Amount = SelectedCreditPlan.Amount
            };

            proxyCreditOrder = _proxyCreditService
                .CreateOrder(request);
        }
        finally
        {
            IsLoadingIndicatorVisible = false;
        }

        Core.Util.ProcessesUtil.GoToUrlDefault(proxyCreditOrder.Url);
    }

    private void UpdateBalance()
    {
        IsGettingBallance = true;

        DispatcherService.InvokeOnUiThreadAsync(UpdateBalanceAsync,
            null, () => IsGettingBallance = false);
    }

    private void UpdateBalanceAsync()
    {
        var newBalanceAmount = GetBalance();
        BalanceAmount = newBalanceAmount;
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

    private int _countProxies;
    public int CountProxies
    {
        get => _countProxies;
        set
        {
            if (SetProperty(ref _countProxies, value))
            {
                DispatcherService.InvokeOnUiThreadAsync(UpdateProxiesListCountAsync);
            }
        }
    }

    public List<int> CountsProxies { get; } = new List<int>() { 5, 10, 100, 500 };

    private void UpdateProxiesListCountAsync()
    {
        var currentCount = _proxyAccessViewModels.Count;
        _access = null;

        if (currentCount > _countProxies)
        {
            while (_proxyAccessViewModels.Count > _countProxies)
             _proxyAccessViewModels.RemoveAt(_proxyAccessViewModels.Count - 1);
        }
        else
        {
            _proxyAccessViewModels.AddItems(_countProxies - currentCount);
            UpdateProxyAccess();
        }


        OnPropertyChanged(nameof(Access));
    }

    public IProxyCountry Country
    {
        get => _proxyService.CurrentCountry;
        set
        {
            if (_proxyService.CurrentCountry != value)
            {
                _proxyService.CurrentCountry = value;
                OnPropertyChanged();
                UpdateProxyAccessAsync();
            }
        }
    }

    private bool _isGettingAccess;
    public bool IsGettingAccess
    {
        get => _isGettingAccess;
        set => SetProperty(ref _isGettingAccess, value);
    }

    private bool _isGettingBallance;
    public bool IsGettingBallance
    {
        get => _isGettingBallance;
        set => SetProperty(ref _isGettingBallance, value);
    }

    private void UpdateProxyAccessAsync()
    {
        IsGettingAccess = true;
        DispatcherService.InvokeOnUiThreadAsync(UpdateProxyAccess,
            null, () => IsGettingAccess = false);
    }

    private void UpdateProxyAccess()
    {
        var request = new ProxyAccessRequestDto
        {
            HostType = ProxyHostType.Hostname,
            IpType = ProxyIpType.Sticky,
            ProtocolType = ProxyProtocolType.Http,
            Count = _proxyAccessViewModels.Count,
        };

        var urls = _proxyService
            .GetAccess(request)
            .Select(access => access.Url)
            .ToList();

        for (var i = 0; i < urls.Count; ++i)
        {
            _proxyAccessViewModels[i].Url = urls[i];
        }
    }
}
