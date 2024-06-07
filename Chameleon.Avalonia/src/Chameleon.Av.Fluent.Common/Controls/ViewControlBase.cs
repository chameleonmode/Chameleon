using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces;
using System.Reflection;

namespace Chameleon.Avalonia.Fluent.Common.Controls;

public abstract class ViewControlBase
     : UserControl
{
    public ViewControlBase()
    {
        var viewModelAttribute = GetType().GetCustomAttribute<ViewModelAttribute>();
        if (viewModelAttribute == null)
        {
            throw new ArgumentNullException();
        }

        var viemModel = ContainerServiceHelper.Resolve(viewModelAttribute.Type);
        DataContext = viemModel;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is IHaveInitialize haveInitialize)
        {
            haveInitialize.InvokeInitializeAsyncCommand(e);
        }
    }
}

public abstract class ViewControlBase<TViewModel>
    : ViewControlBase
    where TViewModel : ObservableObjectBase
{
    public TViewModel ViewModel => (TViewModel)DataContext;
}
