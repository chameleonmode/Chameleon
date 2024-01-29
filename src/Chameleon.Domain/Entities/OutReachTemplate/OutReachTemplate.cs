using Chameleon.Interfaces.OutReach;

namespace Chameleon.Domain.Entities
{
    public class OutReachTemplate
        : IOutReachTemplate
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string Subject { get; set; }
    }
}
