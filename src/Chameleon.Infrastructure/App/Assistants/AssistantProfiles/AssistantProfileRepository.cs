using AutoMapper;

using Chameleon.Domain.Entities.Assistants;
using Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api;
using Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api.Dto;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Assistants;
using Chameleon.Interfaces.Repository;
using Chameleon.lib.Common.Interfaces.Sys;

using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfiles {
	public class AssistantProfileRepository
        : Repository<AssistantProfile,
            IAssistantProfile,
            long,
            AssistantProfileDto,
            CreateAssistantProfileDto,
            AssistantProfileDto,
            GetAllRequestDto
            >
        , IAssistantProfileRepository
    {
        private readonly IAssistantProfileApi _assistantProfileApi;
        public AssistantProfileRepository(
            IMapper mapper,
            IAssistantProfileApi assistantProfileApi,
            IEventAggregator eventAggregator
            ) : base(mapper, assistantProfileApi, eventAggregator)
        {
            _assistantProfileApi = assistantProfileApi;
        }

        public void DeleteAssistantProfile(IAssistantProfile assistantProfile)
        {
            _assistantProfileApi.DeleteAssistantProfile(assistantProfile.Id, assistantProfile.ProfileId);
        }

        public IList<IAssistantProfile> GetAllAssistantProfilesById(long userAssistantId)
        {
            var dtos = _assistantProfileApi.GetAllAssistantProfilesById(userAssistantId);
            var entities = _mapper.Map<IList<AssistantProfile>>(dtos);
            return entities.Cast<IAssistantProfile>().ToList();
        }

        public void ShareUserProfile(int profileId, IList<long> assistantUserIds, IList<string> permissionNames)
        {
            var dto = new CreateAssistantProfileDto()
            {
                ProfileId = profileId,
                AssistantUserIds = assistantUserIds,
                PermissionNames = permissionNames
            };

            _assistantProfileApi.ShareUserProfile(dto);
        }
    }    
}
