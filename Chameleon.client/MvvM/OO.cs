using Chameleon.lib.Helpers;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveValidation;
using System.Collections;
using System.ComponentModel;

namespace Chameleon.client.MvvM;

public interface IInitializer {
	TaskCompletionSource<bool> LoadedTCS { get; }
	Task Initialize(object? param = null);
}
public abstract partial class OO : ObservableObject, IInitializer, IValidatableObject {
	[ObservableProperty] string? title;
	[ObservableProperty] string? tags;
	[ObservableProperty] bool loaded;

	private long isBusy;
	public bool IsBusy => Interlocked.Read(ref isBusy) > 0;

	private IObjectValidator? validator;
	public IObjectValidator? Validator {
		get => validator;
		set {
			validator?.Dispose();
			validator = value;
			validator?.Revalidate();
		}
	}
	bool INotifyDataErrorInfo.HasErrors => Validator?.IsValid == false || Validator?.HasWarnings == true;

	public virtual Dictionary<string, Action> CommandMap { get; } = [];
	public virtual Dictionary<string, Func<Task>> AsyncCommandMap { get; } = [];

	public IAsyncRelayCommand InitializeAsyncCommand { get; }
	public TaskCompletionSource<bool> LoadedTCS { get; } = new();

	public OO() {
		Validator = GetValidator();
		InitializeAsyncCommand = new AsyncRelayCommand<object>(
		async (p) => {
			_ = Interlocked.Increment(ref isBusy);
			OnPropertyChanged(nameof(IsBusy));
			try {
				await Init(p);
			} finally {
				_ = Interlocked.Decrement(ref isBusy);
				OnPropertyChanged(nameof(IsBusy));
			}
			Loaded = true;
			_ = LoadedTCS.TrySetResult(false);
		},
		AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
	}
	public virtual Task Init(object? param) => Task.CompletedTask;
	public virtual Task OnNavigatedTo(object? param) => LoadedTCS.Task;
	public virtual Task OnNavigatingFrom(object param) => Task.CompletedTask;
  
	public Task InvokeInitialize(object? p = null) => InitializeAsyncCommand.ExecuteAsync(p);
	public Task Initialize(object? param) => InvokeInitialize(param);

	[RelayCommand]
	public void CfV(string what) {
		EX.Try(() => CommandMap[what](), caught: e => Toaster.Error(what, e.Message));
	}

	[RelayCommand]
	public async Task AsyncCfV(object what) {
		var cmd = what.ToString();
		await EX.Try(async () => await AsyncCommandMap[cmd!](), caught: e => Toaster.Error(cmd ?? "", e.Message));
	}

	public virtual void OnPropertyMessagesChanged(string propertyName) {
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}

	public IEnumerable GetErrors(string? propertyName) {
		var error = Validator == null
			? Array.Empty<ValidationMessage>()
			: (IEnumerable)(string.IsNullOrEmpty(propertyName)
				? Validator.ValidationMessages
				: Validator.GetMessages(propertyName!));
		return error;
	}

	protected virtual IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<OO>();
		return builder.Build(this);
	}

	public bool IsValidationValid() {
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
	public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
}
