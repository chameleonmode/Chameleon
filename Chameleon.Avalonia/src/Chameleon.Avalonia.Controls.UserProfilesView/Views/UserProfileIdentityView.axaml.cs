using Avalonia;

using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.UserProfileView;

public partial class UserProfileIdentityView : SubPageViewControl, IUserProfileIdentityView {
	public UserProfileIdentityView()
	{
		InitializeComponent();
		//Description = "Customize profile related data";
		//PreviewImage = ApplicationHelper.TryGetResource<IconSource>("ProfilePageIcon");
	}

	public override Visual? AnimateVisual { get => UPView; set => base.AnimateVisual = value; }
}