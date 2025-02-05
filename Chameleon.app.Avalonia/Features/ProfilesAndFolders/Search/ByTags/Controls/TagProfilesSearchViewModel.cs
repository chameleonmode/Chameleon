using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Features.Search.ByTags.Controls;
public class TagProfilesSearchViewModel : ObservableObject {
	public string Type { get; }

	private readonly ReadOnlyObservableCollection<ObsProfile> profiles;
	public ReadOnlyObservableCollection<ObsProfile> Profiles => profiles;

	public TagProfilesSearchViewModel(TagItemDto tagItem) {

		Type = tagItem.Type;

		_ = UserProfilesRepo
		.Connect()
		.Filter(f => tagItem.Ids.Any(id => id == f.id.ToString()))
		.Transform(i => new ObsProfile(i))
		.Bind(out profiles)
		.Subscribe();
	}
}
