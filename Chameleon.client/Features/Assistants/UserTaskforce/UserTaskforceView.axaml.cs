using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Features.Assistants.UserTaskforce;
public partial class UserTaskforceView : ChameleonPageBase {
	public UserTaskforceView()
	{
		InitializeComponent();
		ControlName = "Assistant Taskforce";
		Description = "Resource Management";
		PreviewImage = App.TryGetResource<IconSource>("User");
	}
}