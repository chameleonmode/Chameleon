using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Chameleon.App
{
    public interface IProfileAppService 
        : IAsyncCrudAppService<
            ProfileDto, 
            int,
            PagedAndSortedResultRequestDto, 
            CreateProfileDto, 
            UpdateProfileDto
            >
    {
        void MoveUserProfileToFolder(MoveUserProfileFromFolderDto input);
    }
}
