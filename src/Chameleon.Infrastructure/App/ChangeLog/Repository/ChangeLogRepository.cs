using AutoMapper;
using Chameleon.Infrastructure.App.ChangeLog.Api;
using Chameleon.Infrastructure.App.ChangeLog.Api.Dto;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.ChangeLog;
using Chameleon.Interfaces.Repository;
using Prism.Events;

namespace Chameleon.Infrastructure.App.ChangeLog.Repository
{
    public interface IChangeLogRepository
        : IRepository<IChangeLog>
    {
    }

    public class ChangeLogRepository
       : Repository<Domain.Entities.ChangeLog.ChangeLog,
           IChangeLog,
           ChangeLogDto
           >
       , IChangeLogRepository
    {
        public ChangeLogRepository(
            IMapper mapper,
            IChangeLogApi apiClient,
            IEventAggregator eventAggregator
            ) : base(mapper, apiClient, eventAggregator)
        {
        }
    }
}
