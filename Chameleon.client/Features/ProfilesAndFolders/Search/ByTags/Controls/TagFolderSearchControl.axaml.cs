using Avalonia.Controls;

namespace Chameleon.client.Features.Search.ByTags.Controls;

public partial class TagFolderSearchControl : UserControl {
	public TagFolderSearchControl() {
		InitializeComponent();
		var currentDataContext = this.DataContext;
	}
}