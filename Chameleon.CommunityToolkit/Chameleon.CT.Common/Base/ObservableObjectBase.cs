using Chameleon.Common.Icons;
using Chameleon.Interfaces;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.CT.Common.Base;

public abstract partial class ObservableObjectBase : ObservableObject,
    IPageViewModel
{
    private readonly IDispatcherService _dispatcherService;
    private readonly IEventAggregator eventAggregator;
    private readonly IContentDialogService _cntentDialogService;
    private readonly IClipboardService _IClipboardService;

    private long _isBusy; 
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

    [ObservableProperty]
    private bool _loaded;
    [ObservableProperty]
    public string title = "ObservableObjectBase";
    [ObservableProperty]
    public string glyph = FontIcons.FontIconsInfos[0].Glyph;

    public TaskCompletionSource LoadedTCS { get; } = new();

    public virtual Dictionary<string, Action> CommandMap { get; } = [];
    public virtual Dictionary<string, Func<Task>> AsyncCommandMap { get; } = [];

    public ObservableObjectBase()
    {
        _dispatcherService = ContainerServiceHelper.Resolve<IDispatcherService>();// ?? new DispatcherService();
        _cntentDialogService = ContainerServiceHelper.Resolve<IContentDialogService>();
        eventAggregator = ContainerServiceHelper.Resolve<IEventAggregator>() ?? new EventAggregator();
       _IClipboardService = ContainerServiceHelper.Resolve<IClipboardService>();


    InitializeAsyncCommand = new AsyncRelayCommand<object>(
        async (p) =>
        {
            await IsBusyFor(()=>InitAsync(p));
            Loaded = true;
            LoadedTCS.TrySetResult();
        },
        AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    public IDispatcherService DispatcherService => _dispatcherService;
    public IContentDialogService ContentDialogService => _cntentDialogService;
    public IEventAggregator EventAggregator => eventAggregator;
    public IClipboardService ClipboardService => _IClipboardService;
    public IAsyncRelayCommand InitializeAsyncCommand { get; }

    public virtual Task InitAsync(object? param)
    {
        return Task.CompletedTask;
    }

    public async Task IsBusyFor(Func<Task> unitOfWork)
    {
        Interlocked.Increment(ref _isBusy);
        OnPropertyChanged(nameof(IsBusy));

        try
        {
            await unitOfWork();
        }
        finally
        {
            Interlocked.Decrement(ref _isBusy);
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    public Task InvokeInitializeAsyncCommand(object p = null) 
    {
        return InitializeAsyncCommand.ExecuteAsync(p);
    }

    public virtual Task OnNavigatedToAsync(object? param)
    {
        // await InvokeInitializeAsyncCommand(param);
        return Task.CompletedTask;
    }

    [RelayCommand]
    public void CfromV(string what)
    {
        CommandMap[what]?.Invoke();
    }

    [RelayCommand]
    public async Task AsyncCfromV(string what)
    {
        var cmdt = AsyncCommandMap[what];
        if(cmdt != null)
            await cmdt();
    }

    [RelayCommand]
    private async Task Copy(object param)
    {
        await ClipboardService.SetTextAsync(param as string);
    }
}
