using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.Repository;

namespace Chameleon.Infrastructure.Users
{
    public interface IUserAssistantRepository
        : IRepository<IUserAssistant, long, GetAllRequestDto>
    {
        void AddProfiles(IUserAssistant userAssistant);
        void SetCanCreateProfiles(long assistantId, bool canCreateProfiles);
    }
}
