using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace Chameleon.Av.Fluent.Common.Controls;

public class AutoViewModelLocatorControl : UserControl
{
    public AutoViewModelLocatorControl()
    {
        DataContext = AutoLocateVM() ??
            throw new NullReferenceException($"ViewModel for {GetType().Name} not found.");
    }

    /// <summary>
    /// locates a view model by firest looking for a view model attribute then fallse back to check the naming convention 
    /// retuns null if not fond
    /// </summary>
    /// <returns>ViewModel or Null</returns>
    private object? AutoLocateVM()
    {
        var viewType = GetType();

        Type? vmt =
            viewType.GetCustomAttribute<ViewModelAttribute>()?.Type ?? 
            Type.GetType($"{viewType.Namespace}.ViewModels.{viewType.Name}Model, {viewType.GetTypeInfo().Assembly.FullName}");
        

        return ContainerServiceHelper.Resolve(vmt);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is IHaveInitialize sp)
            sp.InvokeInitializeAsyncCommand(e);
    }
}

public abstract class ViewControlBase<TViewModel>
    : AutoViewModelLocatorControl
    where TViewModel : ObservableObject
{
    public TViewModel ViewModel => (TViewModel)DataContext;
}