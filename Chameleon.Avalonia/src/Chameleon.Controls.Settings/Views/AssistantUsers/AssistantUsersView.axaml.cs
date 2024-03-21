using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Interfaces.App.UserSettings.View;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class AssistantUsersView : SubPageViewControl, IAssistantUsersView
{
    public AssistantUsersView()
    {
        InitializeComponent();
    }
}