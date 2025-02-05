using Avalonia;
using Avalonia.Controls;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.app.Avalonia.Features.Search.ByTags.Controls;

public partial class TagProfilesSearchControl : UserControl
{
	public static readonly StyledProperty<TagItemDto?> TagItemProperty =
			AvaloniaProperty.Register<TagProfilesSearchControl, TagItemDto?>(nameof(TagItem));

	public TagItemDto? TagItem {
		get => GetValue(TagItemProperty);
		set => SetValue(TagItemProperty, value);
	}

	public TagProfilesSearchControl() {
		InitializeComponent();

		_ = this.GetObservable(TagItemProperty).Subscribe(tagItem => {
			DataContext = tagItem != null ? new TagProfilesSearchViewModel(tagItem) : (object?)null;
		});
	}
}