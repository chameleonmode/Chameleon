using Chameleon.app.Avalonia.app;
using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia.Views;

public partial class AssistantUsersView : ChameleonPageBase
{
    public AssistantUsersView()
    {
        InitializeComponent(); 
        ControlName = "Assistant Taskforce";
        Description = "Invite users to your taskforce and set their access to specific profiles";
        PreviewImage = AppLayers.TryGetResource<IconSource>("User");
    }
}