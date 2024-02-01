using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.Users
{
    public class UserAssistantApi
        : ApiLayer<UserAssistantDto, long, CreateUserAssistantDto, UserAssistantDto>
        , IUserAssistantApi
    {
        public UserAssistantApi(
            IApiClient apiClient
            ) : base(apiClient, "AssistantUser")
        {
        }

        public void AddProfiles(UserAssistantDto input)
        {
            _apiClient.Post(GetEndpointUrl("AddProfiles"), input);
        }
        public void SetCanCreateProfiles(long assistantId, bool canCreateProfiles) 
        {
            _apiClient.Post(GetEndpointUrl($"SetCanCreateProfiles?assistantId={assistantId}&canCreateProfiles={canCreateProfiles}"), "");
        }
    }
}
