using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.ProxyCredit.Views;
using Chameleon.Interfaces.App.Settings;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class ProxyCreditView : SubPageViewControl, IProxyCreditView
{
    public ProxyCreditView()
    {
        InitializeComponent();
    }
}