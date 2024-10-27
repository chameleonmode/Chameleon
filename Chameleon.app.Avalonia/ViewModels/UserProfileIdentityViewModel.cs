using Chameleon.lib.Common.ServiceManagers;

using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using Chameleon.lib.Common.Extensions;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Api.Repos;
using System.Collections.ObjectModel;
using DynamicData;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Interfaces.Sys;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class UserProfileIdentityViewModel : ViewModelObjectBase {
	private readonly BehaviorSubject<Func<UP, bool>> filter;
	private readonly BehaviorSubject<Func<ObsAddressDto, bool>> adrezfilter;

	[ObservableProperty]
	private bool isSaving;
	[ObservableProperty]
	private ObsProfile? profileVM;
	[ObservableProperty]
	private UserProfileDto? userProfile;

	private readonly ReadOnlyObservableCollection<ObsAddressDto> addresses;
	private readonly ReadOnlyObservableCollection<UPBusinessDto> businesses;
	private readonly ReadOnlyObservableCollection<UPLoginDto> logins;
	private readonly ReadOnlyObservableCollection<UPPersonDto> persons;

	public ReadOnlyObservableCollection<ObsAddressDto> Addresses => addresses;
	public bool HasAddresses => Addresses?.Count > 0;
	public ReadOnlyObservableCollection<UPBusinessDto> Businesses => businesses;
	public bool HasBusiness => Businesses?.Count > 0;
	public ReadOnlyObservableCollection<UPLoginDto> Logins => logins;
	public bool HasLogins => Logins?.Count > 0;
	public ReadOnlyObservableCollection<UPPersonDto> Persons => persons;
	public bool HasPersons => Persons?.Count > 0;

	public int UserProfileId => UserProfile?.id ?? 0;
	public Func<UP, bool> FilterPredicate => p => p.ProfileId == UserProfile?.id;
	public Func<ObsAddressDto, bool> AdrezFilterPredicate => p => p.Dto?.ProfileId == UserProfile?.id;

	public UserProfileIdentityViewModel()
	{
		filter = new BehaviorSubject<Func<UP, bool>>(FilterPredicate);
		adrezfilter = new BehaviorSubject<Func<ObsAddressDto, bool>>(AdrezFilterPredicate);

		_ = UPAdditionalDataRepo.Instance.Personz
			.Connect()
			.Filter(filter)
			.Bind(out persons)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasPersons));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Loginz
			.Connect()
			.Filter(filter)
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
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);
		await LoadReload();
	}
	public override async Task OnNavigatedToAsync(object? param)
	{
		await base.OnNavigatedToAsync(param);

		if (param is UserProfileDto up) {
			UserProfile = up;
			ProfileVM = new ObsProfile(up, false);
			filter.OnNext(FilterPredicate);
			adrezfilter.OnNext(AdrezFilterPredicate);
			Title = ProfileVM?.Title;
		}
	}

	[RelayCommand]
	private async Task Discard()
	{
		await LoadReload(true);
	}

	[RelayCommand]
	private async Task SaveChanges()
	{
		if (!ProfileVM!.Title.Is())
			return;

		IsSaving = true;

		try {
			Logins.ForEach(async l => await OnSaveLogin(l));
			//
			Persons.ForEach(async l => await OnSavePerson(l));
			//
			Addresses.ForEach(async l => await OnSaveAddress(l.Dto!));
			//
			Businesses.ForEach(async l => await OnSaveBusiness(l));

			//TODO: check valid for saving only valid data (postoped / agreed)
			//UserProfile userProfile = _mapper.Map<UserProfile>(UserProfileModel);

			//await Task.Run(() => _userProfileService.Save(userProfile));
			var res = await UserProfilesRepo.Instance.Put(UserProfile!);
			if (res != null) {
				UserProfile = res;
				ProfileVM = new ObsProfile(UserProfile, false);
				Toaster.ShowSuccess($"Update was successful.");
			}
		} catch (Exception ex) {
			// Handle the exception (e.g., log it, show a notification, etc.)
			Toaster.ShowErr($"{ex.Message}");
		} finally {
			// Code to execute after the task completes, regardless of success or failure
			IsSaving = false;
		}
	}

	#region Persons

	[RelayCommand]
	private async Task AddPerson()
	{
		_ = await UPAdditionalDataRepo.Instance.Personz.Create(new UPPersonDto(){
			ProfileId = UserProfile.id
		});
		OnPropertyChanged(nameof(HasPersons));
	}

	[RelayCommand]
	private async Task OnSavePerson(UPPersonDto p)
	{
		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Personz, p);
	}

	[RelayCommand]
	private async Task DeletePerson(UPPersonDto p)
	{
		_ = await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Personz, p);
		OnPropertyChanged(nameof(HasPersons));
	}
	#endregion

	#region Business                                                              

	[RelayCommand]
	private async Task OnAddBusiness()
	{
		_ = await UPAdditionalDataRepo.Instance.Biz.Create(new UPBusinessDto() {
			ProfileId = UserProfile.id
		});
		OnPropertyChanged(nameof(HasBusiness));
	}

	[RelayCommand]
	private async Task OnSaveBusiness(UPBusinessDto p)
	{
		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Biz, p);
	}

	[RelayCommand]
	private async Task DeleteBusiness(UPBusinessDto p)
	{
		_ = await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Biz, p);
		OnPropertyChanged(nameof(HasBusiness));
	}
	#endregion

	#region Addresses
	[RelayCommand]
	private async Task OnAddAddress()
	{
		_ = await UPAdditionalDataRepo.Instance.Addrez.Create(new UPAddressDto() {
			ProfileId = UserProfile.id
		});
		OnPropertyChanged(nameof(HasAddresses));
	}

	[RelayCommand]
	private async Task OnSaveAddress(UPAddressDto p)
	{
		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Addrez, p);
	}

	[RelayCommand]
	private async Task OnDeleteAddress(UPAddressDto p)
	{
		_ = await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Addrez, p);
		OnPropertyChanged(nameof(HasAddresses));
	}
	#endregion

	#region Logins     

	[RelayCommand]
	private async Task OnAddLogin()
	{
		_ = await UPAdditionalDataRepo.Instance.Loginz.Create(new UPLoginDto() {
			ProfileId = UserProfile.id
		});
		OnPropertyChanged(nameof(HasLogins));
	}

	[RelayCommand]
	private async Task OnSaveLogin(UPLoginDto p)
	{
		_ = await UPAdditionalDataRepo.Save(UPAdditionalDataRepo.Instance.Loginz, p);
	}

	[RelayCommand]
	private async Task OnDeleteLogin(UPLoginDto p)
	{
		_ = await UPAdditionalDataRepo.Delete(UPAdditionalDataRepo.Instance.Loginz, p);
		OnPropertyChanged(nameof(HasLogins));
	}
	#endregion

	public static UserProfileIdentityViewModel Instance { get; } = IoC.GetService<UserProfileIdentityViewModel>()!;

	public static bool LoadedIniit { get; private set; }
	public static async Task LoadReload(bool force = false)
	{
		if (LoadedIniit && !force)
			return;

		await Task.WhenAll([
			UPAdditionalDataRepo.Instance.Personz.Load(),
			UPAdditionalDataRepo.Instance.Loginz.Load(),
			UPAdditionalDataRepo.Instance.Biz.Load(),
			UPAdditionalDataRepo.Instance.Addrez.Load()
		]);

		LoadedIniit = true;
	}
}
