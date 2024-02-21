using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IOutReachTemplateAppService
        : IAsyncCrudAppService<
            OutReachTemplateDto,
            int,
            OutReachTemplateGetAllRequestDto,
            CreateOutReachTemplateDto,
            UpdateOutReachTemplateDto
            >
    {
    }
}
