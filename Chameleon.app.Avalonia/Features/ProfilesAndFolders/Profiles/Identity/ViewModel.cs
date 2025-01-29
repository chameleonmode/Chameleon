using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Mapster;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity;
public partial class ViewModel : ViewModelObjectBase {
	private readonly BehaviorSubject<Func<UP, bool>> filter;
	private readonly BehaviorSubject<Func<ObsAddressViewModel, bool>> adrezfilter;

	[ObservableProperty]
	private bool isSaving;
	[ObservableProperty]
	private ObsProfile? profileVM;
	[ObservableProperty]
	private UserProfileViewModel? userProfile;

	private readonly ReadOnlyObservableCollection<ObsAddressViewModel> addresses;
	private readonly ReadOnlyObservableCollection<UPBusinessViewModel> businesses;
	private readonly ReadOnlyObservableCollection<UPLoginViewModel> logins;
	private readonly ReadOnlyObservableCollection<UPPersonViewModel> persons;

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

	public ViewModel() {
		filter = new BehaviorSubject<Func<UP, bool>>(FilterPredicate);
		adrezfilter = new BehaviorSubject<Func<ObsAddressViewModel, bool>>(AdrezFilterPredicate);

		_ = UPAdditionalDataRepo.Instance.Personz
			.Connect()
			.Filter(filter)
			.Transform(x => x.Adapt<UPPersonViewModel>())
			.Bind(out persons)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasPersons));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Loginz
			.Connect()
			.Filter(filter)
			.Transform(x => x.Adapt<UPLoginViewModel>())
			.Bind(out logins)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasLogins));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Biz
			.Connect()
			.Filter(filter)
			.Transform(x => x.Adapt<UPBusinessViewModel>())
			.Bind(out businesses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasBusiness));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Addrez
			.Connect()
			.Transform(a => new ObsAddressViewModel(a.Adapt<UPAddressViewModel>()))
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
		await UPAdditionalDataRepo.Instance.LoadReload();
	}
	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);

		if (param is UserProfileDto up) {
			UserProfile = up.Adapt<UserProfileViewModel>();
			ProfileVM = new ObsProfile(up, false);
			filter.OnNext(FilterPredicate);
			adrezfilter.OnNext(AdrezFilterPredicate);
			Title = ProfileVM?.Title;
		}
	}

	[RelayCommand]
	private async Task Discard() {
		await UPAdditionalDataRepo.Instance.LoadReload(true);
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
				Toaster.Error("User profile has validation errors");
				return;
			}

			var res = await UserProfilesRepo.Instance.Put(UserProfile.Adapt<UserProfileDto>());
			if (res != null) {
				UserProfile = res.Adapt<UserProfileViewModel>();
				ProfileVM = new ObsProfile(UserProfile.Adapt<UserProfileDto>(), false);
				Toaster.Success($"Update was successful.");
			}
		} catch (Exception ex) {
			// Handle the exception (e.g., log it, show a notification, etc.)
			Toaster.Error($"{ex.Message}");
		} finally {
			// Code to execute after the task completes, regardless of success or failure
			IsSaving = false;
		}
	}

	#region Persons

	[RelayCommand]
	private async Task AddPerson() {

		if (persons.Any(x => x.Id == 0)) {
			Toaster.Error("Please complete the avialable person form");
			return;
		}

		_ = await UPAdditionalDataRepo.Instance.Personz.Initialize(new UPPersonDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasPersons));
	}

	[RelayCommand]
	private async Task OnSavePerson(UPPersonViewModel p) {

		if (p.Validator?.IsValid == false) {
			Toaster.Error("Person form is not valid");
			return;
		}

		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Personz, p.Adapt<UPPersonDto>());
	}

	[RelayCommand]
	private async Task DeletePerson(UPPersonViewModel p) {
		_ = p.Id == 0
			? await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Personz, p.Adapt<UPPersonDto>())
			: await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Personz, p.Adapt<UPPersonDto>());
		OnPropertyChanged(nameof(HasPersons));
	}
	#endregion

	#region Business                                                              

	[RelayCommand]
	private async Task OnAddBusiness() {
		if (businesses.Any(x => x.Id == 0)) {
			Toaster.Error("Please complete the avialable business form");
			return;
		}
		_ = await UPAdditionalDataRepo.Instance.Biz.Initialize(new UPBusinessDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasBusiness));
	}

	[RelayCommand]
	private async Task OnSaveBusiness(UPBusinessViewModel p) {

		if (p.Validator?.IsValid == false) {
			Toaster.Error("Business form is not valid");
			return;
		}

		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Biz, p.Adapt<UPBusinessDto>());
	}

	[RelayCommand]
	private async Task DeleteBusiness(UPBusinessViewModel p) {
		_ = p.Id == 0
			? await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Biz, p.Adapt<UPBusinessDto>())
			: await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Biz, p.Adapt<UPBusinessDto>());
		OnPropertyChanged(nameof(HasBusiness));
	}
	#endregion

	#region Addresses
	[RelayCommand]
	private async Task OnAddAddress() {

		if (addresses.Any(x => x.Id == 0)) {
			Toaster.Error("Please complete the avialable address form");
			return;
		}

		_ = await UPAdditionalDataRepo.Instance.Addrez.Create(new UPAddressDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasAddresses));
	}

	[RelayCommand]
	private async Task OnSaveAddress(ObsAddressViewModel p) {

		if (p.Validator?.IsValid == false) {
			Toaster.Error("Address form is not valid");
			return;
		}

		if (p.Dto != null) {
			_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Addrez, p.Dto.Adapt<UPAddressDto>());
		}
	}

	[RelayCommand]
	private async Task OnDeleteAddress(ObsAddressViewModel p) {
		if (p.Dto != null) {
			_ = p.Dto.Id == 0
				? await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Addrez, p.Dto.Adapt<UPAddressDto>())
				: await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Addrez, p.Dto.Adapt<UPAddressDto>());
			OnPropertyChanged(nameof(HasAddresses));
		}
	}
	#endregion

	#region Logins     

	[RelayCommand]
	private async Task OnAddLogin() {

		if (logins.Any(x => x.Id == 0)) {
			Toaster.Error("Please complete the avialable login form");
			return;
		}

		var login = new UPLoginDto() {
			ProfileId = UserProfile?.Id
		};
		_ = await UPAdditionalDataRepo.Instance.Loginz.Initialize(login);
		OnPropertyChanged(nameof(HasLogins));
	}

	[RelayCommand]
	private async Task OnSaveLogin(UPLoginViewModel p) {
		if (p.Validator?.IsValid == false) {
			Toaster.Error("Login form is not valid");
			return;
		}
		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Loginz, p.Adapt<UPLoginDto>());
	}

	[RelayCommand]
	private async Task OnDeleteLogin(UPLoginViewModel p) {

		_ = p.Id == 0
			? await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Loginz, p.Adapt<UPLoginDto>())
			: await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Loginz, p.Adapt<UPLoginDto>());

		OnPropertyChanged(nameof(HasLogins));
	}
	#endregion

	public static ViewModel Instance { get; } = IoC.GetService<ViewModel>()!;
}
