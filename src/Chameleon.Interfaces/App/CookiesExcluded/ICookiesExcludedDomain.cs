using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.CookiesExcluded
{
    public interface ICookiesExcludedDomain
        : IUserProfileRequiredEntity
    {
        string Domain { get; set; }
    }
}
