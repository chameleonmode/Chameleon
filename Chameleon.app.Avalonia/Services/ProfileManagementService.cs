using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using DynamicData;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Services;

public interface IProfileManagementService {
	ReadOnlyObservableCollection<ObsProfile> AllProfiles { get; }
}

public class ProfileManagementService : IProfileManagementService {

	private readonly ReadOnlyObservableCollection<ObsProfile> allProfiles;
	public ReadOnlyObservableCollection<ObsProfile> AllProfiles => allProfiles;

	private readonly UserProfilesRepo userProfilesRepo;
	public SourceCache<UserProfileDto, int> ProfileDtoCache => userProfilesRepo.SourceCache;

	public ProfileManagementService(UserProfilesRepo userProfilesRepo) {
		this.userProfilesRepo = userProfilesRepo;
		_ = ProfileDtoCache.Connect()
				.Transform(dto => new ObsProfile(dto, hasActionOptions: true))
				.SortAndBind(out allProfiles, Compares.ObsProfileCompares.AscendingComparer)
				.DisposeMany()
				.Subscribe();
	}

	public static ProfileManagementService Instance { get; } = new ProfileManagementService(UserProfilesRepo.Instance);
}