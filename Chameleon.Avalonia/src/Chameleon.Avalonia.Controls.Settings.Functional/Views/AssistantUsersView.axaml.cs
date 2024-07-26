using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Interfaces.App.UserSettings.View;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class AssistantUsersView : SubPageViewControl, IAssistantUsersView
{
    public AssistantUsersView()
    {
        InitializeComponent(); 
        ControlName = "Assistant Taskforce";
        Description = "Invite users to your taskforce and set their access to specific profiles";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("User");
    }
}