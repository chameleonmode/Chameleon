using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfileInfo : IEntity
    {
        string Notes { get; set; }
        bool IsFavourite { get; set; }
        string? Title { get; set; }
        int? FolderId { get; set; }
        long? CreatorUserId { get; set; }
        double? LimitCache { get; set; }
        string[] PermissionNames { get; set; }
    }
}
