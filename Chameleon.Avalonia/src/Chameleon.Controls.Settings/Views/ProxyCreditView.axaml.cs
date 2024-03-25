using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.ProxyCredit.Views;
using Chameleon.Interfaces.App.Settings;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Avalonia.Controls.Settings;

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