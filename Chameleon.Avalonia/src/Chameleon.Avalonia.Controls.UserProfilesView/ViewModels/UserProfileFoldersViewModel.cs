using Chameleon.Core.Collections.Views;
using Chameleon.Core.Collections;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Prism.Events;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.UserProfiles;
using CommunityToolkit.Mvvm.Input;
using Chameleon.Interfaces.UserProfiles;
using Avalonia.Controls;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.Common.Helpers;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileFoldersViewModel
        : SubPageViewModelBase
        , IUserProfileFoldersViewModel
{
    private readonly IApplicationUser _currentUser;
    private readonly IAuthSession _authSession;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IDialogWindowsService _dialogWindowsService;

    private ObservableCollection<IUserProfileFolder, UserProfileFolderViewModel> _mapping;

    public UserProfileFoldersViewModel(
        IAuthSession authSession,
        IUserProfileFolderService userProfileFolderService,
        IDialogWindowsService dialogWindowsService,
        IApplicationUser currentUser
        )
    {
        _currentUser = currentUser;
        _authSession = authSession;
        _userProfileFolderService = userProfileFolderService;
        _dialogWindowsService = dialogWindowsService;

        //EventAggregator
        //    .GetEvent<LoginSuccessEvent>()
        //    .SubscribeOnce(OnAuthenticated);

        EventAggregator
          .GetEvent<DeleteUserProfileFolderEvent>()
          .Subscribe(OnDeleteFolder);

        EventAggregator
           .GetEvent<UpdateStaleDataEvent>()
           .Subscribe(LoadAsync);
    }
    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);
        IsWaiting = true;

        if (!Loaded)
        {
            LoadAsync();
        //OnPropertyChanged(nameof(AllProfiles));
        }
        AllProfiles.IsSelected = AllProfiles.IsSelected;
        IsWaiting = false;

        //if(SelectedFolder != null)
        // SelectedFolder.IsSelected = true;
    }

    [RelayCommand]
    private void Create()
    {
        EventAggregator
            .GetEvent<CreateUserProfileFolderEvent>()
            .Publish();
    }

    public IApplicationUser CurrentUser => _currentUser;
    public bool IsCreateBtnEnabled => !CurrentUser?.IsAssistant ?? false;

    private void OnDeleteFolder(UserProfileFolderEventArgs args)
    {
        AllProfiles.Open();
    }

    private UserProfileFolderViewModel _allProfiles;
    public UserProfileFolderViewModel AllProfiles
    {
        get
        {
            if (_allProfiles == null)
            {
                var folder = new UserProfileFolder { Title = "All profiles"};
                _allProfiles = new UserProfileFolderViewModel(_currentUser,
                    folder,
                    _userProfileFolderService,this
                    )
                { IsFavoriteButtonVisible = false };

                OnNavigatingTo(null);
                //_allProfiles.Open();
                //_allProfiles.IsSelected = true;
            }

            return _allProfiles;
        }
    }
    [ObservableProperty]
    private UserProfileFolderViewModel _selectedFolder;

    private ObservableCollectionView<UserProfileFolderViewModel> _viewModels;
    public ObservableCollectionView<UserProfileFolderViewModel> ViewModels
    {
        get
        {
            if (_viewModels == null && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<UserProfileFolderViewModel>(_mapping);
            }
            return _viewModels;
        }
    }

    private bool _isWaiting = true;
    public bool IsWaiting
    {
        get => _isWaiting;
        set => SetProperty(ref _isWaiting, value);
    }

    private void LoadAsync()
    {
        ViewModels?.Clear();
        _viewModels = null;

        var folders = _userProfileFolderService.GetAll();

        _mapping = new ObservableCollection<IUserProfileFolder, UserProfileFolderViewModel>(
            folders, folder => new UserProfileFolderViewModel(_currentUser,
                folder,
                _userProfileFolderService,
                this
                )
            );

        OnPropertyChanged(nameof(ViewModels));
    }

    private Func<IUserProfileFolder, bool> _filter;
    public Func<IUserProfileFolder, bool> Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
            {
                SetViewModelsFilter();
            }
        }
    }

    private void SetViewModelsFilter()
    {
        if (_viewModels == null)
        {
            return;
        }

        if (_filter == null)
        {
            _viewModels.Filter = null;
        }
        else
        {
            _viewModels.Filter = (viewModel) => _filter(viewModel.UserProfileFolder);
        }
    }

    public void Refresh()
    {
        _viewModels.Refresh();
    }


    public async void OnNavigatingTo(IUserProfileFolder p = null)
    {
        while (!Loaded)
            await Task.Delay(250);

        if (p != null)
        {
            //EventAggregator
            //    .GetEvent<OpenUserProfileFolderEvent>()
            //    .Publish(new UserProfileFolderEventArgs(p));
            _allProfiles.IsSelected = false;

            foreach (var item in _mapping)
                item.IsSelected = false;

            var pvm = _mapping.FirstOrDefault(vm =>vm.UserProfileFolder.Id == p.Id);
            if (pvm != null)
            {
                pvm.IsSelected = true;
                ContainerServiceHelper.Resolve<IUserProfilesViewModel>().Open(p);
            }
        }
        else
        {
            _allProfiles.Open();
        }
        //SearchText = p.Title;
    }

    public async void SetSelectedById(int id)
    {
        while (!Loaded)
            await Task.Delay(250);

        OnNavigatingTo(_mapping.FirstOrDefault(m => m.UserProfileFolder.Id == id)?.UserProfileFolder);
    }
}
