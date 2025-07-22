using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Chameleon.client.Features.Projects.Profiles;

public partial class UserProfileUserControl : UserControl {
	public UserProfileUserControl() {
		InitializeComponent();

		DoubleTapped += OnPageTapped;
	}

	private void OnPageTapped(object? sender, TappedEventArgs e) {
		if (e.Source is Visual v && v.FindAncestorOfType<Button>(true) is null && DataContext is ObsProfile up) 
			up.Navigate();
	}
}