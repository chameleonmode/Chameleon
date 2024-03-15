using Chameleon.Interfaces.Dialogs;
using Prism.Services.Dialogs;

using IDialogService = Prism.Services.Dialogs.IDialogService;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia;
using Chameleon.Core.Extensions;
using Avalonia.Controls.ApplicationLifetimes;
using Prism.Ioc;
using Chameleon.Avalonia.Prism.Interfaces.Dialogs;
using System.Xml.Linq;
using Tmds.DBus.Protocol;
using DryIoc;
using Prism.Common;
using Avalonia.Controls.Primitives;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public static class TheseWindowExtension
{
    /// <summary>
    /// Get the <see cref="IDialogAware"/> ViewModel from a <see cref="IDialogWindow"/>.
    /// </summary>
    /// <param name="dialogWindow"><see cref="IDialogWindow"/> to get ViewModel from.</param>
    /// <returns>ViewModel as a <see cref="IDialogAware"/>.</returns>
    public static IDialogAware GetDialogViewModel(this IDialogWindow dialogWindow)
    {
        return (IDialogAware)dialogWindow.DataContext;
    }
}
public class PopupDialogManagerService : IPopupDialogWinowService
{
    private readonly IContainerExtension _containerExtension;
    private readonly IDialogService _dialogService;
    public PopupDialogManagerService(IDialogService dialogService, 
        IContainerExtension containerExtension)
    {
        _dialogService = dialogService;
        _containerExtension = containerExtension;
    }


    public Task<IPopupDialogResult?> Create<T>() where T : INotifyPropertyChanged
    {
        throw new NotImplementedException();
    }

    public void Close(object? result = null)
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync(object? result = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void ShowDialogInWindow<TDialog,TWindow>(string message, Action<int?> result)
    {
        // PRO TIP: Use `nameof(DialogView)` instead of "DialogView" to catch errors early on
        _dialogService.ShowDialog(Common.Helpers.ApplicationHelper.GetMainWindow(),
            typeof(TDialog).Name, 
            new DialogParameters($"message={message}"),
            (r) => { result((int?)r?.Result); },
            typeof(TWindow).Name);
    }

    public void ShowDialog(string wname, string message, Action<int?> result)
    {
        // PRO TIP: Use `nameof(DialogView)` instead of "DialogView" to catch errors early on
        ShowDialog(wname, new DialogParameters($"message={message}"), (r) => { result((int?)r?.Result); });
    }

    public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult?> callback)
    {
        _dialogService.ShowDialog(
        name,
         parameters,
             r =>
             {
                 if (r is null)
                 {
                     callback(null);
                 }
                 else
                 {

                     callback(r);
                 }
             });
    }
    public void ShowDialog(Window owner, string name, IDialogParameters parameters, Action<IDialogResult> callback)
    {
        _dialogService.ShowDialog(
            owner,
         name,
          parameters,
              r =>
              {
                  if (r is null)
                  {
                      callback(null);
                  }
                  else
                  {

                      callback(r);
                  }
              });
    }

    public IDialog Create(Type dialogType)
    {
        IDialogWindow dialogWindow = CreateDialogWindow(null);
        ConfigureDialogWindowEvents(dialogWindow, null);
        ConfigureDialogWindowContent(dialogType, dialogWindow, null);
        return new Chameleon.Infrastructure.Dialogs.DialogWindow(dialogWindow);
    }

    /// <summary>
    /// Create a new <see cref="IDialogWindow"/>.
    /// </summary>
    /// <param name="name">The name of the hosting window registered with the IContainerRegistry.</param>
    /// <returns>The created <see cref="IDialogWindow"/>.</returns>
    protected virtual IDialogWindow CreateDialogWindow(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return _containerExtension.Resolve<IDialogWindow>();
        else
            return _containerExtension.Resolve<IDialogWindow>(name);
    }


    /// <summary>
    /// Configure <see cref="IDialogWindow"/> and <see cref="IDialogAware"/> events.
    /// </summary>
    /// <param name="dialogWindow">The hosting window.</param>
    /// <param name="callback">The action to perform when the dialog is closed.</param>
    protected virtual void ConfigureDialogWindowEvents(IDialogWindow dialogWindow, Action<IDialogResult>? callback)
    {
        Action<IDialogResult> requestCloseHandler = null;
        requestCloseHandler = (o) =>
        {
            dialogWindow.Result = o;
            dialogWindow.Close();
        };

        EventHandler? loadedHandler = null;
        loadedHandler = (o, e) =>
        {
            dialogWindow.Opened -= loadedHandler;
            dialogWindow.GetDialogViewModel().RequestClose += requestCloseHandler;
        };
        dialogWindow.Opened += loadedHandler;

        EventHandler<WindowClosingEventArgs>? closingHandler = null;
        closingHandler = (o, e) =>
        {
            if (!dialogWindow.GetDialogViewModel().CanCloseDialog())
                e.Cancel = true;
        };
        dialogWindow.Closing += closingHandler;

        EventHandler? closedHandler = null;
        closedHandler = (o, e) =>
        {
            dialogWindow.Closed -= closedHandler;
            dialogWindow.Closing -= closingHandler;
            dialogWindow.GetDialogViewModel().RequestClose -= requestCloseHandler;

            dialogWindow.GetDialogViewModel().OnDialogClosed();

            if (dialogWindow.Result == null)
                dialogWindow.Result = new DialogResult();

            callback?.Invoke(dialogWindow.Result);

            dialogWindow.DataContext = null;
            dialogWindow.Content = null;
        };
        dialogWindow.Closed += closedHandler;
    }

    /// <summary>
    /// Configure <see cref="IDialogWindow"/> content.
    /// </summary>
    /// <param name="dialogName">The name of the dialog to show.</param>
    /// <param name="window">The hosting window.</param>
    /// <param name="parameters">The parameters to pass to the dialog.</param>
    protected virtual void ConfigureDialogWindowContent(Type dialogName, IDialogWindow window, IDialogParameters? parameters)
    {
        var content = _containerExtension.Resolve(dialogName);
        var dialogContent = content as TemplatedControl;
        if (dialogContent == null)
            throw new NullReferenceException("A dialog's content must be a FrameworkElement");

        var viewModel = dialogContent.DataContext as IDialogAware;
        if (viewModel == null)
            throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");

        ConfigureDialogWindowProperties(window, dialogContent, viewModel);

        MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
    }

    /// <summary>
    /// Configure <see cref="IDialogWindow"/> properties.
    /// </summary>
    /// <param name="window">The hosting window.</param>
    /// <param name="dialogContent">The dialog to show.</param>
    /// <param name="viewModel">The dialog's ViewModel.</param>
    protected virtual void ConfigureDialogWindowProperties(IDialogWindow window, TemplatedControl dialogContent, IDialogAware viewModel)
    {
        var windowStyle = Dialog.GetWindowStyle(dialogContent);
        if (windowStyle != null)
        {
        }
        else
        {

        }

        window.Content = dialogContent;
        window.DataContext = viewModel; //we want the host window and the dialog to share the same data context

        if (window.Owner == null)
        {
            //var tl = TopLevel.GetTopLevel(dialogContent);
        }

    }
}
