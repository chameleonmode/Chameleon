using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.Prospector.Api.Dto;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.Prospector.Api
{
    public class UserProfileApiProspector
        : ApiLayer<
            UserProfileProspectorBlogsOfInterestDto
            , int
            , CreateUserProfileProspectorBlogsOfInterestDto
            , UserProfileProspectorBlogsOfInterestDto
            >
        , IUserProfileApiProspector
    {
        public UserProfileApiProspector(IApiClient apiClient)
            : base(apiClient, "ProspectorBlogsOfInterest")
        { }
    }
}
