using Chameleon.Core.Extensions;
using Chameleon.Common.Exceptions;
using Chameleon.Interfaces.App.ContentDiscoverey;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Interfaces.Windows;
using Chameleon.Prism.Events;
using System;
using System.Drawing;
using System.Windows;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Common.Icons;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Services;
//using Chameleon.Avalonia.Prism.Interfaces.MessageBox;
//using MessageBoxOptions = Chameleon.MessageBox.Services.MessageBoxOptions;

namespace Chameleon.Application.Events
{
    public class UserProfileEventHandler : IUserProfileEventHandler
    {
        //private readonly IMainWindow _mainWindow;
        //private readonly IUserProfileView _userProfileView;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserProfileService _userProfileService;
        private readonly ISystemBrowserManager _systemBrowserManager;
        private readonly IOutReachTemplateView _outReachView;
        private readonly IAuthSession _authSession;
       // private readonly IDialogWindowsService _dialogWindowsService;
        private readonly IErrorContentDialogView _userFriendlyExceptionView;
        private readonly INavigationService _navigationSrvice;
        public UserProfileEventHandler(
            //IMainWindow mainWindow,
            IUserProfileService userProfileService,
            //IUserProfileView userProfileView,
            IEventAggregator eventAggregator,
            ISystemBrowserManager systemBrowserManager,
            IOutReachTemplateView outReachView,
            IAuthSession authSession,
            INavigationService navigationService
            // IDialogWindowsService dialogWindowsService
            //TODO: IErrorContentDialogView userFriendlyExceptionView
            )
        {
            _outReachView = outReachView;
            //_messageBoxService = messageBoxService;
            //_mainWindow = mainWindow;
            //_userProfileView = userProfileView;
            _eventAggregator = eventAggregator;
            _userProfileService = userProfileService;
            _systemBrowserManager = systemBrowserManager;
            _authSession = authSession;
            //_dialogWindowsService = dialogWindowsService;
            //_userFriendlyExceptionView = userFriendlyExceptionView;
            _navigationSrvice = navigationService;

            _eventAggregator
                .GetEvent<OpenUserProfileEvent>()
                .Subscribe(args => OnOpenUserDetails(args.UserProfile));

            _eventAggregator
                .GetEvent<OpenUserBrowserEvent>()
                .Subscribe(args => OnOpenUserBrowser(args.UserProfile));

            _eventAggregator
                .GetEvent<CreateUserProfileEvent>()
                .Subscribe(args => CreateUserProfile(args.FolderId));

            _eventAggregator
                .GetEvent<DeleteUserProfileEvent>()
                .Subscribe(args => DeleteUserProfileEvent(args.UserProfile));

            _eventAggregator
                .GetEvent<OpenUserRssEvent>()
                .Subscribe(args => OnOpenUserRss(args.UserProfile));

            _eventAggregator
                .GetEvent<OpenUserContentDiscovereyEvent>()
                .Subscribe(args => OnOpenUserContentDiscoverey(args.UserProfile));

            _eventAggregator
                .GetEvent<OpenUserOutReachEvent>()
                .Subscribe(args => OnOpenUserOutReach(args.UserProfile));

            _eventAggregator
                .GetEvent<OpenUserSystemBrowserEvent>()
                .Subscribe(args => OnOpenUserSystemBrowser(args));

            _eventAggregator
                .GetEvent<UnfavoriteUserProfileEvent>()
                .Subscribe(args => UnfavoriteUserProfileEvent(args.UserProfile));

            _eventAggregator
                .GetEvent<FavoriteUserProfileEvent>()
                .Subscribe(args => FavoriteUserProfileEvent(args.UserProfile));

            _eventAggregator
                .GetEvent<AddUserProfileToFolderEvent>()
                .Subscribe(args => AddToFolder(args.UserProfileFolder, args.UserProfile));

            _eventAggregator
                .GetEvent<RemoveUserProfileFromFolderEvent>()
                .Subscribe(args => RemoveFromFolder(args.UserProfile));

            _eventAggregator
                .GetEvent<OpenOutReachTemplateEvent>()
                .Subscribe(args => OpenOutReachTemplate(args.OutReachTemplate));

            _eventAggregator
                .GetEvent<OutReachOpenEvent>()
                .Subscribe(args => OpenOutReach(args.UserProfile));

            _eventAggregator
                .GetEvent<OutReachLinksOpenEvent>()
                .Subscribe(args => OpenOutReachLink(args.UserProfile));
        }

        private void OnOpenUserContentDiscoverey(IUserProfileInfo profileInfo)
        {
            OpenUserDetails(profileInfo, UserProfileViewTab.ContentDiscoverey, OutReachViewTab.Rss);
        }

        private void OpenOutReachLink(IUserProfileInfo profileInfo)
        {
            OpenUserDetails(profileInfo, UserProfileViewTab.OutReach, OutReachViewTab.Link);
        }

