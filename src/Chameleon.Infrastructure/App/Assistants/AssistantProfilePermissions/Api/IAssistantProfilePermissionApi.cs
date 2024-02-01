using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions.Api.Dto;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions.Api
{
    public interface IAssistantProfilePermissionApi
       : IApiLayer<AssistantProfilePermissionDto, int, CreateAssistantProfilePermissionDto, AssistantProfilePermissionDto>
       , ISingletonDependency
    {
        IList<AssistantProfilePermissionDto> GetAllProfilePermissions(long assistantId, int profileId);
        void InsertProfilePermission(IAssistantProfilePermission assistantProfilePermission);
        void DeleteProfilePermission(long profileAssistantId, int profilePermissionId);
    }
}
