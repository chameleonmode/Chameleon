using Avalonia.Controls.Primitives;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.MainWindow;
using Chameleon.Interfaces.Views;
using Chameleon.Prism.Events;
using Prism.Services.Dialogs;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public class DialogWindowsService : IDialogWindowsService
{
    private readonly IPopupDialogService _dialogManager;
    private readonly IEventAggregator _eventAggregator;
    public DialogWindowsService(
        IPopupDialogService dialogManager
        , IEventAggregator eventAggregator)
    {
        _dialogManager = dialogManager;
        _eventAggregator = eventAggregator;
    }

    public ButtonResult ShowDialogWindow<TViewModel>(IViewControl viewControl, string title, Action<TViewModel?>? initialize)
        where TViewModel : class
    {
        var dialog = _dialogManager.Create(typeof(IDialogWindowView));
        var dialogView = (IDialogWindowView)dialog.Content;
        dialogView.InnerContent = viewControl;

        var frameworkElement = viewControl as TemplatedControl;
        if (frameworkElement != null)
        {
            var viewModel = frameworkElement.DataContext as TViewModel;
            if (initialize != null)
            {
                initialize(viewModel);
            }
        }

        dialogView.Title = title;

        SetBlackout(true);
        dialog.ShowDialog();
        SetBlackout(false);

        return (ButtonResult)dialog.Result;
    }

    public ButtonResult ShowDialogWindow(IViewControl viewControl, string title)
    {
        return ShowDialogWindow<object>(viewControl, title, null);
    }

    private void SetBlackout(bool args)
    {
        _eventAggregator
            .GetEvent<MainWindowBlackoutEvent>()
            .Publish(new MainWindowBlackoutEventArgs(args));
    }

    int IDialogWindowsService.ShowDialogWindow(IViewControl viewControl, string title)
    {
        return (int)ShowDialogWindow(viewControl, title);
    }

    int IDialogWindowsService.ShowDialogWindow<TViewModel>(IViewControl viewControl, string title, Action<TViewModel> initialize)
    {
        return (int)ShowDialogWindow<TViewModel>(viewControl, title, null);
    }
}
