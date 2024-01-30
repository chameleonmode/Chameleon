namespace Chameleon.Interfaces.Auth
{
    public interface IAuthPermissions
    {
        string[] Permissions { get; set; }
        bool CanCreateProfiles { get; set; }
    }
}
