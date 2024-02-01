using Chameleon.Interfaces.Assistants;

namespace Chameleon.Domain.Entities.Assistants
{
    public class AssistantProfile 
        : IAssistantProfile
    {
        public long Id { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
    }
}
