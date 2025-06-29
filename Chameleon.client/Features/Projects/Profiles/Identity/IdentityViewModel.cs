using Chameleon.client.Features.Projects.Profiles.Identity.Addresses;
using Chameleon.client.Features.Projects.Profiles.Identity.Businesses;
using Chameleon.client.Features.Projects.Profiles.Identity.Logins;
using Chameleon.client.Features.Projects.Profiles.Identity.Persons;
using Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.lib.Api.Dto;

namespace Chameleon.client.Features.Projects.Profiles.Identity;

public partial class IdentityViewModel : ViewModelObjectBase {
	[ObservableProperty] bool isSaving;
	[ObservableProperty] UserProfileIdentityVM? userProfile;
	[ObservableProperty] ObsProfile? profileVM;
	[ObservableProperty] PersonsViewModel? personsVM;
	[ObservableProperty] BusinessesViewModel? businessesVM;
	[ObservableProperty] AddressesViewModel? addressesVM;
	[ObservableProperty] LoginsViewModel? loginsVM;

	public IdentityViewModel() {
		AsyncCommandMap["SaveChanges"] = SaveChanges;
	}

	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
	}

	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);

		if (param is UserProfileDto up) {
			ProfileVM = ProfilesViewModel.Instance.Profiles.FirstOrDefault(p => p.Dto.id == up.id) ?? new (up);
			ProfileUIContextManager.ApplyContextToProfile(ProfileVM, ProfileUIContext.DialogSelection);
			UserProfile = new UserProfileIdentityVM(up) {
				Tags = await TagsRepo.Instance
				.GetTagsAsync(TagItemType.Profile, up.ID)
				.ToStringAsync()
				.RunInBackgroundWithResult()
			};
			PersonsVM = new(UserProfile);
			BusinessesVM = new(UserProfile);
			AddressesVM = new(UserProfile);
			LoginsVM = new(UserProfile);

			PersonsVM.UpdateFilter();
			BusinessesVM.UpdateFilter();
			AddressesVM.UpdateFilter();
			LoginsVM.UpdateFilter();

			Title = ProfileVM.Title;
			ShowHeaderRegion = false;
		}
	}

	private async Task SaveChanges() {
		if (IsSaving) return; // Prevent multiple concurrent saves
		try {
			IsSaving = true;
			var saveAllTasks = Task.WhenAll([
				LoginsVM!.SaveAll().RunInBackground(),
				PersonsVM!.SaveAll().RunInBackground(),
				AddressesVM!.SaveAll().RunInBackground(),
				BusinessesVM!.SaveAll().RunInBackground()
			]);

			if (UserProfile!.Validator?.IsValid == false) {
				Toaster.Info("Profile validation failed. Some changes may not be saved.");
			}

			var res = await UserProfilesRepo.Instance.Put(UserProfile!.ToDto());

			await saveAllTasks;

			if (res != null) {
				_ = TagsRepo.Instance
				.SaveTagsAsync(TagItemType.Profile, UserProfile.Id.ToString(), UserProfile.Tags.ToTagsList())
				.RunInBackground();

				UserProfile.Tags = await TagsRepo.Instance
				.GetTagsAsync(TagItemType.Profile, UserProfile.Id.ToString()).ToStringAsync()
				.RunInBackgroundWithResult();

				PersonsVM.UpdateFilter();
				BusinessesVM.UpdateFilter();
				AddressesVM.UpdateFilter();
				LoginsVM.UpdateFilter();

				Toaster.Success($"Update was successful.");
			} else {
				Toaster.Error("Failed to update profile. Server returned null response.");
			}
		} finally {
			ShowValidationErrors();
			IsSaving = false;
		}
	}

	void ShowValidationErrors() {
		LoginsVM?.ValidateAll();
		PersonsVM?.ValidateAll();
		AddressesVM?.ValidateAll();
		BusinessesVM?.ValidateAll();
	}
}
