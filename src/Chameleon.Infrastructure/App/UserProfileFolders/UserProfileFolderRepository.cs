using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.UserProfileFolders;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.UserProfileFolders;

using Chameleon.Interfaces.Repository;

namespace Chameleon.Infrastructure.Profiles
{
    public class UserProfileFolderRepository 
        : Repository<UserProfileFolder,
            IUserProfileFolder,
            int, 
            UserProfileFolderDto, 
            CreateUserProfileFolderDto, 
            UserProfileFolderDto,
            GetAllRequestDto
            >
        , IUserProfileFolderRepository
    {
        public UserProfileFolderRepository(
            IMapper mapper,
            IUserProfileFolderApi apiClient,
            IEventAggregator eventAggregator
            ) : base(mapper, apiClient, eventAggregator)
        {
        }

        protected override void OnSaved(IUserProfileFolder entity)
        {
            base.OnSaved(entity);

            _eventAggregator
                .GetEvent<SavedUserProfileFolderEvent>()
                .Publish(new UserProfileFolderEventArgs(entity));
        }
    }
}
