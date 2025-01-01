using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

using Chameleon.app.Avalonia.Models.Observable;

namespace Chameleon.app.Avalonia.Controls;

public partial class UserProfileUserControl : UserControl {
	public UserProfileUserControl()
	{
		InitializeComponent();

		DoubleTapped += OnPageTapped;
	}

	private void OnPageTapped(object? sender, TappedEventArgs e)
	{
		if (e.Source is Visual v) {
			if (v.FindAncestorOfType<Button>(true) is null
				&& DataContext is ObsProfile up 
				&& up.IsActionOptionsVisible) {
				up.Open();
			}
		}
	}
}