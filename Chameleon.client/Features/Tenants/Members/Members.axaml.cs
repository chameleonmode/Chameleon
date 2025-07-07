using Chameleon.client.UI.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Tenants.Members;

public partial class TenantMembersView : ChameleonPageBase {
	public TenantMembersView() {
		InitializeComponent();
		ControlName = "Live Assistant Taskforce";
		Description = "Resource Management";
		PreviewImage = App.TryGetResource<IconSource>("User");
	}
}