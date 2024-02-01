using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api.Dto
{
    public class CreateAssistantProfileDto
    {
        public int ProfileId { get; set; }

        public IList<long> AssistantUserIds { get; set; }

        public IList<string> PermissionNames { get; set; }
    }
}
