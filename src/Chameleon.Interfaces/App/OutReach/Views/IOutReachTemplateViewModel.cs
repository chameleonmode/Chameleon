using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.OutReach.Views;

public interface IOutReachTemplateViewModel
    : ISingletonDependency
{
    IOutReachTemplate OutReachTemplate { get; set; }
    IUserProfile UserProfile { get; set; }
    int Id { get; set; }
    string UserProfileTitle { get; set; }
}
