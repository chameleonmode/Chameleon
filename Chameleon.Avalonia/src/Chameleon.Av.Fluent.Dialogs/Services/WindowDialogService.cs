
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Interfaces.Views;
using ExCSS;
using FluentAvalonia.Core;

namespace Chameleon.Av.Fluent.Dialogs.Services;

public class WindowDialogService : IWindowDialogService
{
    private Dictionary<object, AcrylicWindow> windows = [];
    public Task ShowDialogAsync(Action<object, EventArgs>[] events)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
        
    }

    public void ShowTopmost<TView, TViewModel>(Action<TViewModel> initialize, int width, Action<TViewModel>? OnClosed = null) where TViewModel : class
    {
        if (ContainerServiceHelper.Resolve<TView>() is Control view)
        {
            //if (view.Parent != null)
            //{
            //    var _originalHost = (Control)view.Parent;
            //    switch (_originalHost)
            //    {
            //        case Panel p:
            //            p.Children.Remove(view);
            //            break;
            //        case Decorator d:
            //            d.Child = null;
            //            break;
            //        case ContentControl cc:
            //            cc.Content = null;
            //            break;
            //        case ContentPresenter cp:
            //            cp.Content = null;
            //            break;
            //    }
            //}
            var viewModel = view.DataContext as TViewModel;
            initialize?.Invoke(viewModel);

            if (!windows.TryGetValue(viewModel, out AcrylicWindow w))
            {
                w = new()
                {
                    Topmost = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Width = width
                };
                windows[viewModel] = w;

                w.Closed += (s, e) =>
                {
                    w.MainPanel.Children.Remove(view);
                    windows.Remove(viewModel);
                    OnClosed?.Invoke(viewModel);
                };
                w.MainPanel.Children.Add(view);
            }

            w.Show();
        }
    }
}
