using Avalonia;
using Avalonia.Controls;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.app.Avalonia.Features.Search.ByTags.Controls;

public partial class TagFolderSearchControl : UserControl {

	public static readonly StyledProperty<TagItemDto?> TagItemProperty =
					AvaloniaProperty.Register<TagFolderSearchControl, TagItemDto?>(nameof(TagItem));

	public TagItemDto? TagItem {
		get => GetValue(TagItemProperty);
		set => SetValue(TagItemProperty, value);
	}

	public TagFolderSearchControl() {
		InitializeComponent();

		_ = this.GetObservable(TagItemProperty).Subscribe(tagItem => {
			DataContext = tagItem != null ? new TagFolderSearchViewModel(tagItem) : (object?)null;
		});
	}
}