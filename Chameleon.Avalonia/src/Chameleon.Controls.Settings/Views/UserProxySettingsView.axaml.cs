using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserSettings;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class UserProxySettingsView : SubPageViewControl, IUserProxySettingsView
{
    public UserProxySettingsView()
    {
        InitializeComponent();
        ControlName = "Proxy Settings";
        Description = "Customize your default homepages here";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("Proxy");
        //if (DataContext is UserProxySettingsViewModel vm)
        //{
        //    vm.PropertyChanged += (o, e) =>
        //    {
        //        if (e.PropertyName == nameof(vm.ViewModels))
        //        {
        //            vm.DispatcherService.InvokeOnUiThread(() => 
        //            {
        //                // Note: Attached properties not propagating correctly, workaround
        //                ProxiesDataGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

        //                this.InvalidateArrange();
        //                ProxiesDataGrid.InvalidateArrange();
        //            });
        //        }
        //    };
        //}
    }
}