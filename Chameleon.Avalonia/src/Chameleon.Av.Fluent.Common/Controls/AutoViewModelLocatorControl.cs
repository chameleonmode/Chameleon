using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using System.Reflection;

namespace Chameleon.Av.Fluent.Common.Controls;

public class AutoViewModelLocatorControl : UserControl
{
    public AutoViewModelLocatorControl()
    {
        var viewType = GetType();
        var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
        var vm = $"{viewType.Namespace}.ViewModels.{viewType.Name}Model, {viewAssemblyName}";
        var viewModelType = Type.GetType(vm);

        if (viewModelType != null)
        {
            var subPageViewModel = ContainerServiceHelper.Resolve(viewModelType);
            //if (!Design.IsDesignMode)
            //{
            //    if (subPageViewModel is IInnerUserControl sp)
            //        sp.InvokeAsyncRelayCommand(null);
            //}
            DataContext = subPageViewModel;
        }
        else
        {

        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is IHaveInitialize sp)
            sp.InvokeInitializeAsyncCommand(null);
    }
}
