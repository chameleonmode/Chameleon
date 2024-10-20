using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.app.Avalonia.ViewModels.Controllers;

namespace Chameleon.app.Avalonia.Controls;

public partial class UserProfilesUserControl : UserControl {
	public UserProfilesUserControl()
	{
		InitializeComponent();
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		_ = UserProfilesViewModel.Instance.InvokeInitializeAsyncCommand(this);
	}
}