using Avalonia.Controls;
using Avalonia.Interactivity;

using Chameleon.app.Avalonia.ViewModels.Controllers;

namespace Chameleon.app.Avalonia.Controls;

public partial class FoldersUserControl : UserControl
{
    public FoldersUserControl()
    {
        InitializeComponent();
    }

	protected override void OnLoaded(RoutedEventArgs e)
	{
		_ = UserProfileFoldersViewModel.Instance.InvokeInitializeAsyncCommand(this);
	}
}