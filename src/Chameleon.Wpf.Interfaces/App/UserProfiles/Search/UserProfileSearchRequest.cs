namespace Chameleon.Interfaces.UserProfiles
{
    public class UserProfileSearchRequest : IUserProfileSearchRequest
    {
        public int? ExcludeFolderId { get; set; }
        public string Keyword { get; set; }
    }
}
