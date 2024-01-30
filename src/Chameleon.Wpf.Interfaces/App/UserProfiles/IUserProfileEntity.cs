using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfileRequiredEntity
        : IEntity
    {
        int ProfileId { get; set; }
    }

    public interface IUserProfileOptionalEntity
        : IEntity
    {
        int? ProfileId { get; set; }
    }
}
