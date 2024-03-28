using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Interfaces;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class InviteUserOrAddProfilesView : AutoViewModelLocatorControl,
    IInviteUserOrAddProfilesView
{
    public InviteUserOrAddProfilesView()
    {
        InitializeComponent();
    }

    //protected override void OnLoaded(RoutedEventArgs e)
    //{
    //    base.OnLoaded(e);
    //    if (DataContext is IHaveInitialize sp)
    //        sp.InvokeInitializeAsyncCommand(e);
    //}
}