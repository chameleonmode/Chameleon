using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api.Dto
{
    public class AssistantProfileDto : IEntityDto<long>
    {
        public long Id { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
    }
}
