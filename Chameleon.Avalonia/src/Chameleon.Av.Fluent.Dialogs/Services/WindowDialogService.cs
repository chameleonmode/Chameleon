
using Avalonia.Controls;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Interfaces.Views;

namespace Chameleon.Av.Fluent.Dialogs.Services;

public class WindowDialogService : IWindowDialogService
{
    public Task ShowDialogAsync(Action<object, EventArgs>[] events)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
        
    }

    public void ShowTopmost<TView, TViewModel>(Action<TViewModel> initialize) where TViewModel : class
    {
        if (ContainerServiceHelper.Resolve<TView>() is Control view)
        {
            var viewModel = view.DataContext as TViewModel;

            initialize?.Invoke(viewModel);

            AcrylicWindow window = new()
            {
                Topmost = true,
                Width = 156
            };
            window.MainPanel.Children.Add(view);
            window.Show(ApplicationHelper.GetMainWindow());
        }
    }
}
