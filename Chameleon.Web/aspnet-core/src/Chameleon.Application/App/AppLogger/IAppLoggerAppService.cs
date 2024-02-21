using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface IAppLoggerAppService
        : IAsyncCrudAppService<
            AppLoggerDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateAppLoggerDto,
            UpdateAppLoggerDto
            >
    {
        Task RemoveAll();
    }
}
