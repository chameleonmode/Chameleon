using Avalonia;
using Avalonia.Controls;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Search.ByTags;
using Chameleon.app.Avalonia.Models;
using Chameleon.lib.Common.Models.Dto;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Features.Search.ByTags;

public partial class View : UserControl {

	private readonly ViewModel viewModel = new();

	public ViewModel ViewModel => viewModel;

	public static readonly StyledProperty<MainAppSearchItem?> HashTagProperty =
					AvaloniaProperty.Register<View, MainAppSearchItem?>(nameof(HashTag));

	public MainAppSearchItem? HashTag {
		get => GetValue(HashTagProperty);
		set => SetValue(HashTagProperty, value);
	}


	public View() {
		InitializeComponent();
		_ = this.GetObservable(HashTagProperty).Subscribe(searchTerm => {
			viewModel.Items.Clear();
			if(searchTerm is not null) {
				var tagDto = (TagDto)searchTerm.ViewModel!;
				var items = tagDto.Items.Select(x => new TagItemDto(x.Key, x.Value));
				viewModel.Items = new ObservableCollection<TagItemDto>(items);
			}
		});
	}
}