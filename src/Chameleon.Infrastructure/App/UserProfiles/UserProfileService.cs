using Chameleon.Common.Exceptions;
using Chameleon.Core.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.UpgradePlan;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserProfiles.Additional;
using Chameleon.Interfaces.WebBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.Profiles
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserProfilePersonRepository _personRepository;
        private readonly IUserProfileBusinessRepository _businessRepository;
        private readonly IUserProfileAddressRepository _addressRepository;
        private readonly IUserProfileLoginRepository _loginRepository;
        private readonly IWebBrowserUserAgentRepository _webBrowserUserAgentRepository;
        private readonly IUserProfileOutReachRssRepository _outReachRssRepository;
        private readonly IAuthSession _authSession;
        private readonly IDialogWindowsService _dialogWindowsService;
        private readonly IUpgradePlanPopupView _upgradePlanPopupView;
        private readonly IApplicationUser _applicationUser;
        private readonly IShareFoldersService _shareFoldersService;

        private Lazy<Domain.Entities.UserProfiles> _profiles;
        private Domain.Entities.UserProfiles Profiles => _profiles.Value;

        public UserProfileService(
            IUserProfileRepository repository,
            IUserProfilePersonRepository personRepository,
            IUserProfileBusinessRepository businessRepository,
            IUserProfileAddressRepository addressRepository,
            IUserProfileLoginRepository loginRepository,
            IWebBrowserUserAgentRepository webBrowserUserAgentRepository,
            IUserProfileOutReachRssRepository outReachRssRepository,
            IAuthSession authSession,
            IDialogWindowsService dialogWindowsService,
            IUpgradePlanPopupView upgradePlanPopupView,
            IApplicationUser applicationUser,
            IShareFoldersService shareFoldersService
            )
        {
            _userProfileRepository = repository;
            _personRepository = personRepository;
            _businessRepository = businessRepository;
            _addressRepository = addressRepository;
            _loginRepository = loginRepository;
            _webBrowserUserAgentRepository = webBrowserUserAgentRepository;
            _outReachRssRepository = outReachRssRepository;
            _authSession = authSession;
            _dialogWindowsService = dialogWindowsService;
            _upgradePlanPopupView = upgradePlanPopupView;
            _applicationUser = applicationUser;
            _shareFoldersService = shareFoldersService;

            InitProfiles();
        }

        private void InitProfiles()
        {
            _profiles = new Lazy<Domain.Entities.UserProfiles>(() => GetUserProfiles(true), true);
        }

        private Domain.Entities.UserProfiles GetUserProfiles(bool ignoreCache = false)
        {
            var entities = _userProfileRepository.GetAll(ignoreCache);

            if (_applicationUser.IsAssistant)
            {
                SetAssistantProfilePermissions(entities);
            }

            var response = new Domain.Entities.UserProfiles { entities };

            return response;
        }

        private void SetAssistantProfilePermissions(IUserProfile[] profiles)
        {
            foreach (var profile in profiles)
            {
                profile.PermissionNames = _shareFoldersService.GetAllProfilePermissionNames(_authSession.UserId, profile.Id, profile.FolderId);
            }
        }

        public IUserProfiles GetAll()
        {
            return Profiles;
        }

        public void Sync()
        {
            InitProfiles();
        }


        public IUserProfile Get(int Id, bool ignoreCache = false)
        {
            if (!ignoreCache && _profiles.IsValueCreated)
            {
                var profile = _profiles.Value.FirstOrDefault(p => p.Id == Id);
                if (profile != null) return profile;
            }

            try
            {
                var profile = _userProfileRepository.Get(Id);
                InitializeWebBrowserUserAgent(profile);
                return profile;
            }
            catch (Exception ex)
            {
                if (_applicationUser.IsAssistant && ex.Message == $"There is no entity Profile with id = {Id}!")
                {
                    throw new UserFriendlyException("The profile has been unshared or deleted. Please, sync profiles", "Profile is unavailable");
                }

                throw;
            }
        }

        public List<IUserProfile> GetAllByUserId(int id)
        {
            return _userProfileRepository.GetAllByUserId(id).ToList();
        }

        private void InitializeWebBrowserUserAgent(IUserProfile profile)
        {
            var userAgentId = profile.WebBrowser.UserAgentId;
            if (!userAgentId.HasValue)
            {
                return;
            }

            if (profile.WebBrowser.UserAgent != null)
            {
                return;
            }

            IWebBrowserUserAgent userAgent = null;
            this.TryCatchIgnore(() => 
            { 
                userAgent = _webBrowserUserAgentRepository.Get(userAgentId.Value); 
            });
            profile.WebBrowser.UserAgent = userAgent;

            //var userAgent = _webBrowserUserAgentRepository.Get(userAgentId.Value);
            //profile.WebBrowser.UserAgent = userAgent;
        }

        public IUserProfile Create(int? folderId)
        {
            var userProfiles = Profiles;
            var profile = new UserProfile
            {
                FolderId = folderId,
                Title = $"New Profile {userProfiles.Count + 1}"
            };

            _userProfileRepository.Insert(profile);
            InitializeWebBrowserUserAgent(profile);

            this.InvokeOnUiThread(() => Profiles.Add(profile));
            return profile;
        }

        public void Delete(IUserProfile userProfile)
        {
            _userProfileRepository.Delete(userProfile);
            this.InvokeOnUiThread(() => Profiles.Remove(userProfile));
        }

        public void Save(IUserProfile userProfile)
        {
            _userProfileRepository.InsertOrUpdate(userProfile);
        }

        public Task ImportAsync(IList<IUserProfile> userProfiles)
        {
            return Task.Run(() => Import(userProfiles));
        }

        public void Import(IList<IUserProfile> userProfiles)
        {
            foreach (var userProfile in userProfiles)
            {
                Import(userProfile);
            }
        }

        public void ShowOutOfLimitPopup()
        {
            Action<IUpgradePlanViewModel> initialize = viewModel =>
            {
                viewModel.LimitExceededText = "You have reached the maximum number of profiles.";
            };

            _dialogWindowsService.ShowDialogWindow(_upgradePlanPopupView, "PROFILES LIMIT REACHED", initialize);
        }

        public void Import(IUserProfile userProfile)
        {
            var profile = Profiles.GetByTitle(userProfile.Title);
            if (profile != null)
            {
                userProfile.Id = profile.Id;
            }
            else
            {
                _userProfileRepository.Insert(userProfile);
                this.InvokeOnUiThread(() => Profiles.Add(userProfile));
            }

            SetProfileId(userProfile, userProfile.Addresses);
            _addressRepository.Insert(userProfile.Addresses);

            SetProfileId(userProfile, userProfile.Businesses);
            _businessRepository.Insert(userProfile.Businesses);

            SetProfileId(userProfile, userProfile.Persons);
            _personRepository.Insert(userProfile.Persons);

            SetProfileId(userProfile, userProfile.Logins);
            _loginRepository.Insert(userProfile.Logins);

            SetProfileId(userProfile, userProfile.OutReachRsses);
            _outReachRssRepository.Insert(userProfile.OutReachRsses);
        }

        private void EnsureProfileHasUniqueName(IUserProfile userProfile)
        {
            while (Profiles.HasWithTitle(userProfile.Title))
            {
                userProfile.Title += "_1";
            }
        }

        private void SetProfileId<T>(IUserProfile userProfile, IList<T> items)
            where T : IUserProfileRequiredEntity
        {
            foreach (var item in items)
            {
                item.ProfileId = userProfile.Id;
            }
        }

        public IReadOnlyList<IUserProfile> Search(IUserProfileSearchRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            IEnumerable<IUserProfile> profiles = Profiles.AsEnumerable();
            if (request.ExcludeFolderId.HasValue)
            {
                profiles = profiles
                    .Where(profile => profile.FolderId != request.ExcludeFolderId.Value);
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                profiles = profiles
                    .Where(profile => profile.Title.IndexOf(request.Keyword, StringComparison.InvariantCultureIgnoreCase) != -1);
            }

            return profiles
                .ToList()
                .AsReadOnly();
        }

        public void SetProfileIsFavorite(int profileId, bool isFavorite)
        {
            try
            {
                _userProfileRepository.SetProfileIsFavorite(profileId, isFavorite);
            }
            catch (Exception ex)
            {
                if (_applicationUser.IsAssistant && ex.Message == $"There is no entity Profile with id = {profileId}!")
                {
                    throw new UserFriendlyException("The profile has been unshared or deleted. Please, sync profiles", "Profile is unavailable");
                }

                throw;
            }
        }

        public void SetProfileCacheLimit(int profileId, double limitCache)
        {
            _userProfileRepository.SetProfileCacheLimit(profileId, limitCache);
        }

        public void MoveUserProfileToFolder(List<int> profileIds, int? folderId)
        {
            _userProfileRepository.MoveUserProfileToFolder(profileIds, folderId);
        }

        public bool IsSharedProfile(IUserProfile profile) => 
            profile?.CreatorUserId != _authSession?.UserId && profile?.CreatorUserId != null;
    }
}
