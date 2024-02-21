using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface IFolderAppService
        : IAsyncCrudAppService<
            FolderDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateFolderDto,
            UpdateFolderDto
            >
    {
        Task AddProfile(FolderProfileDto input);
        Task RemoveProfile(FolderProfileDto input);
    }
}
