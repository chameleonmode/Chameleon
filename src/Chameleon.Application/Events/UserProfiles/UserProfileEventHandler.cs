using Chameleon.Common.Exceptions;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.Common.Helpers;

namespace Chameleon.Application.Events
{
    public class UserProfileEventHandler : IUserProfileEventHandler
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserProfileService _userProfileService;
        private readonly ISystemBrowserManager _systemBrowserManager;
        public UserProfileEventHandler(
            IUserProfileService userProfileService,
            IEventAggregator eventAggregator,
            ISystemBrowserManager systemBrowserManager
            )
        {
            _eventAggregator = eventAggregator;
            _userProfileService = userProfileService;
            _systemBrowserManager = systemBrowserManager;

            _eventAggregator
                .GetEvent<DeleteUserProfileEvent>()
                .Subscribe(args => DeleteUserProfileEvent(args.UserProfile));

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

        private IUserProfile GetProfile(IUserProfileInfo profileInfo)
        {
            return _userProfileService.Get(profileInfo.Id, true);
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

        private static void ShowErrorDialog(string title, string text)
        {
            MesageBoxHelper.ShowErrorAsync(
                title,
                text);
        }
    }
}
