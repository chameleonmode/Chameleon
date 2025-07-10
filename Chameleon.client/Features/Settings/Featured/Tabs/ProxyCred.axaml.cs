using Chameleon.client;
using Chameleon.client.UI.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Settings.Featured;

public partial class ProxyCreditView : ChameleonPageBase
{
    public ProxyCreditView()
    {
        InitializeComponent();
        ControlName = "Proxy Credit";
        Description = "Proxy credit settings";
        PreviewImage = App.TryGetResource<IconSource>("ProxyCred");
    }
}