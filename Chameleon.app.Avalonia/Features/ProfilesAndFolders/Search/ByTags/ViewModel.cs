using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Search.ByTags;
public partial class ViewModel : ObservableObject {

	[ObservableProperty]
	private ObservableCollection<TagItemDto> items = new();


}
