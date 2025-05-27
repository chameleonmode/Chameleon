using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Services;

public class ProfileManagementService {
		public static SortExpressionComparer<ObsProfile> AscendingComparer => SortExpressionComparer<ObsProfile>.Ascending(p => p.Dto!.title!);
		public static SortExpressionComparer<ObsProfile> DescendingComparer => SortExpressionComparer<ObsProfile>.Descending(p => p.Dto!.title!);
	
	private readonly ReadOnlyObservableCollection<ObsProfile> allProfiles;
	public ReadOnlyObservableCollection<ObsProfile> AllProfiles => allProfiles;

	public ProfileManagementService() {
		_ = UserProfilesRepo.Connect()
				.Transform(dto => new ObsProfile(dto, hasActionOptions: true))
				.SortAndBind(out allProfiles, AscendingComparer)
				.DisposeMany()
				.Subscribe();
	}

	public static ProfileManagementService Instance { get; } = new ProfileManagementService();
}