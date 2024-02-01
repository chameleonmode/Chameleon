namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfileSearchRequest
    {
        int? ExcludeFolderId { get; }
        string Keyword { get; }
    }
}