        private void OpenOutReach(IUserProfileInfo profileInfo)
        {
            OpenUserDetails(profileInfo, UserProfileViewTab.OutReach, OutReachViewTab.Rss);
        }

        private void OpenOutReachTemplate(IOutReachTemplate template)
        {
            //_outReachView.SetOutReachTemplate(template, _userProfileView.UserProfile);
            //_mainWindow.ShowWaitIndicator();
            //_mainWindow.SetContent(_outReachView, "");
        }

        private void RemoveFromFolder(IUserProfile userProfile)
        {
            userProfile.FolderId = null;
            _userProfileService.Save(userProfile);
        }

        private void AddToFolder(IUserProfileFolder userProfileFolder, IUserProfile userProfile)
        {
            userProfile.FolderId = userProfileFolder.Id;
            _userProfileService.Save(userProfile);
        }

        private void FavoriteUserProfileEvent(IUserProfile userProfile)
        {
            try
            {
                _userProfileService.SetProfileIsFavorite(userProfile.Id, true);
            }
            catch (UserFriendlyException ex)
            {
                ShowErrorDialog(ex.Title, ex.Message);
            }
        }

        private void UnfavoriteUserProfileEvent(IUserProfile userProfile)
        {
            try
            {
                _userProfileService.SetProfileIsFavorite(userProfile.Id, false);
            }
            catch (UserFriendlyException ex)
            {
                ShowErrorDialog(ex.Title, ex.Message);
            }
        }

        private void DeleteUserProfileEvent(IUserProfile userProfile)
        {
            _userProfileService.Delete(userProfile);
        }

        private void CreateUserProfile(int? folderId)
        {
            IUserProfile profile;
            try
            {
                profile = _userProfileService.Create(folderId);
            }
            catch (Exception ex)
            {
                if (ex.Message == "limit_ex")
                    _userProfileService.ShowOutOfLimitPopup();
                else throw;
                return;
            }

            OpenUserDetails(profile, UserProfileViewTab.Details);

            _eventAggregator
                .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
                .Publish(new ChangeProfilesInFavoriteFolderEventArgs(folderId == null ? 0 : folderId.Value));
        }

        private void OnOpenUserDetails(IUserProfileInfo profileInfo)
        {
            OpenUserDetails(profileInfo, UserProfileViewTab.Details);
        }

        private void OnOpenUserBrowser(IUserProfileInfo profileInfo)
        {
            OpenUserDetails(profileInfo, UserProfileViewTab.Browser);
        }

        private void OnOpenUserRss(IUserProfileInfo profileInfo)
        {
            OpenUserDetails(profileInfo, UserProfileViewTab.Rss);
        }

        private void OnOpenUserOutReach(IUserProfileInfo profileInfo)
        {
            OpenUserDetails(profileInfo, UserProfileViewTab.OutReach);
        }

        private void OpenUserDetails(IUserProfileInfo profileInfo, UserProfileViewTab tab, OutReachViewTab outReachTab = OutReachViewTab.Rss)
        {
            try
            {
                var profile = GetProfile(profileInfo);

                _eventAggregator
                    .GetEvent<UpdateStaleDataEvent>()
                    .Publish();

                OpenUserDetails(profile, tab, outReachTab);
            }
            catch (UserFriendlyException ex)
            {
                ShowErrorDialog(ex.Title, ex.Message);
            }
        }

        private IUserProfile GetProfile(IUserProfileInfo profileInfo)
        {
            return _userProfileService.Get(profileInfo.Id, true);
        }

        private void OpenUserDetails(IUserProfile profile, UserProfileViewTab tab, OutReachViewTab outReachTab = OutReachViewTab.Rss)
        {
            //_userProfileView.SetUserProfile(profile, tab, outReachTab);
            //_mainWindow.SetContent(_userProfileView, profile.Title);
            _navigationSrvice.NavigateToType(typeof(IUserProfileView), profile);
        }

        private void OnOpenUserSystemBrowser(UserProfileSystemBrowserEventArgs args)
        {
            var profileInfo = args.UserProfile;

            try
            {
                var profile = GetProfile(profileInfo);

                _systemBrowserManager
                    .Get(args.BrowserType)
                    .Open(new SystemBrowserLaunchOptions
                    {
                        Url = args.Url,
                        SignIn = args.SignIn,
                        UserProfile = profile
                    });
            }
            catch (NotSupportedException)
            {
                MesageBoxHelper.ShowErrorAsync(
                    "Browser is not installed",
                    "Please install browser first");
            }
            catch (UserFriendlyException ex)
            {
                ShowErrorDialog(ex.Title, ex.Message);
            }
        }

        private void ShowErrorDialog(string title, string text)
        {
            MesageBoxHelper.ShowErrorAsync(
                title,
                text);
        }
    }
}
