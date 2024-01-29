using Chameleon.Interfaces.App.Prospector;

namespace Chameleon.Domain.Entities
{
    public class UserProfileProspectorBlogsOfInterest
        : IUserProfileProspectorBlogsOfInterest
    {
        public ProspectorBlogsOfInterestTypes Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int ProfileId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
