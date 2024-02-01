using Chameleon.Interfaces.Repository;
using System.Collections.Generic;

namespace Chameleon.Interfaces.Assistants
{
    public interface IAssistantProfileRepository 
        : IRepository<IAssistantProfile, long, GetAllRequestDto>
    {
        IList<IAssistantProfile> GetAllAssistantProfilesById(long userAssistantId);
        void DeleteAssistantProfile(IAssistantProfile assistantProfile);
        void ShareUserProfile(int profileId, IList<long> assistantUserIds, IList<string> permissionNames);
    }
}
