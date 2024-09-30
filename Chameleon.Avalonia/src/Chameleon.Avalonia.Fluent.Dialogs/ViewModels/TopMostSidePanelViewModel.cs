using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Av.Fluent.Dialogs.ViewModels;

public class TopMostSidePanelViewModel : SubPageViewModelBase, ITopMostSidePanelViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly IApplicationUser _applicationUser;
    public TopMostSidePanelViewModel(IUserProfileService userProfileService,
        IApplicationUser applicationUser)
    {
        _userProfileService = userProfileService;
        _applicationUser = applicationUser;
    }   
    public List<IUserProfileActionsViewModel> RunningList { get; set; } = [];

    private ObservableCollection<IUserProfile, UserProfileViewModel> _mapping;
    private ObservableCollectionView<UserProfileViewModel> _viewModels;
               
    public ObservableCollectionView<UserProfileViewModel> ViewModels
    {
        get
        {
            if ((_viewModels == null || _viewModels.Count == 0) && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<UserProfileViewModel>(_mapping)
                {
                    TrackItemChanges = true,
                    Order = profile => profile.Title
                };
            }

            if (_viewModels != null)
            {
                _viewModels.Filter = FilterProfiles;
            }

            return _viewModels;
        }
    }

    private bool FilterProfiles(IUserProfileActionsViewModel userProfile)
    {
        return RunningList.Any(p => p.UserProfile.Id == userProfile.UserProfile.Id);
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            var profiles = await _userProfileService.GetAllAsync();

            _mapping = new ObservableCollection<IUserProfile, UserProfileViewModel>(profiles, profile => new UserProfileViewModel(
                    _userProfileService,
                    profile as UserProfile,
                    _applicationUser,
                    false,
                    false,
                    false,
                    false,
                    false));
        }

        Update();
    }

    public void Update()
    {
        OnPropertyChanged(nameof(ViewModels));
    }
}
