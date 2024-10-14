using AutoMapper;

using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.Repository;
using Chameleon.lib.Common.Interfaces.Sys;


namespace Chameleon.Infrastructure.Users {
	public class UserAssistantRepository
         : Repository<UserAssistant,
            IUserAssistant,
            long,
            UserAssistantDto,
            CreateUserAssistantDto,
            UserAssistantDto,
            GetAllRequestDto
            >
        , IUserAssistantRepository
    {
        private readonly IUserAssistantApi _userAssistantApi;

        public UserAssistantRepository(
           IMapper mapper,
           IUserAssistantApi userAssistantApi,
           IEventAggregator eventAggregator
            ) : base(mapper, userAssistantApi, eventAggregator)
        {
            _userAssistantApi = userAssistantApi;
        }

        protected override void OnInserted(IUserAssistant entity)
        {
            _eventAggregator
                .GetEvent<SavedUserAssistantEvent>()
                .Publish(new SavedUserAssistantEventArgs(entity.Id,
                                                    entity.UserName,
                                                    entity.EmailAddress,
                                                    entity.Password,
                                                    entity.ProfileIds,
                                                    entity.FolderIds,
                                                    entity.ProfilePermissionIds));
        }

        protected override void OnDeleted(IUserAssistant entity)
        {
            _eventAggregator
                .GetEvent<DeletedUserAssistantEvent>()
                .Publish(new DeletedUserAssistantEventArgs(entity.Id));
        }

        public void AddProfiles(IUserAssistant entity)
        {
            var dto = new UserAssistantDto()
            {
                Id = entity.Id,
                ProfileIds = entity.ProfileIds,
                ProfilePermissionIds = entity.ProfilePermissionIds
            };

            _userAssistantApi.AddProfiles(dto);
        }
        public void SetCanCreateProfiles(long assistantId, bool canCreateProfiles)
        {
            _userAssistantApi.SetCanCreateProfiles(assistantId, canCreateProfiles);
        }
    }
}
