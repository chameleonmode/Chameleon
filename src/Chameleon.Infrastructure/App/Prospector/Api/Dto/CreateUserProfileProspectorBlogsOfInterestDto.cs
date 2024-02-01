using Chameleon.Interfaces.App.Prospector;

namespace Chameleon.Infrastructure.Prospector.Api.Dto
{
    public class CreateUserProfileProspectorBlogsOfInterestDto
    {
        public ProspectorBlogsOfInterestTypes Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int ProfileId { get; set; }
    }
}
