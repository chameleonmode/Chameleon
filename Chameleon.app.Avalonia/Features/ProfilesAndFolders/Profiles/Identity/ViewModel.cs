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
public partial class ViewModel: ViewModelObjectBase {
	private readonly BehaviorSubject<Func<UP, bool>> filter;
	private readonly BehaviorSubject<Func<ObsAddressDto, bool>> adrezfilter;

	[ObservableProperty]
	private bool isSaving;
	[ObservableProperty]
	private ObsProfile? profileVM;
	[ObservableProperty]
	private UserProfileViewModel? userProfile;

	private readonly ReadOnlyObservableCollection<ObsAddressDto> addresses;
	private readonly ReadOnlyObservableCollection<UPBusinessDto> businesses;
	private readonly ReadOnlyObservableCollection<UPLoginViewModel> logins;
	private readonly ReadOnlyObservableCollection<UPPersonViewModel> persons;

	public ReadOnlyObservableCollection<ObsAddressDto> Addresses => addresses;
	public bool HasAddresses => Addresses?.Count > 0;
	public ReadOnlyObservableCollection<UPBusinessDto> Businesses => businesses;
	public bool HasBusiness => Businesses?.Count > 0;
	public ReadOnlyObservableCollection<UPLoginViewModel> Logins => logins;
	public bool HasLogins => Logins?.Count > 0;
	public ReadOnlyObservableCollection<UPPersonViewModel> Persons => persons;
	public bool HasPersons => Persons?.Count > 0;

	public Func<UP, bool> FilterPredicate => p => p.ProfileId == UserProfile?.Id;
	public Func<ObsAddressDto, bool> AdrezFilterPredicate => p => p.Dto?.ProfileId == UserProfile?.Id;

	public ViewModel() {
		filter = new BehaviorSubject<Func<UP, bool>>(FilterPredicate);
		adrezfilter = new BehaviorSubject<Func<ObsAddressDto, bool>>(AdrezFilterPredicate);

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
			.Bind(out businesses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasBusiness));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Addrez
			.Connect()
			.Transform(a => new ObsAddressDto(a))
			.Filter(adrezfilter)
			.Bind(out addresses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasAddresses));
			});

		AsyncCommandMap["AddPerson"] = AddPerson;
		AsyncCommandMap["AddBusiness"] = OnAddBusiness;
		AsyncCommandMap["AddAddress"] = OnAddAddress;
		AsyncCommandMap["AddLogin"] = OnAddLogin;
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

	[RelayCommand]
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

			//TODO: check valid for saving only valid data (postoped / agreed)
			//UserProfile userProfile = _mapper.Map<UserProfile>(UserProfileModel);

			//await Task.Run(() => _userProfileService.Save(userProfile));
			var res = await UserProfilesRepo.Instance.Put(UserProfile!);
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
		_ = await UPAdditionalDataRepo.Instance.Personz.Create(new UPPersonDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasPersons));
	}

	[RelayCommand]
	private async Task OnSavePerson(UPPersonViewModel p) {
		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Personz, p.Adapt<UPPersonDto>());
	}

	[RelayCommand]
	private async Task DeletePerson(UPPersonViewModel p) {
		_ = await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Personz, p.Adapt<UPPersonDto>());
		OnPropertyChanged(nameof(HasPersons));
	}
	#endregion

	#region Business                                                              

	[RelayCommand]
	private async Task OnAddBusiness() {
		_ = await UPAdditionalDataRepo.Instance.Biz.Create(new UPBusinessDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasBusiness));
	}

	[RelayCommand]
	private async Task OnSaveBusiness(UPBusinessDto p) {
		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Biz, p);
	}

	[RelayCommand]
	private async Task DeleteBusiness(UPBusinessDto p) {
		_ = await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Biz, p);
		OnPropertyChanged(nameof(HasBusiness));
	}
	#endregion

	#region Addresses
	[RelayCommand]
	private async Task OnAddAddress() {
		_ = await UPAdditionalDataRepo.Instance.Addrez.Create(new UPAddressDto() {
			ProfileId = UserProfile?.Id
		});
		OnPropertyChanged(nameof(HasAddresses));
	}

	[RelayCommand]
	private async Task OnSaveAddress(ObsAddressDto p) {
		if (p.Dto != null) {
			_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Addrez, p.Dto);
		}
	}

	[RelayCommand]
	private async Task OnDeleteAddress(ObsAddressDto p) {
		if (p.Dto != null) {
			_ = await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Addrez, p.Dto);
			OnPropertyChanged(nameof(HasAddresses));
		}
	}
	#endregion

	#region Logins     

	[RelayCommand]
	private async Task OnAddLogin() {

		if(logins.Any(x => x.Id == 0)) {
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
		if(!p.Validator!.IsValid) {
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
