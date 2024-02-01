using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api.Dto;
using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api
{
    public interface IAssistantProfileApi
       : IApiLayer<AssistantProfileDto, long, CreateAssistantProfileDto, AssistantProfileDto>
       , ISingletonDependency
    {
        IList<AssistantProfileDto> GetAllAssistantProfilesById(long id);
        void DeleteAssistantProfile(long assistantId, int profileId);
        void ShareUserProfile(CreateAssistantProfileDto input);
    }
}
