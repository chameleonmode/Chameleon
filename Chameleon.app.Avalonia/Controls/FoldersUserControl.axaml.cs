using Avalonia.Controls;
using Avalonia.Interactivity;

using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.app.Avalonia.Controls;

public partial class FoldersUserControl : AutoViewModelInitControl {
    public FoldersUserControl()
    {
        InitializeComponent();
    }

	//protected override void OnLoaded(RoutedEventArgs e)
	//{
	//	_ = UserProfileFoldersViewModel.Instance.InvokeInitializeAsyncCommand(this);
	//}
}