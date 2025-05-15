using Chameleon.lib.Common.Interfaces.Systemics;
using Chameleon.lib.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveValidation;
using System.Collections;
using System.ComponentModel;
namespace Chameleon.lib.CommunityToolkit.MvvM;

public abstract partial class ObservableObjectBase : ObservableObject, IAmaViewModel, IValidatableObject {
	[ObservableProperty]
	private string? title;

	[ObservableProperty]
	private string? tags;

	[ObservableProperty]
	private bool loaded;

	private long _isBusy;
	public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

	public virtual Dictionary<string, Action> CommandMap { get; } = [];
	public virtual Dictionary<string, Func<Task>> AsyncCommandMap { get; } = [];

	public IAsyncRelayCommand InitializeAsyncCommand { get; }
	public TaskCompletionSource<bool> LoadedTCS { get; } = new();

	public ObservableObjectBase() {
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
	public virtual Task OnNavigatedToAsync(object? param) => LoadedTCS.Task;
	public Task InvokeInitializeAsyncCommand(object? p = null) => InitializeAsyncCommand.ExecuteAsync(p);
	public Task InitializeAsync(object? param) => InvokeInitializeAsyncCommand(param);

	[RelayCommand]
	public void CfV(string what) {
		try {
			CommandMap[what]();
		} catch (Exception e) {
			Toaster.Error(what, e.Message);
		}
	}

	[RelayCommand]
	public async Task AsyncCfV(string what){
		try {
			await AsyncCommandMap[what]();
		} catch (Exception e) {
			Toaster.Error(what, e.Message);
		}
	}

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
		var error = Validator == null
			? Array.Empty<ValidationMessage>()
			: (IEnumerable)(string.IsNullOrEmpty(propertyName)
				? Validator.ValidationMessages
				: Validator.GetMessages(propertyName!));
		return error;
	}

	protected virtual IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<ObservableObjectBase>();
		return builder.Build(this);
	}

	public bool IsValidationValid(){
		var validationMessages = Validator?.ValidationMessages ?? [];
		if (!validationMessages.Any()) return Validator?.IsValid ?? true;

		var publicPropertyInfos = this.GetType().GetProperties();
		foreach (var propertyInfo in publicPropertyInfos) {
			var propertyName = propertyInfo.Name;
			var proeprtyValidationMessages = Validator?.GetMessages(propertyName) ?? [];
			foreach (var propertyValidation in proeprtyValidationMessages) {
				if (validationMessages.Any(x => x.Message == propertyValidation.Message)) {
					ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
				}
			}
		}
		return false;
	}
}
