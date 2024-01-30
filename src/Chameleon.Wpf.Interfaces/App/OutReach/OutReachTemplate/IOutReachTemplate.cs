using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.OutReach
{
    public interface IOutReachTemplate : IEntity
    {
        string Name { get; set; }
        string Content { get; set; }
        string ContactEmail { get; set; }
        string ContactName { get; set; }
        string Subject { get; set; }
    }
}
