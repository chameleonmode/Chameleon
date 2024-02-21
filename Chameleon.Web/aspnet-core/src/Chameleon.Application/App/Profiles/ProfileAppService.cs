using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Chameleon.App.Entities;
using Chameleon.App.ShareFolders;
using Chameleon.Authorization;
using Chameleon.Authorization.Users;
using Chameleon.Sessions;
using Chameleon.Sessions.Dto;
using Chameleon.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Threading;
using System.Threading.Tasks;
using License = Chameleon.App.Entities.License;
using System.Linq.Dynamic.Core;
using Chameleon.App.Services.License_Key;
using Chameleon.App.Services.License_Key.Dto;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class ProfileAppService
        : AsyncCrudAppService<
            Profile,
            ProfileDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateProfileDto,
            UpdateProfileDto
            >
        , IProfileAppService
    {
        private static readonly SemaphoreSlim _syncRoot = new(1, 1);

        private readonly IRepository<Proxy> _proxyRepository;
        private readonly IWebBrowserSettingsManager _webBrowserSettingManager;
        private readonly IRepository<ProspectorBlogsOfInterest> _prospectorRepository;
        private readonly IRepository<Profile> _profileRepository;
        private readonly IRepository<ProfileAssistant, long> _profileAssistantRepository;
        private readonly ISessionAppService _sessionAppService;
        private readonly IShareFoldersAppService _shareFoldersAppService;
        private readonly IRepository<License> _licenseRepository;
        private readonly IRepository<AssistantLicense> _assistantLicenseRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly UserManager _userManager;
        private readonly ILicenseKeyService _licenseKeyService;

        public ProfileAppService(
            IRepository<ProspectorBlogsOfInterest> prospectorRepository,
            IRepository<Profile> repository,
            IRepository<Proxy> proxyRepository,
            IWebBrowserSettingsManager webBrowserSettingManager,
            IRepository<Profile> profileRepository,
            IRepository<ProfileAssistant, long> profileAssistantRepository,
            ISessionAppService sessionAppService,
            IShareFoldersAppService shareFoldersAppService,
            IRepository<License> licenseRepository,
            IRepository<AssistantLicense> assistantLicenseRepository,
            IRepository<User, long> userRepository,
            UserManager userManager,
            ILicenseKeyService licenseKeyService
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;

            _proxyRepository = proxyRepository;
            _webBrowserSettingManager = webBrowserSettingManager;
            _prospectorRepository = prospectorRepository;
            _profileRepository = profileRepository;
            _profileAssistantRepository = profileAssistantRepository;
            _sessionAppService = sessionAppService;
            _shareFoldersAppService = shareFoldersAppService;
            _licenseRepository = licenseRepository;
            _assistantLicenseRepository = assistantLicenseRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _licenseKeyService = licenseKeyService;
        }

        protected override IQueryable<Profile> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return CreateQuery()
                .OrderBy(entity => entity.CreationTime);
        }

        protected override Task<Profile> GetEntityByIdAsync(int id)
        {
            return Task.Run(() => CreateQuery()
                .FirstOrDefault(profile => profile.Id == id)
                );
        }

        [AbpAuthorize(PermissionNames.Pages_CreateProfiles)]
        private IQueryable<Profile> CreateQuery()
        {
            var userId = AbpSession.UserId.Value;
            var loginInfo = _sessionAppService.GetCurrentLoginInformationsAsync();
            loginInfo.Wait();

            var query = Repository.GetAllIncluding(
                entity => entity.Proxy,
                entity => entity.WebBrowserSetting
            );

            if (loginInfo.Result.User.IsAssistant) 
            {
                var profileIds = _profileAssistantRepository
                    .GetAll()
                    .FilterByUserId(userId)
                    .Select(a => a.ProfileId)
                    .ToList();

                profileIds.AddRange(_shareFoldersAppService.GetAllProfileIdsFromSharedFolder(userId));
                profileIds = profileIds
                    .Distinct()
                    .ToList();

                query = query.Where(a => a.CreatorUserId == userId || profileIds.Contains(a.Id));
            }
            else query = query.FilterByCreatorUserId(AbpSession);

            return query;
        }

        public IList<Profile> GetAllByUserId(long id)
        {
            var query = Repository.GetAll();

            query = query.FilterByCreatorUserId(id);

            return query.ToList();
        }

        public override async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
        {
            await _syncRoot.WaitAsync();
            try
            {
                var loginInfo = await _sessionAppService.GetCurrentLoginInformationsAsync();
                var profile = await GetEntityByIdAsync(input.Id);

                if (profile == null)
                    throw new EntityNotFoundException(typeof(Profile), input.Id);
                if (profile.CreatorUserId != loginInfo.User.Id && profile.CreatorUserId != loginInfo.User.CreatorUserId)
                    throw new UserFriendlyException("You have no permission to update this profile.");

                var proxyId = profile.ProxyId;
                input.ProxyId = proxyId;

                var dto = await base.UpdateAsync(input);

                await UpdateProxyAsync(proxyId, input.Proxy);
                return dto;
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        private async Task UpdateProxyAsync(int proxyId, ProxyBaseDto proxyDto)
        {
            var proxy = await _proxyRepository.GetAsync(proxyId);

            proxy.Host = proxyDto.Host;
            proxy.Port = proxyDto.Port;
            proxy.UserName = proxyDto.UserName;
            proxy.Password = proxyDto.Password;

            await _proxyRepository.UpdateAsync(proxy);
        }
        private async Task<bool> ProfileCreatePermissionCheck(GetCurrentLoginInformationsOutput loginInfo)
        {
            if (loginInfo.User.CreatorUserId != null)
            {
                var assistLicense = await _assistantLicenseRepository.FirstOrDefaultAsync(al => al.UserId == AbpSession.UserId);
                if (assistLicense == null || !assistLicense.CanCreateProfiles)
                    return false;
            }

            return true;
        }
        private async Task<bool> ProfileLimitCheck(GetCurrentLoginInformationsOutput loginInfo) 
        {
            var primaryUserId = loginInfo.User.CreatorUserId ?? loginInfo.User.Id;

            var license = await _licenseRepository.FirstOrDefaultAsync(l => l.UserId == primaryUserId && l.IsActive);
            if (license == null) throw new UserFriendlyException("No license were found for this user.");

            var licenseType = await _licenseKeyService.GetTypeAsync(license.LicenseKey);
            var licenseLimits = new LicenseLimits(licenseType);

            int profilesAmount = await _profileRepository
                .CountAsync(p => p.CreatorUserId == loginInfo.User.Id && !p.IsDeleted);

            return profilesAmount < licenseLimits.MaxProfilesCount;
        }

        [AbpAuthorize(PermissionNames.Pages_CreateProfiles)]
        public override async Task<ProfileDto> CreateAsync(CreateProfileDto input)
        {
            var loginInfo = await _sessionAppService.GetCurrentLoginInformationsAsync();
            if (!await ProfileCreatePermissionCheck(loginInfo))
                throw new UserFriendlyException("This assistant doesn't have permission to create profiles.");

            await _syncRoot.WaitAsync();

            try
            {
                if (!await ProfileLimitCheck(loginInfo))
                    throw new UserFriendlyException("limit_ex", "Limit of allowed amount of profiles were exeeded.");

                var profile = await base.CreateAsync(input);

                var defaultInterests = DefaultBlogOfInterestValues.GetDefaultValues();

                foreach (var item in defaultInterests)
                {
                    item.ProfileId = profile.Id;
                    _prospectorRepository.Insert(item);
                }

                return profile;
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        [AbpAuthorize(PermissionNames.Pages_DeleteProfiles)]
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            var profile = await Repository.GetAsync(input.Id);
            if (profile?.CreatorUserId != AbpSession.UserId)
                throw new UserFriendlyException("You can't delete profile that is not yours.");
            
            await Repository.DeleteAsync(profile.Id);
        }

        protected override Profile MapToEntity(CreateProfileDto createInput)
        {
            var profile = base.MapToEntity(createInput);

            profile.ProxyId = _proxyRepository
                .InsertAndGetId(profile.Proxy);

            profile.WebBrowserSettingId = _webBrowserSettingManager
                .Insert(profile.WebBrowserSetting);

            return profile;
        }

        protected override IQueryable<Profile> ApplySorting(IQueryable<Profile> query, PagedAndSortedResultRequestDto input)
        {
            query = base.ApplySorting(query, input);
            if (input.Sorting.IsNullOrEmpty())
            {
                query = query.OrderBy(entity => entity.CreationTime);
            }
            return query;
        }

        public void MoveUserProfileToFolder(MoveUserProfileFromFolderDto input)
        {
            var profiles = Repository
                .GetAll()
                .Where(profile => input.ProfileIds.Contains(profile.Id))
                .ToList();

            profiles.ForEach(a =>
            {
                a.FolderId = input.FolderId;
                Repository.Update(a);
            });
        }

        public async Task SetProfileIsFavorite(FavoriteProfileDto input)
        {
            var profile = await GetEntityByIdAsync(input.ProfileId);

            if (profile == null)
            {
                throw new EntityNotFoundException(typeof(Profile), input.ProfileId);
            }

            profile.IsFavourite = input.IsFavorite;
            Repository.Update(profile);
        }

        public async Task SetProfileCacheLimit(ProfileCacheLimitDto input)
        {
            var profile = await GetEntityByIdAsync(input.ProfileId);

            if (profile == null)
            {
                throw new EntityNotFoundException(typeof(Profile), input.ProfileId);
            }

            profile.LimitCache = input.LimitCache;
            Repository.Update(profile);
        }
    }

    public static class DefaultBlogOfInterestValues
    {
        public static List<ProspectorBlogsOfInterest> GetDefaultValues()
        {
            var defaultValues
               = new List<ProspectorBlogsOfInterest>
               {
                    new ProspectorBlogsOfInterest
                    {
                        Name = "Comment Luv Premium",
                        Value = ",\"This blog uses premium CommentLuv\" - \"The version of CommentLuv on this site is no longer supported.\"",
                        Type = ProspectorBlogsOfInterestTypes.CommentBacklinks
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Do-follow comments",
                        Value = "\"Notify me of follow-up comments?\"+\"Submit the word you see below:\"",
                        Type = ProspectorBlogsOfInterestTypes.CommentBacklinks
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Intense Debate",
                        Value = "\"if you have a website, link to it here\" \"post a new comment\"",
                        Type = ProspectorBlogsOfInterestTypes.CommentBacklinks
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "KeywordLuv",
                        Value = "\"Enter YourName@YourKeywords\"",
                        Type = ProspectorBlogsOfInterestTypes.CommentBacklinks
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Livefyre",
                        Value = "\"get livefyre\" \"comment help\" -\"Comments have been disabled for this post\"",
                        Type = ProspectorBlogsOfInterestTypes.CommentBacklinks
                    },


                    new ProspectorBlogsOfInterest
                    {
                        Name = "ip.board",
                        Value = "\"powered by ip.board\"",
                        Type = ProspectorBlogsOfInterestTypes.Forum
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Fireboard",
                        Value = "\"powered by Fireboard\"",
                        Type = ProspectorBlogsOfInterestTypes.Forum
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "phpbb",
                        Value = "\"powered by phpbb\"",
                        Type = ProspectorBlogsOfInterestTypes.Forum
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "phpbb3",
                        Value = "\"powered by phpbb3\"",
                        Type = ProspectorBlogsOfInterestTypes.Forum
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "SMF",
                        Value = "\"powered by SMF\"",
                        Type = ProspectorBlogsOfInterestTypes.Forum
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Vbulletin",
                        Value = "\"powered by vbulletin\"",
                        Type = ProspectorBlogsOfInterestTypes.Forum
                    },


                    new ProspectorBlogsOfInterest
                    {
                        Name = "accepting guest posts",
                        Value = "\"accepting guest posts\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "become a contributor",
                        Value = "\"become a contributor\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "become a guest writer",
                        Value = "\"become a guest writer\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "contribute to our site",
                        Value = "\"contribute to our site\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "contributor guidelines",
                        Value = "\"contributor guidelines\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "guest bloggers wanted",
                        Value = "\"guest bloggers wanted\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "guest post courtesy of",
                        Value = "\"guest post courtesy of\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "guest post guidelines",
                        Value = "\"guest post guidelines\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "guest post opportunities",
                        Value = "\"guest post opportunities\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "guest posts wanted",
                        Value = "\"guest posts wanted\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "I\'ve been featured on",
                        Value = "\"I\'ve been featured on\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "my guest blogs",
                        Value = "\"my guest blogs\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "my posts on other blogs",
                        Value = "\"my posts on other blogs\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "sites I\'ve written for",
                        Value = "\"sites I\'ve written for\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "submit article",
                        Value = "\"submit article\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "submit blog post",
                        Value = "\"submit blog post\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "submit guest post",
                        Value = "\"submit guest post\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "submit your content",
                        Value = "\"submit your content\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "this is a guest post by",
                        Value = "\"this is a guest post by\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "this post was written by",
                        Value = "\"this post was written by\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "write for us",
                        Value = "\"write for us\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "writers wanted",
                        Value = "\"writers wanted\"",
                        Type = ProspectorBlogsOfInterestTypes.GuestPosts
                    },


                    new ProspectorBlogsOfInterest
                    {
                        Name = "BlogEngine.NET",
                        Value = "\"Powered by BlogEngine.NET\" inurl:blog \"post a comment\" -\"comments closed\" -\"you must be logged in\"",
                        Type = ProspectorBlogsOfInterestTypes.Blog
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Expression Engine 1",
                        Value = "\"powered by expressionengine\" inurl:blog \"post a comment\" -\"comments closed\" -\"you must be logged in\"",
                        Type = ProspectorBlogsOfInterestTypes.Blog
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Expression Engine 2",
                        Value = "\"powered by expressionengine\" \"post a comment\"",
                        Type = ProspectorBlogsOfInterestTypes.Blog
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Expression Engine 3",
                        Value = "\"powered by expressionengine\"",
                        Type = ProspectorBlogsOfInterestTypes.Blog
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Typepad",
                        Value = "\"powered by Typepad\"",
                        Type = ProspectorBlogsOfInterestTypes.Blog
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Wordpress (no comments) 1",
                        Value = "\"no comments posted yet\"",
                        Type = ProspectorBlogsOfInterestTypes.Blog
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "Wordpress (no comments) 2",
                        Value = "\"why not be the first to have your say\"",
                        Type = ProspectorBlogsOfInterestTypes.Blog
                    },


                    new ProspectorBlogsOfInterest
                    {
                        Name = "best articles of the week",
                        Value = "\"best articles of the week\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "best of",
                        Value = "\"best of\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "best posts of the week",
                        Value = "\"best posts of the week\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "daily link roundup",
                        Value = "\"daily link roundup\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "friday link roundup",
                        Value = "\"friday link roundup\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "link roundup",
                        Value = "\"link roundup\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "monday link roundup",
                        Value = "\"monday link roundup\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "roundup",
                        Value = "\"roundup\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "this week",
                        Value = "\"this week\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "top posts this week",
                        Value = "\"top posts this week\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    },

                    new ProspectorBlogsOfInterest
                    {
                        Name = "weekly link roundup",
                        Value = "\"weekly link roundup\"",
                        Type = ProspectorBlogsOfInterestTypes.LinkRoundups
                    }
               };

            return defaultValues;
        }
    }
}
