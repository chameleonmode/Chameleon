
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Interfaces.Views;
using ExCSS;
using FluentAvalonia.Core;

namespace Chameleon.Av.Fluent.Dialogs.Services;

public class WindowDialogService : IWindowDialogService
{
    private readonly Dictionary<object, AcrylicWindow> windows = [];
    public Task ShowDialogAsync(Action<object, EventArgs>[] events)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
        
    }
    private void PrivateShow<TViewModel>(TViewModel viewModel, Action<TViewModel> initialize, Control view, int width, string title, Action<TViewModel>? onClosed)
    {
        initialize(viewModel);

        if (!windows.TryGetValue(viewModel, out AcrylicWindow w))
        {
            w = new()
            {
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = width,
                Title = title,
            };
            windows[viewModel] = w;

            w.Closed += (s, e) =>
            {
                w.MainPanel.Children.Remove(view);
                windows.Remove(viewModel);
                onClosed?.Invoke(viewModel);
            };
            w.MainPanel.Children.Add(view);
        }

        w.Show();
    }
    public void ShowTopmost<TView, TViewModel>(TViewModel vm, Action<TViewModel> initialize,
        Action<TViewModel>? onClosed = null, string title = "Copy Pasta", int width = 256) where TViewModel : class
    {
        if (ContainerServiceHelper.Resolve<TView>() is Control view)
        {
            view.DataContext = vm;
            PrivateShow(vm, initialize, view, width, title, onClosed);
        }
    }

    public void ShowTopmost<TView, TViewModel>(Action<TViewModel> initialize,
        Action<TViewModel>? onClosed = null, string title = "Copy Pasta", int width = 256) where TViewModel : class
    {
        if (ContainerServiceHelper.Resolve<TView>() is Control view)
        {
            var vm = view.DataContext as TViewModel;
            PrivateShow(vm, initialize, view, width, title, onClosed);
        }
    }
}
