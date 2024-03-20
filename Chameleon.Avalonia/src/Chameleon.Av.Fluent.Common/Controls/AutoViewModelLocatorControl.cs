using Avalonia.Controls;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using System.Reflection;

namespace Chameleon.Av.Fluent.Common.Controls;

public class AutoViewModelLocatorControl:UserControl
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
}
