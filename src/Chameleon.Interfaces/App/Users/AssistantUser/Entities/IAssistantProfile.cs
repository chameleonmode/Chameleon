using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.Assistants
{
    public interface IAssistantProfile 
        : IEntity<long>
    {
        int ProfileId { get; set; }
        string ProfileName { get; set; }
    }
}
