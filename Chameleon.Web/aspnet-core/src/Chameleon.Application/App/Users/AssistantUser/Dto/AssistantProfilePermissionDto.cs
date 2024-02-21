namespace Chameleon.App.Users.AssistantUser.Dto
{
    public class AssistantProfilePermissionDto
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }
        public bool IsGranted { get; set; }
        public long ProfileAssistantId { get; set; }
    }
}
