using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Base;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Addresses;

public partial class AddressesViewModel : ProfileSectionViewModel<UPAddressDto, ObsAddressViewModel> {
	private readonly BehaviorSubject<Func<ObsAddressViewModel, bool>> adrezfilter;
	private readonly List<int> newlyAddedAddress = [];

	public Func<ObsAddressViewModel, bool> AdrezFilterPredicate => p => p.Dto?.ProfileId == userProfile?.Id;

	protected override UPRepo<UPAddressDto> SourceRepository => UPAdditionalDataRepo.Instance.Addrez;

	protected override ObsAddressViewModel CreateViewModel(UPAddressDto dto) => new(dto);

	protected override UPAddressDto GetDtoFromViewModel(ObsAddressViewModel item) => item.ToDto();

	public AddressesViewModel(UserProfileViewModel? userProfile)
			: base(userProfile, nameof(Items), nameof(HasItems)) {
		adrezfilter = new BehaviorSubject<Func<ObsAddressViewModel, bool>>(AdrezFilterPredicate);

		_ = UPAdditionalDataRepo.Instance.Addrez
				.Connect()
				.Transform(a => new ObsAddressViewModel(a))
				.Filter(adrezfilter)
				.Bind(out var addresses)
				.Subscribe((i) => {
					IsLoading = false;
					OnPropertyChanged(nameof(HasItems));
					OnPropertyChanged(nameof(HasAddresses));
				});

		AsyncCommandMap["AddAddress"] = AddItem;
	}

	public override void UpdateFilter() {
		IsLoading = true;
		adrezfilter.OnNext(AdrezFilterPredicate);
	}

	public override async Task AddItem() {
		if (IsLoading || IsBusy)
			return;

		if (Items.Any(IsNewItem) || newlyAddedAddress.Count != 0) {
			return;
		}

		IsLoading = true;
		try {
			var addedAddress = await UPAdditionalDataRepo.Instance.Addrez.Create(new UPAddressDto() {
				ProfileId = userProfile?.Id
			});
			newlyAddedAddress.Add(addedAddress.id);
			OnPropertyChanged(nameof(HasItems));
			OnPropertyChanged(nameof(HasAddresses));
		}
		finally {
			IsLoading = false;
		}
	}

	[RelayCommand]
	public async Task OnSaveAddress(ObsAddressViewModel p) {
		IsLoading = true;
		try {
			_ = p.IsValidationValid();
			if (p.Dto != null) {
				await UPAdditionalDataRepo
						.Save(UPAdditionalDataRepo.Instance.Addrez, p.ToDto())
						.RunInBackground();
				if (p.Id == 0)
					_ = await UPAdditionalDataRepo.DeleteFromCache(UPAdditionalDataRepo.Instance.Addrez, p.ToDto());
				
				lock (newlyAddedAddress) {
					if (newlyAddedAddress.Any(id => p.Id == id))
						_ = newlyAddedAddress.Remove(p.Id);
				}
			}
		}
		finally {
			IsLoading = false;
		}
	}

	public override async Task SaveItem(ObsAddressViewModel item) {
		await OnSaveAddress(item);
	}

	public override async Task SaveAll() {
		IsLoading = true;
		try {
			var itemsToSave = Items.ToList();

			foreach (var item in itemsToSave) {
				await SaveItem(item);
			}

			lock (newlyAddedAddress) {
				newlyAddedAddress.Clear();
			}
		}
		finally {
			IsLoading = false;
		}
	}

	public static AddressesViewModel Create(UserProfileViewModel? userProfile) {
		return new AddressesViewModel(userProfile);
	}
}