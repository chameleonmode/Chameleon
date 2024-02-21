using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IProspectorBlogsOfInterestAppService
         : IAsyncCrudAppService<
            ProspectorBlogsOfInterestDto,
            int,
            ProspectorBlogsOfInterestGetAllRequestDto,
            CreateProspectorBlogsOfInterestDto,
            UpdateProspectorBlogsOfInterestDto
            >
    {
    }
}
