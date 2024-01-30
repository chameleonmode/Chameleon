namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfileAccessor
        : IUserProfileSetter
        , IUserProfileGetter
    {
        new IUserProfile UserProfile { get; set; }
    }
}
