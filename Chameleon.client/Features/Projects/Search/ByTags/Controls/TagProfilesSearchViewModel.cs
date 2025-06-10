using Chameleon.client.Features.Projects;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.client.Services;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Projects.Search.ByTags.Controls;
public partial class TagProfilesSearchViewModel : TagsSearchViewModelBase {
	[ObservableProperty] ObsProfile? selectedProfile;

	private readonly ReadOnlyObservableCollection<ObsProfile> profiles;
	public ReadOnlyObservableCollection<ObsProfile> Profiles => profiles;

	public TagProfilesSearchViewModel(TagItemDto tagItem) : base(tagItem) {
		_ = UserProfilesRepo
				.Connect()
				.Filter(f => tagItem.Ids.Any(id => id == f.id.ToString()))
				.Transform(i => new ObsProfile(i))
				.Bind(out profiles)
				.Subscribe();

		_ = this.WhenValueChanged(x => x.SelectedProfile)
				.Where(profile => profile is not null)
				.Subscribe(profile => Navigator.NavigateToType(typeof(View), profile));
	}
}
