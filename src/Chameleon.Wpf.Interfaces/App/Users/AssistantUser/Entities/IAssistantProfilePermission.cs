using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.App.Assistants
{
    public interface IAssistantProfilePermission 
        : IEntity
    {
        string PermissionName { get; set; }
        string DisplayName { get; set; }
        bool IsGranted { get; set; }
        long ProfileAssistantId { get; set; }
    }
}
