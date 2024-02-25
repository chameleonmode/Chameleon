using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Repository;
using Chameleon.Domain.Entities;
using AutoMapper;

using Chameleon.Interfaces.ExceptionOptions;
using System.Collections.Concurrent;

namespace Chameleon.Infrastructure.ExceptionOptions
{
    public class AppLoggerRepository
     : Repository<AppLogger,
            IAppLogger,
            int,
            AppLoggerDto,
            CreateAppLoggerDto,
            AppLoggerDto,
            GetAllRequestDto
            >
        , IAppLoggerRepository
    {
        public AppLoggerRepository(
           IMapper mapper,
           IAppLoggerApi apiClient,
           IEventAggregator eventAggregator
           ) : base(mapper, apiClient, eventAggregator)
        {
            _entities = new ConcurrentDictionary<int, IAppLogger>();
        }

        protected override void OnInserted(IAppLogger entity)
        {}
    }
}
