using AutoMapper;
using Chameleon.Domain.Entities.Assistants;
using Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions.Api;
using Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions.Api.Dto;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.Repository;

using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions
{
    public class AssistantProfilePermissionRepository
        : Repository<AssistantProfilePermission,
            IAssistantProfilePermission,
            int,
            AssistantProfilePermissionDto,
            CreateAssistantProfilePermissionDto,
            AssistantProfilePermissionDto,
            GetAllRequestDto>
        , IAssistantProfilePermissionRepository
    {
        private readonly IAssistantProfilePermissionApi _assistantProfilePermissionApi;
        public AssistantProfilePermissionRepository(
            IMapper mapper,
            IAssistantProfilePermissionApi assistantProfilePermissionApi,
            IEventAggregator eventAggregator
            ) : base(mapper, assistantProfilePermissionApi, eventAggregator)
        {
            _assistantProfilePermissionApi = assistantProfilePermissionApi;
        }
        public IList<IAssistantProfilePermission> GetAllProfilePermissions(long assistantId, int profileId)
        {
            var dtos = _assistantProfilePermissionApi.GetAllProfilePermissions(assistantId, profileId);
            var entities = _mapper.Map<IList<AssistantProfilePermission>>(dtos);

            return entities
                .Cast<IAssistantProfilePermission>()
                .ToList();
        }

        public void InsertProfilePermission(IAssistantProfilePermission assistantProfilePermission)
        {
            _assistantProfilePermissionApi.InsertProfilePermission(assistantProfilePermission);
        }

        public void DeleteProfilePermission(IAssistantProfilePermission assistantProfilePermission)
        {
            _assistantProfilePermissionApi.DeleteProfilePermission(assistantProfilePermission.ProfileAssistantId, assistantProfilePermission.Id);
        }
    }
}
