using Chameleon.app.Avalonia.Extensions;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity;
public partial class IdentityViewModel : ViewModelObjectBase {
	private readonly BehaviorSubject<Func<UP, bool>> filter;
	private readonly BehaviorSubject<Func<ObsAddressViewModel, bool>> adrezfilter;

	[ObservableProperty]
	private bool isSaving;
	[ObservableProperty]
	private ObsProfile? profileVM;
	[ObservableProperty]
	private UserProfileViewModel? userProfile = new(new UserProfileDto());

	private readonly ReadOnlyObservableCollection<ObsAddressViewModel> addresses;
	private readonly ReadOnlyObservableCollection<UPBusinessViewModel> businesses;
	private readonly ReadOnlyObservableCollection<UPLoginViewModel> logins;
	private readonly ReadOnlyObservableCollection<UPPersonViewModel> persons;
	private readonly TagsRepo tagsRepo = TagsRepo.Instance;

	public ReadOnlyObservableCollection<ObsAddressViewModel> Addresses => addresses;
	public bool HasAddresses => Addresses?.Count > 0;
	public ReadOnlyObservableCollection<UPBusinessViewModel> Businesses => businesses;
	public bool HasBusiness => Businesses?.Count > 0;
	public ReadOnlyObservableCollection<UPLoginViewModel> Logins => logins;
	public bool HasLogins => Logins?.Count > 0;
	public ReadOnlyObservableCollection<UPPersonViewModel> Persons => persons;
	public bool HasPersons => Persons?.Count > 0;

	public Func<UP, bool> FilterPredicate => p => p.ProfileId == UserProfile?.Id;
	public Func<ObsAddressViewModel, bool> AdrezFilterPredicate => p => p.Dto?.ProfileId == UserProfile?.Id;

