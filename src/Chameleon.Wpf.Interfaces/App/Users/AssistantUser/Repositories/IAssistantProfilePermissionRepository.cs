using System.Collections.Generic;

namespace Chameleon.Interfaces.App.Assistants
{
    public interface IAssistantProfilePermissionRepository
    {
        IList<IAssistantProfilePermission> GetAllProfilePermissions(long assistantId, int profileId);
        void InsertProfilePermission(IAssistantProfilePermission assistantProfilePermission);
        void DeleteProfilePermission(IAssistantProfilePermission assistantProfilePermission);
    }
}
