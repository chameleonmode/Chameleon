using AutoMapper;
using Avalonia.Collections;
using Chameleon.Authorization;
using Chameleon.Interfaces.App.UserProfiles.Events.Common;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Common;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.ServiceManagers;
using System.Collections.Specialized;

using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using Chameleon.lib.Common.Extensions;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.app.Avalonia.app;
using Chameleon.Common.Helpers;
using Chameleon.lib.Api.Repos;
using System.Collections.ObjectModel;
using DynamicData;
using System;
using Chameleon.Core.Extensions;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class UserProfileIdentityViewModel : ViewModelObjectBase {
	private readonly IAuthSession _authSession = ContainerServiceHelper.Resolve<IAuthSession>()!;
	private readonly IMapper _mapper;
	//private readonly IUserProfileAdditionalDataService _userProfileAdditionalDataService;
	//private readonly IUserAssistantService _userAssistantService;

	[ObservableProperty]
	private ObsProfile profileVM;
	[ObservableProperty]
	private bool isSaving;

	public ObservableCollection<UPAddressDto> Addresses { get; } = [];
	public ObservableCollection<UPBusinessDto> Businesses { get; } = [];
	public ObservableCollection<UPLoginDto> Logins { get; } = [];
	public ObservableCollection<UPPersonDto> Persons { get; } = [];

	private readonly ReadOnlyObservableCollection<UPAddressDto> addresses;
	private readonly ReadOnlyObservableCollection<UPBusinessDto> businesses;
	private readonly ReadOnlyObservableCollection<CountryzDto> countries;
	private readonly ReadOnlyObservableCollection<UPLoginDto> logins;
	private readonly ReadOnlyObservableCollection<UPPersonDto> persons;

	public ReadOnlyObservableCollection<UPAddressDto> Addrez => addresses;
	public ReadOnlyObservableCollection<UPBusinessDto> Businez => businesses;
	public ReadOnlyObservableCollection<CountryzDto> Countries => countries;
	public ReadOnlyObservableCollection<UPLoginDto> Loginz => logins;
	public ReadOnlyObservableCollection<UPPersonDto> Personz => persons;

	public int UserProfileId => _userProfile?.id ?? 0;
	public bool HasPersons => Personz?.Count > 0;
	public bool HasLogins => Loginz?.Count > 0;
	public bool HasBusiness => Businez?.Count > 0;
	public bool HasAddresses => Addrez?.Count > 0;

	private int _selectedTadIndex;
	public int SelectedTadIndex {
		get => _selectedTadIndex;
		set {
			if (SetProperty(ref _selectedTadIndex, value)) {
				Discard();
			}
		}
	}

	public UserProfileIdentityViewModel(UserProfileDto up)
	{
		_ = UPAdditionalDataRepo.Instance.Countryz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out countries)
			.Subscribe();
		//
		_ = UPAdditionalDataRepo.Instance.Personz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out persons)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasPersons));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Loginz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out logins)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasLogins));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Biz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out businesses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasBusiness));
		});
		//
		_ = UPAdditionalDataRepo.Instance.Addrez
			.Connect(i => i.ProfileId == up.id)
			.Bind(out addresses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasAddresses));
			});

		CommandMap["AddPerson"] = AddPerson;
		CommandMap["AddBusiness"] = OnAddBusiness;
		CommandMap["AddAddress"] = OnAddAddress;
		CommandMap["AddLogin"] = OnAddLogin;
	}

	public override async Task OnNavigatedToAsync(object? param)
	{
		await base.OnNavigatedToAsync(param);

		if (param is UserProfileDto up) {
			UserProfile = up;
		}

		Title = ProfileVM.Title;
	}

	private void OpenUserProfileIdentityTab(UserProfileIdentityTab userProfileIdentityTab)
	{
		SelectedTadIndex = (int)userProfileIdentityTab;
		OnPropertyChanged(nameof(SelectedTadIndex));
	}

	[RelayCommand]
	private void Discard()
	{
		
	}

	private void OnUserProfileSaved(IUserProfile userProfile)
	{
		if (_userProfile == null || userProfile.Id != _userProfile.id) {
			return;
		}

		//ProfileVM = new ObsProfile(userProfile, false);
		//UserProfileModel = _mapper.Map<UserProfileBindable>(_userProfile);
	}

	//private async Task BindUi()
	//{
	//	ProfileVM = new ObsProfile(
	//								 UserProfile,
	//								 false);

	//	Task[] tasks = [
	//			Addresses.AddNewRangeAsync(() => _userProfileAdditionalDataService.GetAddressesAsync(UserProfile.id)),
	//			Persons.AddNewRangeAsync(() => _userProfileAdditionalDataService.GetPersonsAsync(UserProfile.id)),
	//			Logins.AddNewRangeAsync(() => _userProfileAdditionalDataService.GetLoginsAsync(UserProfile.id)),
	//			Businesses.AddNewRangeAsync(() => _userProfileAdditionalDataService.GetBusinessesAsync(UserProfile.id))
	//	];

	//	CollectionChanged(this, null);

	//	UpdateCollectionChangedHandlers(CollectionChanged, false);
	//	UpdateCollectionChangedHandlers(CollectionChanged, true);

	//	try {
	//		if (Countries.Count == 0)
	//			await Countries.AddNewRangeAsync(() => Task.Run(_userProfileAdditionalDataService.GetCountries));

	//		await Task.WhenAll(tasks);
	//		foreach (var a in Addresses)
	//			a.SelectedCountry = Countries.FirstOrDefault(x => a?.CountryId == x.Id) ?? Countries[0];
	//	} catch (Exception ex) {
	//		Toaster.ShowErr(ex.Message);
	//	}
	//}


	#region UserProfile

	private UserProfileDto _userProfile;
	public UserProfileDto UserProfile {
		get => _userProfile;
		set {
			if (SetProperty(ref _userProfile, value)) {
				RestrictConfigurations(_authSession.Permissions);
			}
		}
	}

	[RelayCommand]
	private async Task SaveChanges()
	{
		if (!ProfileVM.Title.Is())
			return;

		IsSaving = true;


		try {
			Logins.ForEach(async l => await OnSaveLogin(l));
			Logins.Clear();
			//
			Persons.ForEach(async l => await OnSavePerson(l));
			Persons.Clear();
			//
			Addresses.ForEach(async l => await OnSaveAddress(l));
			Addresses.Clear();
			//
			Businesses.ForEach(async l => await OnSaveBusiness(l));
			Businesses.Clear();

			//TODO: check valid for saving only valid data (postoped / agreed)
			//UserProfile userProfile = _mapper.Map<UserProfile>(UserProfileModel);

			//await Task.Run(() => _userProfileService.Save(userProfile));
			var res = await UserProfilesRepo.Instance.Put(UserProfile);
			if (res != null) {
			}
		} catch (Exception ex) {
			// Handle the exception (e.g., log it, show a notification, etc.)
			Toaster.ShowErr($"{ex.Message}");
		} finally {
			// Code to execute after the task completes, regardless of success or failure
			IsSaving = false;
		}
	}


	#endregion

	#region Persons

	[RelayCommand]
	private void AddPerson()
	{
		var person = new UPPersonDto() {
			ProfileId = UserProfile.id
		};

		Persons.Add(person);
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
		Persons.Remove(p);
		OnPropertyChanged(nameof(HasPersons));
	}
	#endregion

	#region Business                                                              

	[RelayCommand]
	private void OnAddBusiness()
	{
		var business = new UPBusinessDto() {
			ProfileId = UserProfile.id
		};

		Businesses.Add(business);
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
		Businesses.Remove(p);
		OnPropertyChanged(nameof(HasBusiness));
	}
	#endregion

	#region Addresses
	[RelayCommand]
	private void OnAddAddress()
	{
		var address = new UPAddressDto() {
			ProfileId = UserProfile.id
		};

		Addresses.Add(address);
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
		Addresses.Remove(p);
		OnPropertyChanged(nameof(HasAddresses));
	}
	#endregion

	#region Logins     

	[RelayCommand]
	private void OnAddLogin()
	{
		var login = new UPLoginDto() {
			ProfileId = UserProfile.id
		};
		Logins.Add(login);
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
		Logins.Remove(p);
		OnPropertyChanged(nameof(HasLogins));
	}
	#endregion

	#region Main Configuration

	private void RestrictConfigurations(string[] permissions)
	{
		bool isShared = UserProfile?.creatorUserId != _authSession.UserId;
		IsProxyConfigVisible = !isShared && permissions.Contains(PermissionNames.Pages_Proxy_Config);
		IsCurateConfigVisible = !isShared && permissions.Contains(PermissionNames.Pages_Curate_Config);
		IsYouTubeConfigVisible = !isShared && permissions.Contains(PermissionNames.Pages_YouTube_Config);
	}

	private bool _isProxyConfigVisible;
	public bool IsProxyConfigVisible {
		get => _isProxyConfigVisible;
		set => SetProperty(ref _isProxyConfigVisible, value);
	}

	private bool _isCurateConfigVisible;
	public bool IsCurateConfigVisible {
		get => _isCurateConfigVisible;
		set => SetProperty(ref _isCurateConfigVisible, value);
	}

	private bool _isYouTubeConfigVisible;
	public bool IsYouTubeConfigVisible {
		get => _isYouTubeConfigVisible;
		set => SetProperty(ref _isYouTubeConfigVisible, value);
	}

	#endregion

	#region Synchronization
	[RelayCommand]
	private void SyncChanges()
	{
		//EventAggregator
		//		.GetEvent<OpenUserProfileEvent>()
		//		.Publish(new UserProfileEventArgs(_userProfile));

		Toaster.ShowSuccess("Synchronization is completed");
	}

	#endregion
}
