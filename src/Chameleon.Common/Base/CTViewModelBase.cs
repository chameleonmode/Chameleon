using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.Common.Base;

public abstract partial class CTViewModelBase : ObservableObject
{
    [ObservableProperty]
    public string title;
}
