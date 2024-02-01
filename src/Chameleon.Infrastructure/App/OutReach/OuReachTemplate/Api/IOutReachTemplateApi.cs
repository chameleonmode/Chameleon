using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.OutReach.Api.Dto;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.OutReach.Api
{
    public interface IOutReachTemplateApi
        : IApiLayer<OutReachTemplateDto
            , int
            , CreateOutReachTemplateDto
            , OutReachTemplateDto>
        , ISingletonDependency
    {
    }
}
