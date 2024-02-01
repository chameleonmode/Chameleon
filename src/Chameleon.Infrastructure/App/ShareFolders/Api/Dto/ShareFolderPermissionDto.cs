namespace Chameleon.Infrastructure.App.ShareFolders.Api.Dto
{
    public class ShareFolderPermissionDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }
        public bool IsGranted { get; set; }
    }
}
