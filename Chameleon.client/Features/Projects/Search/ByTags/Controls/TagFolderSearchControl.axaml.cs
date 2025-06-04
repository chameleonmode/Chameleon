using Avalonia.Controls;

namespace Chameleon.client.Features.Projects.Search.ByTags.Controls;

public partial class TagFolderSearchControl : UserControl {
	public TagFolderSearchControl() {
		InitializeComponent();
		var currentDataContext = this.DataContext;
	}
}