using Avalonia.Controls;

namespace Chameleon.app.Avalonia.Features.Search.ByTags.Controls;

public partial class TagFolderSearchControl : UserControl {
	public TagFolderSearchControl() {
		InitializeComponent();
		var currentDataContext = this.DataContext;
	}
}