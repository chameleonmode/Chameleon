using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.Prospector
{
    public interface IUserProfileProspectorBlogsOfInterest
       : IUserProfileRequiredEntity
    {
        ProspectorBlogsOfInterestTypes Type { get; set; }
        string Name { get; set; }
        string Value { get; set; }
    }
}
