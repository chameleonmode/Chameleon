using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions.Api.Dto;
using Chameleon.Interfaces.Api;
using Chameleon.Interfaces.App.Assistants;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions.Api
{
    public class AssistantProfilePermissionApi
        : ApiLayer<AssistantProfilePermissionDto, int, CreateAssistantProfilePermissionDto, AssistantProfilePermissionDto>
        , IAssistantProfilePermissionApi
    {
        public AssistantProfilePermissionApi(
            IApiClient apiClient
            ) : base(apiClient, "AssistantUser")
        {
        }

        public IList<AssistantProfilePermissionDto> GetAllProfilePermissions(long assistantId, int profileId)
        {
            return _apiClient.Get<AssistantProfilePermissionDto[]>(GetEndpointUrl($"GetAllProfilePermissions?assistantId={assistantId}&profileId={profileId}"));
        }

        public void InsertProfilePermission(IAssistantProfilePermission assistantProfilePermission)
        {
            _apiClient.Post(GetEndpointUrl("InsertProfilePermission"), assistantProfilePermission);
        }

        public void DeleteProfilePermission(long profileAssistantId, int profilePermissionId)
        {
            _apiClient.Delete(GetEndpointUrl($"DeleteProfilePermission?profileAssistantId={profileAssistantId}&profilePermissionId={profilePermissionId}"));
        }
    }
}