	public IdentityViewModel() {
		filter = new BehaviorSubject<Func<UP, bool>>(FilterPredicate);
		adrezfilter = new BehaviorSubject<Func<ObsAddressViewModel, bool>>(AdrezFilterPredicate);

		_ = UPAdditionalDataRepo.Instance.Personz
			.Connect()
			.Filter(filter)
			.Transform(x => new UPPersonViewModel(x))
			.Bind(out persons)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasPersons));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Loginz
			.Connect()
			.Filter(filter)
			.Transform(x => new UPLoginViewModel(x))
			.Bind(out logins)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasLogins));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Biz
			.Connect()
			.Filter(filter)
			.Transform(x => new UPBusinessViewModel(x))
			.Bind(out businesses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasBusiness));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Addrez
			.Connect()
			.Transform(a => new ObsAddressViewModel(a))
			.Filter(adrezfilter)
			.Bind(out addresses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasAddresses));
			});

		AsyncCommandMap["AddPerson"] = AddPerson;
		AsyncCommandMap["AddBusiness"] = OnAddBusiness;
		AsyncCommandMap["AddAddress"] = OnAddAddress;
		AsyncCommandMap["AddLogin"] = OnAddLogin;
		AsyncCommandMap["SaveChanges"] = SaveChanges;
	}
	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
		await UPAdditionalDataRepo.Instance
			.LoadReload()
			.RunInBackground();
	}
	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);

		if (param is UserProfileDto up) {
			UserProfile = new UserProfileViewModel(up);
			UserProfile.Tags = await tagsRepo.GetTagsAsync(TagItemType.Profile, UserProfile.Id.ToString())
				.ToStringAsync().RunInBackgroundWithResult();
			ProfileVM = new ObsProfile(up, false);
			filter.OnNext(FilterPredicate);
			adrezfilter.OnNext(AdrezFilterPredicate);
			Title = ProfileVM?.Title;
		}
	}

	[RelayCommand]
	private async Task Discard() {
		await UPAdditionalDataRepo.Instance
			.LoadReload(true).RunInBackground();
	}

	private async Task SaveChanges() {
		IsSaving = true;

		try {
			Logins.ForEach(async l => await OnSaveLogin(l));
			//
			Persons.ForEach(async l => await OnSavePerson(l));
			//
			Addresses.ForEach(async l => await OnSaveAddress(l));
			//
			Businesses.ForEach(async l => await OnSaveBusiness(l));

			if (UserProfile?.Validator?.IsValid == false) {
				//return;
			}

			var res = await UserProfilesRepo.Instance.Put(UserProfile!.ToDto());
			if (res != null) {

				await tagsRepo
					.SaveTagsAsync(TagItemType.Profile, UserProfile!.Id.ToString(), UserProfile.Tags.ToTagsList())
					.RunInBackground();

				UserProfile = new UserProfileViewModel(res);
				UserProfile.Tags = await tagsRepo
					.GetTagsAsync(TagItemType.Profile, UserProfile.Id.ToString()).ToStringAsync()
					.RunInBackgroundWithResult();
				ProfileVM = new ObsProfile(UserProfile.ToDto(), false);
				Toaster.Success($"Update was successful.");
			}
		} catch (Exception ex) {
			// Handle the exception (e.g., log it, show a notification, etc.)
			Toaster.Error($"{ex.Message}");
		} finally {
			ShowValidationErrors();
			// Code to execute after the task completes, regardless of success or failure
			IsSaving = false;
		}
	}

	void ShowValidationErrors() {
		Logins.ForEach(l => l.IsValidationValid());
		Persons.ForEach(l => l.IsValidationValid());
		Addresses.ForEach(l => l.IsValidationValid());
		Businesses.ForEach(l => l.IsValidationValid());
	}

	#region Persons

	[RelayCommand]
	private async Task AddPerson() {
		if (persons.Any(x => x.Id == 0)) {
			return;
		}

		_ = await UPAdditionalDataRepo.Instance.Personz.Initialize(new UPPersonDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasPersons));
	}

	[RelayCommand]
	private async Task OnSavePerson(UPPersonViewModel p) {
		_ = p.IsValidationValid();
		await UPAdditionalDataRepo
			.Save(UPAdditionalDataRepo.Instance.Personz, p.ToDto())
			.RunInBackground();
		if (p.Id == 0)
			_ = await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Personz, p.ToDto());
	}

	[RelayCommand]
	private async Task DeletePerson(UPPersonViewModel p) {
		_ = p.Id == 0
			? await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Personz, p.ToDto())
			: await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Personz, p.ToDto())
								.RunInBackgroundWithResult();
		OnPropertyChanged(nameof(HasPersons));
	}
	#endregion

	#region Business                                                              

	[RelayCommand]
	private async Task OnAddBusiness() {
		if (businesses.Any(x => x.Id == 0)) {
			return;
		}
		_ = await UPAdditionalDataRepo.Instance.Biz.Initialize(new UPBusinessDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasBusiness));
	}

	[RelayCommand]
	private async Task OnSaveBusiness(UPBusinessViewModel p) {
		_ = p.IsValidationValid();
		await UPAdditionalDataRepo
			.Save(UPAdditionalDataRepo.Instance.Biz, p.ToDto())
			.RunInBackground();
		if (p.Id == 0)
			_ = await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Biz, p.ToDto());
	}

	[RelayCommand]
	private async Task DeleteBusiness(UPBusinessViewModel p) {
		_ = p.Id == 0
			? await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Biz, p.ToDto())
			: await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Biz, p.ToDto())
							.RunInBackgroundWithResult();
		OnPropertyChanged(nameof(HasBusiness));
	}
	#endregion

	#region Addresses
	[RelayCommand]
	private async Task OnAddAddress() {
		if (addresses.Any(x => x.Id == 0)) {
			return;
		}

		_ = await UPAdditionalDataRepo.Instance.Addrez.Create(new UPAddressDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasAddresses));
	}

	[RelayCommand]
	private async Task OnSaveAddress(ObsAddressViewModel p) {
		_ = p.IsValidationValid();
		if (p.Dto != null) {
			await UPAdditionalDataRepo
				.Save(UPAdditionalDataRepo.Instance.Addrez, p.ToDto())
				.RunInBackground();
			if (p.Id == 0)
				_ = await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Addrez, p.ToDto());
		}
	}

	[RelayCommand]
	private async Task OnDeleteAddress(ObsAddressViewModel p) {
		if (p.Dto != null) {
			_ = p.Dto.Id == 0
				? await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Addrez, p.Dto.ToDto())
				: await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Addrez, p.Dto.ToDto())
								.RunInBackgroundWithResult();
			OnPropertyChanged(nameof(HasAddresses));
		}
	}
	#endregion

	#region Logins     

	[RelayCommand]
	private async Task OnAddLogin() {
		if (logins.Any(x => x.Id == 0)) {
			return;
		}

		_ = await UPAdditionalDataRepo.Instance.Loginz.Initialize(new UPLoginDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasLogins));
	}

	[RelayCommand]
	private async Task OnSaveLogin(UPLoginViewModel p) {
		_ = p.IsValidationValid();
		await UPAdditionalDataRepo
			.Save(UPAdditionalDataRepo.Instance.Loginz, p!.ToDto())
			.RunInBackground();
		if (p.Id == 0)
			_ = await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Loginz, p.ToDto());
	}

	[RelayCommand]
	private async Task OnDeleteLogin(UPLoginViewModel p) {
		_ = p.Id == 0
			? await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Loginz, p!.ToDto())
			: await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Loginz, p!.ToDto())
							.RunInBackgroundWithResult();
		OnPropertyChanged(nameof(HasLogins));
	}
	#endregion

	public static IdentityViewModel Instance { get; } = IoC.GetService<IdentityViewModel>()!;
}
