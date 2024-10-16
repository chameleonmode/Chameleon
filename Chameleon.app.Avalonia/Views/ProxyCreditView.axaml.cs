using Chameleon.app.Avalonia.app;
using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia.Views;

public partial class ProxyCreditView : ChameleonPageBase
{
    public ProxyCreditView()
    {
        InitializeComponent();
        ControlName = "Proxy Credit";
        Description = "Proxy credit settings";
        PreviewImage = AppLayers.TryGetResource<IconSource>("ProxyCred")!;
    }
}