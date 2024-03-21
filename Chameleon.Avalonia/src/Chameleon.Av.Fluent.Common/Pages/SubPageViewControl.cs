using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.Settings;
using System.Globalization;
using System.Reflection;

namespace Chameleon.Av.Fluent.Common.Pages;

public class SubPageViewControl : ChameleonPageBase
{
    public SubPageViewControl()
    {
        var viewType = GetType();
        var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
        var vm = $"{viewType.Namespace}.ViewModels.{viewType.Name}Model, {viewAssemblyName}";
        var viewModelType = Type.GetType(vm);

        if (viewModelType != null)
        {
            var subPageViewModel = ContainerServiceHelper.Resolve(viewModelType);
            if (!Design.IsDesignMode)
            {
                if (subPageViewModel is ISubPageViewModel sp)
                    sp.InitAsync();
            }
            DataContext = subPageViewModel;
        }
        else
        {

        }

    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

    }
}
