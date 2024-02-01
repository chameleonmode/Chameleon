using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Users.PrimaryUser.Api.Dto;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.App.Users.PrimaryUser.Api
{
    public class PrimaryUserApi
        : ApiLayer<PrimaryUserDto>
        , IPrimaryUserApi
    {
        public PrimaryUserApi(
            IApiClient apiClient
            ) : base(apiClient, "PrimaryUser")
        {
        }

        public void MarkGuidedTourDone()
        {
            var dto = new PrimaryUserDto();

            _apiClient.Post(GetEndpointUrl($"MarkGuidedTourDone"), dto);
        }
    }
}
