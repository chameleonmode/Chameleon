using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using DynamicData;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Features.Search.ByTags.Controls;
public class TagProfilesSearchViewModel : TagsSearchViewModelBase {

	private readonly ReadOnlyObservableCollection<ObsProfile> profiles;
	public ReadOnlyObservableCollection<ObsProfile> Profiles => profiles;

	public TagProfilesSearchViewModel(TagItemDto tagItem) : base(tagItem) {

		_ = UserProfilesRepo
				.Connect()
				.Filter(f => tagItem.Ids.Any(id => id == f.id.ToString()))
				.Transform(i => new ObsProfile(i))
				.Bind(out profiles)
				.Subscribe();
	}
}
