using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Chameleon.Interfaces.Views;
using Chameleon.Avalonia.Prism.Application.Extensions;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Runtime.CompilerServices;
using Chameleon.Interfaces.Services;
using System.Reflection.Metadata;

namespace Chameleon.Avalonia.Prism.Module.Base;

public class DialogViewModelBase : ViewModelBase
        , IDialogAware
{
    private Window? _parentWindow = null;
    protected IDialogParameters? _parameters;
    private DelegateCommand<string>? _closeCommand;

    public event Action<IDialogResult>? RequestClose;

    public Window? ParentWindow { get => _parentWindow; set => SetProperty(ref _parentWindow, value); }

    public DelegateCommand<string> CloseCommand
    {
        get
        {
            _closeCommand ??= new DelegateCommand<string>(CloseDialog);
            return _closeCommand;
        }
    }

    public virtual void CloseDialog(int obj)
    {
        if ((ButtonResult)obj is ButtonResult result)
        {
            CloseDialog(result);
        }
        else
        {
            CloseDialog(ButtonResult.None);
        }
    }

    public virtual void CloseDialog(string parameter)
    {
        if (!Enum.TryParse<ButtonResult>(parameter, true, out var result))
        {
            result = ButtonResult.None;
        }
        CloseDialog(result);
    }

    public virtual void CloseDialog(ButtonResult buttonResult)
    {
        CloseDialog(new DialogResult(buttonResult, _parameters));
    }

    public virtual void CloseDialog()
    {
        CloseDialog(ButtonResult.Cancel);
    }

    public virtual void CloseDialog(DialogResult dialogResult)
    {
        DispatcherService.InvokeOnUiThread(() => RequestClose?.Invoke(dialogResult));
        //Dispatcher.UIThread.Post(() => {  });

       // Application.Current.Dispatcher.InvokeOnUiThread(() => RequestClose?.Invoke(dialogResult));
    }

    public virtual bool CanCloseDialog()
    {
        return true;
    }

    public virtual void OnDialogClosed()
    {
    }

    public virtual void OnDialogOpened(IDialogParameters parameters)
    {
        _parameters = parameters ?? new DialogParameters();
        AddDialogParameter<IViewModel>(this);

        var title = _parameters.GetValue<string>(nameof(Title));
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }
    }

    protected void AddDialogParameter<T>(T value)
    {
        var type = typeof(T);
        var key = type.Name;
        if (_parameters != null && !_parameters.ContainsKey(key))
        {
            _parameters?.Add(key, value);
        }
    }

    protected virtual bool SetPropertyNotNullOrEmpty(ref string storage, string value, [CallerMemberName] string propertyName = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        return SetProperty(ref storage, value, propertyName);
    }
}
