namespace Chameleon.Avalonia.Controls.Settings.Functional.Views;

public partial class ProxyCreditView : SubPageViewControl, IProxyCreditView
{
    public ProxyCreditView()
    {
        InitializeComponent();
        ControlName = "Proxy Credit";
        Description = "Proxy credit settings";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("ProxyCred");
    }
}