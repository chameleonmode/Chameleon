using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions.Api.Dto
{
    public class AssistantProfilePermissionDto 
        : IEntityDto
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }
        public bool IsGranted { get; set; }
        public long ProfileAssistantId { get; set; }
    }
}