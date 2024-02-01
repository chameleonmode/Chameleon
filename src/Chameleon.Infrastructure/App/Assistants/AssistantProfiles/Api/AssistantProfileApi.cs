using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api.Dto;
using Chameleon.Interfaces.Api;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api
{
    public class AssistantProfileApi
        : ApiLayer<AssistantProfileDto, long, CreateAssistantProfileDto, AssistantProfileDto>
        , IAssistantProfileApi
    {
        public AssistantProfileApi(
            IApiClient apiClient
            ) : base(apiClient, "AssistantUser")
        {
        }

        public void DeleteAssistantProfile(long assistantId, int profileId)
        {
            _apiClient.Delete(GetEndpointUrl($"DeleteAssistantProfile?assistantId={assistantId}&profileId={profileId}"));
        }

        public IList<AssistantProfileDto> GetAllAssistantProfilesById(long assistantId)
        {
            return _apiClient.Get<AssistantProfileDto[]>(GetEndpointUrl($"GetAllAssistantProfilesById?assistantId={assistantId}"));
        }

        public void ShareUserProfile(CreateAssistantProfileDto input)
        {
            _apiClient.Post(GetEndpointUrl("ShareUserProfile"), input);
        }
    }
}
