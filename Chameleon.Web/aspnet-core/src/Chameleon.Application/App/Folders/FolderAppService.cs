using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using Chameleon.App.Entities.ShareFolders;
using Chameleon.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class FolderAppService
        : AsyncCrudAppService<
            Folder,
            FolderDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateFolderDto,
            UpdateFolderDto
            >
        , IFolderAppService
    {
        private readonly IRepository<Profile> _profileRepository;
        private readonly ISessionAppService _sessionAppService;
        private readonly IRepository<UserFolder> _userFolderRepository;

        public FolderAppService(
            IRepository<Folder> repository,
            IRepository<Profile> profileRepository,
            ISessionAppService sessionAppService,
            IRepository<UserFolder> userFolderRepository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;

            _profileRepository = profileRepository;
            _sessionAppService = sessionAppService;
            _userFolderRepository = userFolderRepository;
        }

        protected override IQueryable<Folder> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return CreateQuery();
        }

        protected override Task<Folder> GetEntityByIdAsync(int id)
        {
            return Task.Run(() => CreateQuery()
                .FirstOrDefault(profile => profile.Id == id)
                );
        }

        private IQueryable<Folder> CreateQuery()
        {
            var loginInfo = _sessionAppService.GetCurrentLoginInformationsAsync();
            loginInfo.Wait();

            var query = Repository.GetAllIncluding(
                entity => entity.Profiles
                );

            if (loginInfo.Result.User.IsAssistant)
            {
                var folderIds = _userFolderRepository
                    .GetAll()
                    .FilterByUserId(AbpSession.UserId.Value)
                    .Select(a => a.FolderId)
                    .ToList();

               return query.Where(a => folderIds.Contains(a.Id));
            }

            return query.FilterByCreatorUserId(AbpSession);
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            await base.DeleteAsync(input);

            var profiles = _profileRepository
                .GetAll()
                .Where(entity => entity.FolderId == input.Id)
                .ToList();

            foreach (var profile in profiles)
            {
                profile.FolderId = null;
                await _profileRepository.UpdateAsync(profile);
            }
        }

        public async Task AddProfile(FolderProfileDto input)
        {
            var folder = GetById(input.Id);
            var profile = folder.FindProfileOrNull(input.ProfileId);
            if (profile != null)
            {
                throw new Exception($"Profile Id={input.ProfileId} already added");
            }
            folder.Profiles.Add(profile);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task RemoveProfile(FolderProfileDto input)
        {
            var folder = GetById(input.Id);
            var profile = folder.FindProfileOrNull(input.ProfileId);
            if (profile == null)
            {
                throw new KeyNotFoundException($"Profile Id={input.ProfileId} not found");
            }
            folder.Profiles.Remove(profile);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private Folder GetById(int folderId)
        {
            var folder = Repository
                .GetAllIncluding(folder => folder.Profiles)
                .FirstOrDefault(folder => folder.Id == folderId);
            if (folder == null)
            {
                throw new KeyNotFoundException($"Folder Id={folderId} not found");
            }
            return folder;
        }

        protected override IQueryable<Folder> ApplySorting(IQueryable<Folder> query, PagedAndSortedResultRequestDto input)
        {
            query = base.ApplySorting(query, input);
            if (input.Sorting.IsNullOrEmpty())
            {
                query = query.OrderBy(entity => entity.CreationTime);
            }
            return query;
        }

        public override async Task<PagedResultDto<FolderDto>> GetAllAsync(PagedAndSortedResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            return result;
        }
    }
}
