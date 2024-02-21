using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class ProspectorBlogsOfInterestBaseDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ProspectorBlogsOfInterestTypes Type { get; set; }

        [Identity]
        public int ProfileId { get; set; }
    }
}
