using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Controls.AssistantUsers.Interfaces;

namespace Chameleon.Avalonia.Controls.Settings.Functional;

public partial class InviteUserOrAddProfilesView : AutoViewModelLocatorControl,
		IInviteUserOrAddProfilesView {
    public InviteUserOrAddProfilesView()
    {
        InitializeComponent();
    }
}