using Chameleon.Core.Collections.Views;
using Chameleon.Core.Collections;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chameleon.Interfaces.Dialogs;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class PopupUserProfileViewModel : ObservableObject
{
    private readonly IUserProfile _userProfile;
    private readonly IEventAggregator _eventAggregator;

    public PopupUserProfileViewModel(
        IUserProfile userProfile,
        IEventAggregator eventAggregator
        )
    {
        _userProfile = userProfile;
        _eventAggregator = eventAggregator;
    }


    [RelayCommand]
    private void Unselect()
    {
        IsSelected = false;
    }

    public IUserProfile UserProfile => _userProfile;
    public string Title => _userProfile?.Title;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                _eventAggregator
                    .GetEvent<SelectedChangePopupUserProfileEvent>()
                    .Publish(new SelectedUserProfileEventArgs(_userProfile, _isSelected));
            }
        }
    }
    // Title[0] is first character for icon profile
    public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
}

public class AddUserProfilesPopupViewModel : ObservableObjectBase, IAddUserProfilesPopupViewModel
{
    private readonly IUserProfileService _userProfileService;

    private ObservableCollection<IUserProfile, PopupUserProfileViewModel> _mapping;

    public AddUserProfilesPopupViewModel(
        IUserProfileService userProfileService
        )
    {
        _userProfileService = userProfileService;

        EventAggregator
            .GetEvent<SelectedChangePopupUserProfileEvent>()
            .Subscribe(OnSelectedChange);
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        var userProfiles = _userProfileService.GetAll();

        _mapping = new ObservableCollection<IUserProfile, PopupUserProfileViewModel>(
            userProfiles, profile => new PopupUserProfileViewModel(profile, EventAggregator)
            );
        OnPropertyChanged(nameof(ViewModels));
    }

    private void OnSelectedChange(SelectedUserProfileEventArgs args)
    {
        SelectedViewModels = ViewModels.Where(profile => profile.IsSelected);
    }

    private ObservableCollectionView<PopupUserProfileViewModel> _viewModels;
    public ObservableCollectionView<PopupUserProfileViewModel> ViewModels
    {
        get
        {
            if (_viewModels == null && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<PopupUserProfileViewModel>(_mapping)
                {
                    Order = profile => profile.Title,
                    Filter = profile => !profile.UserProfile.FolderId.HasValue
                };
            }

            return _viewModels;
        }
    }

    public bool HasSelected => SelectedViewModels?.Count() > 0;

    private IEnumerable<PopupUserProfileViewModel> _selectedViewModels;

    public IEnumerable<PopupUserProfileViewModel> SelectedViewModels
    {
        get => _selectedViewModels;
        set
        {
            if (SetProperty(ref _selectedViewModels, value))
            {
                OnPropertyChanged(nameof(HasSelected));
            }
        }
    }

    private IUserProfileFolder _folder;
    public IUserProfileFolder Folder
    {
        get => _folder;
        set
        {
            SetProperty(ref _folder, value);
        }
    }

    private bool _listIsVisible = true;
    public bool ListIsVisible
    {
        get => _listIsVisible;
        set
        {
            SetProperty(ref _listIsVisible, value);
        }
    }

    //SaveChangesCommand
    private void AddProfilesToFolderAsync()
    {
        //this.InvokeOnUiThreadAsync(AddProfilesToFolder);

        //_eventAggregator
        //    .GetEvent<CloseDialogWindowEvent>()
        //    .Publish(ButtonResult.OK);
    }

    private void AddProfilesToFolder()
    {
        if (!_selectedViewModels.Any())
        {
            return;
        }

        var ids = _selectedViewModels.Select(a => a.UserProfile.Id).ToList();
        _userProfileService.MoveUserProfileToFolder(ids, Folder.Id);

        EventAggregator
                .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
                .Publish(new ChangeProfilesInFavoriteFolderEventArgs(Folder.Id));
    }

    public void OnDialogClosing(IContentDialogResult result)
    {
        if (result == IContentDialogResult.Primary)
            AddProfilesToFolder();
    }
}
