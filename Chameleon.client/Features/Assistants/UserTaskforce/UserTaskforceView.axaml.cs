using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Assistants.UserTaskforce;
public partial class UserTaskforceView : ChameleonPageBase {
	public UserTaskforceView()
	{
		InitializeComponent();
		ControlName = "Live Assistant Taskforce";
		Description = "Resource Management";
		PreviewImage = App.TryGetResource<IconSource>("User");
	}
}