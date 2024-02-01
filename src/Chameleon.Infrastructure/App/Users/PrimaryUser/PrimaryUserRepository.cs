using AutoMapper;
using Chameleon.Infrastructure.App.Users.PrimaryUser.Api;
using Chameleon.Infrastructure.App.Users.PrimaryUser.Api.Dto;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.Users.PrimaryUser;
using Prism.Events;

namespace Chameleon.Infrastructure.App.Users.PrimaryUser
{
    public class PrimaryUserRepository 
        : Repository<Domain.Entities.Users.PrimaryUser.PrimaryUser,
            IPrimaryUser,
            PrimaryUserDto
            >
        , IPrimaryUserRepository
    {
        private readonly IPrimaryUserApi _primaryUserApi;

        public PrimaryUserRepository(
            IMapper mapper,
            IPrimaryUserApi apiClient,
            IEventAggregator eventAggregator,
            IPrimaryUserApi primaryUserApi
            ) : base(mapper, apiClient, eventAggregator)
        {
            _primaryUserApi = primaryUserApi;
        }

        public void MarkGuidedTourDone()
        {
            _primaryUserApi.MarkGuidedTourDone();
        }
    }
}
