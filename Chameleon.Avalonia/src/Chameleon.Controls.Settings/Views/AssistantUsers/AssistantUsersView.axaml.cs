using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.App.UserSettings.View;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class AssistantUsersView : UserControl , IAssistantUsersView
{
    public AssistantUsersView()
    {
        InitializeComponent();
    }
}