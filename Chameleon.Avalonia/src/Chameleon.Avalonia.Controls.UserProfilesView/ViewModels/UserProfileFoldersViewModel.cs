using Chameleon.Core.Collections.Views;
using Chameleon.Core.Collections;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.UserProfiles;
using CommunityToolkit.Mvvm.Input;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.Common.Helpers;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileFoldersViewModel
        : SubPageViewModelBase
        , IUserProfileFoldersViewModel
{
    private readonly IApplicationUser _currentUser;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IUserProfileService _userProfileService;

    private ObservableCollection<IUserProfileFolder, UserProfileFolderViewModel> _mapping;

    public UserProfileFoldersViewModel(
        IUserProfileFolderService userProfileFolderService,
        IApplicationUser currentUser,
        IUserProfileService userProfileService)
    {
        _currentUser = currentUser;
        _userProfileFolderService = userProfileFolderService;
        _userProfileService = userProfileService;

        EventAggregator
           .GetEvent<UpdateStaleDataEvent>()
           .Subscribe(LoadAsync);

        EventAggregator
            .GetEvent<OpenUserProfileFolderEvent>()
            .Subscribe(async args => await OnNavigatingTo(args.UserProfileFolder));
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
                    _userProfileFolderService,this, _userProfileService
                    )
                { IsFavoriteButtonVisible = false };

                //OnNavigatingTo(null);
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
                this,
                _userProfileService
                )
            );
        _mapping.Insert(0, AllProfiles);

        //AllProfiles.Open();
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


    public async Task OnNavigatingTo(IUserProfileFolder p = null)
    {
        while (!Loaded)
            await Task.Delay(250);

        if (p != null)
        {
            //EventAggregator
            //    .GetEvent<OpenUserProfileFolderEvent>()
            //    .Publish(new UserProfileFolderEventArgs(p));
            //_allProfiles.IsSelected = false;

            foreach (var item in _mapping)
                item.IsSelected = item.UserProfileFolder.Id == p.Id;

            var pvm = _mapping.FirstOrDefault(vm =>vm.UserProfileFolder.Id == p.Id);
            if (pvm != null)
            {
                ContainerServiceHelper.Resolve<IUserProfilesViewModel>().Open(p);
            }
        }
        else
        {
            if (!AllProfiles.UserProfileFolder.Navigated)
            {
                AllProfiles.UserProfileFolder.Navigated = true;
                await AllProfiles.Open();
            }
        }
        //SearchText = p.Title;
    }

    public async void SetSelectedById(int id)
    {
        while (!Loaded)
            await Task.Delay(250);

        await OnNavigatingTo(_mapping.FirstOrDefault(m => m.UserProfileFolder.Id == id)?.UserProfileFolder);
    }
}
