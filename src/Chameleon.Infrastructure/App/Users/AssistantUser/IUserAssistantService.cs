using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.Assistants;
using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.Users
{
    public interface IUserAssistantService
        : ISingletonDependency
    {
        ICollection<IUserAssistant> Get();
        Task<ICollection<IUserAssistant>> GetAsync();
        void Save(IUserAssistant userAssistant);
        void DeleteAssistant(IUserAssistant userAssistant);
        IList<IAssistantProfile> GetAllAssistantProfilesById(long assistantId);
        void DeleteAssistantProfile(IAssistantProfile assistantProfile);
        IList<IAssistantProfilePermission> GetAllProfilePermissions(long assistantId, int profileId);
        void UpdateProfilePermission(IAssistantProfilePermission assistantProfilePermission);
        void ShareUserProfile(int profileId, IList<long> assistantUserIds, IList<string> permissionNames);
        void AddProfiles(IUserAssistant userAssistant);
        void SetCanCreateProfiles(long assistantId, bool canCreateProfiles);
    }
}
