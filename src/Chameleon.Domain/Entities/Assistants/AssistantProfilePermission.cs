using Chameleon.Interfaces.App.Assistants;

namespace Chameleon.Domain.Entities.Assistants
{
    public class AssistantProfilePermission 
        : IAssistantProfilePermission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }
        public bool IsGranted { get; set; }
        public long ProfileAssistantId { get; set; }
    }
}
