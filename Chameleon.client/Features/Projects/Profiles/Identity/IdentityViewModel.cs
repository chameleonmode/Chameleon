using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Addresses;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Businesses;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Logins;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Persons;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;
public partial class IdentityViewModel : ViewModelObjectBase {
	[ObservableProperty] bool isSaving;
	[ObservableProperty] ObsProfile? profileVM;
	[ObservableProperty] UserProfileViewModel? userProfile = new(new UserProfileDto());

	private readonly TagsRepo tagsRepo = TagsRepo.Instance;

	public PersonsViewModel PersonsVM { get; private set; }
	public BusinessesViewModel BusinessesVM { get; private set; }
	public AddressesViewModel AddressesVM { get; private set; }
	public LoginsViewModel LoginsVM { get; private set; }

	public IdentityViewModel() {

		PersonsVM = PersonsViewModel.Create(null);
		BusinessesVM = BusinessesViewModel.Create(null);
		AddressesVM = AddressesViewModel.Create(null);
		LoginsVM = LoginsViewModel.Create(null);

		AsyncCommandMap["SaveChanges"] = SaveChanges;
	}

	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
		_ = UPAdditionalDataRepo.Instance.Load();
	}

	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);

		if (param is UserProfileDto up) {
			UserProfile = new UserProfileViewModel(up);
			UserProfile.Tags = await tagsRepo.GetTagsAsync(TagItemType.Profile, UserProfile.Id.ToString())
				.ToStringAsync().RunInBackgroundWithResult();
			ProfileVM = new ObsProfile(up) { IsShowCheckboxColumn = false };

			PersonsVM = PersonsViewModel.Create(UserProfile);
			BusinessesVM = BusinessesViewModel.Create(UserProfile);
			AddressesVM = AddressesViewModel.Create(UserProfile);
			LoginsVM = LoginsViewModel.Create(UserProfile);

			PersonsVM.UpdateFilter();
			BusinessesVM.UpdateFilter();
			AddressesVM.UpdateFilter();
			LoginsVM.UpdateFilter();

			Title = ProfileVM?.Title;
		}
	}

	[RelayCommand]
	private Task Discard() {
		return UPAdditionalDataRepo.Instance.Load();
	}

	private async Task SaveChanges() {
		if (IsSaving)
			return; // Prevent multiple concurrent saves

		IsSaving = true;

		try {
			var saveAllTasks = Task.WhenAll(new[] {
				LoginsVM.SaveAll().RunInBackground(),
				PersonsVM.SaveAll().RunInBackground(),
				AddressesVM.SaveAll().RunInBackground(),
				BusinessesVM.SaveAll().RunInBackground()
			});
			
			if (UserProfile?.Validator?.IsValid == false) {
				Toaster.Info("Profile validation failed. Some changes may not be saved.");
			}

			var res = await UserProfilesRepo.Instance.Put(UserProfile!.ToDto());

			await saveAllTasks;

			if (res != null) {

				_ = tagsRepo
						.SaveTagsAsync(TagItemType.Profile, UserProfile!.Id.ToString(), UserProfile.Tags.ToTagsList())
						.RunInBackground();

				UserProfile = new UserProfileViewModel(res);

				UserProfile.Tags = await tagsRepo
						.GetTagsAsync(TagItemType.Profile, UserProfile.Id.ToString()).ToStringAsync()
						.RunInBackgroundWithResult();

				ProfileVM = new ObsProfile(UserProfile.ToDto()) { IsShowCheckboxColumn = false };

				PersonsVM = PersonsViewModel.Create(UserProfile);
				BusinessesVM = BusinessesViewModel.Create(UserProfile);
				AddressesVM = AddressesViewModel.Create(UserProfile);
				LoginsVM = LoginsViewModel.Create(UserProfile);

				PersonsVM.UpdateFilter();
				BusinessesVM.UpdateFilter();
				AddressesVM.UpdateFilter();
				LoginsVM.UpdateFilter();

				Toaster.Success($"Update was successful.");
			} else {
				Toaster.Error("Failed to update profile. Server returned null response.");
			}
		} catch (Exception ex) {
			Toaster.Error($"Unexpected error during save: {ex.Message}");
			Debug.WriteLine($"Unexpected exception during save: {ex}");
		} finally {
			ShowValidationErrors();
			IsSaving = false;
		}
	}

	void ShowValidationErrors() {
		LoginsVM.ValidateAll();
		PersonsVM.ValidateAll();
		AddressesVM.ValidateAll();
		BusinessesVM.ValidateAll();
	}

	public static IdentityViewModel Instance { get; } = IoC.GetService<IdentityViewModel>()!;
}
