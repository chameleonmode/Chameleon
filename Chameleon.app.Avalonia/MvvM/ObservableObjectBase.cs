using Chameleon.lib.Common.Interfaces.Systemics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveValidation;
using System.Collections;
using System.ComponentModel;

namespace Chameleon.lib.CommunityToolkit.MvvM;

public abstract partial class ObservableObjectBase : ObservableObject,IAmaViewModel, IValidatableObject {
	[ObservableProperty]
	private string? title;

	[ObservableProperty]
	private bool loaded;

	private long _isBusy;
	public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

	public virtual Dictionary<string, Action> CommandMap { get; } = [];
	public virtual Dictionary<string, Func<Task>> AsyncCommandMap { get; } = [];

	public IAsyncRelayCommand InitializeAsyncCommand { get; }
	public TaskCompletionSource<bool> LoadedTCS { get; } = new();

	public ObservableObjectBase()
	{
		InitializeAsyncCommand = new AsyncRelayCommand<object>(
				async (p) => {
					_ = Interlocked.Increment(ref _isBusy);
					OnPropertyChanged(nameof(IsBusy));

					try {
						await InitAsync(p);
					} finally {
						_ = Interlocked.Decrement(ref _isBusy);
						OnPropertyChanged(nameof(IsBusy));
					}
					Loaded = true;
					_ = LoadedTCS.TrySetResult(false);
				},
				AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
		Validator = GetValidator(); 
	}
	public virtual Task InitAsync(object? param) => Task.CompletedTask;
	public virtual Task OnNavigatedToAsync(object? param) => Task.CompletedTask;

	public Task InvokeInitializeAsyncCommand(object? p = null) => InitializeAsyncCommand.ExecuteAsync(p);
	public Task InitializeAsync(object? param) => InvokeInitializeAsyncCommand(param);

	[RelayCommand]
	public void CfromV(string what) => CommandMap[what]?.Invoke();

	[RelayCommand]
	public async Task AsyncCfromV(string what) => await AsyncCommandMap[what]();
	public Task InitializeAsync(object? param) => InvokeInitializeAsyncCommand(param);

	private IObjectValidator? _objectValidator;
	public IObjectValidator? Validator {
		get => _objectValidator;
		set {
			_objectValidator?.Dispose();
			_objectValidator = value;
			_objectValidator?.Revalidate();
		}
	}

	public virtual void OnPropertyMessagesChanged(string propertyName) {
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}

	bool INotifyDataErrorInfo.HasErrors => Validator?.IsValid == false || Validator?.HasWarnings == true;

	public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

	public IEnumerable GetErrors(string? propertyName) {
		return Validator == null
			? Array.Empty<ValidationMessage>()
			: (IEnumerable)(string.IsNullOrEmpty(propertyName)
				? Validator.ValidationMessages
				: Validator.GetMessages(propertyName!));
	}

	protected virtual IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<ObservableObjectBase>();
		return builder.Build(this);
	}
}
