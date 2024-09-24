using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.Prospector.Api.Dto;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.Prospector.Api
{
    public interface IUserProfileApiProspector
        : IApiLayer<
            UserProfileProspectorBlogsOfInterestDto
            , int
            , CreateUserProfileProspectorBlogsOfInterestDto
            , UserProfileProspectorBlogsOfInterestDto
            >
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
