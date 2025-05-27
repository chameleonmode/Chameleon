using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using DynamicData;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Services;

public class ProfileManagementService {
	private readonly ReadOnlyObservableCollection<ObsProfile> allProfiles;
	public ReadOnlyObservableCollection<ObsProfile> AllProfiles => allProfiles;

	public ProfileManagementService() {
		_ = UserProfilesRepo.Connect()
				.Transform(dto => new ObsProfile(dto, hasActionOptions: true))
				.SortAndBind(out allProfiles, Compares.ObsProfileCompares.AscendingComparer)
				.DisposeMany()
				.Subscribe();
	}

	public static ProfileManagementService Instance { get; } = new ProfileManagementService();
}