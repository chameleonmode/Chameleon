using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Base;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Ioc;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Common.Services;

public class TaskDialogService : ITaskDialogService
{
    //private readonly IHaveContainerProvider _containerProvider = containerProvider;
    //private readonly IHaveContainerRegistry _containerRegistry = containerRegistry;

    public async Task ShowTaskDialog(Type content, Action action)
    {
        var c = ContainerServiceHelper.Current.ContainerProvider?.Resolve(content);
        var vm = ContainerServiceHelper.Current.ContainerProvider?.Resolve<TaskDialogBase>(ContainerServiceHelper.Current.ContainerTypes[content]);
        var td = new TaskDialog
        {
            Title = "FluentAvalonia",
            ShowProgressBar = false,
            Content = c,
        };
        vm.RequestClose += (TaskDialogResul r) => 
        {
            TaskDialogStandardResult re = (TaskDialogStandardResult)r;
         
        };
        // Use the closing event to grab a deferral
        // You can also cancel closing here if you like
        td.Closing += (s, e) =>
        {
            // We only want to use the deferral on the 'Yes' Button
            if ((TaskDialogStandardResult)e.Result == TaskDialogStandardResult.Yes)
            {
                var deferral = e.GetDeferral();

                td.ShowProgressBar = true;
                int value = 0;
                DispatcherTimer timer = null;
                void Tick(object s, EventArgs e)
                {
                    td.SetProgressBarState(++value, TaskDialogProgressState.Normal);
                    if (value == 100)
                    {
                        timer.Stop();

                        // Call this when you're done. It will signal the dialog to resume closing
                        deferral.Complete();
                    }
                }
                timer = new DispatcherTimer(TimeSpan.FromMilliseconds(75), DispatcherPriority.Normal, Tick);

                timer.Start();
            }
        };

        // Don't forget to set the XamlRoot!!
        td.XamlRoot = ApplicationHelper.GetToplevetVisual();
        _ = await td.ShowAsync();
    }
}
