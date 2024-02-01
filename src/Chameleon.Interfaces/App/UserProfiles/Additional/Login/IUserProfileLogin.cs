namespace Chameleon.Interfaces.UserProfiles.Additional
{
    public interface IUserProfileLogin 
        : IUserProfileRequiredEntity
    {
        string Title { get; set; }
        string WebSite { get; set; }
        string Email { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string Notes { get; set; }
    }
}
