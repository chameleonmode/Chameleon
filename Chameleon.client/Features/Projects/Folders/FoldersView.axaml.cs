using Chameleon.client.UI.Controls;

namespace Chameleon.client.Features.Projects.Folders;

public partial class FoldersView : AutoViewModelLocatorControl {
	public FoldersView() {
		InitializeComponent();
	}

	protected override object? ViewModel => FoldersViewModel.Instance;
}