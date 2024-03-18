using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Ioc;
using FluentAvalonia.UI.Controls;
using Prism.Services.Dialogs;

namespace Chameleon.Av.Fluent.Common.Services;

public class DialogManager
{
    private static readonly Dictionary<object, Visual> RegistrationMapper =
        new Dictionary<object, Visual>();

    static DialogManager()
    {
        RegisterProperty.Changed.AddClassHandler<Visual>(RegisterChanged);
    }

    private static void RegisterChanged(Visual sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is null)
        {
            throw new InvalidOperationException("The DialogManager can only be registered on a Visual");
        }

        // Unregister any old registered context
        if (e.OldValue != null)
        {
            RegistrationMapper.Remove(e.OldValue);
        }

        // Register any new context
        if (e.NewValue != null)
        {
            RegistrationMapper.Add(e.NewValue, sender);
        }
    }

    /// <summary>
    /// This property handles the registration of Views and ViewModel
    /// </summary>
    public static readonly AttachedProperty<object?> RegisterProperty = AvaloniaProperty.RegisterAttached<DialogManager, Visual, object?>(
        "Register");

    /// <summary>
    /// Accessor for Attached property <see cref="RegisterProperty"/>.
    /// </summary>
    public static void SetRegister(AvaloniaObject element, object value)
    {
        element.SetValue(RegisterProperty, value);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="RegisterProperty"/>.
    /// </summary>
    public static object? GetRegister(AvaloniaObject element)
    {
        return element.GetValue(RegisterProperty);
    }

    /// <summary>
    /// Gets the associated <see cref="Visual"/> for a given context. Returns null, if none was registered
    /// </summary>
    /// <param name="context">The context to lookup</param>
    /// <returns>The registered Visual for the context or null if none was found</returns>
    public static Visual? GetVisualForContext(object context)
    {
        return RegistrationMapper.TryGetValue(context, out var result) ? result : null;
    }

    /// <summary>
    /// Gets the parent <see cref="TopLevel"/> for the given context. Returns null, if no TopLevel was found
    /// </summary>
    /// <param name="context">The context to lookup</param>
    /// <returns>The registered TopLevel for the context or null if none was found</returns>
    public static TopLevel? GetTopLevelForContext(object context)
    {
        return TopLevel.GetTopLevel(GetVisualForContext(context));
    }
}

public class TaskDialogService : ITaskDialogService
{
    //private readonly IHaveContainerProvider _containerProvider = containerProvider;
    //private readonly IHaveContainerRegistry _containerRegistry = containerRegistry;

    public async Task ShowTaskDialog(Type content, Action action)
    {
        var c = ContainerServiceHelper.Current.ContainerProvider?.Resolve(content);
        var vm = ContainerServiceHelper.Current.ContainerProvider?.Resolve<DialogBase>(ContainerServiceHelper.Current.ContainerTypes[content].Item1);
        var td = new TaskDialog
        {
            Title = "FluentAvalonia",
            ShowProgressBar = false,
            Content = c,
        };
        //vm.RequestClose += (TaskDialogResul r) => 
        //{
        //    TaskDialogStandardResult re = (TaskDialogStandardResult)r;
         
        //};
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

    public async Task<object?> ShowTaskDialog(Type content)
    {
        //await Task.Delay(1000);
        var c = ContainerServiceHelper.Current.ContainerProvider?.Resolve<ITaskDialogView>(content);
        if(c != null)
        {
            var res = await c.ShowTDialogAsync(ContainerServiceHelper.Current.ContainerTypes[content].Item2);
            //await Task.Delay(1000);
            return res;
            //var dialog = c.FindTControl<TaskDialog>(ContainerServiceHelper.Current.ContainerTypes[content].Item2);
            //if(dialog != null)
            //{
            //    return await dialog.ShowAsync();
            //}
        }
                      
        //await Task.Delay(500);
        return null;
    }

    Task<ITaskDialogResult?> ITaskDialogService.ShowTaskDialog(Type content)
    {
        throw new NotImplementedException();
    }
}
